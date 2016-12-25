using UnityEngine;
using System.Collections;

public class RobotAI : MonoBehaviour {
    Transform target;

    Hero enemy;
    float updataTime = 0;
    Vector3 _direction;
    float _angle;

    void Start () {
        enemy = transform.parent.GetComponent<Hero>();
        _direction = enemy.DOWN;
        _angle = 180;
    }
	
	void Update () {
        updataTime += Time.deltaTime; //计时

        if (triggerTime + 0.1f < Time.time)
            target = null;
        if (updataTime >= 0.2f)
        {
            if (target)
            {
                updataTime = 0;
                enemy.SetDirection(target.position);
                enemy.UseSkill("Attack",target);
            }
        }

        if (updataTime >= 0.5 && !target)
        {
            updataTime = 0;
            int randomPoint = (int)(Random.value * 100);
            if (randomPoint <= 50) //50%概率改变方向
            {
                int randomDirection = (int)(Random.value * 100);
                if (randomDirection <= 25)
                {
                    _angle = 0;
                }
                else if (randomDirection <= 50)
                {
                    _angle = -90;
                }
                else if (randomDirection <= 75)
                {
                    _angle = 180;
                }
                else
                {
                    _angle = 90;
                }
                enemy.SetDirection(_angle);
            }
        }
        enemy.heroSkinMove(Vector3.forward);
    }

    float triggerTime = 0;
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "P1" || other.tag == "Enemy")
        {
            target = other.transform;
            triggerTime = Time.time;
        }
    }
}
