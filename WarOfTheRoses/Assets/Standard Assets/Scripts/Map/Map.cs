using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    Object p1_Prefab;
    Object enemy_Prefab;
    Object border_Prefab;
    Object wall_Prefab;
    Object steel_Prefab;
    Object home_Prefab;
    Object river_Prefab;
    Object ice_Prefab;
    Object grass_Prefab;
    Object bog_Prefab;
    Object wall_Ladder_Prefab;
    Object steel_Ladder_Prefab;

    struct Spawn
    {
        public int x;
        public int z;
    }
    Spawn p1_spawn; //p1玩家出生点
    List<Spawn> enemy_spawn; //敌人出生点
    static int ENEMY_INITIAL = 20; //敌人数量初始化
    static int P1_INITIAL = 3; //P1生命初始化
    int enemy_number = 0; //当前敌人数量
    int p1_number = 0; //p1当前数量
    int enemy_count = ENEMY_INITIAL; //敌人数量初始化
    int p1_life = P1_INITIAL; //P1生命初始化
    int home_life = 1; //基地初始化
    bool missionRun = false;
    float mission_RunTime = 0; //关卡运行时间
    uint mission = 1; //关卡

    Text Info_Center;
    Text Info_Mission;
    Text Info_Enemy;
    Text Info_P1;

    void Start()
    {
        LoadXml.LoadXml_Roles(); //读取角色数据
        LoadXml.LoadXml_Carriers(); //读取载具数据
        LoadXml.LoadXml_Weapons(); //读取枪支数据

        p1_spawn = new Spawn(); //p1玩家出生点
        enemy_spawn = new List<Spawn>(); //敌人出生点
        p1_Prefab = Resources.Load("Map/P1");
        enemy_Prefab = Resources.Load("Map/Enemy");
        border_Prefab = Resources.Load("Map/Border");
        wall_Prefab = Resources.Load("Map/Wall");
        steel_Prefab = Resources.Load("Map/Steel");
        home_Prefab = Resources.Load("Map/Home");
        river_Prefab = Resources.Load("Map/River");
        ice_Prefab = Resources.Load("Map/Ice");
        grass_Prefab = Resources.Load("Map/Grass");
        bog_Prefab = Resources.Load("Map/Bog");
        wall_Ladder_Prefab = Resources.Load("Map/Wall_Ladder");
        steel_Ladder_Prefab = Resources.Load("Map/Steel_Ladder");

        Info_Center = GameObject.Find("Canvas/Info_Center").GetComponent<Text>();
        Info_Mission = GameObject.Find("Canvas/Info_Mission").GetComponent<Text>();
        Info_Enemy = GameObject.Find("Canvas/Info_Enemy").GetComponent<Text>();
        Info_P1 = GameObject.Find("Canvas/Info_P1").GetComponent<Text>();
        PrepareMission(); //1秒后开始
    }

    public void EnemyDeath() //敌军阵亡
    {
        enemy_number--;
        Info_Enemy.text = "敌军  ： " + (enemy_count + enemy_number);
        if (enemy_number == 0 && enemy_count == 0)
        {
            Info_Center.text = "胜利！";
            Info_Center.color = Color.yellow;
            missionRun = false;
            NextMission(); //进入下一关
        }
    }

    public void P1Death() //p1阵亡
    {
        p1_number--;
        Info_P1.text = "玩家1： " + (p1_life + p1_number);
        if (p1_number == 0 && p1_life == 0)
        {
            Info_Center.text = "失败！";
            Info_Center.color = Color.gray;
            missionRun = false;
            AgainStart(); //重新开始
        }
    }

    public void HomeDeath() //基地被毁
    {
        home_life--;
        if (home_life == 0)
        {
            Info_Center.text = "失败！";
            Info_Center.color = Color.gray;
            missionRun = false;
            AgainStart(); //重新开始
        }
    }

    void NextMission()
    {
        mission++;
        p1_life++;
        enemy_number = 0; //当前敌人数量
        p1_number = 0; //p1当前数量
        enemy_count = ENEMY_INITIAL; //敌人数量初始化
        home_life = 1; //基地
        Info_Mission.text = "";
        Info_Enemy.text = "";
        Info_P1.text = "";
        Invoke("PrepareMission", 1); //1秒后开始关卡
    }

    void AgainStart() //重新开始
    {
        enemy_number = 0; //当前敌人数量
        p1_number = 0; //p1当前数量
        enemy_count = ENEMY_INITIAL; //敌人数量初始化
        p1_life = P1_INITIAL; ////P1生命初始化
        home_life = 1; //基地
        mission = 1; //关卡
        Info_Mission.text = "";
        Info_Enemy.text = "";
        Info_P1.text = "";
        Invoke("PrepareMission", 1); //1秒后开始关卡
    }

    void PrepareMission() //1秒后开始关卡
    {
        Info_Center.text = "第 " + mission + " 关";
        Info_Center.color = Color.yellow;
        ClearMap(); //清空地图
        Invoke("StartMission", 1); //1秒后开始关卡
    }

    void StartMission() //开始关卡
    {
        Info_Center.text = "";
        Info_Mission.text = "关卡  ： " + mission;
        Info_Enemy.text = "敌军  ： " + enemy_count;
        Info_P1.text = "玩家1： " + p1_life;
        missionRun = true;
        mission_RunTime = 0; //关卡运行时间
        mission = 1; //限定为第一关
        Mission_Map(LoadXml.LoadXml_Map("Map001")); //开始关卡
    }

    void ClearMap() //清空地图
    {
        GameObject obj;
        obj = GameObject.Find("Enemy(Clone)");
        do
        {
            if (obj != null)
                DestroyImmediate(obj);
            obj = GameObject.Find("Enemy(Clone)");
        } while (obj != null);

        obj = GameObject.Find("P1(Clone)");
        do
        {
            if (obj != null)
                DestroyImmediate(obj);
            obj = GameObject.Find("P1(Clone)");
        } while (obj != null);

        obj = GameObject.Find("Ground/Border(Clone)");
        do
        {
            if (obj != null)
                DestroyImmediate(obj);
            obj = GameObject.Find("Ground/Border(Clone)");
        } while (obj != null);

        obj = GameObject.Find("Ground/Wall(Clone)");
        do
        {
            if (obj != null)
                DestroyImmediate(obj);
            obj = GameObject.Find("Ground/Wall(Clone)");
        } while (obj != null);

        obj = GameObject.Find("Ground/Steel(Clone)");
        do
        {
            if (obj != null)
                DestroyImmediate(obj);
            obj = GameObject.Find("Ground/Steel(Clone)");
        } while (obj != null);

        obj = GameObject.Find("Ground/Home(Clone)");
        do
        {
            if (obj != null)
                DestroyImmediate(obj);
            obj = GameObject.Find("Ground/Home(Clone)");
        } while (obj != null);

        obj = GameObject.Find("P1_Bullet(Clone)");
        do
        {
            if (obj != null)
                DestroyImmediate(obj);
            obj = GameObject.Find("P1_Bullet(Clone)");
        } while (obj != null);

        obj = GameObject.Find("Enemy_Bullet(Clone)");
        do
        {
            if (obj != null)
                DestroyImmediate(obj);
            obj = GameObject.Find("Enemy_Bullet(Clone)");
        } while (obj != null);

        obj = GameObject.Find("Ground/Ice(Clone)");
        do
        {
            if (obj != null)
                DestroyImmediate(obj);
            obj = GameObject.Find("Ground/Ice(Clone)");
        } while (obj != null);

        obj = GameObject.Find("Ground/Grass(Clone)");
        do
        {
            if (obj != null)
                DestroyImmediate(obj);
            obj = GameObject.Find("Ground/Grass(Clone)");
        } while (obj != null);

        obj = GameObject.Find("Ground/River(Clone)");
        do
        {
            if (obj != null)
                DestroyImmediate(obj);
            obj = GameObject.Find("Ground/River(Clone)");
        } while (obj != null);

        obj = GameObject.Find("Ground/Bog(Clone)");
        do
        {
            if (obj != null)
                DestroyImmediate(obj);
            obj = GameObject.Find("Ground/Bog(Clone)");
        } while (obj != null);

        obj = GameObject.Find("Canvas/Hp_Slider(Clone)");
        do
        {
            if (obj != null)
                DestroyImmediate(obj);
            obj = GameObject.Find("Canvas/HP_Slider(Clone)");
        } while (obj != null);
    }

    void Mission_Map(int[,] map_layout) //创建关卡地图
    {
        //编号	描述
        //99	特殊填充
        //98	基地
        //97	1p出生点
        //96	2p出生点
        //95	敌军出生点

        //00	地图边缘
        //01	空地
        //02	砖墙
        //03	基地砖墙
        //04	铁墙
        //05	河流
        //06	草丛
        //07	冰路
        //08	沼泽
        //09    有梯子的砖墙
        //10    有梯子的铁墙

        float line = 0;
        GameObject roadblock;
        for (int x = 0; x <= map_layout.GetLength(0) - 1; x++)
        {
            for (int z = 0; z <= map_layout.GetLength(1) - 1; z++)
            {
                if (map_layout[x, z] == 02 || map_layout[x, z] == 03)
                {
                    //roadblock = Instantiate(wall_Prefab, new Vector3((float)z * 1.5f, transform.position.y + 1.5f, line * 1.5f), transform.rotation) as GameObject;
                    //roadblock.AddComponent<Roadblock>();
                    //roadblock.transform.parent = transform;
                }
                else if (map_layout[x, z] == 04)
                {
                    //roadblock = Instantiate(steel_Prefab, new Vector3((float)z * 1.5f, transform.position.y + 1.5f, line * 1.5f), transform.rotation) as GameObject;
                    //roadblock.transform.parent = transform;
                }
                else if (map_layout[x, z] == 05)
                {
                    //roadblock = Instantiate(river_Prefab, new Vector3((float)z * 1.5f, transform.position.y + 0.5f, line * 1.5f), transform.rotation) as GameObject;
                    //roadblock.transform.parent = transform;
                }
                else if (map_layout[x, z] == 06)
                {
                    //roadblock = Instantiate(grass_Prefab, new Vector3((float)z * 1.5f, transform.position.y + 0.5f, line * 1.5f), transform.rotation) as GameObject;
                    //roadblock.transform.parent = transform;
                }
                else if (map_layout[x, z] == 07)
                {
                    //roadblock = Instantiate(ice_Prefab, new Vector3((float)z * 1.5f, transform.position.y + 0.5f, line * 1.5f), transform.rotation) as GameObject;
                    //roadblock.transform.parent = transform;
                }
                else if (map_layout[x, z] == 08)
                {
                    //roadblock = Instantiate(bog_Prefab, new Vector3((float)z * 1.5f, transform.position.y + 0.5f, line * 1.5f), transform.rotation) as GameObject;
                    //roadblock.transform.parent = transform;
                }
                else if (map_layout[x, z] == 09)
                {
                    //roadblock = Instantiate(wall_Ladder_Prefab, new Vector3((float)z * 1.5f, transform.position.y + 1.5f, line * 1.5f), transform.rotation) as GameObject;
                    //roadblock.AddComponent<Roadblock>();
                    //roadblock.transform.parent = transform;
                }
                else if (map_layout[x, z] == 10)
                {
                    //roadblock = Instantiate(steel_Ladder_Prefab, new Vector3((float)z * 1.5f, transform.position.y + 1.5f, line * 1.5f), transform.rotation) as GameObject;
                    //roadblock.transform.parent = transform;
                }
                else if (map_layout[x, z] == 98)
                {
                    //roadblock = Instantiate(home_Prefab, new Vector3((float)z * 1.5f + 0.75f, transform.position.y + 1.5f, line * 1.5f - 0.75f), transform.rotation) as GameObject;
                    //roadblock.transform.Rotate(new Vector3(0, 180, 0));
                    //roadblock.AddComponent<Roadblock>();
                    //roadblock.transform.parent = transform;
                }
                else if (map_layout[x, z] == 00)
                {
                    //roadblock = Instantiate(border_Prefab, new Vector3((float)z * 1.5f, transform.position.y + 1.5f, line * 1.5f), transform.rotation) as GameObject;
                    //roadblock.transform.parent = transform;
                }
                else if (map_layout[x, z] == 97)
                {
                    p1_spawn.x = x;
                    p1_spawn.z = z;
                }
                else if (map_layout[x, z] == 95)
                {
                    Spawn tmp = new Spawn();
                    tmp.x = x;
                    tmp.z = z;
                    enemy_spawn.Add(tmp);
                }
            }
            line++;
        }
    }

    void Mission_Run() //关卡进行中
    {
        if (missionRun == false)
            return;

        if (p1_number < 1 && p1_life >= 1)
        {
            GameObject p1 = Instantiate(p1_Prefab, new Vector3((float)p1_spawn.z * 1.5f + 0.75f, transform.position.y + 0.5f, p1_spawn.x * 1.5f - 0.75f), transform.rotation) as GameObject;
            Hero hero = p1.AddComponent<Hero>();
            hero.heroData = LoadXml.rolesData[101]; //娜塔莎
            hero.isAI = false;
            p1.AddComponent<P1_HeroController>();
            p1.AddComponent<MoveController>();
            //p1.AddComponent<RobotAI>(); //AI
            p1_number++;
            p1_life--;
        }

        if(mission_RunTime+1f <= Time.time)
        {
            for (int i = 0; i <= enemy_spawn.Count-1; i++)
            {
                if (Random.value * 100 + 1 <= 10 && enemy_number < 6 && enemy_count >= 1)
                {
                    GameObject enemy = Instantiate(enemy_Prefab, new Vector3((float)enemy_spawn[i].z * 1.5f + 0.5f, transform.position.y + 0.5f, enemy_spawn[i].x * 1.5f - 0.5f), transform.rotation) as GameObject;
                    enemy.AddComponent<Hero>().heroData = LoadXml.rolesData[102];
                    //enemy.AddComponent<RobotAI>(); //AI
                    //enemy.AddComponent<Tank>().carriersData = LoadXml.carriersData[102];
                    //enemy.AddComponent<EnemyTank_AI_1>(); //敌军士兵AI
                    enemy.AddComponent<MoveController>();
                    enemy_number++;
                    enemy_count--;
                }
            }
            mission_RunTime = Time.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Mission_Run(); //关卡进行中
    }
}
