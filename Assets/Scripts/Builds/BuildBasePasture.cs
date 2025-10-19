using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildBasePasture : BuildBase
{
    protected JsonValue.DataTableCompoundItem[] itemCompounds;
    public JsonValue.DataTableCompoundItem[] GetIntProductIDs
    {
        get
        {
            return itemCompounds;
        }
    }

    protected int[] intProductCompounds;//可生产的产品合成ID
    protected int[] intProductIDs;//产品ID
    protected int[] intProductExpendIDs;//可消耗的产品ID
    public int[] IntProductExpendIDs
    {
        get
        {
            return intProductExpendIDs;
        }
    }
    protected int[] intProductExpendIDNums;//可消耗品的数量
    protected int intIndexProductExpend;//消耗的维护品下标
    protected int intIndexProduct;//当前产出的物品下标

    protected ViewMGToViewInfoPasture mgGroundToViewBuildInfo = new ViewMGToViewInfoPasture();
    protected float floResidueDay;//剩余天数

    string strReadData = null;
    PropertiesEmployee employeeTemp;
    int[] intEmployeeSizes = new int[] { -1, -1 };
    protected int[] intEmployeeChangeValue = new int[] { -1, -1 };//检查值是否改变,因为员工系统中没有全局通知的机制
    EventBuildToViewPasture mgViewBuildInfo = new EventBuildToViewPasture();
    ViewHint.MessageHint viewHint = new ViewHint.MessageHint();
    public override void OnStart()
    {
        base.OnStart();

        intStockMax = 5000;

        //为了使表统一一种规格,导致这里实现时有区别
        //拿到合成ID
        intProductCompounds = ManagerBuildCompound.Instance.GetBuildCompoundProduct(proBuild.intBuildID);
        intProductIDs = new int[intProductCompounds.Length];
        for (int i = 0; i < intProductCompounds.Length; i++)
        {
            intProductIDs[i] = ManagerCompound.Instance.GetValue(intProductCompounds[i]).intProductID;
        }
        //拿到产品ID
        itemCompounds = new JsonValue.DataTableCompoundItem[intProductCompounds.Length];
        for (int i = 0; i < intProductCompounds.Length; i++)
        {
            itemCompounds[i] = ManagerCompound.Instance.GetValue(intProductCompounds[i]);
            //拿到其中一个即可,消耗品都是同样的数量,同种类型
            intProductExpendIDs = itemCompounds[i].intPorductIDStuff;
            intProductExpendIDNums = itemCompounds[i].intPorductIDnum;
        }

        if (floResidueDay <= 0)
        {
            intPastureProductID = itemCompounds[intIndexProduct].intProductID;
            intPastureProductCount = itemCompounds[intIndexProduct].intProductCount;
            intPastureRipeDay = itemCompounds[intIndexProduct].intRipeDay;
            intPastureExpendProductID = intProductExpendIDs[intIndexProductExpend];
            intPastureExpendProductCount = intProductExpendIDNums[intIndexProductExpend];
        }

        UserValue.Instance.BuildProductSeeAdd(GetBuildID, GetIndexGround);
        UserValue.Instance.UpdateStock();

        if (strReadData == null)
        {
            floResidueDay = itemCompounds[intIndexProduct].intRipeDay;
        }
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        CheckEmployeeState();
        //检查库存
        if (ProdectExpend())
        {
            //仓库库存充足
            mgGroundToViewBuildInfo.buildState = ViewBuild_Base.BuildTipsState.Expend;
        }
        else
        {
            //仓库库存不足
            mgGroundToViewBuildInfo.buildState = ViewBuild_Base.BuildTipsState.NotExpend;
        }

        ////金币消耗量
        //if (UserValue.Instance.SetCoinReduce(proBuild.intMaintain))
        //{
        //    mgGroundToViewBuildInfo.booGold = true;
        //    mgCoin.numCoin = UserValue.Instance.GetCoin;
        //    ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin, mgCoin);
        //}
        //else
        //{
        //    //viewHint.strHint = "金币不足!";
        //    //ManagerView.Instance.Show(EnumView.ViewHint);
        //    //ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
        //    mgGroundToViewBuildInfo.booGold = false;
        //}
        if (booBuildToView)
        {
            mgGroundToViewBuildInfo.intResidueTime = (int)floResidueDay;
            mgGroundToViewBuildInfo.intRipeDay = itemCompounds[intIndexProduct].intRipeDay;
            ManagerView.Instance.SetData(EnumView.ViewBuildMain, mgGroundToViewBuildInfo);
        }

    }

    /// <summary>
    /// 维护的产品 检查
    /// </summary>
    protected bool ProdectExpend()
    {
        Dictionary<int, FarmClass.StockCount> dicStockCount = UserValue.Instance.GetStockCountAll();
        if (!dicStockCount.ContainsKey(intProductExpendIDs[intIndexProductExpend]))
        {
            return false;
        }
        if (UserValue.Instance.StockCountReduce(intProductExpendIDs[intIndexProductExpend], intProductExpendIDNums[intIndexProductExpend]))
        {
            floResidueDay -= 1;
            if (floResidueDay <= 0)
            {
                //这里更新仓库
                floResidueDay = itemCompounds[intIndexProduct].intRipeDay;
                UserValue.Instance.StockCountAdd(intPastureProductID, intPastureProductCount);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 维护的金币检查
    /// </summary>
    bool CoinExpent()
    {
        return false;
    }

    //消息来自 ViewBuildPastureInfo
    public override void MGViewBuildInfo(MGViewToBuildBase toGround)
    {
        MGViewToBuildPasture mg = toGround as MGViewToBuildPasture;
        if (mg != null)
        {
            if (booBuildToView)
            {

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
            ManagerValue.actionViewBuildPasture(mgViewBuildInfo);
        }

        ViewBuild_Base.SelectPasture messageSelect = toGround as ViewBuild_Base.SelectPasture;
        if (messageSelect != null)
        {
            if (messageSelect.intIndexProduction != -1 && messageSelect.intIndexProduction != intIndexProduct)//更换生产的产品
            {
                intIndexProduct = messageSelect.intIndexProduction;
                floResidueDay = itemCompounds[intIndexProduct].intRipeDay;
                //将新的生产的产品赋生产值
                intPastureProductID = intProductIDs[messageSelect.intIndexProduction];
                //将原有地生产的产品赋值0
                UserValue.Instance.UpdateStock();
            }
            if (messageSelect.intIndexExpend != -1)//更换消耗的产品
            {
                //将新的消耗品 赋值消耗
                intIndexProductExpend = messageSelect.intIndexExpend;
                intPastureExpendProductID = intProductExpendIDs[intIndexProductExpend];
                intPastureExpendProductCount = intProductExpendIDNums[intIndexProductExpend];
                //将原有的消耗品 赋值0
                UserValue.Instance.UpdateStock();
            }

            if (booBuildToView)
            {
                ShowViewBuildInfo();
            }
        }

    }

    protected void CheckEmployeeState()
    {
        int intTemp = itemCompounds[intIndexProduct].intProductCount;
        if (intEmployeeSizes[0] != -1)
        {
            employeeTemp = UserValue.Instance.GetEmployeeValue(intEmployeeSizes[0]);
            if (employeeTemp == null || employeeTemp.enumState == EnumEmployeeState.Delete)
            {
                intEmployeeSizes[0] = -1;
            }
            else
            {
                if (itemCompounds[intIndexProduct].intProductCount < 5)
                {
                    intTemp += 1;
                }
                else
                {
                    intTemp = intTemp + (int)(intTemp * 0.2f);
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
                if (itemCompounds[intIndexProduct].intProductCount < 5)
                {
                    intTemp += 1;
                }
                else
                {
                    intTemp = intTemp + (int)(intTemp * 0.2f);
                }
            }
        }
        intPastureProductCount = intTemp;

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
        if (!booBuildToView)
        {
            booBuildToView = true;
            ManagerView.Instance.Show(EnumView.ViewBuildMain);
        }
        ManagerView.Instance.Hide(EnumView.ViewHouse);
        mgViewBuildInfo.buildState = mgGroundToViewBuildInfo.buildState;
        mgViewBuildInfo.intIndexGround = GetIndexGround;
        mgViewBuildInfo.intBuildID = proBuild.intBuildID;

        mgViewBuildInfo.intIndexProduct = intIndexProduct;
        mgViewBuildInfo.intProductCount = intPastureProductCount;
        mgViewBuildInfo.intIndexProductExpend = intIndexProductExpend;
        mgViewBuildInfo.intCompoundIDs = intProductCompounds;
        mgViewBuildInfo.intPorductIDs = intProductIDs;
        mgViewBuildInfo.enumEmployeeProperties = enumEmployeeProperties;
        mgViewBuildInfo.dicPropertiesInfo = dicEmployeePropertiesInfo;
        mgViewBuildInfo.intEmployeeSizes = intEmployeeSizes;

        mgViewBuildInfo.intProductIDExpends = intProductExpendIDs;
        mgViewBuildInfo.intProductExpendNums = intProductExpendIDNums;

        mgViewBuildInfo.intResidueTime = (int)floResidueDay;
        mgViewBuildInfo.buildState = mgGroundToViewBuildInfo.buildState;
        mgViewBuildInfo.intProductExpendNum = intProductExpendIDNums[intIndexProductExpend];

        ManagerValue.actionViewBuildPasture(mgViewBuildInfo);
    }


    public override string GameSaveData()
    {
        string strData =

            intIndexProduct + "_" +
            intIndexProductExpend + "_" +

            intPastureProductID + "_" +
            intPastureProductCount + "_" +
            intPastureRipeDay + "_" +
            intPastureExpendProductID + "_" +
            intPastureExpendProductCount + "_" +
            floResidueDay + "_" +
            intEmployeeSizes[0] + "_" +
            intEmployeeSizes[1];
        return strData;
    }

    public override void GameReadData(string strData)
    {
        strReadData = strData;
        int intIndex = 0;
        string[] strDatas = strData.Split('_');

        intIndexProduct = int.Parse(strDatas[intIndex++]);
        intIndexProductExpend = int.Parse(strDatas[intIndex++]);

        intPastureProductID = int.Parse(strDatas[intIndex++]);
        intPastureProductCount = int.Parse(strDatas[intIndex++]);
        intPastureRipeDay = int.Parse(strDatas[intIndex++]);
        intPastureExpendProductID = int.Parse(strDatas[intIndex++]);
        intPastureExpendProductCount = int.Parse(strDatas[intIndex++]);
        floResidueDay = int.Parse(strDatas[intIndex++]);
        intEmployeeSizes[0] = int.Parse(strDatas[intIndex++]);
        intEmployeeSizes[1] = int.Parse(strDatas[intIndex++]);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        UserValue.Instance.BuildProductSeeReduce(GetBuildID, GetIndexGround);
        UserValue.Instance.UpdateStock();

    }
}
