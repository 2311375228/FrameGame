using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildBaseFarm : BuildBase
{
    protected JsonValue.DataTableCompoundItem itemCompound;
    protected int[] intProductCompounds;//可合成的产品ID

    protected float floResidueDay;//剩余天数

    //位置中的员工
    protected int[] intEmployeeSizes = new int[] { -1, -1 };
    protected int[] intEmployeeChangeValue = new int[] { -1, -1 };//检查值是否改变,因为员工系统中没有全局通知的机制
    PropertiesEmployee employeeTemp;
    //刷新ViewBuildFarmInfo的数据 每秒发送一次
    protected ViewMGToViewInfoFarm mgGroundToViewBuildInfo = new ViewMGToViewInfoFarm();
    //推消息给ViewBuildFarmInfo
    protected EventBuildToViewFarm mgViewBuildInfo = new EventBuildToViewFarm();
    protected string strReadData = null;
    public override void OnStart()
    {
        base.OnStart();

        intStockMax = 5000;

        //拿到建筑生产产品的产品合成ID集合
        intProductCompounds = ManagerBuildCompound.Instance.GetBuildCompoundProduct(proBuild.intBuildID);

        //拿到产品合成ID
        itemCompound = ManagerCompound.Instance.GetValue(intProductCompounds[0]);
        if (strReadData == null)
        {
            if (GetIndexGround >= 0 && floResidueDay <= 0)
            {
                intFarmProductID = itemCompound.intProductID;
                intFarmProductCountsing = itemCompound.intProductCount;
                intFarmRipeDay = itemCompound.intRipeDay;
                floResidueDay = itemCompound.intRipeDay;
            }
            UserValue.Instance.BuildProductSeeAdd(GetBuildID, GetIndexGround);
            UserValue.Instance.UpdateStock();
        }
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        //Debug.Log("每一天" + GetIndexGround + ":" + GetBuildID + ":" + GetBuildType);
        CheckEmployeeState();

        floResidueDay -= 1;
        if (floResidueDay <= 0)
        {
            floResidueDay = intFarmRipeDay;
            UserValue.Instance.StockCountAdd(intFarmProductID, intFarmProductCountsing);
        }

        if (booBuildToView)
        {
            mgGroundToViewBuildInfo.intResidueTime = (int)floResidueDay;
            mgGroundToViewBuildInfo.buildState = ViewBuild_Base.BuildTipsState.Planting;
            ManagerView.Instance.SetData(EnumView.ViewBuildMain, mgGroundToViewBuildInfo);
        }

    }

    //消息来自 ViewBuildFarmInfo
    public override void MGViewBuildInfo(MGViewToBuildBase toGround)
    {
        MGViewToBuildFarm mgBuild = toGround as MGViewToBuildFarm;
        if (mgBuild != null)
        {

            if (mgBuild.intIndexEmployeeSize != -1)
            {
                intEmployeeSizes[mgBuild.intIndexEmployeeSize] = mgBuild.intRoleEmployeeID;
                mgBuild.intIndexEmployeeSize = -1;
            }
        }

        ViewBuild_Base.CloseMessage mgClose = toGround as ViewBuild_Base.CloseMessage;
        if (mgClose != null)
        {
            booBuildToView = false;
        }

        ViewBuild_Base.DemolishBuild mgDemolish = toGround as ViewBuild_Base.DemolishBuild;
        if (mgDemolish!=null)
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

    protected virtual void CheckEmployeeState()
    {
        int intTemp = itemCompound.intProductCount;
        if (intEmployeeSizes[0] != -1)
        {
            employeeTemp = UserValue.Instance.GetEmployeeValue(intEmployeeSizes[0]);
            if (employeeTemp == null || employeeTemp.enumState == EnumEmployeeState.Delete)
            {
                intEmployeeSizes[0] = -1;
            }
            else
            {
                if (itemCompound.intProductCount < 5)
                {
                    intTemp += 1;
                }
                else
                {
                    intTemp += (int)(itemCompound.intProductCount * 0.2f);
                }
            }
        }
        if (intEmployeeSizes[1] != -1)
        {
            employeeTemp = UserValue.Instance.GetEmployeeValue(intEmployeeSizes[1]);
            if (employeeTemp == null || employeeTemp.enumState == EnumEmployeeState.Delete)
            {
                intEmployeeSizes[1] = -1;
            }
            else
            {
                if (itemCompound.intProductCount < 5)
                {
                    intTemp += 1;
                }
                else
                {
                    intTemp += (int)(itemCompound.intProductCount * 0.2f);
                }
            }
        }
        intFarmProductCountsing = intTemp;

        //当员工状态改变,或当UserValue.Instance.GetEmployeeValue(id)==null,则会被检查出来
        if (intEmployeeSizes[0] != intEmployeeChangeValue[0] || intEmployeeSizes[1] != intEmployeeChangeValue[1])
        {
            intEmployeeChangeValue[0] = intEmployeeSizes[0];
            intEmployeeChangeValue[1] = intEmployeeSizes[1];
            UserValue.Instance.UpdateStock();
            if (booBuildToView)
            {
                ShowViewBuildInfo();
            }
        }
    }

    public override void ShowViewBuildInfo()
    {
        CheckEmployeeState();

        booBuildToView = true;
        ManagerView.Instance.Hide(EnumView.ViewHouse);
        ManagerView.Instance.Show(EnumView.ViewBuildMain);
        mgViewBuildInfo.buildState = ViewBuild_Base.BuildTipsState.Planting;
        mgViewBuildInfo.intIndexGround = GetIndexGround;
        mgViewBuildInfo.intBuildID = proBuild.intBuildID;
        mgViewBuildInfo.intProductID = intFarmProductID;
        mgViewBuildInfo.intProductCount = intFarmProductCountsing;
        mgViewBuildInfo.intProductRipeDay = intFarmRipeDay;
        mgViewBuildInfo.intCompoundID = itemCompound.intCompoundID;
        mgViewBuildInfo.enumEmployeeProperties = enumEmployeeProperties;
        mgViewBuildInfo.dicPropertiesInfo = dicEmployeePropertiesInfo;
        mgViewBuildInfo.intEmployeeSizes = intEmployeeSizes;
        mgViewBuildInfo.intResidueTime = (int)floResidueDay;
        ManagerValue.actionViewBuildFarmInfo(mgViewBuildInfo);
    }

    public override string GameSaveData()
    {
        string strData = intFarmProductID + "_" +
            intFarmProductCountsing + "_" +
            intFarmRipeDay + "_" +
            floResidueDay + "_" +
            intEmployeeSizes[0] + "_" +
            intEmployeeSizes[1];
        return strData;
    }

    public override void GameReadData(string strData)
    {
        if (strData == null || strData == "")
        {
            return;
        }
        strReadData = strData;

        int intIndex = 0;
        string[] strDatas = strData.Split('_');
        intFarmProductID = int.Parse(strDatas[intIndex++]);
        intFarmProductCountsing = int.Parse(strDatas[intIndex++]);
        intFarmRipeDay = int.Parse(strDatas[intIndex++]);
        floResidueDay = int.Parse(strDatas[intIndex++]);
        intEmployeeSizes[0] = int.Parse(strDatas[intIndex++]);
        intEmployeeSizes[1] = int.Parse(strDatas[intIndex++]);

        intEmployeeChangeValue[0] = intEmployeeSizes[0];
        intEmployeeChangeValue[1] = intEmployeeSizes[1];
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        UserValue.Instance.BuildProductSeeReduce(GetBuildID, GetIndexGround);
        UserValue.Instance.UpdateStock();
    }
}
