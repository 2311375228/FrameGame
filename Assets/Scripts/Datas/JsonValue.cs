using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class JsonValue
{
    //产品
    [System.Serializable]
    public class DataTableBackPackBase
    {
        public List<DataTableBackPackItem> listItem;
    }
    [System.Serializable]
    public class DataTableBackPackItem
    {
        public int intProductID;
        public int intProductType;
        public int intKnapsackGridLimitCount;
        public int intStockMax;

        public string strIconName;
        public string strInfoEnglish;
        public string strInfoChina;
    }
    [System.Serializable]
    public class DataTableBackPackNameBase
    {
        public List<DataTableBackPackItemName> listItem;
    }
    [System.Serializable]
    public class DataTableBackPackItemName
    {
        public int intID;
        public string[] strNames;
        public string[] strNamePlurals;
        /// <summary>
        /// true=复数  false=单数
        /// </summary>
        public string GetName(bool boo)
        {
            if (strNamePlurals[(int)ManagerValue.enumLanguage] == null || strNamePlurals[(int)ManagerValue.enumLanguage] == "")
            {
                return strNames[(int)ManagerValue.enumLanguage];
            }
            return boo ? strNamePlurals[(int)ManagerValue.enumLanguage] : strNames[(int)ManagerValue.enumLanguage];
        }
    }

    //建筑可以生产的产品
    [System.Serializable]
    public class DataTableBuildCompoundBase
    {
        public List<DataTableBuildCompoundItem> listItem;
    }
    [System.Serializable]
    public class DataTableBuildCompoundItem
    {
        public int intbuildID;
        public int intCompundID;
    }

    //建筑
    [System.Serializable]
    public class DataTableBuildingBase
    {
        public List<DataTableBuildingItem> listTable;
    }
    [System.Serializable]
    public class DataTableBuildingItem
    {
        public int intBuildID;
        public int intBuildType;
        public int numPrice;
        public int intMaintain;//每日维护价格
        public int intTimeBuild;//建造时间
        public int intTimeDemolition;//拆除时间
        public string strModelName;
    }
    [System.Serializable]
    public class DataTableBuildNameBase
    {
        public List<DataTableBuildNameItem> listTable;
    }
    [System.Serializable]
    public class DataTableBuildNameItem
    {
        public int intBuildID;
        public string[] strNames;
        public string GetName
        {
            get { return strNames[(int)ManagerValue.enumLanguage]; }
        }
    }

    //合成产品配置
    [System.Serializable]
    public class DataTableCompoundBase
    {
        public List<DataTableCompoundItem> listCompound;
    }
    [System.Serializable]
    public class DataTableCompoundItem
    {
        public int intCompoundID;
        public int intProductID;
        public int intRipeDay;//完成天数
        public int intProductCount;
        public int intPrice;//单价
        public int intCoinConsume;//维护
        public int intProductKind;
        public int[] intPorductIDStuff;
        public int[] intPorductIDnum;
    }

    //员工
    [System.Serializable]
    public class DataTableEmployeeBase
    {
        public List<DataTableEmployeeItem> listItem;
    }
    [System.Serializable]
    public class DataTableEmployeeItem
    {
        public int intEmployeeID;
        public string strEmployeeChina;
        public string strEmployeeEnglish;

        public int intCombatType;
        public int intCombatTypeRank;

        public int intHP;
        public int intATK;
        public int intMP;
        public int intSpeed;

        public int intStrengt;//力量
        public int intAgility;//敏捷
        public int intIntellect;//智力
        public int intStamina;//耐力
        public int intVersatility;//全能
    }

    //装备
    [System.Serializable]
    public class TableEquipmentBase
    {
        public List<TableEquipmentItem> listItem;
    }
    [System.Serializable]
    public class TableEquipmentItem
    {
        public int intEquipmentID;
        public int intPrice;
        public int intKnaspackStockType;
        public int intKnapsackGridLimitCount;
        public string strRandom;
        public int intOrder;//阶级
        public int intHP;
        public int intATK;
        public int intMP;
        public int intSpeed;
        public int intCritical;//暴击
        public int intEnd;//耐久值
        public string strICON;
        public string strNameChina;
        public string strNameEnglish;
        public string strContentChina;
        public string strContentEnglish;
    }

    //敌人
    [System.Serializable]
    public class DataTableEnemyBase
    {
        public string strTableName;
        public List<DataTableEnemyItem> listEnemyItems;
    }
    [System.Serializable]
    public class DataTableEnemyItem
    {
        public int intEnemyID;
        public int intEnemyRank;
        public int intHP;
        public int intAttack;
        public int intMagic;
        public int intCombatType;
        public string strNameChina;
        public string strNameEnglish;
        public string strExplainChina;
        public string strExplainEnglish;
        public string strModelName;
    }

    //敌人队伍
    [System.Serializable]
    public class DataTableEnemyTeamBase
    {
        public List<DataTableEnemyTeamItem> listEnemyTeamItems;
    }
    [System.Serializable]
    public class DataTableEnemyTeamItem
    {
        public int intEnemyTeamID;
        public int intEnemyCount;
        public int[] intEnemyIDs;
        public int[] intCombatTypes;
        public int[] intCombatTypeRanks;
        public string[] strEnemySkills;
        public string[] strEnemySkillRanks;
    }

    //技能
    [System.Serializable]
    public class DataSkillBase
    {
        public List<DataSkillItem> listItems;
    }
    [System.Serializable]
    public class DataSkillItem
    {
        public int intIndex;
        public int intSkillID;
        public int intSkillType;
        public int intCombatType;
        public string strNameChina;
        public string strNameEnglish;
        public string strICON;
        public string strNameEffect;//特效名称
        public int intValue;
        public int intRoleCount;//角色数量
        public int intRandom;//是否随机

    }

    /// <summary>
    /// 产品任务
    /// </summary>
    [System.Serializable]
    public class DataTaskProductBase
    {
        public List<DataTaskProductItem> listItem;
    }
    [System.Serializable]
    public class DataTaskProductItem
    {
        public int intID;
        public DataTaskProductGoods[] awards;
        public int intTaskRank;
        public DataTaskProductGoods[] goods;
    }
    [System.Serializable]
    public class DataTaskProductGoods
    {
        public int intType;
        public int intID;
        public int intCount;
    }
    [System.Serializable]
    public class DataTaskProductNameBase
    {
        public List<DataTaskProductNameItem> listItem;
    }
    [System.Serializable]
    public class DataTaskProductNameItem
    {
        public int intID;
        public string[] strNames;
        public string[] strExplains;

        public string GetName
        {
            get { return strNames[(int)ManagerValue.enumLanguage]; }
        }
        public string GetExplain
        {
            get { return strExplains[(int)ManagerValue.enumLanguage]; }
        }
    }

    /// <summary>
    /// 副本任务
    /// </summary>
    [System.Serializable]
    public class DataTaskDungeonBase
    {
        public List<DataTaskDungeonItem> listItem;
    }
    [System.Serializable]
    public class DataTaskDungeonItem
    {
        public int intID;
        public int intTaskRank;
        public DataTaskDungeonAward[] awards;
    }
    [System.Serializable]
    public class DataTaskDungeonAward
    {
        public int intType;
        public int intID;
        public int intCount;
    }
    [System.Serializable]
    public class DataTaskDuneonNameBase
    {
        public List<DataTaskDuneonNameItem> listItem;
    }
    [System.Serializable]
    public class DataTaskDuneonNameItem
    {
        public int intID;
        public string[] strNames;
        public string[] strExplains;

        public string GetName
        {
            get { return strNames[(int)ManagerValue.enumLanguage]; }
        }
        public string GetExplain
        {
            get { return strExplains[(int)ManagerValue.enumLanguage]; }
        }
    }

    //副本
    [System.Serializable]
    public class DataGameDungeonBase
    {
        public List<DataGameDungeonItem> listItem;
    }
    [System.Serializable]
    public class DataGameDungeonItem
    {
        public int intGameDungeonID;
        public string strDungeonModel;
        public DataGameDungeonItemTaskPoint[] taskPoints;
        public string strDungeonEffect;
        public string strAwark;
    }
    [System.Serializable]
    public class DataGameDungeonItemTaskPoint
    {
        public string strEffect;
        public string strModel;
        public int[] intTeamIDs;
        public int[] intKnaspackType;//奖品类型
        public int[] intAwardRanks;//奖品等级
        public int[] intAwardIDs;//奖品ID
        public int[] intAwardCounts;//奖品数量
    }
    [System.Serializable]
    public class DataGameDungeonNameBase
    {
        public List<DataGameDungeonItemName> listItem;
    }
    [System.Serializable]
    public class DataGameDungeonItemName
    {
        public int intID;
        public string[] strNames;
        public string[] strExplains;
        public string GetName
        {
            get { return strNames[(int)ManagerValue.enumLanguage]; }
        }
        public string GetExplain
        {
            get { return strExplains[(int)ManagerValue.enumLanguage]; }
        }
    }

    //翻译
    [System.Serializable]
    public class DataTranslationBase
    {
        public List<DataTranslationItem> listItem;
    }
    [System.Serializable]
    public class DataTranslationItem
    {
        public int intID;
        public string strEnglish;
        public string strChina;
    }

    /// <summary>
    /// 翻译,单词
    /// </summary>
    [System.Serializable]
    public class DataLanguageWordsBase
    {
        public List<DataLanguageWordsItem> listItem;
    }
    [System.Serializable]
    public class DataLanguageWordsItem
    {
        public int intID;
        public EnumLanguageWords enumWord;
        public string[] strNames;
    }

    /// <summary>
    /// 翻译，句子
    /// </summary>
    [System.Serializable]
    public class DataLanguageStatementBase
    {
        public List<DataLanguageStatementItem> listItem;
    }
    [System.Serializable]
    public class DataLanguageStatementItem
    {
        public int intID;
        public EnumLanguageStatement enumStatment;
        public List<DataLanguageStatmentItemSize> listName;
    }
    [System.Serializable]
    public class DataLanguageStatmentItemSize
    {
        public string strName;
        public int[] intNameSizes;
    }

    /// <summary>
    /// 翻译，故事情节
    /// </summary>
    [System.Serializable]
    public class DataLanguageStoryBase
    {
        public List<DataLanguageStoryItem> listItem;
    }
    [System.Serializable]
    public class DataLanguageStoryItem
    {
        public int intID;
        public EnumLanguageStory enumStory;
        public string[] strNames;
    }
}
