using UnityEngine;

public struct BulletData //子弹数据
{
    public float Pierce; //穿透
    public float Damage; //伤害
    public float FlySpeed; //飞行速度
    public float FlyDistance; //飞行距离
}

public class Bullet : MonoBehaviour
{
    public Object hitEffectPrefab;
    public BulletData bulletData;
    //AudioSource audioSource;
    //AudioClip audioClip_Bomb;
    Vector3 startPosition;

    void Start()
    {
        //audioClip_Bomb = Resources.Load<AudioClip>("Sound/Bomb");
        startPosition = transform.position;
    }

    void Update()
    {
        float l = Weapons.Distance(transform.position, startPosition);
        if (l < bulletData.FlyDistance)
            transform.position = Vector3.Lerp(transform.position, transform.position + transform.TransformDirection(Vector3.forward), Time.deltaTime * bulletData.FlySpeed);
        else
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        if (other.tag == "P1" || other.tag == "Enemy" || other.tag == "RedAI")
        {
            Hero hero = other.gameObject.GetComponent<Hero>();
            if (hero.useTank)
                bulletData.Damage *= 0.1f;
            else if (other.tag == "P1")
                bulletData.Damage *= 0.2f;

            if (bulletData.Pierce > hero.heroData.Armor)
                hero.heroData.Hp -= (int)bulletData.Damage;
            else
                hero.heroData.Hp -= (int)(bulletData.Damage - (hero.heroData.Armor - bulletData.Pierce));
            hero.showHP_SliderTime = 1.5f;
        }
        else if (other.tag == "Red_Tank")
        {
            if (other.transform.parent)
            {
                Hero hero = other.transform.parent.gameObject.GetComponent<Hero>();
                bulletData.Damage *= 0.1f;
                if (bulletData.Pierce > hero.heroData.Armor)
                    hero.heroData.Hp -= (int)bulletData.Damage;
                else
                    hero.heroData.Hp -= (int)(bulletData.Damage - (hero.heroData.Armor - bulletData.Pierce));
                hero.showHP_SliderTime = 1.5f;
            }
        }
        else if (other.tag == "Home" || other.tag == "Wall")
        {
            Roadblock roadblockScript = other.gameObject.GetComponent<Roadblock>();
            if (bulletData.Pierce > roadblockScript.roadblock.Armor)
                roadblockScript.roadblock.Hp -= (int)bulletData.Damage;
            else
                roadblockScript.roadblock.Hp -= (int)(bulletData.Damage - (roadblockScript.roadblock.Armor - bulletData.Pierce));
            roadblockScript.updataTime = 0;
        }
    }

    void OnDestroy()
    {
        GameObject effect = Instantiate(hitEffectPrefab, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
        effect.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //audioSource = effect.AddComponent<AudioSource>();
        //audioSource.spatialBlend = 1;
        //audioSource.maxDistance = 15;
        //audioSource.rolloffMode = AudioRolloffMode.Linear;
        //audioSource.clip = audioClip_Bomb;
        //audioSource.Play();
        Destroy(effect, 1.2f);
    }
}
