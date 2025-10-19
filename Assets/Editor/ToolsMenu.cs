using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using Excel;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class ToolsMenu
{
    static CheckTable table;

    [MenuItem("Tools/ReadExcelTable")]
    static void ReadExcelTable()
    {
        table = new CheckTable();
        table.dicStr = new Dictionary<EnumTableName, string>();

        Type t = typeof(ToolsMenu);
        foreach (EnumTableName temp in Enum.GetValues(typeof(EnumTableName)))
        {
            MethodInfo mi = t.GetMethod(temp.ToString());
            //类,参数组
            string strTemp = (string)mi.Invoke(Activator.CreateInstance(t), new object[] { GetCollect(temp.ToString()) });
            table.dicStr.Add(temp, strTemp);
            //File.WriteAllText(Application.dataPath + "/Resources/DataTables/" + temp + ".json", strTemp);
            File.WriteAllBytes(Application.dataPath + "/Resources/DataTables/" + (int)temp + ".json", Tools.GetFileByte(strTemp));
        }
        table.CheckProductCompound(table.dicStr[EnumTableName.TableProductCompound]);
        foreach (KeyValuePair<EnumTableName, string> temp in table.dicStr)
        {
            switch (temp.Key)
            {
                case EnumTableName.TableBuildCompound:
                    table.CheckBuildCompound(temp.Value);
                    break;
                case EnumTableName.TableTaskProduct:
                    table.CheckTaskProduct(temp.Value);
                    break;
                case EnumTableName.TableTaskDungeon:
                    table.CheckTaskDungeon(temp.Value);
                    break;
                case EnumTableName.TableGameDungeon:
                    table.CheckDungeon(temp.Value);
                    break;
            }
        }
    }

    static DataRowCollection GetCollect(string strTableName)
    {
        string strPath = Application.dataPath + "/Editor/Tables/" + strTableName + ".xlsx";
        FileStream stream = File.Open(strPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        //IExcelDataRead 是C#用于操作Excel文件的类库；
        IExcelDataReader excelreader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelreader.AsDataSet();
        int numTableLength = result.Tables.Count;
        //Tables[0] 下标0代表excel文件中的第一张表
        int columnNum = result.Tables[0].Columns.Count;
        int rowNum = result.Tables[0].Rows.Count;
        DataRowCollection collect = null;
        JsonValue.DataTableEnemyTeamBase dataTableRoot = new JsonValue.DataTableEnemyTeamBase();
        dataTableRoot.listEnemyTeamItems = new List<JsonValue.DataTableEnemyTeamItem>();
        collect = result.Tables[0].Rows;
        stream.Close();
        return collect;
    }

    public string TableBuild(DataRowCollection collect)
    {
        JsonValue.DataTableBuildingBase dataTableRoot = new JsonValue.DataTableBuildingBase();
        dataTableRoot.listTable = new List<JsonValue.DataTableBuildingItem>();
        string str = null;
        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataTableBuildingItem item = new JsonValue.DataTableBuildingItem();
            item.intBuildID = int.Parse(collect[j][0].ToString());
            item.intBuildType = int.Parse(collect[j][2].ToString());
            item.intTimeBuild = int.Parse(collect[j][3].ToString());
            item.intTimeDemolition = int.Parse(collect[j][4].ToString());
            item.numPrice = int.Parse(collect[j][5].ToString());
            item.intMaintain = int.Parse(collect[j][6].ToString());
            item.strModelName = collect[j][7].ToString();
            dataTableRoot.listTable.Add(item);
        }
        Debug.Log(str);

        Debug.Log("Finish Build Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableBuildLanguage(DataRowCollection collect)
    {
        JsonValue.DataTableBuildNameBase dataTableRoot = new JsonValue.DataTableBuildNameBase();
        dataTableRoot.listTable = new List<JsonValue.DataTableBuildNameItem>();
        string str = null;
        int intCount = System.Enum.GetValues(typeof(ManagerValue.EnumLanguageType)).Length;
        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataTableBuildNameItem item = new JsonValue.DataTableBuildNameItem();
            item.intBuildID = int.Parse(collect[j][0].ToString());
            item.strNames = new string[intCount];
            for (int i = 0; i < intCount; i++)
            {
                item.strNames[i] = collect[j][i + 1].ToString();
            }
            dataTableRoot.listTable.Add(item);
        }
        Debug.Log(str);

        Debug.Log("Finish TableBuildLanguage Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableBuildCompound(DataRowCollection collect)
    {
        JsonValue.DataTableBuildCompoundBase dataTableRoot = new JsonValue.DataTableBuildCompoundBase();
        dataTableRoot.listItem = new List<JsonValue.DataTableBuildCompoundItem>();

        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataTableBuildCompoundItem item = new JsonValue.DataTableBuildCompoundItem();
            if (collect[j][0] == null || collect[j][0].ToString() == "")
            {
                break;
            }
            item.intbuildID = int.Parse(collect[j][0].ToString());
            if (item.intbuildID == -1)
            {
                continue;
            }
            item.intCompundID = int.Parse(collect[j][2].ToString());
            dataTableRoot.listItem.Add(item);
        }

        Debug.Log("Finish BuildProduct Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableEmployee(DataRowCollection collect)
    {
        JsonValue.DataTableEmployeeBase dataTableRoot = new JsonValue.DataTableEmployeeBase();
        dataTableRoot.listItem = new List<JsonValue.DataTableEmployeeItem>();

        for (int i = 1; i < collect.Count; i++)
        {
            JsonValue.DataTableEmployeeItem item = new JsonValue.DataTableEmployeeItem();
            item.intEmployeeID = int.Parse(collect[i][0].ToString());
            item.strEmployeeEnglish = collect[i][1].ToString();
            item.strEmployeeChina = collect[i][2].ToString();

            item.intCombatType = int.Parse(collect[i][3].ToString());
            item.intCombatTypeRank = int.Parse(collect[i][4].ToString());
            item.intStrengt = int.Parse(collect[i][5].ToString());
            item.intAgility = int.Parse(collect[i][6].ToString());
            item.intIntellect = int.Parse(collect[i][7].ToString());
            item.intStamina = int.Parse(collect[i][8].ToString());
            item.intVersatility = int.Parse(collect[i][9].ToString());
            item.intHP = int.Parse(collect[i][10].ToString());
            item.intATK = int.Parse(collect[i][11].ToString());
            item.intMP = int.Parse(collect[i][12].ToString());
            item.intSpeed = int.Parse(collect[i][13].ToString());

            dataTableRoot.listItem.Add(item);
        }
        Debug.Log("Finish Employee Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableEnemy(DataRowCollection collect)
    {
        JsonValue.DataTableEnemyBase dataTableRoot = new JsonValue.DataTableEnemyBase();
        dataTableRoot.listEnemyItems = new List<JsonValue.DataTableEnemyItem>();
        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataTableEnemyItem item = new JsonValue.DataTableEnemyItem();
            item.intEnemyID = int.Parse(collect[j][0].ToString());
            item.intCombatType = int.Parse(collect[j][6].ToString());
            item.intEnemyRank = int.Parse(collect[j][7].ToString());
            item.intHP = int.Parse(collect[j][8].ToString());
            item.intAttack = int.Parse(collect[j][9].ToString());
            item.intMagic = int.Parse(collect[j][10].ToString());

            item.strNameChina = collect[j][2].ToString();
            item.strNameEnglish = collect[j][1].ToString();
            item.strExplainChina = collect[j][3].ToString();
            item.strExplainEnglish = collect[j][4].ToString();
            item.strModelName = collect[j][5].ToString();
            dataTableRoot.listEnemyItems.Add(item);
        }
        Debug.Log("Finish TableEnemy Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableEnemyTeam(DataRowCollection collect)
    {
        JsonValue.DataTableEnemyTeamBase dataTableRoot = new JsonValue.DataTableEnemyTeamBase();
        dataTableRoot.listEnemyTeamItems = new List<JsonValue.DataTableEnemyTeamItem>();
        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataTableEnemyTeamItem item = new JsonValue.DataTableEnemyTeamItem();
            item.intEnemyTeamID = int.Parse(collect[j][0].ToString());
            item.intEnemyCount = int.Parse(collect[j][1].ToString());
            item.intEnemyIDs = new int[item.intEnemyCount];
            item.intCombatTypes = new int[item.intEnemyCount];
            item.intCombatTypeRanks = new int[item.intEnemyCount];
            item.strEnemySkills = new string[item.intEnemyCount];
            item.strEnemySkillRanks = new string[item.intEnemyCount];
            for (int k = 0; k < item.intEnemyIDs.Length; k++)
            {
                item.intEnemyIDs[k] = int.Parse(collect[j][2 + k * 5].ToString());
                item.intCombatTypes[k] = int.Parse(collect[j][3 + k * 5].ToString());
                item.intCombatTypeRanks[k] = int.Parse(collect[j][4 + k * 5].ToString());
                item.strEnemySkills[k] = collect[j][5 + k * 5].ToString();
                item.strEnemySkillRanks[k] = collect[j][6 + k * 5].ToString();
            }
            dataTableRoot.listEnemyTeamItems.Add(item);
        }
        Debug.Log("Finish EnemyTeam Table");
        return JsonUtility.ToJson(dataTableRoot);
    }

    public string TableGameDungeon(DataRowCollection collect)
    {
        JsonValue.DataGameDungeonBase dataTableRoot = new JsonValue.DataGameDungeonBase();
        dataTableRoot.listItem = new List<JsonValue.DataGameDungeonItem>();
        string str = null;
        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataGameDungeonItem item = new JsonValue.DataGameDungeonItem();
            item.intGameDungeonID = int.Parse(collect[j][0].ToString());
            item.strAwark = collect[j][2].ToString();
            item.strDungeonModel = collect[j][3].ToString();
            item.strDungeonEffect = collect[j][4].ToString();
            int intTemp = int.Parse(collect[j][5].ToString());
            item.taskPoints = new JsonValue.DataGameDungeonItemTaskPoint[intTemp];
            for (int i = 0; i < intTemp; i++)
            {
                item.taskPoints[i] = new JsonValue.DataGameDungeonItemTaskPoint();
                item.taskPoints[i].strEffect = collect[j][6 + i * 4].ToString();
                item.taskPoints[i].strModel = collect[j][7 + i * 4].ToString();
                string[] strPoint = collect[j][9 + i * 4].ToString().Split('_');
                item.taskPoints[i].intTeamIDs = new int[strPoint.Length];
                for (int m = 0; m < strPoint.Length; m++)
                {
                    item.taskPoints[i].intTeamIDs[m] = int.Parse(strPoint[m]);
                }
                string[] strAwards = collect[j][8 + i * 4].ToString().Split('=');
                item.taskPoints[i].intKnaspackType = new int[strAwards.Length];
                item.taskPoints[i].intAwardRanks = new int[strAwards.Length];
                item.taskPoints[i].intAwardIDs = new int[strAwards.Length];
                item.taskPoints[i].intAwardCounts = new int[strAwards.Length];
                for (int m = 0; m < strAwards.Length; m++)
                {
                    string[] strAward = strAwards[m].Split('_');
                    item.taskPoints[i].intKnaspackType[m] = int.Parse(strAward[0]);
                    item.taskPoints[i].intAwardRanks[m] = int.Parse(strAward[1]);
                    item.taskPoints[i].intAwardIDs[m] = int.Parse(strAward[2]);
                    item.taskPoints[i].intAwardCounts[m] = int.Parse(strAward[3]);
                }
            }
            dataTableRoot.listItem.Add(item);
        }
        Debug.Log(str);
        Debug.Log("Finish GameDungeon Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableGameDungeonLanguage(DataRowCollection collect)
    {
        JsonValue.DataGameDungeonNameBase dataTableRoot = new JsonValue.DataGameDungeonNameBase();
        dataTableRoot.listItem = new List<JsonValue.DataGameDungeonItemName>();
        string str = null;
        int intCount = System.Enum.GetValues(typeof(ManagerValue.EnumLanguageType)).Length;
        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataGameDungeonItemName item = new JsonValue.DataGameDungeonItemName();
            item.intID = int.Parse(collect[j][0].ToString());
            item.strNames = new string[intCount];
            item.strExplains = new string[intCount];
            for (int i = 0; i < intCount; i++)
            {
                item.strNames[i] = collect[j][i + 1].ToString();
            }
            dataTableRoot.listItem.Add(item);
        }
        Debug.Log(str);
        Debug.Log("Finish TableGameDungeonLanguage Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableEquipment(DataRowCollection collect)
    {
        JsonValue.TableEquipmentBase dataTableRoot = new JsonValue.TableEquipmentBase();
        dataTableRoot.listItem = new List<JsonValue.TableEquipmentItem>();
        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.TableEquipmentItem item = new JsonValue.TableEquipmentItem();
            item.intEquipmentID = int.Parse(collect[j][0].ToString());
            item.strNameEnglish = collect[j][1].ToString();
            item.strNameChina = collect[j][2].ToString();
            item.intPrice = int.Parse(collect[j][3].ToString());
            item.intKnapsackGridLimitCount = int.Parse(collect[j][4].ToString());
            item.intKnaspackStockType = int.Parse(collect[j][5].ToString());
            item.strRandom = collect[j][6].ToString();
            item.intOrder = int.Parse(collect[j][7].ToString());
            item.strICON = collect[j][8].ToString();
            item.intHP = int.Parse(collect[j][9].ToString());
            item.intATK = int.Parse(collect[j][10].ToString());
            item.intMP = int.Parse(collect[j][11].ToString());
            item.intSpeed = int.Parse(collect[j][12].ToString());
            item.intCritical = int.Parse(collect[j][13].ToString());
            item.intEnd = int.Parse(collect[j][14].ToString());
            item.strContentChina = collect[j][15].ToString();
            item.strContentEnglish = collect[j][16].ToString();

            dataTableRoot.listItem.Add(item);
        }
        Debug.Log("Finish Equipment Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableProduct(DataRowCollection collect)
    {
        table.dicProduct = new Dictionary<int, JsonValue.DataTableBackPackItem>();

        JsonValue.DataTableBackPackBase dataTableRoot = new JsonValue.DataTableBackPackBase();
        dataTableRoot.listItem = new List<JsonValue.DataTableBackPackItem>();
        string str = null;
        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataTableBackPackItem item = new JsonValue.DataTableBackPackItem();
            if (collect[j][0] == null || collect[j][0].ToString() == "")
            {
                break;
            }
            item.intProductID = int.Parse(collect[j][0].ToString());
            if (item.intProductID == -1)
            {
                continue;
            }
            item.intKnapsackGridLimitCount = int.Parse(collect[j][2].ToString());
            item.intStockMax = int.Parse(collect[j][3].ToString());
            item.intProductType = int.Parse(collect[j][4].ToString());
            item.strIconName = collect[j][5].ToString();
            item.strInfoChina = collect[j][6].ToString();
            item.strInfoEnglish = collect[j][7].ToString();

            dataTableRoot.listItem.Add(item);

            table.dicProduct.Add(item.intProductID, item);
        }
        Debug.Log(str);
        Debug.Log("Finish Product Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableProductLanguage(DataRowCollection collect)
    {

        JsonValue.DataTableBackPackNameBase dataTableRoot = new JsonValue.DataTableBackPackNameBase();
        dataTableRoot.listItem = new List<JsonValue.DataTableBackPackItemName>();
        string str = null;
        int intCount = System.Enum.GetValues(typeof(ManagerValue.EnumLanguageType)).Length;
        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataTableBackPackItemName item = new JsonValue.DataTableBackPackItemName();
            item.intID = int.Parse(collect[j][0].ToString());
            item.strNames = new string[intCount];
            item.strNamePlurals = new string[intCount];
            for (int i = 0; i < intCount; i++)
            {
                item.strNames[i] = collect[j][i * 2 + 1].ToString();
                item.strNamePlurals[i] = collect[j][i * 2 + 2].ToString();
            }

            dataTableRoot.listItem.Add(item);
        }
        Debug.Log(str);
        Debug.Log("Finish Product Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableProductCompound(DataRowCollection collect)
    {
        JsonValue.DataTableCompoundBase dataTableRoot = new JsonValue.DataTableCompoundBase();
        dataTableRoot.listCompound = new List<JsonValue.DataTableCompoundItem>();

        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataTableCompoundItem item = new JsonValue.DataTableCompoundItem();
            if (collect[j][0] == null || collect[j][0].ToString() == "")
            {
                break;
            }
            item.intCompoundID = int.Parse(collect[j][0].ToString());
            if (item.intCompoundID == -1)
            {
                continue;
            }

            item.intProductID = int.Parse(collect[j][1].ToString());
            item.intRipeDay = int.Parse(collect[j][3].ToString());
            item.intProductCount = int.Parse(collect[j][4].ToString());
            item.intPrice = int.Parse(collect[j][5].ToString());
            item.intCoinConsume = int.Parse(collect[j][7].ToString());
            item.intProductKind = int.Parse(collect[j][8].ToString());
            item.intPorductIDStuff = new int[item.intProductKind];
            item.intPorductIDnum = new int[item.intProductKind];
            int tempIndex = 9;
            for (int k = 0; k < item.intProductKind; k++)
            {
                item.intPorductIDStuff[k] = int.Parse(collect[j][tempIndex].ToString());
                item.intPorductIDnum[k] = int.Parse(collect[j][tempIndex + 2].ToString());
                tempIndex += 3;
            }

            dataTableRoot.listCompound.Add(item);
        }
        Debug.Log("Finish ProductCompound Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableSkill(DataRowCollection collect)
    {
        JsonValue.DataSkillBase dataTableRoot = new JsonValue.DataSkillBase();
        dataTableRoot.listItems = new List<JsonValue.DataSkillItem>();

        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataSkillItem item = new JsonValue.DataSkillItem();
            item.intIndex = j;
            item.intSkillID = int.Parse(collect[j][0].ToString());
            item.strNameEnglish = collect[j][1].ToString();
            item.strNameChina = collect[j][2].ToString();
            item.intSkillType = int.Parse(collect[j][3].ToString());
            item.intCombatType = int.Parse(collect[j][4].ToString());
            item.strICON = collect[j][5].ToString();
            item.strNameEffect = collect[j][6].ToString();
            item.intValue = int.Parse(collect[j][7].ToString());
            item.intRoleCount = int.Parse(collect[j][8].ToString());
            item.intRandom = int.Parse(collect[j][9].ToString());
            dataTableRoot.listItems.Add(item);
        }
        Debug.Log("Finish Skill Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableTaskProduct(DataRowCollection collect)
    {
        JsonValue.DataTaskProductBase dataTableRoot = new JsonValue.DataTaskProductBase();
        dataTableRoot.listItem = new List<JsonValue.DataTaskProductItem>();

        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataTaskProductItem item = new JsonValue.DataTaskProductItem();
            item.intID = int.Parse(collect[j][0].ToString());
            if (item.intID == -1)
            {
                continue;
            }
            string[] strTemp = collect[j][2].ToString().Split('=');
            item.awards = new JsonValue.DataTaskProductGoods[strTemp.Length];
            for (int i = 0; i < strTemp.Length; i++)
            {
                string[] strSub = strTemp[i].Split('_');
                if (strSub.Length <= 1)
                {
                    item.awards = new JsonValue.DataTaskProductGoods[] { };
                    break;
                }
                item.awards[i] = new JsonValue.DataTaskProductGoods();
                item.awards[i].intType = int.Parse(strSub[0]);
                item.awards[i].intID = int.Parse(strSub[1]);
                item.awards[i].intCount = int.Parse(strSub[2]);
            }
            item.intTaskRank = int.Parse(collect[j][3].ToString());
            item.goods = new JsonValue.DataTaskProductGoods[int.Parse(collect[j][4].ToString())];
            for (int i = 0; i < item.goods.Length; i++)
            {
                item.goods[i] = new JsonValue.DataTaskProductGoods();
                item.goods[i].intType = int.Parse(collect[j][i * 4 + 5].ToString());
                item.goods[i].intID = int.Parse(collect[j][i * 4 + 6].ToString());
                item.goods[i].intCount = int.Parse(collect[j][i * 4 + 8].ToString());
            }

            dataTableRoot.listItem.Add(item);
        }
        Debug.Log("Finish TaskProduct Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableTaskProductLanguage(DataRowCollection collect)
    {
        JsonValue.DataTaskProductNameBase dataTableRoot = new JsonValue.DataTaskProductNameBase();
        dataTableRoot.listItem = new List<JsonValue.DataTaskProductNameItem>();
        int intCount = System.Enum.GetValues(typeof(ManagerValue.EnumLanguageType)).Length;
        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataTaskProductNameItem item = new JsonValue.DataTaskProductNameItem();
            item.intID = int.Parse(collect[j][0].ToString());
            item.strNames = new string[intCount];
            item.strExplains = new string[intCount];
            for (int i = 0; i < intCount; i++)
            {
                item.strNames[i] = collect[j][i * 2 + 1].ToString();
                item.strExplains[i] = collect[j][i * 2 + 2].ToString();
            }
            dataTableRoot.listItem.Add(item);
        }
        Debug.Log("Finish TableTaskProductLanguage Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableTaskDungeon(DataRowCollection collect)
    {
        JsonValue.DataTaskDungeonBase dataTableRoot = new JsonValue.DataTaskDungeonBase();
        dataTableRoot.listItem = new List<JsonValue.DataTaskDungeonItem>();

        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataTaskDungeonItem item = new JsonValue.DataTaskDungeonItem();
            item.intID = int.Parse(collect[j][0].ToString());
            if (item.intID == -1)
            {
                continue;
            }

            item.intTaskRank = int.Parse(collect[j][2].ToString());
            item.awards = new JsonValue.DataTaskDungeonAward[int.Parse(collect[j][3].ToString())];
            for (int i = 0; i < item.awards.Length; i++)
            {
                item.awards[i] = new JsonValue.DataTaskDungeonAward();
                item.awards[i].intType = int.Parse(collect[j][i * 4 + 4].ToString());
                item.awards[i].intID = int.Parse(collect[j][i * 4 + 5].ToString());
                item.awards[i].intCount = int.Parse(collect[j][i * 4 + 7].ToString());
            }
            dataTableRoot.listItem.Add(item);
        }
        Debug.Log("Finish TableTaskDungeon Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableTaskDungeonLanguage(DataRowCollection collect)
    {
        JsonValue.DataTaskDuneonNameBase dataTableRoot = new JsonValue.DataTaskDuneonNameBase();
        dataTableRoot.listItem = new List<JsonValue.DataTaskDuneonNameItem>();
        int intCount = System.Enum.GetValues(typeof(ManagerValue.EnumLanguageType)).Length;
        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataTaskDuneonNameItem item = new JsonValue.DataTaskDuneonNameItem();
            item.intID = int.Parse(collect[j][0].ToString());
            item.strNames = new string[intCount];
            item.strExplains = new string[intCount];
            for (int i = 0; i < intCount; i++)
            {
                item.strNames[i] = collect[j][i * 2 + 1].ToString();
                item.strExplains[i] = collect[j][i * 2 + 2].ToString();
            }
            dataTableRoot.listItem.Add(item);
        }
        Debug.Log("Finish TableTaskDungeonLanguage Table");
        return JsonUtility.ToJson(dataTableRoot);
    }

    public string TableTranslation(DataRowCollection collect)
    {
        JsonValue.DataTranslationBase dataTableRoot = new JsonValue.DataTranslationBase();
        dataTableRoot.listItem = new List<JsonValue.DataTranslationItem>();

        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataTranslationItem item = new JsonValue.DataTranslationItem();
            item.intID = int.Parse(collect[j][0].ToString());
            item.strChina = collect[j][1].ToString();
            item.strEnglish = collect[j][2].ToString();
            dataTableRoot.listItem.Add(item);
        }
        Debug.Log("Finish ProductCompound Table");
        return JsonUtility.ToJson(dataTableRoot);
    }

    public string TableLanguageWords(DataRowCollection collect)
    {
        JsonValue.DataLanguageWordsBase dataTableRoot = new JsonValue.DataLanguageWordsBase();
        dataTableRoot.listItem = new List<JsonValue.DataLanguageWordsItem>();
        Dictionary<string, EnumLanguageWords> dicTemp = new Dictionary<string, EnumLanguageWords>();
        EnumLanguageWords[] languageWords = (EnumLanguageWords[])Enum.GetValues(typeof(EnumLanguageWords));
        for (int i = 0; i < languageWords.Length; i++)
        {
            dicTemp.Add(languageWords[i].ToString(), languageWords[i]);
        }
        string str = "";
        int intCount = System.Enum.GetValues(typeof(ManagerValue.EnumLanguageType)).Length;
        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataLanguageWordsItem item = new JsonValue.DataLanguageWordsItem();
            string strTemp = collect[j][1].ToString();
            str += collect[j][3].ToString() + "\n";
            //str += strTemp + ",//" + collect[j][3].ToString() + "\n";
            if (!dicTemp.ContainsKey(strTemp))
            {
                continue;
            }
            item.intID = int.Parse(collect[j][0].ToString());
            item.enumWord = dicTemp[strTemp];
            item.strNames = new string[intCount];
            for (int i = 0; i < intCount; i++)
            {
                item.strNames[i] = collect[j][i + 2].ToString();
            }
            dataTableRoot.listItem.Add(item);
        }
        Debug.Log(str);
        Debug.Log("Finish TableLanguageWords Table");
        return JsonUtility.ToJson(dataTableRoot);
    }

    public string TableLanguageStatement(DataRowCollection collect)
    {
        JsonValue.DataLanguageStatementBase dataTableRoot = new JsonValue.DataLanguageStatementBase();
        dataTableRoot.listItem = new List<JsonValue.DataLanguageStatementItem>();
        Dictionary<string, EnumLanguageStatement> dicTemp = new Dictionary<string, EnumLanguageStatement>();
        EnumLanguageStatement[] languageWords = (EnumLanguageStatement[])Enum.GetValues(typeof(EnumLanguageStatement));
        for (int i = 0; i < languageWords.Length; i++)
        {
            dicTemp.Add(languageWords[i].ToString(), languageWords[i]);
        }
        string str = "";
        int intCount = System.Enum.GetValues(typeof(ManagerValue.EnumLanguageType)).Length;
        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataLanguageStatementItem item = new JsonValue.DataLanguageStatementItem();
            string strTemp = collect[j][1].ToString();
            //str += collect[j][1].ToString() + ",//" + collect[j][10].ToString() + "\n";
            str += collect[j][28].ToString() + " \n";
            if (!dicTemp.ContainsKey(strTemp))
            {
                continue;
            }
            item.intID = int.Parse(collect[j][0].ToString());
            item.enumStatment = dicTemp[strTemp];
            item.listName = new List<JsonValue.DataLanguageStatmentItemSize>();
            for (int i = 0; i < intCount; i++)
            {
                item.listName.Add(new JsonValue.DataLanguageStatmentItemSize());
                item.listName[i].strName = collect[j][i * 2 + 2].ToString();
                string[] strTemps = collect[j][i * 2 + 3].ToString().Split('_');
                if (strTemps.Length == 1 && int.Parse(strTemps[0]) == -1)
                {
                    continue;
                }
                item.listName[i].intNameSizes = new int[strTemps.Length];
                for (int m = 0; m < item.listName[i].intNameSizes.Length; m++)
                {
                    item.listName[i].intNameSizes[m] = int.Parse(strTemps[m]);
                }
            }
            dataTableRoot.listItem.Add(item);
        }
        Debug.Log(str);
        Debug.Log("Finish TableLanguageStatement Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
    public string TableLanguageStory(DataRowCollection collect)
    {
        JsonValue.DataLanguageStoryBase dataTableRoot = new JsonValue.DataLanguageStoryBase();
        dataTableRoot.listItem = new List<JsonValue.DataLanguageStoryItem>();
        Dictionary<string, EnumLanguageStory> dicTemp = new Dictionary<string, EnumLanguageStory>();
        EnumLanguageStory[] languageWords = (EnumLanguageStory[])Enum.GetValues(typeof(EnumLanguageStory));
        for (int i = 0; i < languageWords.Length; i++)
        {
            dicTemp.Add(languageWords[i].ToString(), languageWords[i]);
        }
        string str = "";
        int intCount = System.Enum.GetValues(typeof(ManagerValue.EnumLanguageType)).Length;
        for (int j = 1; j < collect.Count; j++)
        {
            JsonValue.DataLanguageStoryItem item = new JsonValue.DataLanguageStoryItem();
            string strTemp = collect[j][1].ToString();
            //str += collect[j][1].ToString()+ ",\n";
            str += strTemp + ",//" + dicTemp[strTemp] + "\n";
            if (!dicTemp.ContainsKey(strTemp))
            {
                continue;
            }
            item.intID = int.Parse(collect[j][0].ToString());
            item.enumStory = dicTemp[strTemp];
            item.strNames = new string[intCount];
            for (int i = 0; i < intCount; i++)
            {
                item.strNames[i] = collect[j][i + 2].ToString();
            }
            dataTableRoot.listItem.Add(item);
        }
        Debug.Log(str);
        Debug.Log("Finish TableLanguageStory Table");
        return JsonUtility.ToJson(dataTableRoot);
    }
}
