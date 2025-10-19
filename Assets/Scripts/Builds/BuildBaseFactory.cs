using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildBaseFactory : BuildBase
{

    public Animator anim;

    //取消工坊合成时间限制,考虑以下这个

    protected int[] proCompoundIDs;//可生产的产品ID，参数种类和数量在表里面获取
    //一个工厂产品可以由多个产品合成
    //开始生产，要检查源材料够不够用

    int[] intCompoundingIDs = new int[] { -1, -1, -1, -1 };
    int[] intCompoundResidueTimes = new int[] { -1, -1, -1, -1 };
    int[] intCompoundingCoins = new int[] { -1, -1, -1, -1 };

    int[] intEmployeeSizes = new int[] { -1, -1 };
    string strReadData = null;
    PropertiesEmployee employeeTemp;
    ViewMGToViewInfoFactory mgUpdateFactory = new ViewMGToViewInfoFactory();
    EventBuildToViewFactory mgViewBuildInfo = new EventBuildToViewFactory();
    MessageMail mgMail = new MessageMail();
    ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();
    public override void OnStart()
    {
        base.OnStart();

        mgMail.enumMail = ViewBarTop_ItemMail.EnumMail.Hammer;
        mgMail.gridItems = new EnumKnapsackStockType[] { EnumKnapsackStockType.Fasture };
        mgMail.strContent = proBuild.strBuildName;
        mgMail.intIndexIDs = new int[1];
        mgMail.intRanks = new int[] { 1 };
        mgMail.intIndexCounts = new int[1];

        proCompoundIDs = ManagerBuildCompound.Instance.GetBuildCompoundProduct(proBuild.intBuildID);
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        CheckEmployeeState();

        bool boo = true;
        if (intCompoundResidueTimes[0] > 0)
        {
            intCompoundResidueTimes[0]--;
            if (intCompoundResidueTimes[0] == 0)
            {
                FinishProduction(intCompoundingIDs[0]);
                intCompoundingIDs[0] = -1;
                intCompoundingCoins[0] = -1;
                intCompoundResidueTimes[0] = -1;
            }
            boo = false;
        }
        if (intCompoundResidueTimes[1] > 0)
        {
            intCompoundResidueTimes[1]--;
            if (intCompoundResidueTimes[1] == 0)
            {
                FinishProduction(intCompoundingIDs[1]);
                intCompoundingIDs[1] = -1;
                intCompoundingCoins[1] = -1;
                intCompoundResidueTimes[1] = -1;
            }
            boo = false;
        }
        if (intCompoundResidueTimes[2] > 0)
        {
            intCompoundResidueTimes[2]--;
            if (intCompoundResidueTimes[2] == 0)
            {
                FinishProduction(intCompoundingIDs[2]);
                intCompoundingIDs[2] = -1;
                intCompoundingCoins[2] = -1;
                intCompoundResidueTimes[2] = -1;
            }
            boo = false;
        }
        if (intCompoundResidueTimes[3] > 0)
        {
            intCompoundResidueTimes[3]--;
            if (intCompoundResidueTimes[3] == 0)
            {
                FinishProduction(intCompoundingIDs[3]);
                intCompoundingIDs[3] = -1;
                intCompoundingCoins[3] = -1;
                intCompoundResidueTimes[3] = -1;
            }
            boo = false;
        }

        if (boo)
        {
            anim.SetBool("play", false);
        }
        else
        {
            anim.SetBool("play", true);
        }

        if (booBuildToView)
        {
            mgUpdateFactory.intCompoundResidueTimes = intCompoundResidueTimes;
            ManagerView.Instance.SetData(EnumView.ViewBuildMain, mgUpdateFactory);
        }
    }
    void FinishProduction(int intCompoundID)
    {
        JsonValue.DataTableCompoundItem itemCompound = ManagerCompound.Instance.GetValue(intCompoundID);

        mgMail.intIndexIDs[0] = itemCompound.intProductID;
        mgMail.intIndexCounts[0] = itemCompound.intProductCount;
        ManagerMessage.Instance.PostEvent(EnumMessage.Mail, mgMail);
    }
    public override void MGViewBuildInfo(MGViewToBuildBase toGround)
    {
        MGViewToBuildFactory mg = toGround as MGViewToBuildFactory;
        if (mg != null)
        {
            if (booBuildToView)
            {
                SendShowData();
            }
        }

        ViewBuild_Base.CloseMessage mgClose = toGround as ViewBuild_Base.CloseMessage;
        if (mgClose != null)
        {
            booBuildToView = false;
        }

        ViewBuild_Base.DemolishBuild mgDemolish = toGround as ViewBuild_Base.DemolishBuild;
        if (mgDemolish != null)
        {
            for (int i = 0; i < intEmployeeSizes.Length; i++)
            {
                if (intEmployeeSizes[i] != -1)
                {
                    UserValue.Instance.GetEmployeeValue(intEmployeeSizes[i]).intIndexGroundWork = -1;
                }
            }
        }

        ViewBuild_Base.SetEmployee mgEmployee = toGround as ViewBuild_Base.SetEmployee;
        if (mgEmployee != null)
        {
            if (mgEmployee.intEmployeeID != -1)
            {
                intEmployeeSizes[mgEmployee.intIndexSize] = mgEmployee.intEmployeeID;
            }
            else
            {
                PropertiesEmployee itemEmployee = UserValue.Instance.GetEmployeeValue(intEmployeeSizes[mgEmployee.intIndexSize]);
                itemEmployee.enumLocation = EnumEmployeeLocation.None;
                itemEmployee.intIndexGroundWork = -1;
                intEmployeeSizes[mgEmployee.intIndexSize] = -1;
            }
            ManagerValue.actionViewBuildFactory(mgViewBuildInfo);
        }
    }

    public override void ShowViewBuildInfo()
    {
        booBuildToView = true;
        ManagerView.Instance.Hide(EnumView.ViewHouse);
        ManagerView.Instance.Show(EnumView.ViewBuildMain);
        SendShowData();
    }
    void CheckEmployeeState()
    {
        if (intEmployeeSizes[0] != -1)
        {
            employeeTemp = UserValue.Instance.GetEmployeeValue(intEmployeeSizes[0]);
            if (employeeTemp.enumState == EnumEmployeeState.Delete)
            {
                intEmployeeSizes[0] = -1;
            }
        }
        if (intEmployeeSizes[1] != -1)
        {
            employeeTemp = UserValue.Instance.GetEmployeeValue(intEmployeeSizes[1]);
            if (employeeTemp.enumState == EnumEmployeeState.Delete)
            {
                intEmployeeSizes[1] = -1;
            }
        }
    }
    void SendShowData()
    {
        CheckEmployeeState();

        mgViewBuildInfo.intIndexGround = GetIndexGround;
        mgViewBuildInfo.intBuildID = proBuild.intBuildID;

        mgViewBuildInfo.intCompoundIDs = proCompoundIDs;
        mgViewBuildInfo.intCompoundingIDs = intCompoundingIDs;
        mgViewBuildInfo.intCompoundResidueTimes = intCompoundResidueTimes;
        mgViewBuildInfo.intCompoundingCoins = intCompoundingCoins;

        mgViewBuildInfo.intEmployeeSizes = intEmployeeSizes;
        mgViewBuildInfo.enumEmployeeProperties = enumEmployeeProperties;
        mgViewBuildInfo.dicPropertiesInfo = dicEmployeePropertiesInfo;

        ManagerValue.actionViewBuildFactory(mgViewBuildInfo);
    }

    public override void GameReadData(string strData)
    {
        int intTemp = 0;
        strReadData = strData;
        string[] strs = strData.Split('_');
        for (int i = 0; i < intCompoundingIDs.Length; i++)
        {
            intCompoundingIDs[i] = int.Parse(strs[intTemp++]);
            intCompoundResidueTimes[i] = int.Parse(strs[intTemp++]);
            intCompoundingCoins[i] = int.Parse(strs[intTemp++]);
        }
        intEmployeeSizes[0] = int.Parse(strs[intTemp++]);
        intEmployeeSizes[1] = int.Parse(strs[intTemp++]);
    }
    public override string GameSaveData()
    {
        string strData = "";
        for (int i = 0; i < intCompoundingIDs.Length; i++)
        {
            strData += intCompoundingIDs[i] + "_";
            strData += intCompoundResidueTimes[i] + "_";
            strData += intCompoundingCoins[i] + "_";
        }
        strData += intEmployeeSizes[0] + "_";
        strData += intEmployeeSizes[1];
        return strData;
    }
}
