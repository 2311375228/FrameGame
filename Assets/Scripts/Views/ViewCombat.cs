using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewCombat : ViewBase
{
    public Text textRoundInfo;
    public Text textOverInfo;

    public Text textTimeGame;
    public Text textTimeCombat;
    public Text textTimeRound;

    public Button btnBack;
    public Button btnDefeat;//失败
    public Button btnOverCombat;//结束战斗
    public GameObject goNextRound;
    public GameObject goOver;

    public Transform transGridAward;

    public ViewCombat_RoleShow itemRoleShow;
    //伤害浮动的数字
    public ViewCombat_RoleHarm itemRoleHarm;
    List<View_PropertiesItem> listAwardItem = new List<View_PropertiesItem>();

    Dictionary<int, ViewCombat_RoleShow> dicRoleShow = new Dictionary<int, ViewCombat_RoleShow>();
    //队列,先进先出
    Queue<ViewCombat_RoleHarm> queueHarmPool = new Queue<ViewCombat_RoleHarm>();
    List<ViewCombat_RoleHarm> listHarmShow = new List<ViewCombat_RoleHarm>();

    float floTimeHarm;
    float floTimeCombat;
    protected override void Start()
    {

        btnBack.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerMessage.Instance.PostEvent(EnumMessage.StopCombat);
            ManagerView.Instance.Show(EnumView.ViewMap);
            ManagerView.Instance.Hide(EnumView.ViewCombat);
            ManagerValue.actionCameraLocation(ControllerCamera.EnumCameraPosition.GameDungeon);
        });
        btnOverCombat.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerMessage.Instance.PostEvent(EnumMessage.StopCombat);
            ManagerView.Instance.Show(EnumView.ViewMap);
            ManagerView.Instance.Hide(EnumView.ViewCombat);
            ManagerValue.actionCameraLocation(ControllerCamera.EnumCameraPosition.GameDungeon);
        });
        btnDefeat.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerMessage.Instance.PostEvent(EnumMessage.StopCombat);
            ManagerView.Instance.Show(EnumView.ViewMap);
            ManagerView.Instance.Hide(EnumView.ViewCombat);
            ManagerValue.actionCameraLocation(ControllerCamera.EnumCameraPosition.GameDungeon);
        });

        for (int i = 0; i < transGridAward.childCount; i++)
        {
            View_PropertiesItem itemAward = transGridAward.GetChild(i).GetComponent<View_PropertiesItem>();
            listAwardItem.Add(itemAward);
        }

        itemRoleShow.gameObject.SetActive(false);
        itemRoleShow.skills.gameObject.SetActive(false);
        itemRoleShow.imageCombatType.transform.parent.gameObject.SetActive(false);
        itemRoleShow.textMP.transform.parent.gameObject.SetActive(false);
        itemRoleHarm.gameObject.SetActive(false);

        goNextRound.SetActive(false);
        goOver.transform.position = goNextRound.transform.position;
        goOver.SetActive(false);

        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Date, MessageUpdateDate);
    }



    void ShowRoleData(CombatRoleShowData roleData)
    {
        if (!dicRoleShow.ContainsKey(roleData.intIndex))
        {
            GameObject goTemp = Instantiate(itemRoleShow.gameObject, itemRoleShow.transform.parent, false);
            ViewCombat_RoleShow itemShow = goTemp.GetComponent<ViewCombat_RoleShow>();
            itemShow.skills.gameObject.SetActive(false);
            itemShow.imageCombatType.transform.parent.gameObject.SetActive(false);
            itemShow.textMP.transform.parent.gameObject.SetActive(false);
            dicRoleShow.Add(roleData.intIndex, itemShow);
        }
        ViewCombat_RoleShow roleShow = dicRoleShow[roleData.intIndex];
        if (roleData.combatState == EnumCombatRoleState.None)
        {
            roleShow.gameObject.SetActive(false);
        }
        else
        {
            roleShow.gameObject.SetActive(true);
        }
        Vector3 vecPosition = Camera.main.WorldToScreenPoint(roleData.vecPosition);
        vecPosition.y = vecPosition.y + 200 * Screen.height / 1080;
        roleShow.transform.position = vecPosition;
        roleShow.imageCombatType.sprite = ManagerSkill.Instance.GetCombatType(roleData.combatType);
        roleShow.textHP.text = roleData.intHP + "/" + roleData.intHPMax;
        roleShow.textATK.text = roleData.intATK.ToString();
        roleShow.textMP.text = roleData.intMP.ToString();
        roleShow.sliderHP.value = roleData.intHP;
        roleShow.sliderHP.maxValue = roleData.intHPMax;
        for (int i = 0; i < roleShow.skills.items.Length; i++)
        {
            if (i < roleData.intSkillLength && roleData.proSkills != null && roleData.proSkills[i] != null)
            {
                roleShow.skills.items[i].textValueMain.text = roleData.intSkillRanks[i].ToString();
                roleShow.skills.items[i].imageValueMain.sprite = ManagerResources.Instance.GetSkillSprite(roleData.proSkills[i].strICON);
                roleShow.skills.items[i].gameObject.SetActive(true);
            }
            else
            {
                roleShow.skills.items[i].gameObject.SetActive(false);
            }
        }

        //vecHarmTarget = roleData.vecPosition;
        //vecHarmTarget.y += 50 * Screen.height / 1080;

        vecPosition = Camera.main.WorldToScreenPoint(roleData.vecHarmShow);
        if (roleData.harmValue.intATK > 0)
        {
            GetHarmItem("-" + roleData.harmValue.intATK, Color.gray, vecPosition);
        }
        if (roleData.harmValue.intMP > 0)
        {
            GetHarmItem("-" + roleData.harmValue.intMP, Color.green, vecPosition);
        }
        if (roleData.harmValue.intHP > 0)
        {
            GetHarmItem("+" + roleData.harmValue.intHP, Color.red, vecPosition);
        }
    }

    private void GetHarmItem(string strTemp, Color color, Vector3 vecPosition)
    {
        ViewCombat_RoleHarm roleHarm = null;
        if (queueHarmPool.Count == 0)
        {
            GameObject goTemp = Instantiate(itemRoleHarm.gameObject, itemRoleHarm.transform.parent, false);
            roleHarm = goTemp.GetComponent<ViewCombat_RoleHarm>();
            roleHarm.actionShow = EventActionHarm;
        }
        else
        {
            roleHarm = queueHarmPool.Dequeue();
        }

        roleHarm.textHarm.text = strTemp;
        roleHarm.textHarm.color = color;
        roleHarm.vecTargetHeight = vecPosition;
        Vector3 vecTemp = vecPosition;
        vecTemp.y = vecPosition.y + 200 * Screen.height / 1080;
        roleHarm.transform.position = vecTemp;

        roleHarm.gameObject.SetActive(false);
        listHarmShow.Add(roleHarm);
    }

    public override void Show()
    {
        base.Show();
        floTimeCombat = 0;
        if (ManagerValue.actionCombatShow == null)
        {
            ManagerValue.actionCombatShow = ShowRoleData;
        }

        textOverInfo.text = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.TheFRHBSTYE, null);
        btnBack.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Back);
        btnDefeat.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Defeated);
        btnOverCombat.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Confirm);
    }

    public override void SetData(Message message)
    {
        CombatOver over = message as CombatOver;
        if (over != null)
        {
            if (over.booOver)
            {
                if (over.booTeam)
                {
                    VictoryCombat(over);
                    goOver.SetActive(true);
                    ManagerValue.actionAudioCombat(EnumAudioCombat.MUSIC_EFFECT_Orchestral_Brass_Positive_11_stereo);
                }
                else
                {
                    textRoundInfo.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Defeated);//"战败";
                    goNextRound.SetActive(true);
                    btnDefeat.gameObject.SetActive(true);
                    ManagerValue.actionAudioCombat(EnumAudioCombat.MUSIC_EFFECT_Orchestral_Brass_Negative_01_stereo);
                }
            }
            else
            {
                goOver.SetActive(false);
            }

            if (over.booTeam)
            {
                textRoundInfo.text = over.strRoundInfo + over.floTime.ToString("0");
                goNextRound.SetActive(over.booRound);
                btnDefeat.gameObject.SetActive(false);
            }
            else
            {
                textRoundInfo.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Defeated);//"战败";
                goNextRound.SetActive(over.booRound);
                btnDefeat.gameObject.SetActive(true);
                ManagerValue.actionAudioCombat(EnumAudioCombat.MUSIC_EFFECT_Orchestral_Brass_Negative_01_stereo);
            }
            PropertiesDungeon dungeonItem = UserValue.Instance.dicDungeon[over.intDungeonID];
            textTimeRound.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.EnemyTeam) + ":" + (over.intIndexTeam + 1) + "/" + dungeonItem.points[over.intDungeonIndex].teams.Length;
        }
    }

    void VictoryCombat(CombatOver over)
    {
        //获胜
        for (int i = 0; i < listAwardItem.Count; i++)
        {
            listAwardItem[i].gameObject.SetActive(false);
        }
        JsonValue.DataGameDungeonItem itemDungeon = ManagerCombat.Instance.GetGameDungeonItem(over.intDungeonID);
        for (int i = 0; i < itemDungeon.taskPoints[over.intDungeonIndex].intAwardIDs.Length; i++)
        {
            listAwardItem[i].gameObject.SetActive(true);
            EnumKnapsackStockType knapsackType = (EnumKnapsackStockType)itemDungeon.taskPoints[over.intDungeonIndex].intKnaspackType[i];
            if (knapsackType == EnumKnapsackStockType.Coin)
            {
                listAwardItem[i].imageValueMain.sprite = ManagerResources.Instance.GetFrameRank("icon_itemicon_coin");
            }
            else if (knapsackType == EnumKnapsackStockType.Sword
                 || knapsackType == EnumKnapsackStockType.Bow
                 || knapsackType == EnumKnapsackStockType.Wand
                 || knapsackType == EnumKnapsackStockType.Armor
                 || knapsackType == EnumKnapsackStockType.Shoes)
            {
                string strICON = ManagerCombat.Instance.GetEquipmentItem(itemDungeon.taskPoints[over.intDungeonIndex].intAwardIDs[i]).strICON;
                listAwardItem[i].imageValueMain.sprite = ManagerResources.Instance.GetEquipmentSprite(strICON);
            }
            else if (knapsackType == EnumKnapsackStockType.Farm
                || knapsackType == EnumKnapsackStockType.Fasture
                || knapsackType == EnumKnapsackStockType.Factory)
            {
                string strICON = ManagerProduct.Instance.GetProductTableItem(itemDungeon.taskPoints[over.intDungeonIndex].intAwardIDs[i]).strIconName;
                listAwardItem[i].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(strICON);
            }
            listAwardItem[i].imageValue.sprite = ManagerResources.Instance.GetFrameRank(itemDungeon.taskPoints[over.intDungeonIndex].intAwardRanks[i].ToString());
            listAwardItem[i].textValueMain.text = itemDungeon.taskPoints[over.intDungeonIndex].intAwardCounts[i].ToString();
        }

        //检查是否包含任务
        List<PropertiesTask> listTask = UserValue.Instance.listTask;
        for (int i = 0; i < listTask.Count; i++)
        {
            if (listTask[i].intDungeonID == over.intDungeonID && listTask[i].intDungeonIndex == over.intDungeonIndex)
            {
                listTask[i].booDown = true;
                break;
            }
        }

        //是否开启下一个关卡
        PropertiesDungeon dungeonItem = UserValue.Instance.dicDungeon[over.intDungeonID];
        if (!(itemDungeon.taskPoints[over.intDungeonIndex].strEffect == null || itemDungeon.taskPoints[over.intDungeonIndex].strEffect == ""))
        {
            if (UserValue.Instance.dicDungeon.ContainsKey(dungeonItem.intDungeonID + 1))
            {
                PropertiesDungeon dungeonNext = UserValue.Instance.dicDungeon[dungeonItem.intDungeonID + 1];
                dungeonNext.booFinishDungeon = true;
            }
        }
        for (int i = 0; i < dungeonItem.points.Length; i++)
        {
            if (dungeonItem.points[i].intPointIndex == over.intDungeonIndex)
            {
                dungeonItem.points[i].intWinCount++;
            }
        }

        //获得星星的规则
        //10回合内击败boss 一颗星星
        //没有阵亡 一颗星星
        //一回合击败boss 一颗星星
        int intTempStar = 0;
        if (over.intRoleDeath == 0)
        {
            intTempStar += 1;
        }
        if (over.intRoundCount == 1)
        {
            intTempStar += 1;
        }
        if (over.intRoundCount < 10)
        {
            intTempStar += 1;
        }
        if (dungeonItem.points[over.intDungeonIndex].intStar < intTempStar)
        {
            dungeonItem.points[over.intDungeonIndex].intStar = intTempStar;
        }

        goNextRound.SetActive(false);
        goOver.SetActive(true);

        MessageMail mail = new MessageMail();
        mail.enumMail = ViewBarTop_ItemMail.EnumMail.Task;
        mail.gridItems = new EnumKnapsackStockType[itemDungeon.taskPoints[over.intDungeonIndex].intKnaspackType.Length];
        mail.strContent = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Tasks);//"任务";
        mail.intIndexIDs = new int[itemDungeon.taskPoints[over.intDungeonIndex].intKnaspackType.Length];
        mail.intRanks = new int[itemDungeon.taskPoints[over.intDungeonIndex].intKnaspackType.Length];
        mail.intIndexCounts = new int[itemDungeon.taskPoints[over.intDungeonIndex].intKnaspackType.Length];
        for (int i = 0; i < mail.intIndexIDs.Length; i++)
        {
            mail.gridItems[i] = (EnumKnapsackStockType)itemDungeon.taskPoints[over.intDungeonIndex].intKnaspackType[i];
            mail.intIndexIDs[i] = itemDungeon.taskPoints[over.intDungeonIndex].intAwardIDs[i];
            mail.intRanks[i] = itemDungeon.taskPoints[over.intDungeonIndex].intAwardRanks[i];
            mail.intIndexCounts[i] = itemDungeon.taskPoints[over.intDungeonIndex].intAwardCounts[i];
        }
        ManagerMessage.Instance.PostEvent(EnumMessage.Mail, mail);
    }

    protected override void Update()
    {
        //浮动效果
        if (listHarmShow.Count > 0)
        {
            floTimeHarm += Time.deltaTime;
            if (floTimeHarm > 0.5f)
            {
                floTimeHarm = 0;
                listHarmShow[0].gameObject.SetActive(true);
                listHarmShow.RemoveAt(0);
            }
        }
        floTimeCombat += Time.deltaTime;
        textTimeCombat.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.BattleDuration) + ":" + ((int)floTimeCombat);
    }

    void EventActionHarm(ViewCombat_RoleHarm harm)
    {
        queueHarmPool.Enqueue(harm);
    }

    void MessageUpdateDate(ManagerMessage.MessageBase message)
    {
        MessageDate date = message as MessageDate;
        if (date != null)
        {
            textTimeGame.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.GameDuration) + " " + date.numYear + ":" + date.numMonth + ":" + date.numDay;
        }
    }

    private void OnDestroy()
    {
        ManagerValue.actionCombatShow -= ShowRoleData;
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Update_Date, MessageUpdateDate);
    }
}
