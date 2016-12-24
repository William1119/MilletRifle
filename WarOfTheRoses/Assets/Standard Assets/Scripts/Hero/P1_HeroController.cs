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
            hero.Move(hero.LEFT_UP);
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            hero.Move(hero.LEFT_DOWN);
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            hero.Move(hero.RIGHT_UP);
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            hero.Move(hero.RIGHT_DOWN);
        else if (Input.GetKey(KeyCode.W))
            hero.Move(hero.UP);
        else if (Input.GetKey(KeyCode.S))
            hero.Move(hero.DOWN);
        else if (Input.GetKey(KeyCode.A))
            hero.Move(hero.LEFT);
        else if (Input.GetKey(KeyCode.D))
            hero.Move(hero.RIGHT);
    }
}