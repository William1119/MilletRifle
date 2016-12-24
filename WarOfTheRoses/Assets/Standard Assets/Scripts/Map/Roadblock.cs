using UnityEngine;
using UnityEngine.UI;

public struct RoadblockData //障碍物数据
{
    public int Hp;
    public int Armor; //护甲
}

public class Roadblock : MonoBehaviour
{
    public RoadblockData roadblock;
    public float updataTime = 2; //显示时间

    //主摄像机对象
    Camera camera1;
    GameObject HP_Slider;
    Slider HP_Slider_Slider;
    bool HP_SliderActive = false;

    void Start()
    {
        if (tag == "Steel")
        {
            roadblock.Hp = 100;
            roadblock.Armor = 150;
        }
        else if (tag == "Wall" || tag == "Home")
        {
            roadblock.Hp = 100;
            roadblock.Armor = 15;
        }

        camera1 = Camera.main;

        Object HP_SliderPrefab = Resources.Load("UI/HP_Slider"); //加载血条
        Transform canvas = GameObject.Find("Canvas").transform;
        HP_Slider = Instantiate(HP_SliderPrefab, canvas.position, canvas.rotation) as GameObject;
        HP_Slider.transform.SetParent(canvas);
        HP_Slider.SetActive(HP_SliderActive);
        HP_Slider_Slider = HP_Slider.GetComponent<Slider>();
        HP_Slider_Slider.maxValue = roadblock.Hp;
        HP_Slider_Slider.value = roadblock.Hp;
    }

    void Update()
    {
        if (roadblock.Hp <= 0)
        {
            Destroy(HP_Slider); //删除血条
            Destroy(gameObject);
            Map mapScript = GameObject.Find("Ground").GetComponent<Map>();
            if (gameObject.tag == "Home")
                mapScript.HomeDeath(); //基地被毁
        }
    }

    void FixedUpdate()
    {
        bool active = true;
        if (updataTime < 1.5f)
        {
            updataTime += Time.deltaTime; //计时
            //得到3D世界中的坐标
            Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2f);
            //换算成2D屏幕中的坐标
            Vector2 position = camera1.WorldToScreenPoint(worldPosition);
            HP_Slider.transform.position = position;
            HP_Slider_Slider.value = roadblock.Hp;
        }
        else
            active = false;

        if (HP_SliderActive != active)
        {
            HP_SliderActive = active;
            HP_Slider.SetActive(HP_SliderActive);
        }
    }
}
