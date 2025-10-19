using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewNPCShop : ViewBase
{
    //每7天补充一次
    //立即出售
    //立即购买

    //这里需要一张销售商品表

    //出售商品价格百分比 1.3~2
    public int intOnSalePricePercent;
    //收购商品价格的百分比 0.8~1.2
    public int intPurchasePricePercent;
    protected override void Start()
    {
        base.Start();
    }
}
