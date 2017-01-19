using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DogfaceAI : MonoBehaviour {
    Transform target;
    public List<Vector3> path; //AI路线
    int currentPoint = 0;

    Hero enemy;
    float updataTime = 0;
    Vector3 _direction;
    float _angle;

    RaycastHit redHit;
    int mask;

    void Start()
    {
        enemy = transform.parent.GetComponent<Hero>();
        _direction = enemy.DOWN;
        _angle = 180;
        redHit = new RaycastHit();
        mask = LayerMask.GetMask("Wall") + LayerMask.GetMask("Enemy") + LayerMask.GetMask("Border") + LayerMask.GetMask("RedAI") + LayerMask.GetMask("RedTank") + LayerMask.GetMask("P1");
    }

    void Update()
    {
        updataTime += Time.deltaTime; //计时

        if (triggerTime + 0.1f < Time.time)
            target = null;
        if (updataTime >= 0.2f)
        {
            if (target)
            {
                if (Physics.Raycast(transform.position, target.position - transform.position, out redHit, 25, mask))
                {
                    if (redHit.transform.tag == "Wall" || redHit.transform.tag == "Border")
                    {
                        if (path != null && currentPoint < 5)
                        {
                            updataTime = 0;
                            enemy.SetDirection(path[currentPoint]);
                            if ((int)transform.position.x == (int)path[currentPoint].x && (int)transform.position.z == (int)path[currentPoint].z)
                                currentPoint++;
                        }
                    }
                    else
                    {
                        updataTime = 0;
                        enemy.SetDirection(target.position);
                        if (transform.parent.tag == "RedAI")
                            enemy.UseSkill("RedAIAttack", target);
                        else
                            enemy.UseSkill("BlueAIAttack", target);
                    }
                }
            }
            else if (path != null && currentPoint < 5)
            {
                updataTime = 0;
                enemy.SetDirection(path[currentPoint]);
                if ((int)transform.position.x == (int)path[currentPoint].x && (int)transform.position.z == (int)path[currentPoint].z)
                    currentPoint++;
            }
        }

        //if (updataTime >= 0.5 && !target && currentPoint >= 5)
        //{
        //    updataTime = 0;
        //    int randomPoint = (int)(Random.value * 100);
        //    if (randomPoint <= 50) //50%概率改变方向
        //    {
        //        int randomDirection = (int)(Random.value * 100);
        //        if (randomDirection <= 25)
        //        {
        //            _angle = 0;
        //        }
        //        else if (randomDirection <= 50)
        //        {
        //            _angle = -90;
        //        }
        //        else if (randomDirection <= 75)
        //        {
        //            _angle = 180;
        //        }
        //        else
        //        {
        //            _angle = 90;
        //        }
        //        enemy.SetDirection(_angle);
        //    }
        //}
        enemy.heroSkinMove(Vector3.forward);
    }

    float triggerTime = 0;
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "P1" || other.tag == "RedAI" || other.tag == "Enemy")
        {
            target = other.transform;
            triggerTime = Time.time;
        }
    }
}
