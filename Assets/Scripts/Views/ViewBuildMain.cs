using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuildMain : ViewBase
{
    public Text textTitle;

    public Button btnClose;
    public Button btnDemolishBuild;

    public RectTransform rectUp;
    public RectTransform rectDown;
    public RectTransform rectCentre;

    public View_PropertiesBase buildProperties;//建筑属性

    int intIndexSize;//点击的员工框
    int intIndexGround = -1;
    EnumEmployeeProperties[] enumEmployeeProperties;

    //采矿场
    int[] intOres = new int[] { 1008, 1009, 1010, 1011 };

    EmployeeData employeeData = new EmployeeData();
    ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();
    ViewBuild_Base.CloseMessage messageClose = new ViewBuild_Base.CloseMessage();
    ViewBuild_Base.DemolishBuild messageDemolish = new ViewBuild_Base.DemolishBuild();
    Dictionary<ViewBuild_Base.EnumViewUp, ViewBuild_Base> dicViewUp = new Dictionary<ViewBuild_Base.EnumViewUp, ViewBuild_Base>();
    Dictionary<ViewBuild_Base.EnumViewDown, ViewBuild_Base> dicViewDown = new Dictionary<ViewBuild_Base.EnumViewDown, ViewBuild_Base>();
    protected override void Start()
    {
        base.Start();

        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            messageClose.intIndexGround = intIndexGround;
            messageClose.messageType = MGViewToBuildBase.EnumMessageType.Close;
            SendToGround(messageClose);
            ManagerView.Instance.Hide(EnumView.ViewBuildMain);
        });
        btnDemolishBuild.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Unable);
            viewHint.actionConfirm = () =>
            {
                messageDemolish.intIndexGround = intIndexGround;
                messageDemolish.messageType = MGViewToBuildBase.EnumMessageType.Demolish;
                SendToGround(messageDemolish);

                MessageDemolishBuild mgDemolishBuild = new MessageDemolishBuild();
                mgDemolishBuild.intIndexGround = intIndexGround;
                ManagerMessage.Instance.PostEvent(EnumMessage.DemolishBuild, mgDemolishBuild);
                ManagerView.Instance.Hide(EnumView.ViewBuildMain);
            };
            if (UserValue.Instance.GetDicGround.ContainsKey(intIndexGround))
            {
                PropertiesGround ground = UserValue.Instance.GetDicGround[intIndexGround];
                long longPriceTemp = 0;
                if (ground.GetState == EnumGroudState.Purchased)
                {
                    longPriceTemp = ManagerValue.DemolishLandrecycleCoin(ManagerValue.intGroundCount, false);
                }
                else
                {
                    BuildBase build = UserValue.Instance.GetBuildValue(intIndexGround);
                    longPriceTemp = ManagerValue.DemolishBuildRecycleCoin(build.IntBuildTotalPrice);
                }
                viewHint.strHint = ManagerLanguage.Instance.GetWord(EnumLanguageWords.IsTBTBD) + "\n" + UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, longPriceTemp.ToString("N0")) + " " + ManagerLanguage.Instance.GetWord(EnumLanguageWords.GoldCoins);//"是否拆除建筑!";
                ManagerView.Instance.Show(EnumView.ViewHint);
                ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
            }

        });

        for (int i = 0; i < buildProperties.items.Length; i++)
        {
            buildProperties.items[i].gameObject.SetActive(false);
        }
    }
    public override void Show()
    {
        base.Show();

        if (ManagerValue.actionViewBuildFarmInfo == null)
        {
            ManagerValue.actionViewBuildFarmInfo = MGBuildBaseFarm;
        }
        if (ManagerValue.actionViewBuildPasture == null)
        {
            ManagerValue.actionViewBuildPasture = MGBuildBaseFarm;
        }
        if (ManagerValue.actionViewBuildFactory == null)
        {
            ManagerValue.actionViewBuildFactory = MGBuildBaseFarm;
        }
        if (ManagerValue.actionViewBuildMain == null)
        {
            ManagerValue.actionViewBuildMain = MGBuildBaseMain;
        }
        if (ManagerValue.actionViewBuildMainDate == null)
        {
            ManagerValue.actionViewBuildMainDate = UpdateDateView;
        }

        btnDemolishBuild.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Demolish);
    }
    public override void Hide()
    {
        base.Hide();
        if (intIndexGround != -1)
        {
            messageClose.intIndexGround = intIndexGround;
            messageClose.messageType = MGViewToBuildBase.EnumMessageType.Close;
            SendToGround(messageClose);
        }
        intIndexGround = -1;
    }
    /// <summary>
    /// ControllerGround.cs 发来的消息，处理场地状态
    /// </summary>
    public override void SetData(Message message)
    {
        ViewMGToViewInfoFarm mgViewFarm = message as ViewMGToViewInfoFarm;
        ViewMGToViewInfoPasture mgViewPasture = message as ViewMGToViewInfoPasture;
        ViewMGToViewInfoFactory mgViewFactory = message as ViewMGToViewInfoFactory;
        ViewMGToViewInfoTower mgViewTower = message as ViewMGToViewInfoTower;
        if (mgViewFarm != null)
        {
            ViewSetDataUp(ViewBuild_Base.EnumViewUp.ViewBuild_FarmProduct, mgViewFarm);
        }
        if (mgViewPasture != null)
        {
            ViewSetDataUp(ViewBuild_Base.EnumViewUp.ViewBuild_PastureProduct, mgViewPasture);
        }
        if (mgViewFactory != null)
        {
            ViewSetDataUp(ViewBuild_Base.EnumViewUp.ViewBuild_ProductFactory, mgViewFactory);
        }
        if (mgViewTower != null)
        {
            textTitle.text = ManagerBuild.Instance.GetBuildName(mgViewTower.intBuildID);
            ViewShowUp(ViewBuild_Base.EnumViewUp.ViewBuild_Tower, null);
            ViewShowDown(ViewBuild_Base.EnumViewDown.None, null);
            ViewSetDataUp(ViewBuild_Base.EnumViewUp.ViewBuild_Tower, mgViewTower);
        }

        ViewMGEmployeeAddTo mgEmployeeAdd = message as ViewMGEmployeeAddTo;
        if (mgEmployeeAdd != null)
        {
            PropertiesEmployee itemEmployee = UserValue.Instance.GetEmployeeValue(mgEmployeeAdd.intEmployeeID);
            itemEmployee.intIndexGroundWork = intIndexGround;
            itemEmployee.enumLocation = EnumEmployeeLocation.Ground;

            mgEmployeeAdd.intIndexSize = intIndexSize;
            dicViewDown[ViewBuild_Base.EnumViewDown.ViewBuild_Employee].SetData(mgEmployeeAdd);
        }
    }

    /// <summary>
    /// 消息来源：ControllerGround，夹带BuildBase中的MG实现
    /// </summary>
    void MGBuildBaseFarm(EventBuildToViewBase mgBuild)
    {
        textTitle.text = ManagerBuild.Instance.GetBuildName(mgBuild.intBuildID);

        EventBuildToViewFarm mgToInfoFarm = mgBuild as EventBuildToViewFarm;
        EventBuildToViewPasture mgToInfoPasture = mgBuild as EventBuildToViewPasture;
        EventBuildToViewFactory mgToInfoFactory = mgBuild as EventBuildToViewFactory;

        btnDemolishBuild.gameObject.SetActive(true);

        //处理未关闭界面状况下,切换地导致未关闭发送消息的问题
        if (intIndexGround != -1 && intIndexGround != mgBuild.intIndexGround)
        {
            messageClose.intIndexGround = intIndexGround;
            messageClose.messageType = MGViewToBuildBase.EnumMessageType.Close;
            SendToGround(messageClose);
            ViewSubHide();
        }
        intIndexGround = mgBuild.intIndexGround;

        if (mgToInfoFarm != null)
        {
            ViewShowUp(ViewBuild_Base.EnumViewUp.ViewBuild_FarmProduct, mgToInfoFarm);
            ViewShowDown(ViewBuild_Base.EnumViewDown.ViewBuild_Employee, mgToInfoFarm);

            if (mgToInfoFarm != null)
            {
                //intTempGround = mgToInfoFarm.intIndexGround;
                for (int i = 0; i < intOres.Length; i++)
                {
                    if (intOres[i] == mgToInfoFarm.intBuildID)
                    {
                        btnDemolishBuild.gameObject.SetActive(false);
                        break;
                    }
                }
            }
        }
        if (mgToInfoPasture != null)
        {
            ViewShowUp(ViewBuild_Base.EnumViewUp.ViewBuild_PastureProduct, mgToInfoPasture);
            ViewShowDown(ViewBuild_Base.EnumViewDown.ViewBuild_Employee, mgToInfoPasture);
        }
        if (mgToInfoFactory != null)
        {
            ViewShowUp(ViewBuild_Base.EnumViewUp.ViewBuild_ProductFactory, mgToInfoFactory);
            ViewShowDown(ViewBuild_Base.EnumViewDown.None, mgToInfoPasture);
        }


        for (int i = 0; i < buildProperties.items.Length; i++)
        {
            //if (i < enumEmployeeProperties.Length)
            //{
            //    Sprite sprite = ManagerResources.Instance.GetEmployeeProperties(enumEmployeeProperties[i]);
            //    buildProperties.items[i].imageValueMain.sprite = sprite;
            //    buildProperties.items[i].gameObject.SetActive(true);
            //}
            //else
            //{
            //    buildProperties.items[i].gameObject.SetActive(false);
            //}
            buildProperties.items[i].gameObject.SetActive(false);
        }
    }

    void MGBuildBaseMain(EventBuildToViewBase mgBuild)
    {
        btnDemolishBuild.gameObject.SetActive(true);
        JsonValue.DataTableBuildingItem build = ManagerBuild.Instance.GetBuildItem(mgBuild.intBuildID);
        if (build != null)
        {
            textTitle.text = ManagerBuild.Instance.GetBuildName(mgBuild.intBuildID);
        }
        else
        {
            switch (mgBuild.enumKeyUp)
            {
                case ViewBuild_Base.EnumViewUp.ViewBuild_BuyGround:
                    ViewBuild_Base.GroundToView mgGroundBuy = mgBuild as ViewBuild_Base.GroundToView;
                    if (mgGroundBuy.groundState == EnumGroudState.Hinder)
                    {
                        textTitle.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Obstacle);
                        btnDemolishBuild.gameObject.SetActive(false);
                    }
                    else if (mgGroundBuy.groundState == EnumGroudState.Unpurchased)
                    {
                        textTitle.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Wasteland);
                        btnDemolishBuild.gameObject.SetActive(false);
                    }
                    break;
                case ViewBuild_Base.EnumViewUp.ViewBuild_BuyBuild:
                    textTitle.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Land);

                    break;
            }
        }

        if (mgBuild.enumKeyUp == ViewBuild_Base.EnumViewUp.ViewBuild_BuildConstruction)
        {
            btnDemolishBuild.gameObject.SetActive(false);
        }

        //处理未关闭界面状况下,切换地导致未关闭发送消息的问题
        if (intIndexGround != -1 && intIndexGround != mgBuild.intIndexGround)
        {
            messageClose.intIndexGround = intIndexGround;
            messageClose.messageType = MGViewToBuildBase.EnumMessageType.Close;
            SendToGround(messageClose);
            ViewSubHide();
        }

        intIndexGround = mgBuild.intIndexGround;
        ViewShowUp(mgBuild.enumKeyUp, mgBuild);
        ViewShowDown(mgBuild.enumKeyDown, mgBuild);
    }

    void ViewShowUp(ViewBuild_Base.EnumViewUp enumKey, EventBuildToViewBase message)
    {
        if (!dicViewUp.ContainsKey(enumKey) && enumKey != ViewBuild_Base.EnumViewUp.None)
        {
            GameObject goView = Resources.Load<GameObject>("ViewSubPrefabs/" + enumKey.ToString());
            goView = GameObject.Instantiate(goView, rectUp, false);
            goView.transform.localPosition = Vector3.zero;
            dicViewUp.Add(enumKey, goView.GetComponent<ViewBuild_Base>());
            dicViewUp[enumKey].SendToGround = SendToGround;
            dicViewUp[enumKey].getCentre = GetCentre;
        }
        foreach (ViewBuild_Base.EnumViewUp temp in dicViewUp.Keys)
        {
            if (enumKey == temp)
            {
                dicViewUp[temp].Show();
            }
            else
            {
                dicViewUp[temp].Hide();
            }
        }
        if (enumKey != ViewBuild_Base.EnumViewUp.None)
        {
            dicViewUp[enumKey].BuildMessage(message);
        }
    }

    void ViewShowDown(ViewBuild_Base.EnumViewDown enumKey, EventBuildToViewBase message)
    {
        if (!dicViewDown.ContainsKey(enumKey) && enumKey != ViewBuild_Base.EnumViewDown.None)
        {
            GameObject goView = Resources.Load<GameObject>("ViewSubPrefabs/" + enumKey.ToString());
            goView = GameObject.Instantiate(goView, rectDown, false);
            goView.transform.localPosition = Vector3.zero;
            dicViewDown.Add(enumKey, goView.GetComponent<ViewBuild_Base>());
            dicViewDown[enumKey].SendToGround = SendToGround;
            dicViewDown[enumKey].actionEmployeeAdd = ActionEmpoyeeAdd;
            dicViewDown[enumKey].getCentre = GetCentre;
        }
        foreach (ViewBuild_Base.EnumViewDown temp in dicViewDown.Keys)
        {
            if (enumKey == temp)
            {
                dicViewDown[temp].Show();
            }
            else
            {
                dicViewDown[temp].Hide();
            }
        }
        if (enumKey != ViewBuild_Base.EnumViewDown.None)
        {
            dicViewDown[enumKey].BuildMessage(message);
        }
    }

    void ViewSubHide()
    {
        foreach (ViewBuild_Base.EnumViewUp temp in dicViewUp.Keys)
        {
            dicViewUp[temp].HideSub();
        }
        foreach (ViewBuild_Base.EnumViewDown temp in dicViewDown.Keys)
        {
            dicViewDown[temp].HideSub();
        }
    }

    void UpdateDateView(ViewGroundToBuildMainDateBase message)
    {
        if (dicViewUp.ContainsKey(message.enumUp))
        {
            dicViewUp[message.enumUp].SetDate(message);
        }
        if (dicViewDown.ContainsKey(message.enumDown))
        {
            dicViewDown[message.enumDown].SetDate(message);
        }
    }

    ViewBuild_Base GetCentre(ViewBuild_Base.EnumViewCentre enumKey)
    {
        GameObject goView = Resources.Load<GameObject>("ViewSubPrefabs/" + enumKey.ToString());
        goView = GameObject.Instantiate(goView, rectCentre, false);
        goView.transform.localPosition = Vector3.zero;
        ViewBuild_Base viewBuild = goView.GetComponent<ViewBuild_Base>();
        viewBuild.getCentre = GetCentre;
        return goView.GetComponent<ViewBuild_Base>();
    }

    void ViewSetDataUp(ViewBuild_Base.EnumViewUp enumKey, ViewBase.Message message)
    {
        if (dicViewUp.ContainsKey(enumKey))
        {
            dicViewUp[enumKey].SetData(message);
        }
    }

    void ActionEmpoyeeAdd(int intIndex)
    {
        intIndexSize = intIndex;

        employeeData.booEmployee = true;
        employeeData.intIndex = intIndex;
        employeeData.enumView = EnumView.ViewBuildMain;
        employeeData.enumEmployeeProperties = enumEmployeeProperties;
        employeeData.dicPropertiesInfo = new Dictionary<EnumEmployeeProperties, string>();

        EmployeeAdd(employeeData);
    }

    void SendToGround(MGViewToBuildBase message)
    {
        ManagerValue.actionGround(intIndexGround, message);
    }

    private void OnDestroy()
    {
        ManagerValue.actionViewBuildFarmInfo -= MGBuildBaseFarm;
        ManagerValue.actionViewBuildFarmInfo = null;

        ManagerValue.actionViewBuildPasture -= MGBuildBaseFarm;
        ManagerValue.actionViewBuildPasture = null;

        ManagerValue.actionViewBuildFactory -= MGBuildBaseFarm;
        ManagerValue.actionViewBuildFactory = null;

        ManagerValue.actionViewBuildMain -= MGBuildBaseMain;
        ManagerValue.actionViewBuildMain = null;

        ManagerValue.actionViewBuildMainDate -= UpdateDateView;
        ManagerValue.actionViewBuildMainDate = null;
    }

    enum EnumBuild
    {
        None,
        Farm,
        Pasture,
        Factory,
    }
}
