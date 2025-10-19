using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 数据读取是以重新建造的方式
/// </summary>
public class SaveGameRead
{
    List<string> listFilePath = new List<string>();
    List<JsonSaveGame.GameRoot> listSaveGame = new List<JsonSaveGame.GameRoot>();

    public JsonSaveGame.GameRoot[] LoadingReadGame()
    {
        if (!Directory.Exists(ManagerValue.GetSavePath))
        {
            Directory.CreateDirectory(ManagerValue.GetSavePath);
        }

        Debug.Log(ManagerValue.GetSavePath);
        for (int i = 0; i < 5; i++)
        {
            if (!File.Exists(ManagerValue.GetSavePath + i))
            {
                //File.CreateText(ManagerValue.strSavePath + i);
                FileStream fs = new FileStream(ManagerValue.GetSavePath + i, FileMode.Create);
                fs.Close();
            }
            listFilePath.Add(ManagerValue.GetSavePath + i);
        }

        listSaveGame.Clear();
        string str = null;
        JsonSaveGame.GameRoot[] jsonRoots = new JsonSaveGame.GameRoot[5];
        for (int i = 0; i < 5; i++)
        {
            string strTemp = "";
            //StreamReader reader = new StreamReader(listFilePath[i], Encoding.UTF8);
            //while ((str = reader.ReadLine()) != null)
            //{
            //    strTemp += str;
            //}
            //reader.Close();
            //for (int i = 0; i < 5; i++)
            //{
            //    if (!File.Exists(ManagerValue.GetSavePath + i))
            //    {
            //        Directory.CreateDirectory(ManagerValue.GetSavePath);
            //        File.Create(ManagerValue.GetSavePath + i);
            //    }
            //}
            if (!File.Exists(ManagerValue.GetSavePath))
            {
                Directory.CreateDirectory(ManagerValue.GetSavePath);
            }
            if (!File.Exists(listFilePath[i]))
            {
                FileStream stram = File.Create(listFilePath[i]);
                stram.Close();
            }
            byte[] b = File.ReadAllBytes(listFilePath[i]);
            if (b.Length != 0)
            {
                strTemp = Tools.GetFileString(b);
            }
            listSaveGame.Add(JsonUtility.FromJson<JsonSaveGame.GameRoot>(strTemp));
            if (listSaveGame[i] == null)
            {
                jsonRoots[i] = null;
            }
            else
            {
                jsonRoots[i] = listSaveGame[i];
            }
        }
        return jsonRoots;
    }

    public void ReadGame(int intIndex)
    {
        JsonSaveGame.GameRoot root = listSaveGame[intIndex];
        ManagerValue.saveGame = root;
        ManagerValue.intGroundCount = root.intHindCount;
        UserValue.Instance.SetCoid = root.intCoin;
        UserValue.Instance.SetCoinMax = ManagerValue.longCoinMax;
        ManagerValue.longEndYearCoin = root.longEndYearCoin;
        ManagerValue.intNPCSellProductCount = root.intNPCSellProductCount;
        UserValue.Instance.SetNickname = root.strGameNickName;
        ManagerValue.enumGameMode = (EnumGameMode)root.intGameMode;
        ManagerValue.intDay = root.intDate[0];
        ManagerValue.intMonth = root.intDate[1];
        ManagerValue.intYear = root.intDate[2];
        ManagerValue.intTotalDay = root.intDate[3];
        ManagerValue.intTaskRank = root.intTaskRank;
        ReadEmployee(root);
        ManagerValue.ActionReadSaveGame(GetGroundData(root.grounds));
        ManagerValue.intNewMailCount = root.intNewMailCount;
        ReadMail(root.mails);
        ReadStock(root.stock);
        ReadBackpack(root.listBackpackGrid);
        Dictionary<int, PropertiesDungeon> dicDungeon = UserValue.Instance.dicDungeon;
        dicDungeon.Clear();
        for (int i = 0; i < root.dungeonMap.dungeons.Length; i++)
        {
            PropertiesDungeon dungeon = new PropertiesDungeon();
            dungeon.booFinishDungeon = root.dungeonMap.dungeons[i].booFinishDungeon;
            dungeon.intDungeonID = root.dungeonMap.dungeons[i].intDungeonID;
            dungeon.points = new PropertiesDungeon.DungeonPoint[root.dungeonMap.dungeons[i].points.Length];
            JsonSaveGame.DungeonItem dun = root.dungeonMap.dungeons[i];
            for (int j = 0; j < dungeon.points.Length; j++)
            {
                dungeon.points[j] = new PropertiesDungeon.DungeonPoint();
                dungeon.points[j].intPointIndex = dun.points[j].intEnemyPointIndex;
                dungeon.points[j].intStar = dun.points[j].intStar;
                dungeon.points[j].intWinCount = dun.points[j].intWinCount;
                dungeon.points[j].teams = new PropertiesDungeon.Team[dun.points[j].teams.Length];
                for (int m = 0; m < dungeon.points[j].teams.Length; m++)
                {
                    dungeon.points[j].teams[m] = new PropertiesDungeon.Team();
                    dungeon.points[j].teams[m].intIDs = dun.points[j].teams[m].intIDs;
                }
            }

            dicDungeon.Add(dungeon.intDungeonID, dungeon);
        }
        ReadTask(root.tasks);
    }

    PropertiesGround[] GetGroundData(JsonSaveGame.Ground[] ground)
    {
        UserValue.Instance.GetAllBuild().Clear();
        UserValue.Instance.GetAllBuildType().Clear();
        UserValue.Instance.GetAllBuildProduceSee().Clear();

        PropertiesGround[] temp = new PropertiesGround[ground.Length];
        for (int i = 0; i < ground.Length; i++)
        {
            temp[i] = new PropertiesGround();
            temp[i].SetIndex = ground[i].intIndex;
            temp[i].SetIntGround = ground[i].intGround;
            temp[i].SetState = (EnumGroudState)ground[i].intState;
            temp[i].SetPrice = ground[i].intPrice;
            temp[i].intObstacleMat = ground[i].intObstacleMat;
            temp[i].intBuildID = ground[i].intBuildID;
            temp[i].vecPosition = new Vector3(ground[i].floX, ground[i].floY, ground[i].floZ);
            temp[i].strReadData = ground[i].strBuild;
        }
        return temp;
    }

    void ReadStock(JsonSaveGame.StockItem[] stock)
    {
        if (UserValue.Instance.GetStockCountAll() != null)
        {
            UserValue.Instance.GetStockCountAll().Clear();
        }
        if (UserValue.Instance.GetStockProduction() != null)
        {
            UserValue.Instance.GetStockProduction().Clear();
        }
        Dictionary<int, FarmClass.StockProduction> dicStockProduction = UserValue.Instance.GetStockProduction();
        Dictionary<int, FarmClass.StockCount> dicStockCount = UserValue.Instance.GetStockCountAll();
        for (int i = 0; i < stock.Length; i++)
        {
            dicStockProduction.Add(stock[i].intProductID, new FarmClass.StockProduction());
            dicStockProduction[stock[i].intProductID].intIndex = i;
            dicStockProduction[stock[i].intProductID].intProductID = stock[i].intProductID;

            dicStockCount.Add(stock[i].intProductID, new FarmClass.StockCount());
            dicStockCount[stock[i].intProductID].intProductID = stock[i].intProductID;
            dicStockCount[stock[i].intProductID].intStockCount = stock[i].intStockCount;
        }
    }

    void ReadBackpack(List<JsonSaveGame.GridBase> listGrids)
    {
        ClearBackpackData();
        UserValue.EnumKnapsackType[] knapsackTypes = new UserValue.EnumKnapsackType[] { UserValue.EnumKnapsackType.Backpack_1, UserValue.EnumKnapsackType.Backpack_2 };
        for (int i = 0; i < knapsackTypes.Length; i++)
        {
            BackpackGrid[] grids = UserValue.Instance.GetKnapsackGrids(knapsackTypes[i]);
            for (int j = 0; j < listGrids[i].grids.Length; j++)
            {
                BackpackGrid item = grids[listGrids[i].grids[j].intIndex];
                item.intIndex = listGrids[i].grids[j].intIndex;
                item.intID = listGrids[i].grids[j].intID;
                item.intRank = listGrids[i].grids[j].intRank;
                item.intCount = listGrids[i].grids[j].intCount;
                item.enumStockType = (EnumKnapsackStockType)listGrids[i].grids[j].intGridItem;

                if (item.enumStockType == EnumKnapsackStockType.Sword
                    || item.enumStockType == EnumKnapsackStockType.Bow
                    || item.enumStockType == EnumKnapsackStockType.Wand
                    || item.enumStockType == EnumKnapsackStockType.Armor
                    || item.enumStockType == EnumKnapsackStockType.Shoes)
                {
                    item.intID = listGrids[i].grids[j].equipment.intTableID;
                    item.intPrice = listGrids[i].grids[j].equipment.intPrice;
                    item.intLimitCount = ManagerCombat.Instance.GetEquipmentItem(item.intID).intKnapsackGridLimitCount;
                    item.equipment.dicEquipment = new Dictionary<BackpackGrid.EnumEquipmentCombat, int>();
                    item.icon = ManagerCombat.Instance.GetEquipmentItem(item.intID).strICON;
                    for (int k = 0; k < listGrids[i].grids[j].equipment.value.Length; k++)
                    {
                        item.equipment.dicEquipment.Add((BackpackGrid.EnumEquipmentCombat)listGrids[i].grids[j].equipment.value[k].intType, listGrids[i].grids[j].equipment.value[k].intValue);
                    }
                }
                else if (item.enumStockType == EnumKnapsackStockType.Farm
                    || item.enumStockType == EnumKnapsackStockType.Fasture
                    || item.enumStockType == EnumKnapsackStockType.Factory)
                {
                    item.intLimitCount = ManagerProduct.Instance.GetProductTableItem(item.intID).intKnapsackGridLimitCount;
                    item.intPrice = listGrids[i].grids[j].product.intPrice;
                    item.icon = ManagerProduct.Instance.GetProductTableItem(item.intID).strIconName;
                }
            }
        }
    }

    void ReadBuild()
    {

    }

    void ReadMail(JsonSaveGame.Mail[] mails)
    {
        ManagerValue.listMail.Clear();
        List<ViewBarTop_ItemMail.Mail> listMail = ManagerValue.listMail;
        for (int i = 0; i < mails.Length; i++)
        {
            listMail.Add(new ViewBarTop_ItemMail.Mail());
            listMail[i].booSee = mails[i].booSee;
            listMail[i].booGet = mails[i].booGet;
            listMail[i].intIndex = mails[i].intIndex;
            listMail[i].enumMail = (ViewBarTop_ItemMail.EnumMail)mails[i].enumMail;
            listMail[i].strContent = mails[i].strContent;
            listMail[i].strIconName = mails[i].strIconName;

            listMail[i].gridItems = new EnumKnapsackStockType[mails[i].gridItems.Length];
            listMail[i].intIndexIDs = new int[mails[i].intIndexIDs.Length];
            listMail[i].intRanks = new int[mails[i].intRanks.Length];
            listMail[i].intIndexCounts = new int[mails[i].intIndexCounts.Length];
            for (int j = 0; j < mails[i].gridItems.Length; j++)
            {
                listMail[i].gridItems[j] = (EnumKnapsackStockType)mails[i].gridItems[j];
                listMail[i].intIndexIDs[j] = mails[i].intIndexIDs[j];
                listMail[i].intRanks[j] = mails[i].intRanks[j];
                listMail[i].intIndexCounts[j] = mails[i].intIndexCounts[j];
            }
        }
    }

    void ReadEmployee(JsonSaveGame.GameRoot root)
    {
        Dictionary<int, PropertiesEmployee> dicEmployee = UserValue.Instance.GetEmployeeAll();
        Dictionary<int, PropertiesEmployee> dicEmployeeGuest = UserValue.Instance.GetEmployeeGuestAll();
        dicEmployee.Clear();
        dicEmployeeGuest.Clear();

        for (int i = 0; i < 2; i++)
        {
            JsonSaveGame.Employee[] employees = null;
            if (i == 0)
            {
                employees = root.employee;
            }
            else if (i == 1)
            {
                employees = root.guest;
            }
            for (int j = 0; j < employees.Length; j++)
            {
                PropertiesEmployee item = UserValue.Instance.AddEmployeeRead();
                item.intIndexID = employees[j].intIndexID;
                if (i == 0)
                {
                    UserValue.Instance.SetEmployeeRead(item);
                }
                else if (i == 1)
                {
                    UserValue.Instance.SetEmployeeGuestRead(item);
                }
                item.enumState = (EnumEmployeeState)employees[j].intState;
                item.enumIdentification = (EnumEmployeeIdentification)employees[j].intType;
                item.enumLocation = (EnumEmployeeLocation)employees[j].intLocation;
                item.combatAttackType = (EnumCombatAttackType)employees[j].intCombatType;
                item.intTableEmployeeID = employees[j].intTableEmployeeID;
                item.intRank = employees[j].intRank;
                item.intIndexGroundWork = employees[j].intIndexGroundWork;
                item.intIndexGround = employees[j].intIndexGround;
                item.intMonthlyMoney = employees[j].intMontylyMoney;
                for (int k = 0; k < employees[j].intEmployeePropertiesKey.Length; k++)
                {
                    int intTemp = employees[j].intEmployeePropertiesValue[k];
                    item.dicEmployeeProperties[(EnumEmployeeProperties)employees[j].intEmployeePropertiesKey[k]] = intTemp;
                }

                item.intIndexCombat = employees[j].intIndexCombat;
                item.intSkillRanks = employees[j].intSkillRanks;
                if (employees[j].proSkills != null)
                {
                    item.proSkills = new PropertiesSkill[employees[j].proSkills.Length];
                    for (int k = 0; k < item.proSkills.Length; k++)
                    {
                        if (employees[j].proSkills[k].intSkillID == 0)
                        {
                            continue;
                        }
                        item.proSkills[k] = new PropertiesSkill();
                        item.proSkills[k].intSkillID = employees[j].proSkills[k].intSkillID;
                        item.proSkills[k].enumSkill = (PropertiesSkill.EnumSkill)employees[j].proSkills[k].intSkill;
                        item.proSkills[k].combatType = (EnumCombatAttackType)employees[j].proSkills[k].intCombatType;
                        item.proSkills[k].intValue = employees[j].proSkills[k].intValue;
                        item.proSkills[k].intRoleCount = employees[j].proSkills[k].intRoleCount;
                        item.proSkills[k].booRandom = employees[j].proSkills[k].booRandom;
                        item.proSkills[k].strICON = ManagerSkill.Instance.GetSkillValue(item.proSkills[k].intSkillID).strICON;
                    }
                }
                item.intHP = employees[j].intHP;
                item.intATK = employees[j].intATK;
                item.intMP = employees[j].intMP;
                item.intCombatTypeRank = employees[j].intCombatTypeRank;
                item.floSpeed = employees[j].floSpeed;

                item.intEquipmentIDs = employees[j].intEquipmentIDs;
                item.enumAddition = (EnumEmployeeAddition)employees[j].intAdditioin;
                item.intAdditionBuildID = employees[j].intAdditionBuildID;
                item.intAdditionProductID = employees[j].intAdditionProductID;
                item.intAdditionValue = employees[j].intAdditionValue;

                item.intEmployWorkValue = employees[j].intEmployeeWorkValue;
            }
        }
    }

    void ReadTask(JsonSaveGame.TaskItem[] tasks)
    {
        List<PropertiesTask> listTask = UserValue.Instance.listTask;
        listTask.Clear();
        for (int i = 0; i < tasks.Length; i++)
        {
            PropertiesTask taskItem = new PropertiesTask();
            taskItem.enumTask = (EnumTaskType)tasks[i].intType;
            taskItem.intID = tasks[i].intID;
            taskItem.intRank = tasks[i].intRank;
            taskItem.intAwardCion = tasks[i].intAwardCoin;
            taskItem.intPenaltyCoin = tasks[i].intPenaltyCoin;
            taskItem.booDown = tasks[i].booDown;
            taskItem.booFinish = tasks[i].booFinishs;
            if (taskItem.enumTask == EnumTaskType.Dungeon)
            {
                taskItem.intDungeonID = tasks[i].intDungeonID;
                taskItem.intDungeonIndex = tasks[i].intDungeonIndex;
            }
            else
            {
                taskItem.intGoodsIDs = new int[tasks[i].intGoodsIDs.Length];
                taskItem.intGoodsCounts = new int[tasks[i].intGoodsCounts.Length];
                for (int t = 0; t < tasks[i].intGoodsIDs.Length; t++)
                {
                    taskItem.intGoodsIDs[t] = tasks[i].intGoodsIDs[t];
                    taskItem.intGoodsCounts[t] = tasks[i].intGoodsCounts[t];
                }
            }
            listTask.Add(taskItem);
        }
    }

    void ClearBackpackData()
    {
        UserValue.EnumKnapsackType[] knapsackTypes = new UserValue.EnumKnapsackType[] { UserValue.EnumKnapsackType.Backpack_1, UserValue.EnumKnapsackType.Backpack_2 };
        for (int i = 0; i < knapsackTypes.Length; i++)
        {
            BackpackGrid[] grids = UserValue.Instance.GetKnapsackGrids(knapsackTypes[i]);
            for (int j = 0; j < grids.Length; j++)
            {
                grids[j].enumStockType = EnumKnapsackStockType.None;
            }
        }
    }

    public void DeleteGame(int intIndex)
    {
        File.WriteAllText(ManagerValue.GetSavePath + intIndex, "");
    }
}
