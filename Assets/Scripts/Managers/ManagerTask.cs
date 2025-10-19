using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManagerTask : ManagerTaskLogic
{
    public int intYear;

    Dictionary<int, JsonValue.DataTaskProductItem> dicTaskProduct;
    Dictionary<int, JsonValue.DataTaskDungeonItem> dicTaskDungeon;
    Dictionary<int, JsonValue.DataTaskDuneonNameItem> dicTaskDungeonName;
    Dictionary<int, JsonValue.DataTaskProductNameItem> dicTaskProductName;
    List<int[]> listTaskProduct = new List<int[]>();
    List<int[]> listTaskDungeon = new List<int[]>();
    static ManagerTask _instance;
    public static ManagerTask Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ManagerTask();

                //产品任务
                string strData = ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableTaskProduct);
                JsonValue.DataTaskProductBase json = JsonUtility.FromJson<JsonValue.DataTaskProductBase>(strData);
                _instance.dicTaskProduct = new Dictionary<int, JsonValue.DataTaskProductItem>();
                for (int i = 0; i < json.listItem.Count; i++)
                {
                    _instance.dicTaskProduct.Add(json.listItem[i].intID, json.listItem[i]);
                }

                //副本任务
                strData = ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableTaskDungeon);
                JsonValue.DataTaskDungeonBase jsonDungeon = JsonUtility.FromJson<JsonValue.DataTaskDungeonBase>(strData);
                _instance.dicTaskDungeon = new Dictionary<int, JsonValue.DataTaskDungeonItem>();
                for (int i = 0; i < jsonDungeon.listItem.Count; i++)
                {
                    _instance.dicTaskDungeon.Add(jsonDungeon.listItem[i].intID, jsonDungeon.listItem[i]);
                }
                strData = ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableTaskDungeonLanguage);
                JsonValue.DataTaskDuneonNameBase jsonDungeonName = JsonUtility.FromJson<JsonValue.DataTaskDuneonNameBase>(strData);
                _instance.dicTaskDungeonName = new Dictionary<int, JsonValue.DataTaskDuneonNameItem>();
                for (int i = 0; i < jsonDungeonName.listItem.Count; i++)
                {
                    _instance.dicTaskDungeonName.Add(jsonDungeonName.listItem[i].intID, jsonDungeonName.listItem[i]);
                }
                strData = ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableTaskProductLanguage);
                JsonValue.DataTaskProductNameBase jsonProductName = JsonUtility.FromJson<JsonValue.DataTaskProductNameBase>(strData);
                _instance.dicTaskProductName = new Dictionary<int, JsonValue.DataTaskProductNameItem>();
                for (int i = 0; i < jsonProductName.listItem.Count; i++)
                {
                    _instance.dicTaskProductName.Add(jsonProductName.listItem[i].intID, jsonProductName.listItem[i]);
                }

                for (int i = 1; i <= 4; i++)
                {
                    List<int> listTemp = new List<int>();
                    foreach (JsonValue.DataTaskProductItem temp in _instance.dicTaskProduct.Values)
                    {
                        if (temp.intTaskRank == i)
                        {
                            listTemp.Add(temp.intID);

                        }

                    }
                    _instance.listTaskProduct.Add(listTemp.ToArray());
                }

                //excel改为关卡对应的奖励，等级对应
                //分类等级任务副本任务ID，方便查找
                for (int i = 1; i <= 5; i++)
                {
                    List<int> listTemp = new List<int>();
                    foreach (JsonValue.DataTaskDungeonItem temp in _instance.dicTaskDungeon.Values)
                    {
                        if (temp.intTaskRank == i)
                        {
                            listTemp.Add(temp.intID);
                        }
                    }
                    _instance.listTaskDungeon.Add(listTemp.ToArray());
                }
            }
            return _instance;
        }
    }

    public int[] GetTaskProductIDs(int intTaskRank = -1)
    {
        if (intTaskRank == -1)
        {
            return dicTaskProduct.Keys.ToArray();
        }
        List<int> listTemp = new List<int>();
        foreach (JsonValue.DataTaskProductItem temp in dicTaskProduct.Values)
        {
            if (temp.intTaskRank == intTaskRank)
            {
                listTemp.Add(temp.intID);
            }
        }
        return listTemp.ToArray();
    }

    public int[] GetTaskDungeonIDs(int intTaskRank = -1)
    {
        if (intTaskRank == -1)
        {
            return dicTaskDungeon.Keys.ToArray();
        }
        List<int> listTemp = new List<int>();
        foreach (JsonValue.DataTaskDungeonItem temp in dicTaskDungeon.Values)
        {
            listTemp.Add(temp.intID);
        }
        return listTemp.ToArray();
    }

    public JsonValue.DataTaskDungeonItem GetDungeonItem(int intID)
    {
        return dicTaskDungeon[intID];
    }

    public JsonValue.DataTaskProductItem GetProductItem(int intID)
    {
        return dicTaskProduct[intID];
    }
    public string GetDungeonName(int intID)
    {
        return dicTaskDungeonName[intID].GetName;
    }
    public string GetDungeonExplain(int intID)
    {
        return dicTaskDungeonName[intID].GetExplain;
    }
    public string GetTaskProductName(int intID)
    {
        return dicTaskProductName[intID].GetName;
    }
    public string GetTaskProductExplain(int intID)
    {
        return dicTaskProductName[intID].GetExplain;
    }

    public PropertiesTask[] GetPropertiesTaskItem()
    {
        //一年6分钟,5年半个小时
        //第一年出得任务都是第一梯种植建筑
        Dictionary<EnumTaskType, List<int>> dicTemp = new Dictionary<EnumTaskType, List<int>>();
        switch (ManagerValue.enumGameMode)
        {
            case EnumGameMode.GuideMode:
                TaskGuideMode(dicTemp);
                break;
            case EnumGameMode.NoviceMode:
                TaskNoviceMode(dicTemp);
                break;
            case EnumGameMode.NormalMode:
                TaskNormalMode(dicTemp);
                break;
            case EnumGameMode.DifficultMode:
                TaskDifficultMode(dicTemp);
                break;
        }

        return TaskRank(dicTemp);
    }

    PropertiesTask[] TaskRank(Dictionary<EnumTaskType, List<int>> dicTemp)
    {
        //365*10=1小时

        //处罚金额，每年加50金币
        //奖励金额按照物品总价格的20%至120%随机出
        //处罚金额按照物品总价值的40%至200随机出，要和固定处罚金叠加

        List<PropertiesTask> listTemp = new List<PropertiesTask>();

        //任务的影响因素 游戏时间 农场数量 金币数量
        //副本任务由于耗时过长，每年只能有一个任务
        int intCountFarm = dicTemp.ContainsKey(EnumTaskType.Farm) ? dicTemp[EnumTaskType.Farm].Count : 0;
        int intCountPasture = dicTemp.ContainsKey(EnumTaskType.Pasture) ? dicTemp[EnumTaskType.Pasture].Count : 0;
        int intCountFactory = dicTemp.ContainsKey(EnumTaskType.Factory) ? dicTemp[EnumTaskType.Factory].Count : 0;
        foreach (KeyValuePair<EnumTaskType, List<int>> temp in dicTemp)
        {
            int intAwardCoinPrice = (ManagerValue.intYear - ManagerValue.intInitYear) * 50;
            int intPenaltyCoin = (ManagerValue.intYear - ManagerValue.intInitYear) * 80;

            if (temp.Key == EnumTaskType.Dungeon)
            {
                if (temp.Value.Count == 0)
                {
                    continue;
                }
                PropertiesTask proDungeon = new PropertiesTask();
                //随机一个副本id
                proDungeon.intDungeonID = temp.Value.Count == 1 ? temp.Value[0] - 1 : temp.Value[Random.Range(0, temp.Value.Count)] - 1;
                proDungeon.intDungeonIndex = Random.Range(0, 9);
                proDungeon.enumTask = EnumTaskType.Dungeon;
                int intRandomRank = 0;// Random.Range(1, 6);

                //开启的副本，决定任务奖励的档次
                if (temp.Value.Count <= 1)
                {
                    intRandomRank = 0;// Random.Range(1, 3);
                }
                else if (temp.Value.Count == 2 || temp.Value.Count == 3)
                {
                    intRandomRank = Random.Range(0, 2);//Random.Range(1, 4);
                }
                else if (temp.Value.Count == 4 || temp.Value.Count == 5)
                {
                    intRandomRank = Random.Range(0, 4);
                }
                else if (temp.Value.Count == 6)
                {
                    intRandomRank = Random.Range(0, 5);
                }
                proDungeon.intID = listTaskDungeon[intRandomRank][Random.Range(0, listTaskDungeon[intRandomRank].Length)];
                proDungeon.intRank = intRandomRank;
                proDungeon.booDown = false;
                JsonValue.DataTaskDungeonItem itemDungeon = GetDungeonItem(proDungeon.intID);

                int intDungeonTotalPrice = 0;
                for (int i = 0; i < itemDungeon.awards.Length; i++)
                {
                    intDungeonTotalPrice = (ManagerCompound.Instance.GetProductPrice(itemDungeon.awards[i].intID) * itemDungeon.awards[i].intCount);
                }
                intAwardCoinPrice = intAwardCoinPrice + (int)(Random.Range(0.2f, 1.4f) * intDungeonTotalPrice);
                intPenaltyCoin = intPenaltyCoin + (int)(Random.Range(0.4f, 2.0f) * intDungeonTotalPrice);

                proDungeon.intAwardCion = intAwardCoinPrice;
                proDungeon.intPenaltyCoin = intPenaltyCoin;
                listTemp.Add(proDungeon);
            }
            else
            {
                for (int t = 0; t < temp.Value.Count; t++)
                {
                    PropertiesTask item = new PropertiesTask();
                    //在等级任务中，随机一个任务id
                    item.enumTask = EnumTaskType.Farm;
                    int intTemp = 0;
                    if (temp.Value[t] - 1 >= listTaskProduct.Count)
                    {
                        intTemp = listTaskProduct.Count - 1;
                    }
                    intTemp = Random.Range(0, intTemp);
                    int intTempID = Random.Range(0, listTaskProduct[intTemp].Length);
                    item.intID = listTaskProduct[intTemp][intTempID];
                    item.intRank = temp.Value[t];
                    item.booDown = false;
                    JsonValue.DataTaskProductItem itemProduct = GetProductItem(item.intID);
                    item.booFinish = new bool[itemProduct.goods.Length];
                    item.intGoodsIDs = new int[itemProduct.goods.Length];
                    item.intGoodsCounts = new int[itemProduct.goods.Length];

                    int intProductTotalPrice = 0;
                    for (int i = 0; i < itemProduct.goods.Length; i++)
                    {
                        item.intGoodsIDs[i] = itemProduct.goods[i].intID;
                        item.intGoodsCounts[i] = itemProduct.goods[i].intCount;
                        intProductTotalPrice += (ManagerCompound.Instance.GetProductPrice(itemProduct.goods[i].intID) * item.intGoodsCounts[i]);
                    }
                    intAwardCoinPrice = intAwardCoinPrice + (int)(Random.Range(0.2f, 1.4f) * intProductTotalPrice);
                    intPenaltyCoin = intPenaltyCoin + (int)(Random.Range(0.4f, 2.0f) * intProductTotalPrice);

                    item.intAwardCion = intAwardCoinPrice;
                    item.intPenaltyCoin = intPenaltyCoin;
                    listTemp.Add(item);
                }
            }

        }
        return listTemp.ToArray();
    }
}
