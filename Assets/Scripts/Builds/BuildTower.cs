using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 城堡
/// </summary>
public class BuildTower : BuildBase
{
    int intEmployeeMaxNum = 15;//员工最大数量
    int intBuildRank;//城堡等级
    Dictionary<int, PropertiesEmployee> dicEmployee = new Dictionary<int, PropertiesEmployee>();
    ViewMGToViewInfoTower mgViewInfo = new ViewMGToViewInfoTower();
    ViewBuild_Base.TowerToView messageTower = new ViewBuild_Base.TowerToView();
    string[] strReads = null;

    //每年的这个日期增加一个员工，最多20个员工
    int intMonth;
    int intDay;
    public override void OnStart()
    {
        base.OnStart();

        if (strReads == null)
        {
            proBuild = new PropertiesBuild();
            JsonValue.DataTableBuildingItem itemBuild = ManagerBuild.Instance.GetBuildItem(GetBuildID);
            proBuild.numPrice = itemBuild.numPrice;
            proBuild.intBuildType = itemBuild.intBuildType;
            proBuild.intBuildID = GetBuildID;
            proBuild.strBuildName = ManagerBuild.Instance.GetBuildName(GetBuildID);
            proBuild.strModelName = itemBuild.strModelName;

            for (int i = 0; i < 2; i++)
            {
                AddEmployee();
            }

            intMonth = ManagerValue.intMonth;
            intDay = ManagerValue.intDay;
        }
    }

    void AddEmployee()
    {
        PropertiesEmployee itemEmployee = UserValue.Instance.AddEmployee();
        itemEmployee.intIndexGround = GetIndexGround;
        itemEmployee.enumState = EnumEmployeeState.NoHire;
        itemEmployee.enumIdentification = EnumEmployeeIdentification.Farmer;
        dicEmployee.Add(itemEmployee.intIndexID, itemEmployee);
    }

    public override void GameReadData(string strData)
    {
        strReads = strData.Split('_');
        int intEmployeeCount = int.Parse(strReads[1]);
        for (int i = 3; i < 3 + intEmployeeCount; i++)
        {
            PropertiesEmployee item = UserValue.Instance.GetEmployeeValue(int.Parse(strReads[i]));
            dicEmployee.Add(item.intIndexID, item);
        }
        intMonth = int.Parse(strReads[3 + intEmployeeCount]);
        intDay = int.Parse(strReads[3 + intEmployeeCount + 1]);
    }
    /// <summary>
    /// 组成形式；数据类型个数_数据类型一长度_数据类型二长度_数据_数据
    /// </summary>
    public override string GameSaveData()
    {
        //数据类型两个_第一个数据类型员工长度_第二个数据类型月和日两个
        string strReadData = "2_" + dicEmployee.Count + "_" + "2_";
        //int intIndexTemp = 0;
        foreach (PropertiesEmployee temp in dicEmployee.Values)
        {
            //if (intIndexTemp++ == dicEmployee.Count - 1)
            //{
            //    strReadData += temp.intIndexID;
            //    continue;
            //}
            strReadData += temp.intIndexID + "_";
        }
        strReadData += intMonth + "_" + intDay;
        return strReadData;
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        if (mgData.numDay == intDay && mgData.numMonth == intMonth && dicEmployee.Count < intEmployeeMaxNum)
        {
            AddEmployee();
            if (booBuildToView)
            {
                ManagerValue.actionViewBuildMain(messageTower);
            }
        }
    }
    public override void MGViewBuildInfo(MGViewToBuildBase toGround)
    {
        ViewBuild_Base.DemolishBuild mg = toGround as ViewBuild_Base.DemolishBuild;
        if (mg != null)
        {
            ViewHint.MessageHint viewHint = new ViewHint.MessageHint();
            ManagerValue.actionAudio(EnumAudio.Ground);
            List<int> listEmployWork = new List<int>();
            foreach (PropertiesEmployee temp in dicEmployee.Values)
            {
                if (temp.intIndexGroundWork != -1)
                {
                    listEmployWork.Add(temp.intIndexID);
                }
            }
            if (listEmployWork.Count > 0)
            {
                //"有" + listEmployWork.Count + "名员工在工作中,拆除建筑后,员工将离开岗位,并失去这些员工。";
                viewHint.strHint = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.EmployeesAAWDWCTTLTP, new string[] { listEmployWork.Count.ToString() });
            }
            else
            {
                //"拆除建筑后,将失去这些员工。"; 
                viewHint.strHint = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.AfterDEWBL, new string[] { dicEmployee.Count.ToString() });
            }
            BuildBase build = UserValue.Instance.GetBuildValue(GetIndexGround);
            viewHint.strHint += "\n" + ManagerValue.DemolishBuildRecycleCoin(build.GetBuildID) + ManagerLanguage.Instance.GetWord(EnumLanguageWords.GoldCoins);

            viewHint.actionConfirm = () =>
            {
                foreach (PropertiesEmployee temp in dicEmployee.Values)
                {
                    UserValue.Instance.RecyclingStationEmployee(temp.intIndexID);
                }
                MessageDemolishBuild mgDemolishBuild = new MessageDemolishBuild();
                mgDemolishBuild.intIndexGround = GetIndexGround;
                ManagerMessage.Instance.PostEvent(EnumMessage.DemolishBuild, mgDemolishBuild);
                ManagerView.Instance.Hide(EnumView.ViewBuildMain);
            };
            ManagerView.Instance.Show(EnumView.ViewHint);
            ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
        }

        ViewBuild_Base.CloseMessage messageClose = toGround as ViewBuild_Base.CloseMessage;
        if (messageClose != null)
        {
            booBuildToView = false;
        }
    }

    public override void ShowViewBuildInfo()
    {
        booBuildToView = true;
        //ManagerView.Instance.Show(EnumView.ViewHouse);
        ManagerView.Instance.Show(EnumView.ViewBuildMain);
        mgViewInfo.intBuildID = GetBuildID;
        mgViewInfo.intEmployeeMaxNum = intEmployeeMaxNum;
        mgViewInfo.dicEmployee = dicEmployee;
        mgViewInfo.intTowerRank = intBuildRank;
        mgViewInfo.intMonth = intMonth;
        mgViewInfo.intDay = intDay;
        mgViewInfo.intIndexGround = GetIndexGround;
        mgViewInfo.strBuildName = ManagerBuild.Instance.GetBuildName(proBuild.intBuildID);//proBuild.strBuildName;
        //ManagerView.Instance.SetData(EnumView.ViewBuildMain, mgViewInfo);

        messageTower.enumKeyUp = ViewBuild_Base.EnumViewUp.ViewBuild_Tower;
        messageTower.enumKeyDown = ViewBuild_Base.EnumViewDown.None;
        messageTower.intIndexGround = GetIndexGround;
        messageTower.intBuildID = GetBuildID;

        messageTower.intDay = intDay;
        messageTower.intMonth = intMonth;
        messageTower.intPersonMax = intEmployeeMaxNum;
        messageTower.dicEmployee = dicEmployee;
        ManagerValue.actionViewBuildMain(messageTower);

    }
}
