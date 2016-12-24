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
        if (updataTime >= 0.1)
        {
            if (target)
            {
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
                    _direction = enemy.UP;
                    _angle = 0;
                }
                else if (randomDirection <= 50)
                {
                    _direction = enemy.LEFT;
                    _angle = -90;
                }
                else if (randomDirection <= 75)
                {
                    _direction = enemy.DOWN;
                    _angle = 180;
                }
                else
                {
                    _direction = enemy.RIGHT;
                    _angle = 90;
                }
                enemy.SetDirection(_angle);
            }
        }
        enemy.Move(_direction);
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
