using UnityEngine;
using UnityEngine.EventSystems;

public class MyJoystack : MonoBehaviour, IPointerUpHandler
{
    public float MoveMaxDistance = 100; //最大拖动距离
    public float ActiveMoveDistance = 20; //激活移动的最低距离 

    Vector3 Origin; //原点
    Vector3 Current; //当前点
    float dis;
    Vector3 MovePosiNorm;  //标准化移动的距离  
    bool _turnBase = false; //开始移动

    GameObject P1;
    Hero hero;
    Transform backound;

    void Awake()
    {
        EventTriggerListener.Get(gameObject).onDrag = OnDrag;
        EventTriggerListener.Get(gameObject).onDragOut = OnDragOut;
        EventTriggerListener.Get(gameObject).onDown = OnMoveStart;
    }

    // Use this for initialization  
    
    void Start()
    {
        Origin = transform.localPosition;//设置原点
        Current = transform.localPosition;//设置当前点
        backound = GameObject.Find("Canvas/Joystack/Backound").transform;
    }

    // Update is called once per frame  
    void Update()
    {
        //dis = Vector3.Distance(transform.localPosition, Current);//拖动距离，这不是最大的拖动距离，是根据触摸位置算出来的  
        //if (dis >= MoveMaxDistance)       //如果大于可拖动的最大距离  
        //{
        //    Vector3 vec = Current + (transform.localPosition - Current) * MoveMaxDistance / dis;  //求圆上的一点：(目标点-原点) * 半径/原点到目标点的距离  
        //    transform.localPosition = vec;
        //}

        if (Vector3.Distance(transform.localPosition, Current) > ActiveMoveDistance) //距离大于激活移动的距离  
        {
            MovePosiNorm = (transform.localPosition - Current).normalized;
            MovePosiNorm = new Vector3(MovePosiNorm.x, 0, MovePosiNorm.y);
        }
        else
            MovePosiNorm = Vector3.zero;

        if (_turnBase && dis >= ActiveMoveDistance) //移动
        {
            if (P1 == null)
                P1 = GameObject.Find("P1(Clone)");

            if (P1 != null && hero == null)
                hero = P1.GetComponent<Hero>();

            if (hero != null)
                hero.Move(MovePosiNorm);
        }
    }

    void OnDrag(GameObject go, Vector2 delta)
    {
        transform.localPosition += new Vector3(delta.x, delta.y, 0);
        dis = Vector3.Distance(transform.localPosition, Current);//拖动距离，这不是最大的拖动距离，是根据触摸位置算出来的  
        if (dis >= MoveMaxDistance)
        {
            Current += new Vector3(delta.x, delta.y, 0);
            backound.localPosition = Current;
        }
    }

    void OnDragOut(GameObject go)
    {
        transform.localPosition = Origin;
        _turnBase = false;
    }

    void OnMoveStart(GameObject go)
    {
        _turnBase = true;
    }

    // 当按钮抬起的时候自动调用此方法  
    public void OnPointerUp(PointerEventData eventData)
    {
        backound.localPosition = Origin;
        Current = Origin;
    }
}