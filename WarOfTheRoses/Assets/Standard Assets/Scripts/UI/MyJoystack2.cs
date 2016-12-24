using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//共享式轮盘
public class MyJoystack2 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float MoveMaxDistance = 60; //可拖动的最大距离
    public RawImage icon_ChangeGrey;

    static public Vector3 Current; //全局当前点向量
    static public RaycastHit hitTarget; //全局瞄准的目标

    GameObject P1;
    Hero hero;
    GameObject circular; //圆
    GameObject center; //圆心
    GameObject frontSight; //准星
    
    float dis;
    Vector3 Origin;
    Vector3 frontSightOrigin;

    Vector3 MovePosiNorm;  //标准化移动的距离
    float ActiveMoveDistance = 1; //激活移动的最低距离
    bool _turnBase = false; //开始移动
    bool isDown = false;

    void Awake()
    {
        EventTriggerListener.Get(gameObject).onDrag = OnDrag;
        EventTriggerListener.Get(gameObject).onDragOut = OnDragOut;
        EventTriggerListener.Get(gameObject).onDown = OnMoveStart;
    }

    void Start () {
        circular = GameObject.Find("Canvas/Joystack2/Circular");
        center = GameObject.Find("Canvas/Joystack2/Center");
        frontSight = GameObject.Find("Canvas/Joystack2/FrontSight");
        Origin = center.transform.localPosition;//设置原点
        frontSightOrigin = frontSight.transform.localPosition;//设置准星原点
    }

    void OnDrag(GameObject go, Vector2 delta)
    {
        center.transform.localPosition += new Vector3(delta.x, delta.y, 0);
    }

    void OnDragOut(GameObject go)
    {
        center.transform.localPosition = Origin;
        _turnBase = false;
    }

    void OnMoveStart(GameObject go)
    {
        _turnBase = true;
    }

    // 当按钮被按下后系统自动调用此方法  
    public void OnPointerDown(PointerEventData eventData)
    {
        circular.SetActive(true);
        center.SetActive(true);
        circular.transform.localPosition = transform.localPosition;
        Origin = transform.localPosition; ;//设置原点
        center.transform.localPosition = transform.localPosition + Current;
        isDown = true;
        icon_ChangeGrey.color = new Color(0.24f, 0.24f, 0.24f, 1);
    }

    // 当按钮抬起的时候自动调用此方法  
    public void OnPointerUp(PointerEventData eventData)
    {
        isDown = false;
        Current = center.transform.localPosition - circular.transform.localPosition;
        circular.SetActive(false);
        center.SetActive(false);
        icon_ChangeGrey.color = Color.white;
    }

    // Update is called once per frame
    void Update () {
        if (P1 == null)
            P1 = GameObject.Find("P1(Clone)");

        if (P1 != null && hero == null)
            hero = P1.GetComponent<Hero>();

        if (hero != null)
        {
            if (isDown)
            {
                dis = Vector3.Distance(center.transform.localPosition, Origin);//拖动距离，这不是最大的拖动距离，是根据触摸位置算出来的  
                if (dis >= MoveMaxDistance) //如果大于可拖动的最大距离  
                {
                    Vector3 vec = Origin + (center.transform.localPosition - Origin) * MoveMaxDistance / dis;  //求圆上的一点：(目标点-原点) * 半径/原点到目标点的距离  
                    center.transform.localPosition = vec;
                }

                if (_turnBase && dis >= ActiveMoveDistance) //移动
                {
                    Vector3 pos = frontSightOrigin + (center.transform.localPosition - Origin) * 5; //准星移动，5倍
                    if (pos.x > Screen.width / 2) pos.x = Screen.width / 2;
                    if (pos.x < -Screen.width / 2) pos.x = -Screen.width / 2;
                    if (pos.y > Screen.height / 2) pos.y = Screen.height / 2;
                    if (pos.y < -Screen.height / 2) pos.y = -Screen.height / 2;
                    frontSight.transform.localPosition = pos;

                    MovePosiNorm = (center.transform.localPosition - Origin).normalized;
                    MovePosiNorm = new Vector3(MovePosiNorm.x, 0, MovePosiNorm.y);
                    float angle = Mathf.Atan2(MovePosiNorm.x, MovePosiNorm.z) * Mathf.Rad2Deg;
                    hero.SetDirection(angle);
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(frontSight.transform.position);
            Physics.Raycast(ray, out hitTarget);
            //if (Physics.Raycast(ray, out hitTarget))
            //    hero.SetDirection(hitTarget.point);
        }
    }
}
