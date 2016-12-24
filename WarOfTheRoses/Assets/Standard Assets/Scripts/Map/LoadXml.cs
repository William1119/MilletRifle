using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public struct CarriersData //载具数据
{
    public int Id;
    public string Name;
    public string Type;
    public string ResPath;
    public int Level;
    public int Hp;
    public float Pierce; //穿透
    public float Damage; //伤害
    public float RateFire; //射速
    public float Armor; //护甲
    public float Speed; //速度
}

public struct RolesData //角色数据
{
    public int Id;
    public string Name;
    public string ResPath;
    public int Hp;
    public float Armor; //护甲
    public float MoveSpeed; //移动速度
    public string Desc; //人物介绍
}

public class LoadXml
{
    //-----------角色
    public static Dictionary<int, RolesData> rolesData = new Dictionary<int, RolesData>(); //角色数据
    public static void LoadXml_Roles()
    {
        string filename = "Config/Roles";
        string data = Resources.Load(filename).ToString();
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(data);
        //得到RECORDS节点下的所有子节点
        XmlNodeList xmlNodeList = xml.SelectSingleNode("RECORDS").ChildNodes;

        foreach (XmlElement xl1 in xmlNodeList)
        {
            RolesData tmpData = new RolesData();

            tmpData.Id = int.Parse(xl1.Attributes["Id"].Value);
            tmpData.Name = xl1.Attributes["Name"].Value;
            tmpData.ResPath = xl1.Attributes["ResPath"].Value;
            tmpData.Hp = int.Parse(xl1.Attributes["Hp"].Value);
            tmpData.Armor = float.Parse(xl1.Attributes["Armor"].Value);
            tmpData.MoveSpeed = float.Parse(xl1.Attributes["MoveSpeed"].Value) / 100;

            rolesData.Add(tmpData.Id, tmpData);
        }
    }

    //-----------载具
    public static Dictionary<int, CarriersData> carriersData = new Dictionary<int, CarriersData>(); //载具数据
    public static void LoadXml_Carriers()
    {
        string filename = "Config/Carriers";
        string data = Resources.Load(filename).ToString();
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(data);
        //得到RECORDS节点下的所有子节点
        XmlNodeList xmlNodeList = xml.SelectSingleNode("RECORDS").ChildNodes;

        foreach (XmlElement xl1 in xmlNodeList)
        {
            CarriersData tmpData = new CarriersData();

            tmpData.Id = int.Parse(xl1.Attributes["Id"].Value);
            tmpData.Name = xl1.Attributes["Name"].Value;
            tmpData.Type = xl1.Attributes["Type"].Value;
            tmpData.ResPath = xl1.Attributes["ResPath"].Value;
            tmpData.Level = int.Parse(xl1.Attributes["Level"].Value);
            tmpData.Hp = int.Parse(xl1.Attributes["Hp"].Value);
            tmpData.Pierce = float.Parse(xl1.Attributes["Pierce"].Value);
            tmpData.Damage = float.Parse(xl1.Attributes["Damage"].Value);
            tmpData.RateFire = float.Parse(xl1.Attributes["RateFire"].Value);
            tmpData.Armor = float.Parse(xl1.Attributes["Armor"].Value);
            tmpData.Speed = float.Parse(xl1.Attributes["Speed"].Value) / 10;

            carriersData.Add(tmpData.Id, tmpData);
        }
    }

    //-----------地图
    static int MAP_WIDTH = 30; //地图宽度
    static int MAP_HIGHT = 31; //地图高度
    static public int[,] LoadXml_Map(string mapName)
    {
        string filename = "Config/Map/" + mapName;
        string data = Resources.Load(filename).ToString();
        //创建xml文档
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(data);
        //得到RECORDS节点下的所有子节点
        XmlNodeList xmlNodeList = xml.SelectSingleNode("RECORDS").ChildNodes;
        MAP_HIGHT = xmlNodeList.Count;
        MAP_WIDTH = xmlNodeList.Item(0).Attributes.Count;
        int[,] mission_data = new int[MAP_HIGHT, MAP_WIDTH];
        //遍历所有子节点
        int x = MAP_HIGHT - 1;
        foreach (XmlElement xl1 in xmlNodeList)
        {
            XmlAttributeCollection nodes = xl1.Attributes;
            for (int y = 0; y < nodes.Count; y++)
            {
                if (nodes.Item(y).Value == "") nodes.Item(y).Value = "0";
                mission_data[x, y] = int.Parse(nodes.Item(y).Value);
            }
            x--;
        }
        return mission_data;
    }

    //-----------武器
    public static Dictionary<int, WeaponsData> weaponsData = new Dictionary<int, WeaponsData>();
    public static void LoadXml_Weapons()
    {
        string filename = "Config/Weapons";
        //创建xml文档
        string data = Resources.Load(filename).ToString();

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(data);
        //得到RECORDS节点下的所有子节点
        XmlNodeList xmlNodeList = xml.SelectSingleNode("RECORDS").ChildNodes;
        //遍历所有子节点
        foreach (XmlElement xl1 in xmlNodeList)
        {
            WeaponsData tmpData = new WeaponsData();

            tmpData.Id = int.Parse(xl1.Attributes["Id"].Value);
            tmpData.Name = xl1.Attributes["Name"].Value;
            tmpData.Type = xl1.Attributes["Type"].Value;
            tmpData.ResPath = xl1.Attributes["ResPath"].Value;
            tmpData.Damage = float.Parse(xl1.Attributes["Damage"].Value);
            tmpData.RateFire = float.Parse(xl1.Attributes["RateFire"].Value);
            tmpData.Clip = int.Parse(xl1.Attributes["Clip"].Value);
            tmpData.ReloadTime = float.Parse(xl1.Attributes["ReloadTime"].Value);
            tmpData.Recoil = float.Parse(xl1.Attributes["Recoil"].Value);
            tmpData.GunRange = float.Parse(xl1.Attributes["GunRange"].Value);
            tmpData.Pierce = float.Parse(xl1.Attributes["Pierce"].Value);
            tmpData.FlySpeed = float.Parse(xl1.Attributes["FlySpeed"].Value);
            tmpData.Block = float.Parse(xl1.Attributes["Block"].Value);
            tmpData.Desc = xl1.Attributes["Desc"].Value;

            weaponsData.Add(tmpData.Id, tmpData);
        }
    }
}
