using UnityEngine;
using System.Collections;

public class RobotAI : MonoBehaviour {
    public Transform target;

    Hero enemy;
    float updataTime = 0;
    Vector3 _direction;
    float _angle;

    void Start () {
        enemy = GetComponent<Hero>();
        _direction = enemy.DOWN;
        _angle = 180;
    }
	
	void Update () {
        updataTime += Time.deltaTime; //计时
        if (updataTime >= 0.1)
        {
            if (target)
            {
                _direction = (target.position - transform.position).normalized;
                _direction = new Vector3(_direction.x, 0, _direction.y);
                _angle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
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
            }
        }
        enemy.Move(_direction);
        enemy.SetDirection(_angle);
    }
}
