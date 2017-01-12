using UnityEngine;

public class P1_HeroController : MonoBehaviour
{
    Hero hero;
    // Use this for initialization  
    void Start()
    {
        hero = GetComponent<Hero>();
    }

    void Update()
    {
        //获取玩家的操作   
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            hero.Move(hero.LEFT_UP);
            if (hero.skillProtectTime <= 0) //技能保护检查
                hero.SetDirection(-45);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            hero.Move(hero.LEFT_DOWN);
            if (hero.skillProtectTime <= 0) //技能保护检查
                hero.SetDirection(-135);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            hero.Move(hero.RIGHT_UP);
            if (hero.skillProtectTime <= 0) //技能保护检查
                hero.SetDirection(45);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            hero.Move(hero.RIGHT_DOWN);
            if (hero.skillProtectTime <= 0) //技能保护检查
                hero.SetDirection(135);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            hero.Move(hero.UP);
            if (hero.skillProtectTime <= 0) //技能保护检查
                hero.SetDirection(0);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            hero.Move(hero.DOWN);
            if (hero.skillProtectTime <= 0) //技能保护检查
                hero.SetDirection(180);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            hero.Move(hero.LEFT);
            if (hero.skillProtectTime <= 0) //技能保护检查
                hero.SetDirection(-90);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            hero.Move(hero.RIGHT);
            if (hero.skillProtectTime <= 0) //技能保护检查
                hero.SetDirection(90);
        }
    }
}