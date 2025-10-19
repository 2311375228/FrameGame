using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManagerCombat
{
    Dictionary<int, JsonValue.DataTableEnemyItem> dicEnemy;
    Dictionary<int, JsonValue.DataTableEnemyTeamItem> dicEnemyTeam;
    Dictionary<int, JsonValue.DataGameDungeonItem> dicGameDungeonItem;
    Dictionary<int, JsonValue.DataGameDungeonItemName> dicGameDungeonItemName;
    Dictionary<int, JsonValue.TableEquipmentItem> dicEquipment;
    public static ManagerCombat _instance;
    public static ManagerCombat Instance
    {
        get
        {
            if (_instance == null)
            {
                //任务
                _instance = new ManagerCombat();

                //敌人
                _instance.dicEnemy = new Dictionary<int, JsonValue.DataTableEnemyItem>();
                JsonValue.DataTableEnemyBase tempEnemy = JsonUtility.FromJson<JsonValue.DataTableEnemyBase>(ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableEnemy));
                for (int i = 0; i < tempEnemy.listEnemyItems.Count; i++)
                {
                    JsonValue.DataTableEnemyItem item = tempEnemy.listEnemyItems[i];

                    _instance.dicEnemy.Add(item.intEnemyID, item);
                }

                //敌人队伍
                _instance.dicEnemyTeam = new Dictionary<int, JsonValue.DataTableEnemyTeamItem>();
                JsonValue.DataTableEnemyTeamBase tempEnemyTeam = JsonUtility.FromJson<JsonValue.DataTableEnemyTeamBase>(ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableEnemyTeam));
                for (int i = 0; i < tempEnemyTeam.listEnemyTeamItems.Count; i++)
                {
                    JsonValue.DataTableEnemyTeamItem item = tempEnemyTeam.listEnemyTeamItems[i];

                    _instance.dicEnemyTeam.Add(item.intEnemyTeamID, item);
                }

                //副本
                _instance.dicGameDungeonItem = new Dictionary<int, JsonValue.DataGameDungeonItem>();
                JsonValue.DataGameDungeonBase tempGameDungeon = JsonUtility.FromJson<JsonValue.DataGameDungeonBase>(ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableGameDungeon));
                for (int i = 0; i < tempGameDungeon.listItem.Count; i++)
                {
                    JsonValue.DataGameDungeonItem item = tempGameDungeon.listItem[i];

                    _instance.dicGameDungeonItem.Add(item.intGameDungeonID, item);
                }
                _instance.dicGameDungeonItemName = new Dictionary<int, JsonValue.DataGameDungeonItemName>();
                JsonValue.DataGameDungeonNameBase tempGameDungeonName = JsonUtility.FromJson<JsonValue.DataGameDungeonNameBase>(ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableGameDungeonLanguage));
                if (tempGameDungeonName.listItem==null)
                {
                    Debug.Log("?");
                }
                for (int i = 0; i < tempGameDungeonName.listItem.Count; i++)
                {
                    JsonValue.DataGameDungeonItemName item = tempGameDungeonName.listItem[i];
                    _instance.dicGameDungeonItemName.Add(item.intID, item);
                }

                //装备
                _instance.dicEquipment = new Dictionary<int, JsonValue.TableEquipmentItem>();
                JsonValue.TableEquipmentBase tempEquipment = JsonUtility.FromJson<JsonValue.TableEquipmentBase>(ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableEquipment));
                for (int i = 0; i < tempEquipment.listItem.Count; i++)
                {
                    JsonValue.TableEquipmentItem item = tempEquipment.listItem[i];

                    _instance.dicEquipment.Add(item.intEquipmentID, item);
                }
            }
            return _instance;
        }
    }

    public JsonValue.DataTableEnemyItem GetEnemyItem(int intEnemyID)
    {
        return dicEnemy[intEnemyID];
    }

    public JsonValue.DataTableEnemyTeamItem GetEnemyTeamItem(int intEnemyTeamID)
    {
        return dicEnemyTeam[intEnemyTeamID];
    }

    public JsonValue.DataGameDungeonItem GetGameDungeonItem(int intGameDungeonID)
    {
        return dicGameDungeonItem[intGameDungeonID];
    }
    public string GetGameDungeonName(int intID)
    {
        return dicGameDungeonItemName[intID].GetName;
    }
    public string GetGameDungeonExplain(int intID)
    {
        return "?";
    }
    public void CreateDungeonDate()
    {
        UserValue.Instance.dicDungeon.Clear();

        //确定关卡出现的小怪,每一个关卡的多余位置,是留给之前已经出现过的小怪的位置
        int[] intDungeon0 = new int[] { 10001, 10005 };
        int[] intDungeon1 = new int[] { 10006, 10010 };
        int[] intDungeon2 = new int[] { 10011, 10020 };
        int[] intDungeon3 = new int[] { 10021, 10030 };
        int[] intDungeon4 = new int[] { 10031, 10040 };
        int[] intDungeon5 = new int[] { 10041, 10050 };
        List<int[]> listTemp = new List<int[]>();
        listTemp.Add(intDungeon0);
        listTemp.Add(intDungeon1);
        listTemp.Add(intDungeon2);
        listTemp.Add(intDungeon3);
        listTemp.Add(intDungeon4);
        listTemp.Add(intDungeon5);

        int[] intIDs = dicGameDungeonItem.Keys.ToArray();
        for (int i = 0; i < intIDs.Length; i++)
        {
            for (int j = i; j < intIDs.Length; j++)
            {
                if (intIDs[i] > intIDs[j])
                {
                    int intTemp = intIDs[i];
                    intIDs[i] = intIDs[j];
                    intIDs[j] = intTemp;
                }
            }
        }

        Dictionary<int, PropertiesDungeon> dicDungeon = UserValue.Instance.dicDungeon;
        for (int i = 0; i < listTemp.Count; i++)
        {
            PropertiesDungeon dungeon = new PropertiesDungeon();
            dungeon.intDungeonID = intIDs[i];
            dungeon.points = new PropertiesDungeon.DungeonPoint[9];
            int intTeamMin = 0;//副本点,最小队伍数
            int intTeamMax = 0;//副本点,最大队伍数
            int intEnemyMin = 0;//副本点,每只队伍最小敌人数
            int intEnemyMax = 0;//副本点,每只队伍最大敌人数
            switch (i)
            {
                case 0:
                    intTeamMin = 1;
                    intTeamMax = 3;
                    intEnemyMin = 1;
                    intEnemyMax = 3;
                    break;
                case 1:
                    intTeamMin = 2;
                    intTeamMax = 5;
                    intEnemyMin = 2;
                    intEnemyMax = 4;
                    break;
                case 2:
                    intTeamMin = 3;
                    intTeamMax = 6;
                    intEnemyMin = 3;
                    intEnemyMax = 6;
                    break;
                case 3:
                    intTeamMin = 5;
                    intTeamMax = 9;
                    intEnemyMin = 4;
                    intEnemyMax = 6;
                    break;
                case 4:
                    intTeamMin = 5;
                    intTeamMax = 9;
                    intEnemyMin = 4;
                    intEnemyMax = 6;
                    break;
                case 5:
                    intTeamMin = 5;
                    intTeamMax = 9;
                    intEnemyMin = 4;
                    intEnemyMax = 6;
                    break;
            }
            for (int j = 0; j < dungeon.points.Length; j++)
            {
                dungeon.points[j] = new PropertiesDungeon.DungeonPoint();
                dungeon.points[j].teams = new PropertiesDungeon.Team[Random.Range(intTeamMin, intTeamMax)];
                for (int k = 0; k < dungeon.points[j].teams.Length; k++)
                {
                    dungeon.points[j].teams[k] = new PropertiesDungeon.Team();
                    dungeon.points[j].teams[k].intIDs = new int[Random.Range(intEnemyMin, intEnemyMax)];
                    for (int m = 0; m < dungeon.points[j].teams[k].intIDs.Length; m++)
                    {
                        //队伍中的每个小怪ID设置
                        dungeon.points[j].teams[k].intIDs[m] = Random.Range(listTemp[i][0], listTemp[i][1] + 1);
                    }
                    //如果单只队伍的小怪数量大于2,则添加之前副本的小怪
                    if (dungeon.points[j].teams[k].intIDs.Length > 2 && i > 1)
                    {
                        for (int m = 2; m < dungeon.points[j].teams[k].intIDs.Length; m++)
                        {
                            dungeon.points[j].teams[k].intIDs[m] = Random.Range(listTemp[0][0], listTemp[Random.Range(0, i + 1)][1] + 1);
                        }
                    }
                }
            }
            dicDungeon.Add(dungeon.intDungeonID, dungeon);
        }
    }

    public JsonValue.TableEquipmentItem GetEquipmentItem(int intEquipmentTableID)
    {
        return dicEquipment[intEquipmentTableID];
    }
}
