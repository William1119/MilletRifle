using UnityEngine;
using UnityEngine.EventSystems;

public class CallSkill : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string skillName; //技能名称
    public bool aRunningFire = false; //是否连续攻击
    public bool autoUse = false; //是否自动使用

    GameObject P1;
    Hero hero;

    // 按钮是否是按下状态  
    bool isDown = false;

    //按钮最后一次是被按住状态时候的时间  
    float lastIsDownTime;

    void Update()
    {
        if (isDown)
        {
            if (P1 == null)
                P1 = GameObject.Find("P1(Clone)");

            if (P1 != null && hero == null)
                hero = P1.GetComponent<Hero>();
            //延迟时间
            if (Time.time - lastIsDownTime > hero.weapon._rateFire)
            {
                if (aRunningFire)
                {
                    lastIsDownTime = Time.time;
                    UseSkill();
                }
                else if(autoUse && MyJoystack2.hitTarget.transform != null)
                {
                    if (MyJoystack2.hitTarget.transform.tag == "Enemy")
                    {
                        lastIsDownTime = Time.time;
                        UseSkill();
                    }
                }
            }
        }
    }

    void UseSkill() //使用技能
    {
        if (P1 == null)
            P1 = GameObject.Find("P1(Clone)");

        if (P1 != null && hero == null)
            hero = P1.GetComponent<Hero>();

        if (hero != null)
            hero.UseSkill(skillName,null);
    }

    // 当按钮被按下后系统自动调用此方法  
    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
        lastIsDownTime = Time.time;
        if (aRunningFire)
            UseSkill(); //使用技能
    }

    // 当按钮抬起的时候自动调用此方法  
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!aRunningFire && !autoUse)
            UseSkill(); //使用技能
        isDown = false;
    }
}
