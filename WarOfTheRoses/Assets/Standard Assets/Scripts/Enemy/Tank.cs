using UnityEngine;
using UnityEngine.UI;

public class Tank : MonoBehaviour
{
    //方向
    public readonly int UP = 0;
    public readonly int LEFT = 1;
    public readonly int DOWN = 2;
    public readonly int RIGHT = 3;

    //坦克基本行为
    public readonly string ATTACK = "attack1";
    public readonly string SKILL1 = "skill1";
    public readonly string SKILL2 = "skill2";
    public readonly string SKILL3 = "skill3";

    float lastAttackTime = 0; //最后一次攻击时间
    float disguiseTime = 0; //伪装时间
    bool disguiseState = false;
    public float showHP_SliderTime = 0; //显示时间

    float moveTime = 0; //移动时间
    float changeSpeedTime = 0; //变速时间
    float changeSpeed = 0; //变速值

    Object bulletPrefab; //子弹
    Object bulletEffectPrefab; //子弹特效
    Object bulletHitEffectPrefab; //子弹命中特效

    public int _direction = 0; //当前方向

    public CarriersData carriersData; //当前坦克数据

    GameObject tankSkin;
    Renderer tankSkinRenderer;

    //主摄像机对象
    Camera camera1;

    GameObject HP_Slider;
    Slider HP_Slider_Slider;
    bool HP_SliderActive = true;

    AudioSource[] audioSource;
    AudioClip audioClip_Fire;
    AudioClip audioClip_Move;
    MoveController moveController;

    //初始化
    void Start()
    {
        SetDirection(DOWN); //转向
        bulletPrefab = Resources.Load("Bullet/Enemy_Bullet"); //加载子弹资源

        bulletEffectPrefab = Resources.Load("Effect/Bullet");
        bulletHitEffectPrefab = Resources.Load("Effect/Hit");
        Object tankSkinPrefab = Resources.Load(carriersData.ResPath);
        tankSkin = Instantiate(tankSkinPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), transform.rotation) as GameObject;
        tankSkin.transform.Rotate(new Vector3(-90, 0, 0));
        tankSkin.transform.parent = transform;
        tankSkinRenderer = tankSkin.GetComponent<Renderer>();

        camera1 = Camera.main;

        Object HP_SliderPrefab = Resources.Load("UI/HP_Slider"); //加载血条
        Transform canvas = GameObject.Find("Canvas").transform;
        HP_Slider = Instantiate(HP_SliderPrefab, canvas.position, canvas.rotation) as GameObject;
        HP_Slider.transform.SetParent(canvas);
        //得到3D世界中的坐标
        Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2f);
        //换算成2D屏幕中的坐标
        Vector2 position = camera1.WorldToScreenPoint(worldPosition);
        HP_Slider.transform.position = position;
        HP_Slider_Slider = HP_Slider.GetComponent<Slider>();
        HP_Slider_Slider.maxValue = carriersData.Hp;
        HP_Slider_Slider.value = carriersData.Hp;

        audioSource = GetComponents<AudioSource>();

        audioClip_Fire = Resources.Load<AudioClip>("Sound/Fire");
        audioClip_Move = Resources.Load<AudioClip>("Sound/Move");
        audioSource[0].clip = audioClip_Fire;
        audioSource[1].loop = true;
        audioSource[1].clip = audioClip_Move;
        moveController = GetComponent<MoveController>();
    }

    void Update()
    {
        if (carriersData.Hp <= 0)
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
                tankSkinRenderer.enabled = true; //解除伪装
            }
            if (changeSpeedTime != 0 && changeSpeedTime + 0.1f <= Time.time)
                changeSpeed = 0; //解除变速

            if (moveTime > 0)
            {
                if (!audioSource[1].isPlaying)
                    audioSource[1].Play();
                moveTime -= Time.deltaTime;
                audioSource[1].volume = moveTime;
            }
        }
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
            HP_Slider_Slider.value = carriersData.Hp;
        }
    }

    public void Attack(string attack_mode) //攻击
    {
        if (Time.time - lastAttackTime > 15 / carriersData.RateFire) //计算攻击间隔
        {
            lastAttackTime = Time.time;
            if (attack_mode == ATTACK)
            {
                ShootBullet("Dummy_shoot", bulletPrefab, bulletEffectPrefab, bulletHitEffectPrefab); //发射一个子弹
                audioSource[0].clip = audioClip_Fire;
                audioSource[0].Play();
            }
        }
    }

    void ShootBullet(string point, Object bulletPrefab, Object effectPrefab, Object buttleHitEffectPrefab) //从一个绑点发射一个子弹
    {
        Transform shootPoint = Weapons.GetTransformPoint(transform, point); //设置子弹发射点
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation) as GameObject;
        GameObject effect = Instantiate(effectPrefab, bullet.transform.position, bullet.transform.rotation) as GameObject;
        effect.transform.parent = bullet.transform;
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.hitEffectPrefab = buttleHitEffectPrefab;
        bulletScript.bulletData.Pierce = carriersData.Pierce;
        bulletScript.bulletData.Damage = carriersData.Damage;
        bulletScript.bulletData.FlySpeed = 30;
        bulletScript.bulletData.FlyDistance = 13;
    }

    public void Move(int direction) //移动和转向
    {
        SetDirection(direction); //转向
        moveController.Move(transform.TransformDirection(Vector3.forward), carriersData.Speed + changeSpeed,"");
        moveTime = 0.4f;
    }

    void SetDirection(int direction) //转向
    {
        if (_direction != direction)
        {
            transform.Rotate(Vector3.up, (_direction - direction) * 90);
            _direction = direction;
        }
    }

    public void Disguise(bool isDisguise) //坦克伪装
    {
        disguiseTime = Time.time; //伪装时间
        disguiseState = true;
        tankSkinRenderer.enabled = false;
    }

    public void ChangeSpeed(float speed) //坦克变速
    {
        changeSpeedTime = Time.time; //变速时间
        changeSpeed = speed; //变速值
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Grass")
            Disguise(true);
        if (other.tag == "Ice")
            ChangeSpeed(carriersData.Speed*0.5f);
        if (other.tag == "Bog")
            ChangeSpeed(-carriersData.Speed*0.5f);
    }
}
