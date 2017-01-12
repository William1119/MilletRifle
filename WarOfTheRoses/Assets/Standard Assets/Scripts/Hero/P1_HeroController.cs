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
            hero.SetDirection(-45);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            hero.Move(hero.LEFT_DOWN);
            hero.SetDirection(-135);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            hero.Move(hero.RIGHT_UP);
            hero.SetDirection(45);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            hero.Move(hero.RIGHT_DOWN);
            hero.SetDirection(135);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            hero.Move(hero.UP);
            hero.SetDirection(0);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            hero.Move(hero.DOWN);
            hero.SetDirection(180);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            hero.Move(hero.LEFT);
            hero.SetDirection(-90);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            hero.Move(hero.RIGHT);
            hero.SetDirection(90);
        }
    }
}