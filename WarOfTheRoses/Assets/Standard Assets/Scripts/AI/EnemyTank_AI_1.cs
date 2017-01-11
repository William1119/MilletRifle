using UnityEngine;

public class EnemyTank_AI_1 : MonoBehaviour
{
    Tank enemy;
    float updataTime = 0;
    // Use this for initialization  
    void Start()
    {
        enemy = GetComponent<Tank>();
    }

    void Update()
    {
        updataTime += Time.deltaTime; //计时
        if (updataTime >= 0.5)
        {
            updataTime = 0;
            int randomPoint = (int)(Random.value * 100);
            if (randomPoint <= 50)
            { //50%概率改变方向
                int randomDirection = (int)(Random.value * 100);
                if (randomDirection <= 25)
                    enemy.Move(enemy.UP);
                else if (randomDirection <= 50)
                    enemy.Move(enemy.LEFT);
                else if (randomDirection <= 75)
                    enemy.Move(enemy.DOWN);
                else
                    enemy.Move(enemy.RIGHT);
            }
            else
                enemy.Move(enemy._direction);

            int randomPoint2 = (int)(Random.value * 100);
            if (randomPoint2 <= 20)
            { //20%概率攻击
                enemy.Attack(enemy.ATTACK);
            }
        }
        else
            enemy.Move(enemy._direction);
    }
}