using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonSaveGame
{
    [System.Serializable]
    public class GameRoot
    {
        public int intSaveGameIndex;
        public int intGameMode;//游戏模式
        public long intCoin;//金币数量
        public long longEndYearCoin;//
        public int intNPCSellProductCount;//NPC一年收购的商品总数
        public int intHindCount;//清除的障碍物数量
        public int intTaskRank;//当前执行的任务等级
        public string strGameNickName;//玩家昵称
        public int[] intDate;//日期
        public Ground[] grounds;

        public List<GridBase> listBackpackGrid;
        public int intNewMailCount;
        public Mail[] mails;
        public StockItem[] stock;
        public Employee[] employee;
        public Employee[] guest;
        public int[] listEmployeeIndexs;

        public DungeonMap dungeonMap;
        public TaskItem[] tasks;
    }

    /// <summary>
    /// 土地
    /// </summary>
    [System.Serializable]
    public class Ground
    {
        public int intIndex;
        public int intGround;
        public int intState;
        public int intPrice;
        public int intObstacleMat;
        public int intBuildID;
        public float floX;
        public float floY;
        public float floZ;
        public string strBuild;
    }

    /// <summary>
    /// 邮件
    /// </summary>
    [System.Serializable]
    public class Mail
    {
        public bool booSee;
        public bool booGet;
        public int intIndex;
        public int enumMail;
        public string strContent;
        public string strIconName;

        public int[] gridItems;
        public int[] intIndexIDs;
        public int[] intRanks;
        public int[] intIndexCounts;
    }

    /// <summary>
    /// 背包
    /// </summary>
    [System.Serializable]
    public class GridBase
    {
        public Grid[] grids;
    }
    [System.Serializable]
    public class Grid
    {
        public int intIndex;
        public int intGridItem;
        public int intID;
        public int intRank;
        public int intCount;

        public Product product;
        public Equipment equipment;
    }
    [System.Serializable]
    public class Product
    {
        public int intPrice;
    }
    [System.Serializable]
    public class Equipment
    {
        public int intPrice;
        public int intTableID;
        public EquipmentCombat[] value;
    }
    [System.Serializable]
    public class EquipmentCombat
    {
        public int intType;
        public int intValue;
    }

    /// <summary>
    /// 员工
    /// </summary>
    [System.Serializable]
    public class Employee
    {
        public int intState;
        public int intType;
        public int intLocation;
        public int intCombatType;
        public int intTableEmployeeID;
        public int intIndexID;
        public int intRank;
        public int intIndexGroundWork;
        public int intIndexGround;
        public int intMontylyMoney;
        public int[] intEmployeePropertiesKey;
        public int[] intEmployeePropertiesValue;

        public int intIndexCombat;
        public int[] intSkillRanks;
        public Skill[] proSkills;
        public int intHP;
        public int intATK;
        public int intMP;
        public int intCombatTypeRank;
        public float floSpeed;
        public string strEmployeeName;

        public int[] intEquipmentIDs;

        public int intAdditioin;
        public int intAdditionBuildID;
        public int intAdditionProductID;
        public int intAdditionValue;

        public int intEmployeeWorkValue;
    }
    [System.Serializable]
    public class Skill
    {
        public int intSkillID;
        public int intSkill;
        public int intCombatType;
        public int intValue;
        public int intRoleCount;
        public bool booRandom;
    }

    /// <summary>
    /// 仓库
    /// </summary>
    [System.Serializable]
    public class StockItem
    {
        public int intProductID;
        public int intStockCount;
    }

    /// <summary>
    /// 任务
    /// </summary>
    [System.Serializable]
    public class TaskItem
    {
        public int intType;
        public int intID;
        public int intRank;
        public int intAwardCoin;
        public int intPenaltyCoin;
        public bool booDown;
        public bool[] booFinishs;
        public int[] intGoodsIDs;
        public int[] intGoodsCounts;

        public int intDungeonID;
        public int intDungeonIndex;
    }

    /// <summary>
    /// 副本地图
    /// </summary>
    [System.Serializable]
    public class DungeonMap
    {
        public DungeonItem[] dungeons;//地图点的通关情况
    }
    [System.Serializable]
    public class DungeonItem
    {
        public bool booFinishDungeon;//是否通关副本
        public int intDungeonID;//副本ID
        public DungeonPoint[] points;
    }
    [System.Serializable]
    public class DungeonPoint
    {
        public int intEnemyPointIndex;//副本敌人点序号
        public int intStar;//获得的星星数量
        public int intWinCount;
        public DungeonTeam[] teams;//敌人ID集合
    }
    [System.Serializable]
    public class DungeonTeam
    {
        public int[] intIDs;
    }

}