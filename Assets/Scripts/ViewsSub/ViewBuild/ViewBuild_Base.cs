using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ViewBuild_Base : MonoBehaviour
{
    public System.Action<int> actionEmployeeAdd;
    public Action<MGViewToBuildBase> SendToGround;
    public delegate ViewBuild_Base GetViewCentre(EnumViewCentre enumKey);
    public GetViewCentre getCentre;
    protected virtual void Start() { }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
    public virtual void ShowSub() { }
    public virtual void HideSub() { }
    public virtual void SetData(ViewBase.Message message) { }
    public virtual void SetDate(ViewGroundToBuildMainDateBase message) { }

    /// <summary>
    /// 来自建筑的消息
    /// </summary>
    public virtual void BuildMessage(EventBuildToViewBase message) { }

    public class CloseMessage : MGViewToBuildBase { }
    public class DemolishBuild : MGViewToBuildBase { }
    public class SetEmployee : MGViewToBuildBase
    {
        public int intEmployeeID;
        public int intIndexSize;
    }
    /// <summary>
    /// 发送到土地
    /// </summary>
    public class TowerToBuild : MGViewToBuildBase
    {

    }
    /// <summary>
    /// 发送到界面
    /// </summary>
    public class TowerToView : EventBuildToViewBase
    {
        public int intRank;
        public int intMonth;
        public int intDay;
        public int intPersonMax;
        public Dictionary<int, PropertiesEmployee> dicEmployee;
    }
    /// <summary>
    /// 牧场选择产出与消耗的消息
    /// </summary>
    public class SelectPasture : MGViewToBuildBase
    {
        public int intIndexProduction;
        public int intIndexExpend;
    }

    /// <summary>
    /// 发送到界面 承载两种消息 障碍物 已经购买的土地
    /// </summary>
    public class GroundToView : EventBuildToViewBase
    {
        public EnumGroudState groundState;
        public EnumView view;//需要关联的界面
        public long numPrice;//仅接收者知道，并由接受者处理
        public string strModelNameNew;//新的模型 ViewBuilding发送这个数据，ViewBuyHint接收数据
        public string strModelNameOld;//旧的模型
    }
    /// <summary>
    /// 购买建筑
    /// </summary>
    public class BuyBuildToView : EventBuildToViewBase
    {
        public EnumGroudState groundState;
        public string strModelNameNew;
    }

    /// <summary>
    /// 钱仓 消息到界面
    /// </summary>
    public class MoneyWarehouseToView : EventBuildToViewBase
    {
        public int intRank;
        public int intRankMax;
        public int intCoin;
        public int intCoinMax;
        public int intExpence;
        public int[] intUpgradeMaterials;
    }
    /// <summary>
    /// 钱仓 消息到建筑
    /// </summary>
    public class MoneyWarehouseToBuild : MGViewToBuildBase
    {

    }
    /// <summary>
    /// 销售商店 消息到界面
    /// </summary>
    public class SaleShopToView : EventBuildToViewBase
    {
        public int intRank;
        public int intSellMaxCount;
        public int intCoinAnnualRevenue;
        public int[] intSellProdects;
        public int[] intsellProductCounts;
        public int[] intSellPrices;
        public int[] intSellState;

        public int intDay;
        public int intMonth;
    }

    public class SaleShopRandomToBuild : MGViewToBuildBase { }
    public class SaleShopSellStateBoBuild : MGViewToBuildBase { }
    public class SaleShopUpgradeToBuild : MGViewToBuildBase { };

    /// <summary>
    /// 工地 消息到界面
    /// </summary>
    public class ConstructionToView : EventBuildToViewBase
    {
        public int intTargetBuildID;//正在建造的目标建筑
        public float floResidueDay;
        public float floTotalTime;
    }

    /// <summary>
    /// 工地 消息到建筑 取消建造
    /// </summary>
    public class ConstructionCancelToBuild : MGViewToBuildBase { }
    /// <summary>
    /// 工地 消息到界面 每天的刷新
    /// </summary>
    public class ContructionToView_Date : ViewGroundToBuildMainDateBase
    {
        public float floResidueDay;
    }

    /// <summary>
    /// 仓库 消息到界面
    /// </summary>
    public class WarehouseToView : EventBuildToViewBase
    {

    }
    /// <summary>
    /// 仓库 消息到建筑
    /// </summary>
    public class WarehouseToBuild : MGViewToBuildBase
    {

    }

    public enum EnumViewUp
    {
        None,
        /// <summary>
        /// 农业
        /// </summary>
        ViewBuild_FarmProduct,
        /// <summary>
        /// 牧业
        /// </summary>
        ViewBuild_PastureProduct,
        /// <summary>
        /// 工坊
        /// </summary>
        ViewBuild_ProductFactory,
        /// <summary>
        /// 城堡
        /// </summary>
        ViewBuild_Tower,
        /// <summary>
        /// 出售商店
        /// </summary>
        ViewBuild_SaleShopUp,
        /// <summary>
        /// 钱仓
        /// </summary>
        ViewBuild_MoneyWarehouse,
        /// <summary>
        /// 买地
        /// </summary>
        ViewBuild_BuyGround,
        /// <summary>
        /// 买建筑
        /// </summary>
        ViewBuild_BuyBuild,
        /// <summary>
        /// 工地
        /// </summary>
        ViewBuild_BuildConstruction,
        /// <summary>
        /// 仓库
        /// </summary>
        ViewBuild_Warehouse,
    }
    public enum EnumViewDown
    {
        None,
        /// <summary>
        /// 员工
        /// </summary>
        ViewBuild_Employee,
        /// <summary>
        /// 出售商店
        /// </summary>
        ViewBuild_SaleShopDown,
        /// <summary>
        /// 钱仓升级
        /// </summary>
        ViewBuild_MoneyWarehouseDown,
    }

    public enum EnumViewCentre
    {
        None,
        /// <summary>
        /// 牧场产出与消耗选择
        /// </summary>
        ViewBuild_ProductPastureSelect,
        /// <summary>
        /// 工厂产出选择
        /// </summary>
        ViewBuild_ProductFactorySelect,
        /// <summary>
        /// 购买建筑 选择建筑
        /// </summary>
        ViewBuild_BuildSelect,
        /// <summary>
        /// 仓库 存储物品选择
        /// </summary>
        ViewBuild_WarehouseSelect,
    }

    public enum BuildTipsState
    {
        None,
        /// <summary>
        /// 种植中
        /// </summary>
        Planting,
        /// <summary>
        /// 消耗品不足
        /// </summary>
        NotExpend,
        /// <summary>
        /// 消耗品充足
        /// </summary>
        Expend,
        /// <summary>
        /// 没有员工
        /// </summary>
        NotWorker,
        /// <summary>
        /// 正在采矿
        /// </summary>
        Minig,
        /// <summary>
        /// 冶炼
        /// </summary>
        Smelt,
    }
}
