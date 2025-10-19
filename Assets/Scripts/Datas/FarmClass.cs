using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FarmClass
{
    /// <summary>
    /// 单独记录,如果归零,则需要删除
    /// </summary>
    public class StockCount
    {
        public int intProductID;
        /// <summary>
        /// 存储了多少
        /// </summary>
        public int intStockCount;
    }

    /// <summary>
    /// 生产与消耗
    /// 因为改为获取每一个建筑的状态
    /// 所以不用管单独,只管总和
    /// </summary>
    public class StockExpend
    {
        public int intProductID;
        /// <summary>
        /// 每天的消耗
        /// </summary>
        public int intDayExpend;
    }

    /// <summary>
    /// 消耗和生产要分开,不然捋不清楚
    /// 消耗是生产的附属,增加消耗则需要往生产中添加对象
    /// 消耗属于被查找对象
    /// </summary>
    public class StockProduction
    {
        public int intIndex;
        public int intProductID;
        public int intBuildCount;//有多少个建筑在生产
        public int intTotalRipeDay;
        public int intTotalRipeCount;
        public int intStockMax;
    }
}