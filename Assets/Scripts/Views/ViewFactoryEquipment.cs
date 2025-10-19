using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ViewFactoryEquipment : ViewBase
{
    public Text textTitle;
    public Text textSelectTitle;//选择物品标题
    public Text textSelectPage;//选择物品页码
    public Text textEquipmentUPName;//升级属性名
    public Text textEquipmentUPRandom;//升级概率
    public Text textEquipmentValue;//装备原有值
    public Text textEquipmentValueUP;//装备升级后的值
    public Text textTime;//剩余时间
    public Text textLoadContent;//正在升级时的提示内容

    public Button btnClose;
    public Button btnPageLeft;
    public Button btnPageRight;
    public Button btnEquipmentShop;//店铺装备
    public Button btnEquipmentForging;//锻造装备
    public Button btnEquipmentIntensify;//强化装备
    public Button btnConfirmEquipmentUP;//装备升级
    public Button btnConfirmForging;//确定锻造
    public Button btnCancelForging;//取消强化
    public Button btnDemolishBuild;//拆除建筑

    public View_PropertiesItem itemEquipment;//装备
    public Transform transEquipmentRoot;//装备显示
    public Transform transEquipmentPropertiesRoot;//装备属性显示
    public Transform transEquipmentConsume;//锻造的消耗品
    public Transform transEquipmentIntensify;//强化消耗品

    public GameObject goItemForging;//锻造装备
    public GameObject goItemEquipmentIntensify;//强化装备
    public GameObject goItemEquipmentLoading;//锻造中
    public GameObject goImageScroll;

    List<View_PropertiesItem> listEquipmentItem;
    List<Text> listEquipmentProperties;
    List<View_PropertiesItem> listForgingConsume;
    List<View_PropertiesItem> listIntensifyConsume;

    //武器强化 10级以内掉1级,10级以上删除武器

    //强化武器属性,超过当前属性值,则有失败的概率,超过当前属性值的3倍,有删除武器的风险

    //法杖 升级材料
    //1.木头+铜块 2.木头+银块 3.木头+金块
    //4.木头+银块+金块 5.木头+铜块+银块+金块
    //6.木头+银块+金块+晶石
    EquipmentUPMat[] wandUp = new EquipmentUPMat[]
    {
        //1.木头+铜块
        new EquipmentUPMat(new int[2]{10007,20011},new int[2]{1,1}),
        //2.木头+银块
        new EquipmentUPMat(new int[2]{10007,20012},new int[2]{1,1}),
        //3.木头+金块
        new EquipmentUPMat(new int[2]{10007,20013},new int[2]{1,1}),
        //4.木头+银块+金块
        new EquipmentUPMat(new int[3]{10007,20012,20013},new int[3]{1,1,1}),
        //5.木头+铜块+银块+金块
        new EquipmentUPMat(new int[4]{10007,20011,20012,20013 },new int[4]{1,1,1,1}),
        //6.木头+银块+金块+晶石
        new EquipmentUPMat(new int[4]{10007,20012,20013,30012},new int[4]{1,1,1,1}),
    };

    //剑 升级材料
    //1.木板+铁块 2.木板+铁块+铜块 3.铁块+铜块
    //4.铁块 5.铁块+银块+金块 6.铁块+金块+晶石
    EquipmentUPMat[] swordUP = new EquipmentUPMat[]
    {
        //1.木板+铁块
        new EquipmentUPMat(new int[2]{20014,20010},new int[2]{1,1}),
        //2.木板+铁块+铜块
        new EquipmentUPMat(new int[3]{20014,20010,20011},new int[3]{1,1,1}),
        //3.铁块+铜块
        new EquipmentUPMat(new int[2]{20010,20011},new int[2]{1,1}),
        //4.铁块
        new EquipmentUPMat(new int[1]{20010},new int[1]{1}),
        //5.铁块+银块+金块
        new EquipmentUPMat(new int[3]{20010,20012,20013},new int[3]{1,1,1}),
        //6.铁块+金块+晶石
        new EquipmentUPMat(new int[3]{20010,20013,30012},new int[3]{1,1,1}),
    };

    //弓 升级材料
    //1.木板+羊毛 2.木板+羊毛+丝绸 3.木板+蚕丝+羊毛+丝绸
    //4.木板+兽皮+羊毛+丝绸 5.兽皮+晶石 6.丝绸+兽皮+晶石
    EquipmentUPMat[] bowUP = new EquipmentUPMat[]
    {
        //1.木板+羊毛
        new EquipmentUPMat(new int[2]{20014,20004 },new int[2]{1,1 }),
        //2.木板+羊毛+丝绸
        new EquipmentUPMat(new int[3]{20014,20004,30017 },new int[3]{1,1,1 }),
        //3.木板+蚕丝+羊毛+丝绸
        new EquipmentUPMat(new int[4]{20014,20008,20004,30017 },new int[4]{1,1,1,1 }),
        //4.木板+兽皮+羊毛+丝绸
        new EquipmentUPMat(new int[4]{20014,20003,20004, 30017},new int[4]{ 1,1,1,1}),
        //5.兽皮+晶石
        new EquipmentUPMat(new int[2]{20003, 30012},new int[2]{1,1}),
        //6.丝绸+兽皮+晶石
        new EquipmentUPMat(new int[3]{30017,20003,30012},new int[3]{1,1,1 }),
    };

    //盾 升级材料
    //1.木板 2.木板+铜块 3.木板+铁块+铜块 4.铁块+铜块
    //5.铜块+铁块+银块 6.银块+金块+晶石
    EquipmentUPMat[] armorUP = new EquipmentUPMat[]
    {
        //1.木板
        new EquipmentUPMat(new int[1]{ 20014},new int[]{1 }),
        //2.木板+铜块
        new EquipmentUPMat(new int[2]{20014,20011 },new int[2]{1,1 }),
        //3.木板+铁块+铜块
        new EquipmentUPMat(new int[3]{20014,20010,20011 },new int[3]{1,1,1 }),
        //4.铁块+铜块
        new EquipmentUPMat(new int[2]{ 20010, 20011, },new int[2]{1,1 }),
        //5.铜块+铁块+银块
        new EquipmentUPMat(new int[3]{ 20011,20010,20012},new int[3]{ 1,1,1}),
        //6.银块+金块+晶石
        new EquipmentUPMat(new int[3]{20012,20013,30012 },new int[3]{1,1,1 }),
    };

    //鞋 升级材料
    //1.兽皮 2.兽皮+羊毛 3.兽皮+羊毛+丝绸 4.兽皮+丝绸+铁块
    //5.兽皮+羊毛+丝绸+银块+铁块 6.兽皮+羊毛+丝绸+银块+金块+晶石
    EquipmentUPMat[] shoesUP = new EquipmentUPMat[]
    {
        //1.兽皮
        new EquipmentUPMat(new int[1]{20003 },new int[1]{1 }),
        //2.兽皮+羊毛
        new EquipmentUPMat(new int[2]{20003,20004 },new int[2]{ 1,1}),
        //3.兽皮+羊毛+丝绸
        new EquipmentUPMat(new int[3]{ 20003,20004,30017},new int[3]{1,1,1 }),
        //4.兽皮+丝绸+铁块
        new EquipmentUPMat(new int[3]{20003,30017, 20010},new int[3]{1,1,1 }),
        //5.兽皮+羊毛+丝绸+银块+铁块
        new EquipmentUPMat(new int[5]{20003,20004,30017, 20012,20010},new int[5]{1,1,1,1,1 }),
        //6.兽皮+羊毛+丝绸+银块+金块+晶石
        new EquipmentUPMat(new int[6]{20003,20004,30017, 20012,20013,30012 },new int[6]{1,1,1,1,1,1 }),
    };

    /// <summary>
    /// 升级材料每10次升一个档次,1级颜色基础材料,2级+20%,3级+100%,4级+200,5级+500%
    /// 颜色共5个级别,1级颜色每次强化基础升级,2级+2,3级+5,4级+7,5级+9
    /// 共60档次,基础9级装备以下,1-5档次每次加1,6档次每次+8
    /// 基础11-20级装备,1-5档次每次加2,6档次每次+8
    /// 基础21-50级装备,1-5档次每次加3,6档次每次+10
    /// 每一级别在原有基础上,+10%,1-2档次100%,3档次 90%->50%,
    /// 4档次 50%->10%,5档次 10%->1%,6档次 1%->0.001%
    /// 最高档次 后面5级别成功概率 0.001%
    /// </summary>

    int intIndexGround;

    bool booSelfShop;//是不是工坊本身
    bool booForgingOr;//是否是强化
    int intPage;//页码
    int intPageTotal;
    int intSelectItem;//选择的格子
    /// <summary>
    /// 这个是顺序读取,中间不存在空物
    /// </summary>
    List<BackpackGrid> listEquipmentData = new List<BackpackGrid>();

    EventBuildToViewEquipment mgToViewEquipment;
    MGViewToBuildEquipment mgEquipment = new MGViewToBuildEquipment();
    ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();
    protected override void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            ManagerView.Instance.Hide(EnumView.ViewFactoryEquipment);
            SendClose();
        });
        btnPageLeft.onClick.AddListener(() =>
        {
            if (intPage > 0)
            {
                intPage -= 1;
                intSelectItem = 0;
                textSelectPage.text = (intPage + 1) + "/" + (intPageTotal + 1);
                if (booForgingOr)
                {
                    ShowEquipmentItem(intSelectItem, listForgingConsume);
                }
                else
                {
                    ShowEquipmentItem(intSelectItem, listIntensifyConsume);
                }
            }
        });
        btnPageRight.onClick.AddListener(() =>
        {
            if (intPage < intPageTotal)
            {
                intPage += 1;
                intSelectItem = 0;
                textSelectPage.text = (intPage + 1) + "/" + (intPageTotal + 1);
                if (booForgingOr)
                {
                    ShowEquipmentItem(intSelectItem, listForgingConsume);
                }
                else
                {
                    ShowEquipmentItem(intSelectItem, listIntensifyConsume);
                }
            }
        });
        //武器工坊本身武器
        btnEquipmentShop.onClick.AddListener(() =>
        {
            goItemForging.SetActive(true);
            goItemEquipmentIntensify.SetActive(false);

            booSelfShop = true;
            ShopEquipmentScroll();
        });
        //锻造 背包装备
        btnEquipmentForging.onClick.AddListener(() =>
        {
            goItemForging.SetActive(true);
            goItemEquipmentIntensify.SetActive(false);

            booSelfShop = false;
            BackpackEquipmentScroll();

        });
        //强化
        btnEquipmentIntensify.onClick.AddListener(() =>
        {
            goItemForging.SetActive(false);
            goItemEquipmentIntensify.SetActive(true);

            booSelfShop = false;
            BackpackEquipment();
        });
        //确定升级
        btnConfirmEquipmentUP.onClick.AddListener(() =>
        {

        });
        //确定锻造
        btnConfirmForging.onClick.AddListener(() =>
        {
            int intIndexData = intPage * listEquipmentItem.Count + intSelectItem;
            if (intIndexData >= listEquipmentData.Count)
            {
                hintBar.strHintBar = "物品为空,请重新选择";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
                return;
            }
            BackpackGrid item = listEquipmentData[intIndexData];
            JsonValue.TableEquipmentItem equipment = ManagerCombat.Instance.GetEquipmentItem(item.intID);
            int intPrice = (int)(equipment.intPrice * 1.2f);
            if (intPrice > UserValue.Instance.GetCoin)
            {
                hintBar.strHintBar = "金币不足";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
                return;
            }
            if (!booSelfShop)
            {
                EquipmentUPMat matUP = null;
                switch ((EnumKnapsackStockType)equipment.intKnaspackStockType)
                {
                    case EnumKnapsackStockType.Sword:
                        matUP = swordUP[0];
                        break;
                    case EnumKnapsackStockType.Bow:
                        matUP = bowUP[0];
                        break;
                    case EnumKnapsackStockType.Wand:
                        matUP = wandUp[0];
                        break;
                    case EnumKnapsackStockType.Armor:
                        matUP = armorUP[0];
                        break;
                    case EnumKnapsackStockType.Shoes:
                        matUP = shoesUP[0];
                        break;
                }
                //检查材料
                for (int i = 0; i < matUP.intProductIDs.Length; i++)
                {
                    if (!UserValue.Instance.KnapsackProductChectCount(matUP.intProductIDs[i], matUP.intProductCounts[i]))
                    {
                        string strProductName = ManagerProduct.Instance.GetName(matUP.intProductIDs[i], false);
                        hintBar.strHintBar = strProductName + " 产品不足";
                        ManagerView.Instance.Show(EnumView.ViewHintBar);
                        ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
                        return;
                    }
                }

                if (UserValue.Instance.SetCoinReduce(intPrice))
                {
                    for (int i = 0; i < matUP.intProductIDs.Length; i++)
                    {
                        UserValue.Instance.KnapsackProductReduce(matUP.intProductIDs[i], matUP.intProductCounts[i]);
                    }
                }


            }

            SendEquipmentForging(item, 5);
        });

        //取消锻造
        btnCancelForging.onClick.AddListener(() =>
        {

        });
        //拆除建筑
        btnDemolishBuild.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);

            viewHint.actionConfirm = () =>
            {
                MessageDemolishBuild mgDemolishBuild = new MessageDemolishBuild();
                mgDemolishBuild.intIndexGround = intIndexGround;
                ManagerMessage.Instance.PostEvent(EnumMessage.DemolishBuild, mgDemolishBuild);
                ManagerView.Instance.Hide(EnumView.ViewFactoryEquipment);
            };
            viewHint.strHint = "是否拆除建筑!";
            ManagerView.Instance.Show(EnumView.ViewHint);
            ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
        });

        goItemEquipmentIntensify.transform.position = goItemForging.transform.position;
        goItemEquipmentLoading.transform.position = goItemForging.transform.position;

        goItemForging.SetActive(true);
        goItemEquipmentIntensify.SetActive(false);
        goItemEquipmentLoading.SetActive(false);
    }

    public override void Show()
    {
        base.Show();

        if (listEquipmentItem == null)
        {
            ManagerValue.actionViewBuildEquipment = EventBuildToView;

            listEquipmentItem = new List<View_PropertiesItem>();
            for (int i = 0; i < transEquipmentRoot.childCount; i++)
            {
                listEquipmentItem.Add(transEquipmentRoot.GetChild(i).GetComponent<View_PropertiesItem>());
                listEquipmentItem[i].GetComponent<Button>().onClick.AddListener(OnClickSelectEquipment(i));
            }

            listEquipmentProperties = new List<Text>();
            for (int i = 0; i < transEquipmentPropertiesRoot.childCount; i++)
            {
                listEquipmentProperties.Add(transEquipmentPropertiesRoot.GetChild(i).GetComponent<Text>());
            }

            listForgingConsume = new List<View_PropertiesItem>();
            for (int i = 0; i < transEquipmentConsume.childCount; i++)
            {
                listForgingConsume.Add(transEquipmentConsume.GetChild(i).GetComponent<View_PropertiesItem>());
            }

            listIntensifyConsume = new List<View_PropertiesItem>();
            for (int i = 0; i < transEquipmentIntensify.childCount; i++)
            {
                listIntensifyConsume.Add(transEquipmentIntensify.GetChild(i).GetComponent<View_PropertiesItem>());
            }
        }
    }
    public override void SetData(Message message)
    {
        ViewMGToViewEquipment mgEquipmentView = message as ViewMGToViewEquipment;
        if (mgEquipmentView != null)
        {
            if (mgEquipmentView.intResidueTime != -1)
            {
                textTime.text = "剩余时间:" + mgEquipmentView.intResidueTime;
                goItemEquipmentLoading.SetActive(true);
            }
            else
            {
                goItemEquipmentLoading.SetActive(false);
            }
        }
    }

    void EventBuildToView(EventBuildToViewBase mgBuild)
    {
        mgToViewEquipment = mgBuild as EventBuildToViewEquipment;
        if (mgToViewEquipment != null)
        {
            goItemForging.SetActive(true);
            goItemEquipmentIntensify.SetActive(false);

            intIndexGround = mgToViewEquipment.intIndexGround;
            textTitle.text = ManagerBuild.Instance.GetBuildName(mgToViewEquipment.intBuildID);

            booSelfShop = true;
            ShopEquipmentScroll();

            if (mgToViewEquipment.forgingGrid.enumStockType != EnumKnapsackStockType.None)
            {
                goItemEquipmentLoading.SetActive(true);
                textTime.text = "剩余时间:" + mgToViewEquipment.intResidueTime;
            }
            else
            {
                goItemEquipmentLoading.SetActive(false);
            }
            ShowEquipmentImage(mgToViewEquipment.forgingGrid);
        }

    }

    /// <summary>
    /// 铁匠工坊 本身的装备卷轴
    /// </summary>
    void ShopEquipmentScroll()
    {
        booForgingOr = true;
        for (int i = 0; i < listEquipmentData.Count; i++)
        {
            listEquipmentData[i].enumStockType = EnumKnapsackStockType.None;
        }
        for (int i = 0; i < mgToViewEquipment.intEquipmentIDs.Length; i++)
        {
            if (listEquipmentData.Count <= i)
            {
                for (int j = 0; j < listEquipmentItem.Count; j++)
                {
                    BackpackGrid item = new BackpackGrid();
                    listEquipmentData.Add(item);
                }
            }
            listEquipmentData[i].intID = mgToViewEquipment.intEquipmentIDs[i];
            listEquipmentData[i].enumStockType = (EnumKnapsackStockType)ManagerCombat.Instance.GetEquipmentItem(listEquipmentData[i].intID).intKnaspackStockType;
            listEquipmentData[i].intRank = 11;
            listEquipmentData[i].intCount = 1;

            listEquipmentData[i].icon = ManagerCombat.Instance.GetEquipmentItem(listEquipmentData[i].intID).strICON;
        }

        intPage = 0;
        intPageTotal = listEquipmentData.Count / (listEquipmentItem.Count + 1);
        textSelectPage.text = (intPage + 1) + "/" + (intPageTotal + 1);
        intSelectItem = 0;
        ShowEquipmentItem(intSelectItem, listForgingConsume);
    }

    /// <summary>
    /// 铁匠工坊 获取背包卷轴数据
    /// </summary>
    void BackpackEquipmentScroll()
    {
        booForgingOr = true;
        for (int i = 0; i < listEquipmentData.Count; i++)
        {
            listEquipmentData[i].enumStockType = EnumKnapsackStockType.None;
        }
        int intIndexTemp = 0;
        BackpackGrid[] equipments = UserValue.Instance.GetKnapsackTarget(new EnumKnapsackStockType[]
        {  EnumKnapsackStockType.Sword,
            EnumKnapsackStockType.Bow,
            EnumKnapsackStockType.Wand,
            EnumKnapsackStockType.Armor,
            EnumKnapsackStockType.Shoes});
        for (int i = 0; i < equipments.Length; i++)
        {
            if (equipments[i].intRank < 10)
            {
                continue;
            }
            if (listEquipmentData.Count <= i)
            {
                for (int j = 0; j < listEquipmentItem.Count; j++)
                {
                    BackpackGrid item = new BackpackGrid();
                    listEquipmentData.Add(item);
                }
            }
            listEquipmentData[intIndexTemp].enumStockType = equipments[i].enumStockType;
            listEquipmentData[intIndexTemp].intRank = equipments[i].intRank;
            listEquipmentData[intIndexTemp].intCount = equipments[i].intCount;
            listEquipmentData[intIndexTemp].intID = equipments[i].intID;
            listEquipmentData[intIndexTemp].icon = equipments[i].icon;
            intIndexTemp++;
        }

        intPage = 0;
        intPageTotal = listEquipmentData.Count / (listEquipmentItem.Count + 1);
        textSelectPage.text = (intPage + 1) + "/" + (intPageTotal + 1);
        intSelectItem = 0;
        ShowEquipmentItem(intSelectItem, listForgingConsume);
    }

    /// <summary>
    /// 铁匠工坊 获取背包装备数据
    /// </summary>
    void BackpackEquipment()
    {
        booForgingOr = false;
        for (int i = 0; i < listEquipmentData.Count; i++)
        {
            listEquipmentData[i].enumStockType = EnumKnapsackStockType.None;
        }
        int intIndexTemp = 0;
        BackpackGrid[] equipments = UserValue.Instance.GetKnapsackTarget(new EnumKnapsackStockType[]
        {  EnumKnapsackStockType.Sword,
            EnumKnapsackStockType.Bow,
            EnumKnapsackStockType.Wand,
            EnumKnapsackStockType.Armor,
            EnumKnapsackStockType.Shoes});
        for (int i = 0; i < equipments.Length; i++)
        {
            if (equipments[i].intRank > 10)
            {
                continue;
            }
            if (listEquipmentData.Count <= i)
            {
                for (int j = 0; j < listEquipmentItem.Count; j++)
                {
                    BackpackGrid item = new BackpackGrid();
                    listEquipmentData.Add(item);
                }
            }
            listEquipmentData[intIndexTemp].enumStockType = equipments[i].enumStockType;
            listEquipmentData[intIndexTemp].intRank = equipments[i].intRank;
            listEquipmentData[intIndexTemp].intCount = equipments[i].intCount;
            listEquipmentData[intIndexTemp].intID = equipments[i].intID;
            listEquipmentData[intIndexTemp].icon = equipments[i].icon;
            intIndexTemp++;
        }

        intPage = 0;
        intPageTotal = listEquipmentData.Count / (listEquipmentItem.Count + 1);
        textSelectPage.text = (intPage + 1) + "/" + (intPageTotal + 1);
        intSelectItem = 0;
        ShowEquipmentItem(intSelectItem, listIntensifyConsume);
    }

    /// <summary>
    /// 在界面上显示装备数据
    /// </summary>
    void ShowEquipmentItem(int intIndex, List<View_PropertiesItem> listItem)
    {
        for (int i = 0; i < listEquipmentItem.Count; i++)
        {
            listEquipmentItem[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        listEquipmentItem[intIndex].transform.GetChild(0).gameObject.SetActive(true);

        for (int i = 0; i < listEquipmentItem.Count; i++)
        {
            if (listEquipmentData[i].enumStockType != EnumKnapsackStockType.None)
            {
                listEquipmentItem[i].imageValueMain.gameObject.SetActive(true);
                BackpackGrid item = listEquipmentData[i];
                listEquipmentItem[i].imageValueMain.sprite = ManagerResources.Instance.GetEquipmentSprite(item.icon);
                RectTransform rect = listEquipmentItem[i].imageValueMain.GetComponent<RectTransform>();
                rect.sizeDelta = Tools.SetSpriteRectSize(rect.sizeDelta, listEquipmentItem[i].imageValueMain.sprite);
                listEquipmentItem[i].imageValue.sprite = ManagerResources.Instance.GetFrameRank(item.intRank.ToString());
            }
            else
            {
                listEquipmentItem[i].imageValue.sprite = ManagerResources.Instance.GetFrameRank("1");
                listEquipmentItem[i].imageValueMain.gameObject.SetActive(false);
            }
        }
        int intTemp = intPage * listEquipmentData.Count + intSelectItem;
        ShowEquipmentImage(listEquipmentData[intTemp]);
        ShowEquipmentUpMat(intTemp, listItem);
    }

    /// <summary>
    /// 显示升级所需材料
    /// </summary>
    void ShowEquipmentUpMat(int intIndexData, List<View_PropertiesItem> listItem)
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            listItem[i].gameObject.SetActive(false);
        }
        if (intIndexData >= listEquipmentData.Count)
        {
            return;
        }
        BackpackGrid item = listEquipmentData[intPage * listEquipmentItem.Count + intSelectItem];
        if (item.enumStockType == EnumKnapsackStockType.None)
        {
            return;
        }
        JsonValue.TableEquipmentItem equipment = ManagerCombat.Instance.GetEquipmentItem(item.intID);
        int intPrice = (int)(equipment.intPrice * 1.2f);

        EquipmentUPMat matUP = null;
        if (!booSelfShop)
        {
            switch ((EnumKnapsackStockType)equipment.intKnaspackStockType)
            {
                case EnumKnapsackStockType.Sword:
                    matUP = swordUP[0];
                    break;
                case EnumKnapsackStockType.Bow:
                    matUP = bowUP[0];
                    break;
                case EnumKnapsackStockType.Wand:
                    matUP = wandUp[0];
                    break;
                case EnumKnapsackStockType.Armor:
                    matUP = armorUP[0];
                    break;
                case EnumKnapsackStockType.Shoes:
                    matUP = shoesUP[0];
                    break;
            }
        }
        else
        {
            matUP = new EquipmentUPMat(new int[] { }, new int[] { });
        }
        //检查材料
        JsonValue.DataTableBackPackItem itemProduct = null;
        for (int i = 0; i < listItem.Count; i++)
        {
            if (i < matUP.intProductIDs.Length)
            {
                listItem[i].gameObject.SetActive(true);

                itemProduct = ManagerProduct.Instance.GetProductTableItem(matUP.intProductIDs[i]);
                listItem[i].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(itemProduct.strIconName);
                listItem[i].textValueMain.text = ManagerProduct.Instance.GetName(matUP.intProductIDs[i], false);
                listItem[i].textValue.text = matUP.intProductCounts[i].ToString();
                if (UserValue.Instance.KnapsackProductChectCount(matUP.intProductIDs[i], matUP.intProductCounts[i]))
                {
                    listItem[i].textValue.color = Color.white;
                }
                else
                {
                    listItem[i].textValue.color = Color.red;
                }
            }
            else
            {
                listItem[i].gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// 显示装备头像
    /// </summary>
    void ShowEquipmentImage(BackpackGrid item)
    {
        if (item.enumStockType == EnumKnapsackStockType.None)
        {
            itemEquipment.imageValueMain.gameObject.SetActive(false);
            itemEquipment.imageValue.sprite = ManagerResources.Instance.GetFrameRank("1");
            itemEquipment.textValueMain.text = "";
            return;
        }
        itemEquipment.imageValueMain.gameObject.SetActive(true);
        itemEquipment.imageValueMain.sprite = ManagerResources.Instance.GetEquipmentSprite(item.icon);
        RectTransform rect = itemEquipment.imageValueMain.GetComponent<RectTransform>();
        rect.sizeDelta = Tools.SetSpriteRectSize(rect.sizeDelta, itemEquipment.imageValueMain.sprite);
        itemEquipment.imageValue.sprite = ManagerResources.Instance.GetFrameRank(item.intRank.ToString());
        itemEquipment.textValueMain.text = ManagerCombat.Instance.GetEquipmentItem(item.intID).strNameChina;
    }

    UnityAction OnClickSelectEquipment(int intIndex)
    {
        return () =>
        {
            intSelectItem = intIndex;
            for (int i = 0; i < listEquipmentItem.Count; i++)
            {
                listEquipmentItem[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            listEquipmentItem[intIndex].transform.GetChild(0).gameObject.SetActive(true);
            int intTemp = intPage * listEquipmentData.Count + intIndex;

            if (mgToViewEquipment.forgingGrid.enumStockType == EnumKnapsackStockType.None)
            {
                ShowEquipmentImage(listEquipmentData[intTemp]);

                if (booForgingOr)
                {
                    ShowEquipmentUpMat(intTemp, listForgingConsume);
                }
                else
                {
                    ShowEquipmentUpMat(intTemp, listIntensifyConsume);
                }
            }
        };
    }

    void SendEquipmentForging(BackpackGrid itemGrid, int intEquipmentDay)
    {
        mgEquipment.forging.forgingGrid = mgToViewEquipment.forgingGrid;
        mgEquipment.forging.forgingGrid.intID = itemGrid.intID;
        mgEquipment.forging.forgingGrid.enumStockType = itemGrid.enumStockType;
        mgEquipment.forging.forgingGrid.icon = itemGrid.icon;
        mgEquipment.forging.forgingGrid.intRank = itemGrid.intRank;
        mgEquipment.forging.intFinishDay = intEquipmentDay;
        mgEquipment.forging.intIndexGround = mgToViewEquipment.intIndexGround;
        ManagerValue.actionGround(mgEquipment.forging.intIndexGround, mgEquipment.forging);
    }

    void SendClose()
    {
        mgEquipment.close.intIndexGround = mgToViewEquipment.intIndexGround;
        ManagerValue.actionGround(mgEquipment.close.intIndexGround, mgEquipment.close);
    }

    private void OnDestroy()
    {
        ManagerValue.actionViewBuildEquipment -= EventBuildToView;
    }

    /// <summary>
    /// 装备升级时的材料
    /// </summary>
    public class EquipmentUPMat
    {
        public int[] intProductIDs;
        public int[] intProductCounts;
        public EquipmentUPMat(int[] ids, int[] counts)
        {
            intProductIDs = ids;
            intProductCounts = counts;
        }
    }
}
