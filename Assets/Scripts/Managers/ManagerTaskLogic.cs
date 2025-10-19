using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerTaskLogic
{
    private int intFarmCount;

    protected void TaskGuideMode(Dictionary<EnumTaskType, List<int>> dicTemp)
    {
        ManagerValue.intTaskRank = 1;
        //if (Random.Range(0, 2) == 0)
        //{
        //    dicTemp.Add(EnumTaskType.Product, 1);
        //}
        //else
        //{
        //    dicTemp.Add(EnumTaskType.Dungeon, 1);
        //}
        //dicTemp.Add(EnumTaskType.Dungeon, new int[] { 1, 1 });
    }
    protected void TaskNoviceMode(Dictionary<EnumTaskType, List<int>> dicTemp)
    {
        //休闲模式
        //if (ManagerValue.intYear < ManagerValue.intInitYear + 5)
        //{
        //    ManagerValue.intTaskRank = 1;
        //    dicTemp.Add(EnumTaskType.Product, new int[] { 1, 1 });
        //}
        //else if (ManagerValue.intYear >= ManagerValue.intInitYear + 5
        //    && ManagerValue.intYear < ManagerValue.intInitYear + 10)
        //{
        //    ManagerValue.intTaskRank = 1;
        //    dicTemp.Add(EnumTaskType.Dungeon, new int[] { 1, 1 });
        //}
        //else if (Random.Range(0, 2) == 0)
        //{
        //    ManagerValue.intTaskRank = Random.Range(1, 6);
        //    if (Random.Range(0, 2) == 0)
        //    {
        //        dicTemp.Add(EnumTaskType.Product, new int[] { 1, 1 });
        //    }
        //    else
        //    {
        //        dicTemp.Add(EnumTaskType.Dungeon, new int[] { 1, 1 });
        //    }
        //}
    }

    /// <summary>
    /// 正常模式，均衡模式
    /// </summary>
    protected void TaskNormalMode(Dictionary<EnumTaskType, List<int>> dicTemp)
    {
        //365*10=1小时
        int intRandomCount = 0;
        //第一年一个任务
        //第二年两个任务
        //任务级别2以后,每年5个任务,任务都是种植
        //任务级别3以后,包含1个或2个副本任务,
        //任务级别4以后6个任务,每种任务至少包含两个
        //任务级别5以后,8个任务,任务随机

        //按照已拥的工坊数量，和地的数量计算处罚金额，
        //任务量按照工坊数量增加随机值最大值，农场数量增加随机值最大值，工坊生产物品与农场生产的物品要分开

        int intFarmCount = 0;
        int intPastureCount = 0;
        int intFactoryCount = 0;
        Dictionary<int, BuildBase> dicBuild = UserValue.Instance.GetAllBuild();
        foreach (BuildBase temp in dicBuild.Values)
        {
            //这是默认的采矿点，要排除
            if (1008 == temp.GetBuildID || 1009 == temp.GetBuildID || 1010 == temp.GetBuildID || 1011 == temp.GetBuildID)
            {
                continue;
            }
            if (temp.GetBuildID > 1000 && temp.GetBuildID < 2000)
            {
                intFarmCount++;
            }
            if (temp.GetBuildID >= 2000 && temp.GetBuildID < 3000)
            {
                intPastureCount++;
            }
            if (temp.GetBuildID >= 3000 && temp.GetBuildID < 4000)
            {
                intFactoryCount++;
            }
        }

        //等级1是农场，产品数量在这里定义，这里还定义有多少个任务
        ProductTask(1, 1, EnumTaskType.Farm, dicTemp);

        //建筑数量上
        //农场
        if (intFarmCount > 5)
        {
            ProductTask(1, Random.Range(0, 3), EnumTaskType.Pasture, dicTemp);
        }

        //牧场
        if (intPastureCount > 5 && intPastureCount < 10)
        {
            ProductTask(2, Random.Range(0, 2), EnumTaskType.Pasture, dicTemp);
        }
        else if (intPastureCount >= 10)
        {
            ProductTask(3, Random.Range(1, 3), EnumTaskType.Pasture, dicTemp);
        }

        //工坊
        if (intFactoryCount > 5)
        {
            ProductTask(4, Random.Range(0, 2), EnumTaskType.Factory, dicTemp);
        }

        int[] intDungeonIDs = new int[] { 100100, 100101, 100102, 100103, 100104, 100105 };
        Dictionary<int, PropertiesDungeon> dicDungeon = UserValue.Instance.dicDungeon;
        for (int i = 0; i < intDungeonIDs.Length; i++)
        {
            if (!dicDungeon[intDungeonIDs[i]].booFinishDungeon)
            {
                if (!dicTemp.ContainsKey(EnumTaskType.Dungeon))
                {
                    dicTemp.Add(EnumTaskType.Dungeon, new List<int>());
                }
                dicTemp[EnumTaskType.Dungeon].Add(intDungeonIDs[i]);
                break;
            }
        }
    }

    /// <summary>
    ///
    /// intRank=任务等级
    /// intCount=任务数量
    /// enumTask=任务类型
    /// </summary>
    void ProductTask(int intRank, int intCount, EnumTaskType enumTask, Dictionary<EnumTaskType, List<int>> dicTemp)
    {
        if (!dicTemp.ContainsKey(enumTask))
        {
            dicTemp.Add(enumTask, new List<int>());
        }
        for (int i = 0; i < intCount; i++)
        {
            dicTemp[enumTask].Add(intRank);
        }
    }

    protected void TaskDifficultMode(Dictionary<EnumTaskType, List<int>> dicTemp)
    {
        if (ManagerValue.intYear < ManagerValue.intInitYear + 2)
        {
            ManagerValue.intTaskRank = 1;
        }
        else if (ManagerValue.intYear >= ManagerValue.intInitYear + 5
            && ManagerValue.intYear < ManagerValue.intInitYear + 10)
        {
            ManagerValue.intTaskRank = 2;
        }
        else if (ManagerValue.intYear >= ManagerValue.intInitYear + 10
            && ManagerValue.intYear < ManagerValue.intInitYear + 15)
        {
            ManagerValue.intTaskRank = 3;
        }
        else if (ManagerValue.intYear >= ManagerValue.intInitYear + 15
            && ManagerValue.intYear < ManagerValue.intInitYear + 20)
        {
            ManagerValue.intTaskRank = 4;
        }
        else if (ManagerValue.intYear >= ManagerValue.intInitYear + 20)
        {
            ManagerValue.intTaskRank = 5;
        }
    }
}
