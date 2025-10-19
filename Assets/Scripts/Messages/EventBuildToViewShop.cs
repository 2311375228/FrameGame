using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBuildToViewShop : EventBuildToViewBase
{
    public int intRankStore;

    public int intIndexBuy;

    public int intYear;
    public int intMonth;
    public int intDay;

    public int[] intProductBuyIDs;//购买列表
    public int[] intProductBuyNums;
    public int[] intProductBuyPrices;
    public bool[] booProductBuys;//是否已经买过

    public int intSellTime;
    public int[] intProductSellIDs;//
    public EnumKnapsackStockType[] enumGridItems;
    public int[] intProductSellPrices;
    public int[] intProductSellNums;
    public int[] intProductSellRipe;
    public int[] intResidueTimes;
}
