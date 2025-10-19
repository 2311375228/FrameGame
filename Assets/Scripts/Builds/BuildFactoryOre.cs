using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildFactoryOre : BuildBaseFarm
{
    public Animator anim;

    //初始化建筑都是false，执行逻辑中会检查当前产能值，并将产量赋值为0
    bool booPlay = false;
    //因为要将产量设置为0,所以在这里做一个记录
    PropertiesEmployee employeeTemp;
    public override void OnStart()
    {
        base.OnStart();

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Strengt, "力量");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Agility, "敏捷");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Stamina, "耐力");
        intFarmProductCountsing = 0;
        UserValue.Instance.BuildProductSeeAdd(GetBuildID, GetIndexGround);
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        //冶炼工坊,只要有一个工人就可以正常执行
        if (intEmployeeSizes[0] != -1 || intEmployeeSizes[1] != -1)
        {
            if (booPlay == false)
            {
                booPlay = true;
                anim.SetBool("play", true);       
            }
            CheckEmployeeState();
            floResidueDay -= 1;
            if (floResidueDay <= 0)
            {
                floResidueDay = intFarmRipeDay;
                UserValue.Instance.StockCountAdd(intFarmProductID, intFarmProductCountsing);
            }

            mgGroundToViewBuildInfo.buildState = ViewBuild_Base.BuildTipsState.Minig;
            if (booBuildToView)
            {
                mgGroundToViewBuildInfo.intResidueTime = (int)floResidueDay;
                ManagerView.Instance.SetData(EnumView.ViewBuildMain, mgGroundToViewBuildInfo);
            }
        }
        else
        {
            if (booPlay == true)
            {
                CheckEmployeeState();
                intFarmProductCountsing = 0;
                booPlay = false;
                anim.SetBool("play", false);
            }
            mgGroundToViewBuildInfo.buildState = ViewBuild_Base.BuildTipsState.NotWorker;
            if (booBuildToView)
            {
                ManagerView.Instance.SetData(EnumView.ViewBuildMain, mgGroundToViewBuildInfo);
            }

            if (intEmployeeChangeValue[0] != intEmployeeSizes[0] || intEmployeeChangeValue[1] != intEmployeeSizes[1])
            {
                intEmployeeChangeValue[0] = intEmployeeSizes[0];
                intEmployeeChangeValue[1] = intEmployeeSizes[1];
                if (booBuildToView)
                {
                    ShowViewBuildInfo();
                }
            }
        }
    }

    protected override void CheckEmployeeState()
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

    public override void ShowViewBuildInfo()
    {
        booBuildToView = true;
        ManagerView.Instance.Show(EnumView.ViewBuildMain);
        mgViewBuildInfo.buildState = mgGroundToViewBuildInfo.buildState;
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
}