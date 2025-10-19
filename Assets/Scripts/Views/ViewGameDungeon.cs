using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ViewGameDungeon : ViewBase
{
    public Text textTitle;
    public Text textAwardHint;//通关奖励

    public Button btnClose;
    public Button btnFight;

    public Transform transEnemyTeam;
    public View_EnemyItem[] enemyItems;
    public View_PropertiesBase itemAward;//击败奖励

    PropertiesDungeon.DungeonPoint dungeonPoint;
    List<Transform> listTransEnemyTeam = new List<Transform>();

    //这是显示副本中单个任务点,单个任务点有多个敌人队伍

    protected override void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            ManagerView.Instance.Hide(EnumView.ViewGameDungeon);
        });
        btnFight.onClick.AddListener(() =>
        {
            bool booTemp = true;
            int[] intIDs = new int[5] { -1, -1, -1, -1, -1 };
            Dictionary<int, PropertiesEmployee> dicEmployee = null;
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    dicEmployee = UserValue.Instance.GetEmployeeAll();
                }
                else if (i == 1)
                {
                    dicEmployee = UserValue.Instance.GetEmployeeGuestAll();
                }
                foreach (PropertiesEmployee temp in dicEmployee.Values)
                {
                    if (temp.enumLocation == EnumEmployeeLocation.CombatTeam)
                    {
                        intIDs[temp.intIndexCombat] = temp.intIndexID;
                        booTemp = false;
                    }
                }
            }
            if (booTemp)
            {
                ManagerValue.actionAudio(EnumAudio.Unable);
                ManagerView.Instance.Show(EnumView.ViewHint);
                viewHint.strHint = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.NoEITBQ, null);//"战斗队列中没有员工";
                ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
                return;
            }
            else
            {
                ManagerValue.actionAudio(EnumAudio.Ground);
            }

            ManagerView.Instance.Show(EnumView.ViewCombat);
            ManagerView.Instance.Hide(EnumView.ViewGameDungeon);
            ManagerView.Instance.Hide(EnumView.ViewMap);
            ManagerValue.actionInitCombat(intIDs, ManagerValue.intDungeonPointIndex, ManagerValue.intDungeonID);
            ManagerValue.actionCameraLocation(ControllerCamera.EnumCameraPosition.Combat);

        });
    }

    public override void Show()
    {
        base.Show();

        textAwardHint.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.ClearanceReward);
        btnFight.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Battle);

        JsonValue.DataGameDungeonItem itemDungeon = ManagerCombat.Instance.GetGameDungeonItem(ManagerValue.intDungeonID);
        if (listTransEnemyTeam.Count == 0)
        {
            for (int i = 0; i < transEnemyTeam.childCount; i++)
            {
                listTransEnemyTeam.Add(transEnemyTeam.GetChild(i));
                listTransEnemyTeam[i].GetComponent<Button>().onClick.AddListener(OnClickEnemyTeam(i));
            }
        }
        int intIndexTemp = ManagerValue.intDungeonPointIndex;
        for (int i = 0; i < itemAward.items.Length; i++)
        {
            if (i < itemDungeon.taskPoints[intIndexTemp].intAwardIDs.Length)
            {
                itemAward.items[i].gameObject.SetActive(true);
                EnumKnapsackStockType knapsackType = (EnumKnapsackStockType)itemDungeon.taskPoints[intIndexTemp].intKnaspackType[i];

                if (knapsackType == EnumKnapsackStockType.Sword
                    || knapsackType == EnumKnapsackStockType.Bow
                    || knapsackType == EnumKnapsackStockType.Wand
                    || knapsackType == EnumKnapsackStockType.Armor
                    || knapsackType == EnumKnapsackStockType.Shoes)
                {
                    JsonValue.TableEquipmentItem equipmentItem = ManagerCombat.Instance.GetEquipmentItem(itemDungeon.taskPoints[intIndexTemp].intTeamIDs[i]);
                    itemAward.items[i].imageValueMain.sprite = ManagerResources.Instance.GetEquipmentSprite(equipmentItem.strICON);
                }
                else if (knapsackType == EnumKnapsackStockType.Farm
                    || knapsackType == EnumKnapsackStockType.Fasture
                    || knapsackType == EnumKnapsackStockType.Factory)
                {
                    JsonValue.DataTableBackPackItem backpackItem = ManagerProduct.Instance.GetProductTableItem(itemDungeon.taskPoints[intIndexTemp].intAwardIDs[i]);
                    itemAward.items[i].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(backpackItem.strIconName);
                }
                itemAward.items[i].textValueMain.text = itemDungeon.taskPoints[intIndexTemp].intAwardCounts[i].ToString();
                itemAward.items[i].imageValue.sprite = ManagerResources.Instance.GetFrameRank(itemDungeon.taskPoints[intIndexTemp].intAwardRanks[i].ToString());
            }
            else
            {
                itemAward.items[i].gameObject.SetActive(false);
            }
        }

        PropertiesDungeon itemDun = UserValue.Instance.dicDungeon[ManagerValue.intDungeonID];
        dungeonPoint = itemDun.points[intIndexTemp];
        for (int i = 0; i < listTransEnemyTeam.Count; i++)
        {
            if (i < dungeonPoint.teams.Length)
            {
                listTransEnemyTeam[i].gameObject.SetActive(true);
                listTransEnemyTeam[i].GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.SmallMT) + " " + (i + 1);
            }
            else
            {
                listTransEnemyTeam[i].gameObject.SetActive(false);
            }
        }
        ShowEnemyTeamInfo(dungeonPoint, 0);

        textTitle.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.InstanceDetails);
    }

    /// <summary>
    /// 队伍按钮
    /// </summary>
    UnityAction OnClickEnemyTeam(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ShowEnemyTeamInfo(dungeonPoint, intIndex);
        };
    }

    /// <summary>
    /// 单只队伍
    /// </summary>
    /// <param name="team"></param>
    void ShowEnemyTeamInfo(PropertiesDungeon.DungeonPoint dungeonPoint, int intIndexTeam)
    {
        for (int i = 0; i < enemyItems.Length; i++)
        {
            if (dungeonPoint.teams[intIndexTeam].intIDs.Length > i)
            {
                JsonValue.DataTableEnemyItem itemEnemy = ManagerCombat.Instance.GetEnemyItem(dungeonPoint.teams[intIndexTeam].intIDs[i]);

                enemyItems[i].gameObject.SetActive(true);
                //enemyItems[i].imageCombatType.sprite = ManagerSkill.Instance.GetCombatType((EnumCombatAttackType)itemTeam.intCombatTypes[i]);
                //enemyItems[i].textCombatRank.text = itemTeam.intCombatTypeRanks[i].ToString();
                string strEnemyName = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Queue) + (intIndexTeam + 1) + ":" + ManagerLanguage.Instance.GetWord(EnumLanguageWords.SmallMonster) + (i + 1);
                enemyItems[i].textEnemyName.text = strEnemyName;//itemEnemy.strNameChina;
                enemyItems[i].enemyCombatValue.items[0].textValueMain.text = ManagerValue.EnemyHP(dungeonPoint.intWinCount, itemEnemy.intHP).ToString();
                enemyItems[i].enemyCombatValue.items[1].textValueMain.text = itemEnemy.intAttack.ToString();
                //拿到技能数量,ID
                //string[] strSkill = itemTeam.strEnemySkills[i].Split('_');
                //string[] strSkillRank = itemTeam.strEnemySkillRanks[i].Split('_');
                //if (strSkill.Length == 1 && (strSkill[0] == "" || strSkill[0] == null))
                //{
                //    strSkill = new string[] { };
                //    strSkillRank = new string[] { };
                //}
                //for (int k = 0; k < enemyItems[i].enemySkill.items.Length; k++)
                //{
                //    if (strSkill.Length > k && !(strSkill[0] == "" || strSkill[0] == null))
                //    {
                //        enemyItems[i].enemySkill.items[k].gameObject.SetActive(true);
                //        PropertiesSkill tempSkill = ManagerSkill.Instance.GetSkillValue(int.Parse(strSkill[k]));
                //        enemyItems[i].enemySkill.items[k].imageValueMain.sprite = ManagerResources.Instance.GetSkillSprite(tempSkill.strICON);
                //        enemyItems[i].enemySkill.items[k].imageValue.sprite = ManagerSkill.Instance.GetCombatType(tempSkill.combatType);
                //        enemyItems[i].enemySkill.items[k].textValueMain.text = strSkillRank[k];
                //    }
                //    else
                //    {
                //        enemyItems[i].enemySkill.items[k].gameObject.SetActive(false);
                //    }
                //}
            }
            else
            {
                enemyItems[i].gameObject.SetActive(false);
            }
            enemyItems[i].enemySkill.gameObject.SetActive(false);
            enemyItems[i].enemyCombatValue.items[2].gameObject.SetActive(false);
            enemyItems[i].enemyCombatValue.items[3].gameObject.SetActive(false);
            enemyItems[i].imageCombatType.transform.parent.gameObject.SetActive(false);
        }
    }
}