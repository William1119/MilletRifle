using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    //方向
    public readonly Vector3 UP = new Vector3(0, 0, 1);
    public readonly Vector3 LEFT = new Vector3(-1, 0, 0);
    public readonly Vector3 DOWN = new Vector3(0, 0, -1);
    public readonly Vector3 RIGHT = new Vector3(1, 0, 0);
    public readonly Vector3 LEFT_UP = new Vector3(-0.7f, 0, 0.7f);
    public readonly Vector3 LEFT_DOWN = new Vector3(-0.7f, 0, -0.7f);
    public readonly Vector3 RIGHT_UP = new Vector3(0.7f, 0, 0.7f);
    public readonly Vector3 RIGHT_DOWN = new Vector3(0.7f, 0, -0.7f);

    public RolesData heroData; //当前角色数据
    public bool isAI = true;
    public Weapons weapon; //武器

    GameObject heroSkin;
    Renderer heroSkinRenderer;
    Animation heroAnimation;

    //主摄像机对象
    Camera camera1;

    GameObject HP_Slider;
    Slider HP_Slider_Slider;
    bool HP_SliderActive = true;

    AudioSource[] audioSource;
    AudioClip audioClip_Gun;
    AudioClip audioClip_RPG;
    AudioClip audioClip_Charge;
    AudioClip audioClip_Move;
    AudioClip audioClip_Mine;
    MoveController moveController;
    //初始化
    void Start()
    {
        Object heroSkinPrefab = Resources.Load(heroData.ResPath);
        heroSkin = Instantiate(heroSkinPrefab, new Vector3(transform.position.x, transform.position.y-0.5f, transform.position.z), transform.rotation) as GameObject;

        heroSkin.transform.parent = transform;
        heroSkinRenderer = heroSkin.GetComponent<Renderer>();
        heroAnimation = heroSkin.GetComponent<Animation>();
        heroAnimation.CrossFade("battlestand", 0.1f);
        heroAnimation["attack3"].speed = 1.5f;
        heroAnimation["attack1_1"].speed = 1.5f;

        camera1 = Camera.main;
        string sliderPrefab = "";
        if (isAI)
        {
            if (heroData.Id == 101)
                sliderPrefab = "UI/HP_Slider_011";
            else
            {
                int rnd = (int)(Random.value * 100);
                if (rnd <= 25)
                    sliderPrefab = "UI/HP_Slider_02";
                else if (rnd <= 50)
                    sliderPrefab = "UI/HP_Slider_03";
                else if (rnd <= 75)
                    sliderPrefab = "UI/HP_Slider_04";
                else
                    sliderPrefab = "UI/HP_Slider_05";
            }
        }
        else
            sliderPrefab = "UI/HP_Slider_01";

        Object HP_SliderPrefab = Resources.Load(sliderPrefab); //加载血条
        Transform canvas = GameObject.Find("Canvas").transform;
        HP_Slider = Instantiate(HP_SliderPrefab, canvas.position, canvas.rotation) as GameObject;
        HP_Slider.transform.SetParent(canvas);
        //得到3D世界中的坐标
        Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2f);
        //换算成2D屏幕中的坐标
        Vector2 position = camera1.WorldToScreenPoint(worldPosition);
        HP_Slider.transform.position = position;
        HP_Slider_Slider = HP_Slider.GetComponent<Slider>();
        HP_Slider_Slider.maxValue = heroData.Hp;
        HP_Slider_Slider.value = heroData.Hp;

        audioSource = GetComponents<AudioSource>();

        audioClip_Gun = Resources.Load<AudioClip>("Sound/Gun");
        audioClip_RPG = Resources.Load<AudioClip>("Sound/Fire");
        audioClip_Charge = Resources.Load<AudioClip>("Sound/Charge");
        audioClip_Mine = Resources.Load<AudioClip>("Sound/Mine");
        audioClip_Move = Resources.Load<AudioClip>("Sound/Move");
        audioSource[1].loop = true;
        audioSource[1].clip = audioClip_Move;

        weapon = heroSkin.AddComponent<Weapons>();
        if (heroData.Id == 102)
            weapon._weaponID = 101;
        else
            weapon._weaponID = 102;
        weapon._bindPoint = "Dummy_WuQi";
        weapon._frontSight = GameObject.Find("Canvas/Joystack2/FrontSight").transform; //准星
        weapon.isAI = isAI;
        moveController = GetComponent<MoveController>();
    }

    void Update()
    {
        Charge(); //冲锋

        if (heroData.Hp <= 0)
        {
            Destroy(HP_Slider); //删除血条
            Destroy(gameObject);
            Map mapScript = GameObject.Find("Ground").GetComponent<Map>();
            if (gameObject.tag == "Enemy")
                mapScript.EnemyDeath(); //敌军阵亡
            else if (gameObject.tag == "P1")
                mapScript.P1Death(); //p1阵亡
        }
        else
        {
            if (disguiseTime != 0 && disguiseTime + 0.1f <= Time.time)
            {
                disguiseState = false;
                heroSkinRenderer.enabled = true; //解除伪装
            }

            if (moveTime + 0.1f >= Time.time)
            {
                if (!heroAnimation.isPlaying || heroAnimation.IsPlaying("battlestand"))
                    heroAnimation.CrossFade("battlerun", 0.1f);
            }
            else if (!heroAnimation.isPlaying || heroAnimation.IsPlaying("battlerun"))
                heroAnimation.CrossFade("battlestand", 0.1f);
        }

        if (changeSpeedTime > 0)
            changeSpeedTime -= Time.deltaTime;

        if (changeSpeedTime <= 0 && changeSpeed != 0)
            changeSpeed = 0; //解除变速

        if (ladderTime > 0)
            ladderTime -= Time.deltaTime; //爬梯子时间

        if (skillProtectTime > 0)
            skillProtectTime -= Time.deltaTime; //技能保护时间
    }

    void FixedUpdate()
    {
        bool active = true;
        if (disguiseState) //坦克处于伪装状态
            if (showHP_SliderTime > 0)
                showHP_SliderTime -= Time.deltaTime; //计时
            else
                active = false;
        if (HP_SliderActive != active)
        {
            HP_SliderActive = active;
            HP_Slider.SetActive(HP_SliderActive);
        }

        if (active)
        {
            //得到3D世界中的坐标
            Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2f);
            //换算成2D屏幕中的坐标
            Vector2 position = camera1.WorldToScreenPoint(worldPosition);
            HP_Slider.transform.position = position;
            HP_Slider_Slider.value = heroData.Hp;
        }
    }

    float skillProtectTime = 0;
    public void UseSkill(string skillName,Transform target)
    {
        if (skillProtectTime > 0) //技能保护检查
            return;

        if (skillName == "Mine")
            UseMine(); //埋地雷
        else if (skillName == "RPG")
            UseRPG(); //RPG
        else if (skillName == "TripleShot")
            TripleShot(); //三发连射
        else if (skillName == "Attack")
            Attack(); //攻击
        else if (skillName == "RedAIAttack")
            RedAIAttack(target); //攻击
        else if (skillName == "BlueAIAttack")
            BlueAIAttack(target); //攻击
        else if (skillName == "Charge")
            UseCharge(); //冲锋
        else if (skillName == "ReloadClip")
            weapon.CallReloadClip(); //换弹夹
    }

    void Attack() //攻击
    {
        if (weapon.IsCanFire())
        {
            ChangeSpeed(-heroData.MoveSpeed * 0.5f, 0.1f);
            heroAnimation.Stop();
            heroAnimation.CrossFade("attack1_1", 0.1f);
            weapon.Fire(); //武器射击
            audioSource[0].clip = audioClip_Gun;
            if (!audioSource[0].isPlaying)
                audioSource[0].Play();
        }
    }

    void RedAIAttack(Transform target) //红方AI攻击
    {
        if (weapon.IsCanFire())
        {
            ChangeSpeed(-heroData.MoveSpeed * 0.5f, 0.1f);
            heroAnimation.Stop();
            heroAnimation.CrossFade("attack1_1", 0.1f);
            weapon.RedAIFire(target); //武器射击
            audioSource[0].clip = audioClip_Gun;
            if (!audioSource[0].isPlaying)
                audioSource[0].Play();
        }
    }

    void BlueAIAttack(Transform target) //蓝方AI攻击
    {
        if (weapon.IsCanFire())
        {
            ChangeSpeed(-heroData.MoveSpeed * 0.5f, 0.1f);
            heroAnimation.Stop();
            heroAnimation.CrossFade("attack1_1", 0.1f);
            weapon.BlueAIFire(target); //武器射击
            audioSource[0].clip = audioClip_Gun;
            if (!audioSource[0].isPlaying)
                audioSource[0].Play();
        }
    }

    void TripleShot() //三发连射
    {
        if (weapon.IsCanFire())
        {
            skillProtectTime = 0.5f;
            ChangeSpeed(-heroData.MoveSpeed * 0.5f,0.3f);
            heroAnimation.Stop();
            heroAnimation.CrossFade("attack1_1", 0.1f);
            weapon.CallTripleShot(); //三发连射
            audioSource[0].clip = audioClip_Gun;
            if (!audioSource[0].isPlaying)
                audioSource[0].Play();
        }
    }

    Object MinePrefab = Resources.Load("Bullet/P1_Mine"); //加载地雷容器
    Object MineEffectPrefab = Resources.Load("Effect/Mine");
    Object MineHitEffectPrefab = Resources.Load("Effect/MineBomb");
    void UseMine() //埋地雷
    {
        ChangeSpeed(-heroData.MoveSpeed * 1f, 0.1f);
        skillProtectTime = 0.6f;
        Mine(MinePrefab, MineEffectPrefab, MineHitEffectPrefab); //埋地雷
        audioSource[0].clip = audioClip_Mine;
        audioSource[0].Play();
    }

    void Mine(Object bulletPrefab, Object effectPrefab, Object buttleHitEffectPrefab) //埋地雷
    {
        GameObject mine = Instantiate(bulletPrefab, new Vector3(transform.position.x, 0.5f, transform.position.z), transform.rotation) as GameObject;
        GameObject effect = Instantiate(effectPrefab, mine.transform.position, mine.transform.rotation) as GameObject;
        effect.transform.parent = mine.transform;
        Mine mineScript = mine.AddComponent<Mine>();
        mineScript.hitEffectPrefab = buttleHitEffectPrefab;
        mineScript.mine.Pierce = 500;
        mineScript.mine.Damage = 500;
    }

    void UseRPG() //使用RPG
    {
        ChangeSpeed(-heroData.MoveSpeed * 0.5f, 0.1f);
        skillProtectTime = 0.8f;
        heroAnimation.Stop();
        heroAnimation.CrossFade("attack1_1", 0.1f);
        weapon.RPG();
        audioSource[0].clip = audioClip_RPG;
        audioSource[0].Play();
    }

    float chargeDistance = 10;
    float chargeTime = 0.5f;
    float chargeSpeed = 20;
    float lastChargeTime = 0;
    Vector3 startPosition;
    void UseCharge() //冲锋
    {
        skillProtectTime = 0.6f;
        heroAnimation.Stop();
        heroAnimation.CrossFade("attack3", 0.1f);
        startPosition = transform.position;
        lastChargeTime = Time.time;
        //audioSource[0].clip = audioClip_Charge;
        //audioSource[0].Play();
        if (!isAI)
            weapon.SetRedLine(false);
    }

    void Charge()
    {
        if (Time.time - lastChargeTime <= chargeTime)
        {
            float l = Weapons.Distance(transform.position, startPosition);
            if (l < chargeDistance)
                moveController.Move(heroSkin.transform.TransformDirection(Vector3.forward), chargeSpeed,"");
        }
        else if(!isAI)
            weapon.SetRedLine(true);
    }

    string moveMode = "";
    float moveTime = 0;
    public void Move(Vector3 point) //移动
    {
        if (ladderTime > 0)
            moveMode = "Ladder";
        else
            moveMode = "";
        float moveSpeed = heroData.MoveSpeed + changeSpeed;
        if (moveSpeed < 0)
            moveSpeed = 0;
        moveController.Move(transform.TransformDirection(point), moveSpeed, moveMode);
        moveTime = Time.time;
    }

    public void heroSkinMove(Vector3 point) //移动
    {
        if (ladderTime > 0)
            moveMode = "Ladder";
        else
            moveMode = "";
        float moveSpeed = heroData.MoveSpeed + changeSpeed;
        if (moveSpeed < 0)
            moveSpeed = 0;
        moveController.Move(heroSkin.transform.TransformDirection(point), moveSpeed, moveMode);
        moveTime = Time.time;
    }

    public void SetDirection(Vector3 point) //转向
    {
        point.y = heroSkin.transform.position.y;
        heroSkin.transform.LookAt(point);
    }

    float _angle;
    public void SetDirection(float angle) //转向
    {
        heroSkin.transform.Rotate(Vector3.up, angle - _angle);
        _angle = angle;
    }

    float disguiseTime = 0; //伪装时间
    public float showHP_SliderTime = 0; //伪装时血条显示时间
    bool disguiseState = false;
    public void Disguise(bool isDisguise) //伪装
    {
        disguiseTime = Time.time; //伪装时间
        disguiseState = true;
        heroSkinRenderer.enabled = false;
    }

    float changeSpeedTime = 0; //变速时间
    float changeSpeed = 0; //变速值
    public void ChangeSpeed(float speed, float durationTime) //变速
    {
        changeSpeedTime = durationTime; //变速时间
        changeSpeed += speed; //变速值
    }

    void OnTriggerEnter(Collider other)
    {
        
    }

    float ladderTime = 0;
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Grass")
            Disguise(true);
        if (other.tag == "Ice")
            ChangeSpeed(heroData.MoveSpeed * 0.5f,0.1f);
        if (other.tag == "Bog")
            ChangeSpeed(-heroData.MoveSpeed * 0.5f,0.1f);
        if (other.tag == "Ladder")
            ladderTime = 0.1f;
    }

    void OnTriggerExit(Collider other)
    {

    }
}
