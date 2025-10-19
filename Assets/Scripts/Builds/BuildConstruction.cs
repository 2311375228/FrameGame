using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildConstruction : BuildBase
{
    public Transform transSaw;
    public Transform transTowerTop;
    public Transform[] transPickaxes;
    public Transform transMinecart;

    int intTargetBuildID;
    float floResidueDay;
    string[] strDatas;
    Vector3 vecAixs = new Vector3(0, 0, -1);
    Vector3 vecAngle = new Vector3(0, 0, 90);
    ViewBuild_Base.ConstructionToView messageConstruction = new ViewBuild_Base.ConstructionToView();
    ViewBuild_Base.ContructionToView_Date messageConstructionDate = new ViewBuild_Base.ContructionToView_Date();
    public override void OnStart()
    {
        base.OnStart();
    }

    //private void Update()
    //{
    //    transSaw.RotateAround(transSaw.position, vecAixs, 100 * Time.deltaTime);

    //    transTowerTop.localEulerAngles = Vector3.Lerp(transTowerTop.localEulerAngles, vecAngle, 100 * Time.deltaTime);
    //    if (Vector3.Distance(transTowerTop.localEulerAngles, vecAngle) < 1)
    //    {
    //    }
    //}

    public void SetValue(int intBuildID)
    {
        intTargetBuildID = intBuildID;
        JsonValue.DataTableBuildingItem itemBuild = ManagerBuild.Instance.GetBuildItem(intBuildID);
        floResidueDay = itemBuild.intTimeBuild;
    }

    public override void GameReadData(string strData)
    {
        int intIndexTemp = 0;
        strDatas = strData.Split("_");
        floResidueDay = float.Parse(strDatas[0]);
        intTargetBuildID = int.Parse(strDatas[1]);
    }
    public override string GameSaveData()
    {
        string strTemp = floResidueDay.ToString() + "_" + intTargetBuildID;
        return strTemp;
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        floResidueDay -= 1;
        if (floResidueDay < 0)
        {
            JsonValue.DataTableBuildingItem buildPr = ManagerBuild.Instance.GetBuildItem(intTargetBuildID);

            //销毁工地
            MessageDemolishBuild mgDemolishBuild = new MessageDemolishBuild();
            mgDemolishBuild.intIndexGround = GetIndexGround;
            ManagerMessage.Instance.PostEvent(EnumMessage.DemolishBuild, mgDemolishBuild);

            //创建目标建筑
            MessageGround mgGround = new MessageGround();
            mgGround.numIndexGround = GetIndexGround;
            mgGround.strModelNameNew = buildPr.strModelName;
            mgGround.strModelNameOld = ManagerBuild.Instance.GetBuildItem(GetBuildID).strModelName;
            mgGround.groundState = EnumGroudState.BuildingPruchased;
            mgGround.intBuildID = intTargetBuildID;
            ManagerMessage.Instance.PostEvent(EnumMessage.Update_Ground, mgGround);
        }
        if (booBuildToView)
        {
            messageConstructionDate.intBuildID = GetBuildID;
            messageConstructionDate.intIndexGround = GetIndexGround;
            messageConstructionDate.floResidueDay = floResidueDay;
            messageConstructionDate.enumUp = ViewBuild_Base.EnumViewUp.ViewBuild_BuildConstruction;

            ManagerValue.actionViewBuildMainDate(messageConstructionDate);
        }
    }

    public override void MGViewBuildInfo(MGViewToBuildBase toGround)
    {
        ViewBuild_Base.CloseMessage messageClose = toGround as ViewBuild_Base.CloseMessage;
        if (messageClose != null)
        {
            booBuildToView = false;
        }
        ViewBuild_Base.ConstructionCancelToBuild messageCancel = toGround as ViewBuild_Base.ConstructionCancelToBuild;
        if (messageCancel != null)
        {
            int intCoin = 100;
            JsonValue.DataTableBuildingItem itemBuild = ManagerBuild.Instance.GetBuildItem(intTargetBuildID);
            UserValue.Instance.SetCoinReduce(itemBuild.numPrice - intCoin);
            ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);

            ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();
            //hintBar.strHintBar = "可以收回" + (itemBuild.numPrice - intCoin) + "金币，已扣除服务费" + intCoin + "金币。";
            string[] strs = new string[2];
            strs[0] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, (itemBuild.numPrice - intCoin).ToString("N0"));
            strs[1] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, intCoin.ToString("N0"));
            hintBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.GoldCCBRATSF, strs);
            ManagerView.Instance.Show(EnumView.ViewHintBar);
            ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);

            MessageDemolishBuild mgDemolishBuild = new MessageDemolishBuild();
            mgDemolishBuild.intIndexGround = GetIndexGround;
            ManagerMessage.Instance.PostEvent(EnumMessage.DemolishBuild, mgDemolishBuild);
            ManagerView.Instance.Hide(EnumView.ViewBuildMain);
        }
    }

    public override void ShowViewBuildInfo()
    {
        booBuildToView = true;

        messageConstruction.intBuildID = GetBuildID;
        messageConstruction.intTargetBuildID = intTargetBuildID;
        messageConstruction.intIndexGround = GetIndexGround;
        messageConstruction.enumKeyUp = ViewBuild_Base.EnumViewUp.ViewBuild_BuildConstruction;
        messageConstruction.floTotalTime = ManagerBuild.Instance.GetBuildItem(intTargetBuildID).intTimeBuild;

        ManagerView.Instance.Show(EnumView.ViewBuildMain);
        ManagerValue.actionViewBuildMain(messageConstruction);
    }
}
