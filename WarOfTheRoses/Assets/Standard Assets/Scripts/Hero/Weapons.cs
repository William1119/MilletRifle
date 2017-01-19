using UnityEngine;
using UnityEngine.UI;

public struct WeaponsData //武器数据
{
    public int Id;
    public string Name;
    public string Type; //分类
    public string ResPath; //资源路径
    public float Damage; //伤害
    public float RateFire; //射速：发/每秒
    public int Clip; //弹夹容量
    public float ReloadTime; //换弹夹时间
    public float Recoil; //后坐力
    public float GunRange; //射程
    public float Pierce; //穿透力
    public float FlySpeed; //子弹飞行速度
    public float Block; //阻滞力
    public float MoveSpeed; //射击移速
    public string Desc; //描述
}

public class Weapons : MonoBehaviour
{
    public int _weaponID;
    public string _bindPoint;
    public Transform _frontSight;
    public bool isAI;

    public Transform shootPoint;
    public WeaponsData weaponData;
    public float _rateFire;
    GameObject obj; //武器
    LineRenderer redLine;
    Renderer redLine_renderer;
    float frontSightMin = 0; //准星最小范围
    float frontSightMax = 0.5f; //准星最大范围
    float frontSightRange = 0; //准星范围
    float frontSightMinus = 0.01f; //减少值
    float frontSightCD = 0.08f; //准星冷却间隔
    float frontSightTime = 0; //准星计时器

    Text clipShow; //弹夹显示
    Text clipReloadTime;
    int clip;
    int mask;

    void Start()
    {
        weaponData = LoadXml.weaponsData[_weaponID];
        Object weaponPrefab = Resources.Load(weaponData.ResPath); //加载武器
        Transform bindPoint = GetTransformPoint(transform, _bindPoint); //获得武器绑点
        obj = Instantiate(weaponPrefab, bindPoint.position, bindPoint.rotation) as GameObject;
        obj.transform.parent = bindPoint;

        shootPoint = GetTransformPoint(obj.transform, "shoot"); //设置子弹发射点
        redLine = obj.GetComponent<LineRenderer>();
        redLine_renderer = redLine.GetComponent<Renderer>();
        redLine.SetVertexCount(2);
        if(!isAI)
            SetRedLine(true);

        clipShow = GameObject.Find("Canvas/ReloadClip/Clip").GetComponent<Text>(); //弹夹显示
        clip = weaponData.Clip;
        if (!isAI)
            clipShow.text = weaponData.Clip.ToString();
        clipReloadTime = GameObject.Find("Canvas/ReloadClip/ReloadTime").GetComponent<Text>(); //弹夹装填时间显示
        _rateFire = 1 / weaponData.RateFire;

        mask = LayerMask.GetMask("Wall") + LayerMask.GetMask("Enemy");
    }

    void Cooling() //武器准星缩小
    {
        frontSightTime += Time.deltaTime; //计时
        if (frontSightRange > frontSightMin && frontSightTime >= frontSightCD)
        {
            frontSightTime = 0;
            frontSightRange -= frontSightMinus;
            _frontSight.localScale = new Vector3(1, 1, 1) * (1 + (frontSightRange - frontSightMin) / (frontSightMax - frontSightMin));
        }
    }

    void Hot() //武器准星变大
    {
        if (isAI)
            return;
        frontSightRange += weaponData.Recoil; //后坐力准星扩大
        if (frontSightRange > frontSightMax)
            frontSightRange = frontSightMax;
        _frontSight.localScale = new Vector3(1, 1, 1) * (1 + (frontSightRange - frontSightMin) / (frontSightMax - frontSightMin));
    }

    bool redLineON = false;
    public void SetRedLine(bool ON)
    {
        if (redLineON != ON)
        {
            redLineON = ON;
            redLine_renderer.enabled = redLineON;
        }
    }

    public Vector3 redPoint = Vector3.zero;
    RaycastHit redHit = new RaycastHit();
    
    public void RedLine() //红外线
    {
        if (redLineON && !isAI)
        {
            //redLine_renderer.material.mainTextureOffset += new Vector2(-Time.deltaTime * 0.5f, 0);
            //redLine.SetPosition(0, shootPoint.position);
            if (MyJoystack2.hitTarget.transform)
            {
                if (Physics.Raycast(shootPoint.position,MyJoystack2.hitTarget.point - shootPoint.position, out redHit,25 ,mask))
                {
                    redPoint = redHit.point;
                }
                else
                    redPoint = MyJoystack2.hitTarget.point;
            }
            else
                redPoint = shootPoint.position + Vector3.forward * weaponData.GunRange;
            //redLine.SetPosition(1, redPoint);
        }
    }

    float lastAttackTime = 0; //最后一次攻击时间
    public bool IsCanFire() //检查能否射击
    {
        bool isCanFire1 = false;
        if (Time.time - lastAttackTime > weaponData.RateFire / 1000) //计算攻击间隔
            isCanFire1 = true;

        bool isCanFire2 = false;
        if (clip > 0) //是否有子弹
            isCanFire2 = true;

        return isCanFire1 && isCanFire2;
    }

    Object bulletPrefab = Resources.Load("Bullet/P1_Bullet"); //加载子弹容器
    Object bulletEffectPrefab = Resources.Load("Effect/GunBullet");
    Object bulletHitEffectPrefab = Resources.Load("Effect/GunHit");
    public void Fire() //射击
    {
        lastAttackTime = Time.time;
        shootPoint.LookAt(new Vector3(redPoint.x + Random.Range(-frontSightRange, frontSightRange), redPoint.y + Random.Range(-frontSightRange, frontSightRange), redPoint.z + Random.Range(-frontSightRange, frontSightRange)));
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.transform.rotation) as GameObject;
        GameObject effect = Instantiate(bulletEffectPrefab, bullet.transform.position, bullet.transform.rotation) as GameObject;
        effect.transform.parent = bullet.transform;
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.hitEffectPrefab = bulletHitEffectPrefab;
        bulletScript.bulletData.Pierce = weaponData.Pierce;
        bulletScript.bulletData.Damage = weaponData.Damage;
        bulletScript.bulletData.FlySpeed = weaponData.FlySpeed;
        bulletScript.bulletData.FlyDistance = weaponData.GunRange;
        Hot(); //武器准星变大
        clip--;
        if (!isAI)
            clipShow.text = clip.ToString();
        if (clip <= 0)
            CallReloadClip(); //重新填装子弹
    }

    public void FireLine() //直线射击
    {
        lastAttackTime = Time.time;
        shootPoint.localRotation = Quaternion.Euler(0, 295, 0);
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.transform.rotation) as GameObject;
        GameObject effect = Instantiate(bulletEffectPrefab, bullet.transform.position, bullet.transform.rotation) as GameObject;
        effect.transform.parent = bullet.transform;
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.hitEffectPrefab = bulletHitEffectPrefab;
        bulletScript.bulletData.Pierce = weaponData.Pierce;
        bulletScript.bulletData.Damage = weaponData.Damage;
        bulletScript.bulletData.FlySpeed = weaponData.FlySpeed;
        bulletScript.bulletData.FlyDistance = weaponData.GunRange;
        Hot(); //武器准星变大
        clip--;
        if (!isAI)
            clipShow.text = clip.ToString();
        if (clip <= 0)
            CallReloadClip(); //重新填装子弹
    }

    public void RedAIFire(Transform target) //射击
    {
        lastAttackTime = Time.time;
        shootPoint.LookAt(new Vector3(target.position.x + Random.Range(-frontSightRange, frontSightRange), target.position.y + Random.Range(-frontSightRange, frontSightRange), target.position.z + Random.Range(-frontSightRange, frontSightRange)));
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.transform.rotation) as GameObject;
        GameObject effect = Instantiate(bulletEffectPrefab, bullet.transform.position, bullet.transform.rotation) as GameObject;
        effect.transform.parent = bullet.transform;
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.hitEffectPrefab = bulletHitEffectPrefab;
        bulletScript.bulletData.Pierce = weaponData.Pierce;
        bulletScript.bulletData.Damage = weaponData.Damage;
        bulletScript.bulletData.FlySpeed = weaponData.FlySpeed;
        bulletScript.bulletData.FlyDistance = weaponData.GunRange;
        Hot(); //武器准星变大
        clip--;
        if (!isAI)
            clipShow.text = clip.ToString();
        if (clip <= 0)
            CallReloadClip(); //重新填装子弹
    }

    Object enemy_BulletPrefab = Resources.Load("Bullet/Enemy_Bullet"); //加载子弹容器
    public void BlueAIFire(Transform target) //射击
    {
        lastAttackTime = Time.time;
        shootPoint.LookAt(new Vector3(target.position.x + Random.Range(-frontSightRange, frontSightRange), target.position.y + Random.Range(-frontSightRange, frontSightRange), target.position.z + Random.Range(-frontSightRange, frontSightRange)));
        GameObject bullet = Instantiate(enemy_BulletPrefab, shootPoint.position, shootPoint.transform.rotation) as GameObject;
        GameObject effect = Instantiate(bulletEffectPrefab, bullet.transform.position, bullet.transform.rotation) as GameObject;
        effect.transform.parent = bullet.transform;
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.hitEffectPrefab = bulletHitEffectPrefab;
        bulletScript.bulletData.Pierce = weaponData.Pierce;
        bulletScript.bulletData.Damage = weaponData.Damage;
        bulletScript.bulletData.FlySpeed = weaponData.FlySpeed;
        bulletScript.bulletData.FlyDistance = weaponData.GunRange;
        Hot(); //武器准星变大
        clip--;
        if (!isAI)
            clipShow.text = clip.ToString();
        if (clip <= 0)
            CallReloadClip(); //重新填装子弹
    }

    int tripleShot = 0;
    float tripleTime = 0;
    public void CallTripleShot() //三发连射
    {
        tripleTime = Time.time;
        if (clip >= 3)
            tripleShot = 3;
        else
            tripleShot = clip;
    }

    void TripleShot() //三发连射
    {
        if (tripleShot > 0 && Time.time - tripleTime > 0.1f)
        {
            tripleTime = Time.time;
            tripleShot--;
            Fire();
        }
    }

    Object RPGPrefab = Resources.Load("Bullet/P1_RPG"); //加载RPG容器
    Object RPGEffectPrefab = Resources.Load("Effect/Bullet");
    Object RPGHitEffectPrefab = Resources.Load("Effect/Hit");
    public void RPG() //RPG
    {
        shootPoint.LookAt(new Vector3(redPoint.x + Random.Range(-frontSightRange, frontSightRange), redPoint.y + Random.Range(-frontSightRange, frontSightRange), redPoint.z + Random.Range(-frontSightRange, frontSightRange)));
        GameObject bullet = Instantiate(RPGPrefab, shootPoint.position, shootPoint.transform.rotation) as GameObject;
        GameObject effect = Instantiate(RPGEffectPrefab, bullet.transform.position, bullet.transform.rotation) as GameObject;
        effect.transform.parent = bullet.transform;
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.hitEffectPrefab = RPGHitEffectPrefab;
        bulletScript.bulletData.Pierce = 500;
        bulletScript.bulletData.Damage = 500;
        bulletScript.bulletData.FlySpeed = 30;
        bulletScript.bulletData.FlyDistance = weaponData.GunRange;
        Hot(); //武器准星变大
    }

    Object tankBulletPrefab = Resources.Load("Bullet/Red_TankBullet"); //加载RPG容器
    public void TankFire(Transform fireShoot) //坦克攻击
    {
        lastAttackTime = Time.time;
        fireShoot.localRotation = Quaternion.Euler(90, 0, 0);
        GameObject bullet = Instantiate(tankBulletPrefab, fireShoot.position, fireShoot.transform.rotation) as GameObject;
        GameObject effect = Instantiate(RPGEffectPrefab, bullet.transform.position, bullet.transform.rotation) as GameObject;
        effect.transform.parent = bullet.transform;
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.hitEffectPrefab = RPGHitEffectPrefab;
        bulletScript.bulletData.Pierce = 500;
        bulletScript.bulletData.Damage = 500;
        bulletScript.bulletData.FlySpeed = 30;
        bulletScript.bulletData.FlyDistance = weaponData.GunRange;
        Hot(); //武器准星变大
    }

    public void TankFire(Transform fireShoot, Transform target) //坦克攻击
    {
        lastAttackTime = Time.time;
        fireShoot.LookAt(new Vector3(target.position.x + Random.Range(-frontSightRange, frontSightRange), target.position.y + Random.Range(-frontSightRange, frontSightRange), target.position.z + Random.Range(-frontSightRange, frontSightRange)));
        GameObject bullet = Instantiate(tankBulletPrefab, fireShoot.position, fireShoot.transform.rotation) as GameObject;
        GameObject effect = Instantiate(RPGEffectPrefab, bullet.transform.position, bullet.transform.rotation) as GameObject;
        effect.transform.parent = bullet.transform;
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.hitEffectPrefab = RPGHitEffectPrefab;
        bulletScript.bulletData.Pierce = 500;
        bulletScript.bulletData.Damage = 500;
        bulletScript.bulletData.FlySpeed = 30;
        bulletScript.bulletData.FlyDistance = weaponData.GunRange;
        Hot(); //武器准星变大
    }

    float reloadTime = 0;
    bool reloading = false;
    public void CallReloadClip() //重新填装子弹
    {
        if (reloading)
            return;
        reloadTime = weaponData.ReloadTime;
        clip = 0;
        if (!isAI)
            clipShow.text = clip.ToString();
        reloading = true;
        if (!isAI)
            clipReloadTime.enabled = true;
    }

    void ReloadClip() //重新填装子弹
    {
        if (reloadTime > 0)
        {
            reloadTime -= Time.deltaTime;
            if (!isAI)
                clipReloadTime.text = reloadTime.ToString("0.0");
        }
        if (reloadTime <= 0 && reloading)
        {
            clip = weaponData.Clip;
            if (!isAI)
                clipShow.text = clip.ToString();
            reloading = false;
            if (!isAI)
                clipReloadTime.enabled = false;
        }
    }

    void Update()
    {
        RedLine(); //红外线
        Cooling(); //武器冷却
        ReloadClip(); //重新填装子弹
        TripleShot(); //三发连射
    }

    public static Transform GetTransformPoint(Transform check, string name)  //获得指定子节点
    {
        foreach (Transform t in check)
        {
            if (t.name == name)
                return t;
            Transform t2 = GetTransformPoint(t, name);
            if (t2 != null)
                return t2;
        }
        return null;
    }

    public static float Distance(Vector3 point1, Vector3 point2)
    {
        return Mathf.Sqrt((point1.x - point2.x) * (point1.x - point2.x) + (point1.z - point2.z) * (point1.z - point2.z));
    }
}
