using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SaveGameWrite
{
    JsonSaveGame.GameRoot saveGame = new JsonSaveGame.GameRoot();
    List<JsonSaveGame.GameRoot> listSaveGame = new List<JsonSaveGame.GameRoot>();
    ViewHintBar.MessageHintBar bar = new ViewHintBar.MessageHintBar();
    public void SaveGame()
    {
        if (!File.Exists(ManagerValue.GetSavePath))
        {
            Directory.CreateDirectory(ManagerValue.GetSavePath);
        }
        string str = "";
        for (int i = 0; i < 5; i++)
        {
            string strTemp = "";// File.ReadAllText(ManagerValue.strSavePath + i);
                                //StreamReader reader = new StreamReader(ManagerValue.strSavePath + i, Encoding.UTF8);
                                //while ((str = reader.ReadLine()) != null)
                                //{
                                //    strTemp += str;
                                //}
                                //reader.Close();
            if (!File.Exists(ManagerValue.GetSavePath + i))
            {
                FileStream stream = File.Create(ManagerValue.GetSavePath + i);
                stream.Close();
            }
            byte[] b = File.ReadAllBytes(ManagerValue.GetSavePath + i);
            if (b.Length != 0)
            {
                strTemp = Tools.GetFileString(b);
            }

            listSaveGame.Add(JsonUtility.FromJson<JsonSaveGame.GameRoot>(strTemp));
            if (listSaveGame[i] != null
            && ManagerValue.saveGame != null
            && listSaveGame[i].intSaveGameIndex == ManagerValue.saveGame.intSaveGameIndex)
            {
                bar.strHintBar = ManagerLanguage.Instance.GetWord(EnumLanguageWords.GameSaved);//"游戏已经保存";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, bar);

                saveGame.intSaveGameIndex = i;
                SaveGame(saveGame);
                string strData = JsonUtility.ToJson(saveGame);
                //File.WriteAllText(ManagerValue.strSavePath + i, strData);
                //StreamWriter writer = new StreamWriter(ManagerValue.strSavePath + i, false, Encoding.UTF8);
                //writer.Write(strData);
                //writer.Flush();
                //writer.Close();
                //File.WriteAllBytes(ManagerValue.strSavePath + i, Tools.GetFileByte(strData));
                //Debug.Log(ManagerValue.strSavePath);
                //Debug.Log(Application.persistentDataPath);
                using (FileStream fs = new FileStream(ManagerValue.GetSavePath + i, FileMode.Create))
                {
                    byte[] buffer = Tools.GetFileByte(strData);
                    fs.Write(buffer, 0, buffer.Length);
                }

                return;
            }
        }
        bool boo = true;
        for (int i = 0; i < 5; i++)
        {
            if (listSaveGame[i] == null)
            {
                bar.strHintBar = ManagerLanguage.Instance.GetWord(EnumLanguageWords.GameSaved); //"游戏已经保存";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, bar);

                boo = false;
                saveGame.intSaveGameIndex = i;
                SaveGame(saveGame);
                string strData = JsonUtility.ToJson(saveGame);
                //File.WriteAllText(ManagerValue.strSavePath + i, strData);
                using (FileStream fs = new FileStream(ManagerValue.GetSavePath + i, FileMode.Create))
                {
                    byte[] buffer = Tools.GetFileByte(strData);
                    fs.Write(buffer, 0, buffer.Length);
                }
                return;
            }
        }
        if (boo)
        {
            bar.strHintBar = "存档已满,请删除一个存档,再点击保存";
            ManagerView.Instance.Show(EnumView.ViewHintBar);
            ManagerView.Instance.SetData(EnumView.ViewHintBar, bar);
        }
    }

    void SaveGame(JsonSaveGame.GameRoot root)
    {
        root.intGameMode = (int)ManagerValue.enumGameMode;
        root.intCoin = UserValue.Instance.GetCoin;
        root.longEndYearCoin = ManagerValue.longEndYearCoin;
        root.intNPCSellProductCount = ManagerValue.intNPCSellProductCount;
        root.intHindCount = ManagerValue.intGroundCount;
        root.strGameNickName = UserValue.Instance.GetNickname;
        //保存日期
        root.intDate = new int[] { ManagerValue.intDay, ManagerValue.intMonth, ManagerValue.intYear, ManagerValue.intTotalDay };
        root.intTaskRank = ManagerValue.intTaskRank;
        SaveGround(root);
        SaveStock(root);
        SaveBackpack(root);

        SaveMail(root);
        SaveEmployee(root);

        root.dungeonMap = new JsonSaveGame.DungeonMap();
        Dictionary<int, PropertiesDungeon> dicDungeon = UserValue.Instance.dicDungeon;
        root.dungeonMap.dungeons = new JsonSaveGame.DungeonItem[dicDungeon.Count];
        int intIndex = 0;
        foreach (PropertiesDungeon temp in dicDungeon.Values)
        {
            root.dungeonMap.dungeons[intIndex] = new JsonSaveGame.DungeonItem();
            root.dungeonMap.dungeons[intIndex].booFinishDungeon = temp.booFinishDungeon;
            root.dungeonMap.dungeons[intIndex].intDungeonID = temp.intDungeonID;
            root.dungeonMap.dungeons[intIndex].points = new JsonSaveGame.DungeonPoint[temp.points.Length];
            for (int i = 0; i < temp.points.Length; i++)
            {
                root.dungeonMap.dungeons[intIndex].points[i] = new JsonSaveGame.DungeonPoint();
                root.dungeonMap.dungeons[intIndex].points[i].intEnemyPointIndex = temp.points[i].intPointIndex;
                root.dungeonMap.dungeons[intIndex].points[i].intStar = temp.points[i].intStar;
                root.dungeonMap.dungeons[intIndex].points[i].intWinCount = temp.points[i].intWinCount;
                root.dungeonMap.dungeons[intIndex].points[i].teams = new JsonSaveGame.DungeonTeam[temp.points[i].teams.Length];
                for (int j = 0; j < temp.points[i].teams.Length; j++)
                {
                    root.dungeonMap.dungeons[intIndex].points[i].teams[j] = new JsonSaveGame.DungeonTeam();
                    root.dungeonMap.dungeons[intIndex].points[i].teams[j].intIDs = temp.points[i].teams[j].intIDs;
                }
            }

            intIndex++;
        }
        SaveTask(root);
    }

    void SaveGround(JsonSaveGame.GameRoot root)
    {
        //保存土地
        root.grounds = new JsonSaveGame.Ground[UserValue.Instance.GetDicGround.Count];
        int intIndexTemp = 0;
        Dictionary<int, PropertiesGround> buildGround = UserValue.Instance.GetDicGround;
        foreach (PropertiesGround temp in buildGround.Values)
        {
            root.grounds[intIndexTemp] = new JsonSaveGame.Ground();
            root.grounds[intIndexTemp].intIndex = temp.GetIndex;
            root.grounds[intIndexTemp].intGround = temp.GetIntGround;
            root.grounds[intIndexTemp].intState = (int)temp.GetState;
            root.grounds[intIndexTemp].intPrice = temp.GetPrice;
            root.grounds[intIndexTemp].intObstacleMat = temp.intObstacleMat;
            root.grounds[intIndexTemp].intBuildID = temp.intBuildID;
            root.grounds[intIndexTemp].floX = temp.vecPosition.x;
            root.grounds[intIndexTemp].floY = temp.vecPosition.y;
            root.grounds[intIndexTemp].floZ = temp.vecPosition.z;
            if (temp.buildBase != null)
            {
                root.grounds[intIndexTemp].strBuild = temp.buildBase.GameSaveData();
            }
            intIndexTemp++;
        }
    }

    void SaveStock(JsonSaveGame.GameRoot root)
    {
        //保存库存
        FarmClass.StockProduction[] itemStocks = UserValue.Instance.GetStockProductionOrder();
        Dictionary<int, FarmClass.StockCount> itemCount = UserValue.Instance.GetStockCountAll();
        root.stock = new JsonSaveGame.StockItem[itemStocks.Length];
        for (int i = 0; i < itemStocks.Length; i++)
        {
            JsonSaveGame.StockItem item = new JsonSaveGame.StockItem();
            item.intProductID = itemStocks[i].intProductID;
            if (itemCount.ContainsKey(item.intProductID))
            {
                item.intStockCount = itemCount[item.intProductID].intStockCount;
            }
            else
            {
                item.intStockCount = 0;
            }
            root.stock[i] = item;
        }
    }

    void SaveBackpack(JsonSaveGame.GameRoot root)
    {
        //保存背包
        UserValue.EnumKnapsackType[] enumKnapsacks = new UserValue.EnumKnapsackType[] { UserValue.EnumKnapsackType.Backpack_1, UserValue.EnumKnapsackType.Backpack_2 };
        root.listBackpackGrid = new List<JsonSaveGame.GridBase>();
        for (int i = 0; i < enumKnapsacks.Length; i++)
        {
            List<BackpackGrid> listTemp = new List<BackpackGrid>();
            BackpackGrid[] grids = UserValue.Instance.GetKnapsackGrids(enumKnapsacks[i]);
            for (int j = 0; j < grids.Length; j++)
            {
                if (grids[j].enumStockType != EnumKnapsackStockType.None)
                {
                    listTemp.Add(grids[j]);
                }
            }
            root.listBackpackGrid.Add(new JsonSaveGame.GridBase());
            root.listBackpackGrid[i].grids = new JsonSaveGame.Grid[listTemp.Count];
            for (int j = 0; j < listTemp.Count; j++)
            {
                JsonSaveGame.Grid item = new JsonSaveGame.Grid();
                item.intIndex = listTemp[j].intIndex;
                item.intGridItem = (int)listTemp[j].enumStockType;
                item.intID = listTemp[j].intID;
                item.intRank = listTemp[j].intRank;
                item.intCount = listTemp[j].intCount;

                if (listTemp[j].enumStockType == EnumKnapsackStockType.Sword
                    || listTemp[j].enumStockType == EnumKnapsackStockType.Bow
                    || listTemp[j].enumStockType == EnumKnapsackStockType.Wand
                    || listTemp[j].enumStockType == EnumKnapsackStockType.Armor
                    || listTemp[j].enumStockType == EnumKnapsackStockType.Shoes)
                {
                    item.equipment = new JsonSaveGame.Equipment();
                    item.equipment.intPrice = listTemp[j].intPrice;
                    item.equipment.intTableID = listTemp[j].intID;
                    item.equipment.value = new JsonSaveGame.EquipmentCombat[listTemp[j].equipment.dicEquipment.Count];
                    int intIndexEquipment = 0;
                    foreach (KeyValuePair<BackpackGrid.EnumEquipmentCombat, int> temp in listTemp[j].equipment.dicEquipment)
                    {
                        item.equipment.value[intIndexEquipment] = new JsonSaveGame.EquipmentCombat();
                        item.equipment.value[intIndexEquipment].intType = (int)temp.Key;
                        item.equipment.value[intIndexEquipment].intValue = temp.Value;
                        intIndexEquipment++;
                    }
                }
                else if (listTemp[j].enumStockType == EnumKnapsackStockType.Farm
                    || listTemp[j].enumStockType == EnumKnapsackStockType.Fasture
                    || listTemp[j].enumStockType == EnumKnapsackStockType.Factory)
                {
                    item.product = new JsonSaveGame.Product();
                    item.product.intPrice = listTemp[j].intPrice;
                }

                root.listBackpackGrid[i].grids[j] = item;
            }
        }
    }

    void SaveMail(JsonSaveGame.GameRoot root)
    {
        //保存邮件
        List<ViewBarTop_ItemMail.Mail> listMail = ManagerValue.listMail;
        root.mails = new JsonSaveGame.Mail[listMail.Count];
        root.intNewMailCount = ManagerValue.intNewMailCount;
        for (int i = 0; i < listMail.Count; i++)
        {
            root.mails[i] = new JsonSaveGame.Mail();
            root.mails[i].booSee = listMail[i].booSee;
            root.mails[i].booGet = listMail[i].booGet;
            root.mails[i].intIndex = listMail[i].intIndex;
            root.mails[i].enumMail = (int)listMail[i].enumMail;
            root.mails[i].strContent = listMail[i].strContent;
            root.mails[i].strIconName = listMail[i].strIconName;

            root.mails[i].gridItems = new int[listMail[i].gridItems.Length];
            root.mails[i].intIndexIDs = new int[listMail[i].intIndexIDs.Length];
            root.mails[i].intRanks = new int[listMail[i].intRanks.Length];
            root.mails[i].intIndexCounts = new int[listMail[i].intIndexCounts.Length];
            for (int j = 0; j < listMail[i].gridItems.Length; j++)
            {
                root.mails[i].gridItems[j] = (int)listMail[i].gridItems[j];
                root.mails[i].intIndexIDs[j] = listMail[i].intIndexIDs[j];
                root.mails[i].intRanks[j] = listMail[i].intRanks[j];
                root.mails[i].intIndexCounts[j] = listMail[i].intIndexCounts[j];
            }
        }
    }

    void SaveEmployee(JsonSaveGame.GameRoot root)
    {
        //保存工人
        for (int i = 0; i < 2; i++)
        {
            Dictionary<int, PropertiesEmployee> dicEmployee = null;
            JsonSaveGame.Employee[] employees = null;
            if (i == 0)
            {
                dicEmployee = UserValue.Instance.GetEmployeeAll();
                employees = root.employee = new JsonSaveGame.Employee[dicEmployee.Count];
            }
            else if (i == 1)
            {
                dicEmployee = UserValue.Instance.GetEmployeeGuestAll();
                employees = root.guest = new JsonSaveGame.Employee[dicEmployee.Count];
            }

            int intIndexEmployee = 0;
            foreach (PropertiesEmployee temp in dicEmployee.Values)
            {
                employees[intIndexEmployee] = new JsonSaveGame.Employee();
                employees[intIndexEmployee].intState = (int)temp.enumState;
                employees[intIndexEmployee].intType = (int)temp.enumIdentification;
                employees[intIndexEmployee].intLocation = (int)temp.enumLocation;
                employees[intIndexEmployee].intCombatType = (int)temp.combatAttackType;
                employees[intIndexEmployee].intTableEmployeeID = temp.intTableEmployeeID;
                employees[intIndexEmployee].intIndexID = temp.intIndexID;
                employees[intIndexEmployee].intRank = temp.intRank;
                employees[intIndexEmployee].intIndexGroundWork = temp.intIndexGroundWork;
                employees[intIndexEmployee].intIndexGround = temp.intIndexGround;
                employees[intIndexEmployee].intMontylyMoney = temp.intMonthlyMoney;
                employees[intIndexEmployee].intEmployeePropertiesKey = new int[temp.dicEmployeeProperties.Count];
                employees[intIndexEmployee].intEmployeePropertiesValue = new int[temp.dicEmployeeProperties.Count];
                int intIndexTemp = 0;
                foreach (KeyValuePair<EnumEmployeeProperties, int> tempSub in temp.dicEmployeeProperties)
                {
                    employees[intIndexEmployee].intEmployeePropertiesKey[intIndexTemp] = (int)tempSub.Key;
                    employees[intIndexEmployee].intEmployeePropertiesValue[intIndexTemp] = tempSub.Value;
                    intIndexTemp++;
                }

                employees[intIndexEmployee].intIndexCombat = temp.intIndexCombat;
                employees[intIndexEmployee].intSkillRanks = temp.intSkillRanks;
                if (temp.proSkills != null)
                {
                    employees[intIndexEmployee].proSkills = new JsonSaveGame.Skill[temp.proSkills.Length];
                    for (int j = 0; j < employees[intIndexEmployee].proSkills.Length; j++)
                    {
                        if (temp.proSkills[j] == null)
                        {
                            continue;
                        }
                        employees[intIndexEmployee].proSkills[j] = new JsonSaveGame.Skill();
                        employees[intIndexEmployee].proSkills[j].intSkillID = temp.proSkills[j].intSkillID;
                        employees[intIndexEmployee].proSkills[j].intSkill = (int)temp.proSkills[j].enumSkill;
                        employees[intIndexEmployee].proSkills[j].intCombatType = (int)temp.proSkills[j].combatType;
                        employees[intIndexEmployee].proSkills[j].intValue = temp.proSkills[j].intValue;
                        employees[intIndexEmployee].proSkills[j].intRoleCount = temp.proSkills[j].intRoleCount;
                        employees[intIndexEmployee].proSkills[j].booRandom = temp.proSkills[j].booRandom;
                    }
                }
                employees[intIndexEmployee].intHP = temp.intHP;
                employees[intIndexEmployee].intATK = temp.intATK;
                employees[intIndexEmployee].intMP = temp.intMP;
                employees[intIndexEmployee].intCombatTypeRank = temp.intCombatTypeRank;
                employees[intIndexEmployee].floSpeed = temp.floSpeed;

                employees[intIndexEmployee].intEquipmentIDs = temp.intEquipmentIDs;
                employees[intIndexEmployee].intAdditioin = (int)temp.enumAddition;
                employees[intIndexEmployee].intAdditionBuildID = temp.intAdditionBuildID;
                employees[intIndexEmployee].intAdditionProductID = temp.intAdditionProductID;
                employees[intIndexEmployee].intAdditionValue = temp.intAdditionValue;

                employees[intIndexEmployee].intEmployeeWorkValue = temp.intEmployWorkValue;

                intIndexEmployee++;
            }
        }
    }

    void SaveTask(JsonSaveGame.GameRoot root)
    {
        List<PropertiesTask> listTask = UserValue.Instance.listTask;
        root.tasks = new JsonSaveGame.TaskItem[listTask.Count];
        for (int i = 0; i < listTask.Count; i++)
        {
            root.tasks[i] = new JsonSaveGame.TaskItem();
            root.tasks[i].intType = (int)listTask[i].enumTask;
            root.tasks[i].intID = listTask[i].intID;
            root.tasks[i].intRank = listTask[i].intRank;
            root.tasks[i].intAwardCoin = listTask[i].intAwardCion;
            root.tasks[i].intPenaltyCoin = listTask[i].intPenaltyCoin;
            root.tasks[i].booDown = listTask[i].booDown;
            root.tasks[i].booFinishs = listTask[i].booFinish;
            if (listTask[i].enumTask == EnumTaskType.Dungeon)
            {
                root.tasks[i].intDungeonID = listTask[i].intDungeonID;
                root.tasks[i].intDungeonIndex = listTask[i].intDungeonIndex;
            }
            else
            {
                root.tasks[i].intGoodsIDs = new int[listTask[i].intGoodsIDs.Length];
                root.tasks[i].intGoodsCounts = new int[listTask[i].intGoodsCounts.Length];
                for (int t = 0; t < listTask[i].intGoodsIDs.Length; t++)
                {
                    root.tasks[i].intGoodsIDs[t] = listTask[i].intGoodsIDs[t];
                    root.tasks[i].intGoodsCounts[t] = listTask[i].intGoodsCounts[t];
                }
            }
        }
    }
}
