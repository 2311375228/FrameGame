using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 商店
/// </summary>
public class BuildShop : BuildBase
{
    //升级规则，城堡每升级2级，商店可升级;
    public int intRankStore = 1;

    //旅店员工，购买商品刷新时间减半；出租屋员工，出售商品可自动上架；城堡员工，购买价格减少30%；
    //只能出售，牧场产出，工厂产出

    //购买商品
    int[] intProductBuyIDs;//
    int[] intPorudctBuyNums;//
    int[] intProductBuyPrices;//
    bool[] booProductBuys;
    //出售商品
    int[] intProductSellIDs = new int[20];//
    EnumKnapsackStockType[] enumKnaspackItems = new EnumKnapsackStockType[20];
    int[] intProductSellPrices = new int[20];
    int[] intProductSellNums = new int[20];
    int[] intProductSellRipe = new int[20];
    int[] intResidueTimes = new int[20];
    //出售时间是在现在时间点 加上需要的时间点 当到达完成时间 则完成该交易
    //这里不做时间的减少
    //key=时间 value=下标
    Dictionary<int, int[]> dicSellTime = new Dictionary<int, int[]>();

    int intSellTime;
    readonly int intRefreshBuyGoodsTime = 90;
    int intResidueBuyGoodsTime;
    ViewMGToViewInfoShop mgUpdateShop = new ViewMGToViewInfoShop();
    EventBuildToViewShop mgViewBuildInfo = new EventBuildToViewShop();
    ViewHint.MessageHint viewHint = new ViewHint.MessageHint();
    ViewHintBar.MessageHintBar viewHintBar = new ViewHintBar.MessageHintBar();
    //冒险家小屋，就是员工屋子
    public override void OnStart()
    {
        base.OnStart();
        //最小2%，最大18%
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Strengt, "减少售出时间");
        //最小1%，最大6%
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Intellect, "增加出售获得的金币");
        //最小5%，最大15%
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Versatility, "减少刷新时间");

        for (int i = 0; i < intProductSellIDs.Length; i++)
        {
            intProductSellIDs[i] = -1;
        }

        intResidueBuyGoodsTime = intRefreshBuyGoodsTime;
        UpRankStore(1);

        ManagerValue.actionUpdateShopProduct += ActionUpdateShopProduct;
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        intSellTime++;
        if (intResidueBuyGoodsTime == 0)
        {
            intResidueBuyGoodsTime = intRefreshBuyGoodsTime;
            UpRankStore(intRankStore);
        }

        if (dicSellTime.ContainsKey(intSellTime))
        {
            for (int i = 0; i < dicSellTime[intSellTime].Length; i++)
            {
                int productPrice = intProductSellPrices[dicSellTime[intSellTime][i]];

                UserValue.Instance.SetCoinAdd = productPrice;
                ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);

                intProductSellIDs[dicSellTime[intSellTime][i]] = -1;
            }
            dicSellTime.Remove(intSellTime);

            if (booBuildToView)
            {
                ManagerValue.actionViewBuildShop(mgViewBuildInfo);
            }
        }

        if (booBuildToView)
        {
            mgUpdateShop.intYear = mgData.numYear;
            mgUpdateShop.intMonth = mgData.numMonth;
            mgUpdateShop.intDay = mgData.numDay;

            mgUpdateShop.intSellTime = intSellTime;
            ManagerView.Instance.SetData(EnumView.ViewShop, mgUpdateShop);
        }
        mgViewBuildInfo.intYear = mgData.numYear;
        mgViewBuildInfo.intMonth = mgData.numMonth;
        mgViewBuildInfo.intDay = mgData.numDay;
    }

    public override void MGViewBuildInfo(MGViewToBuildBase toGround)
    {
        MGViewToBuildShop mg = toGround as MGViewToBuildShop;
        if (mg != null)
        {
            booBuildToView = mg.booSendInfoToViewBuildInfo;

            if (mg.intRankStore > intRankStore)
            {
                intRankStore = mg.intRankStore;
                UpRankStore(intRankStore);
            }

            if (mg.intMGIndexBuy != -1)
            {
                if (UserValue.Instance.SetCoinReduce(intProductBuyPrices[mg.intMGIndexBuy]))
                {
                    ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);
                    booProductBuys[mg.intMGIndexBuy] = true;
                    mgViewBuildInfo.intIndexBuy = mg.intMGIndexBuy;

                    BackpackGrid itemGrid = new BackpackGrid();
                    ManagerValue.SetProductItem(intProductBuyIDs[mg.intMGIndexBuy], itemGrid);
                    itemGrid.intCount = intPorudctBuyNums[mg.intMGIndexBuy];
                    UserValue.Instance.KnapsackProductAddGrid(itemGrid);

                    viewHintBar.strHintBar = "已购买";
                    ManagerView.Instance.Show(EnumView.ViewHintBar);
                    ManagerView.Instance.SetData(EnumView.ViewHintBar, viewHintBar);
                }
                else
                {
                    viewHint.strHint = "购买‘" + ManagerProduct.Instance.GetName(intProductBuyIDs[mg.intMGIndexBuy], false) + " ’金币不足!";
                    ManagerView.Instance.Show(EnumView.ViewHint);
                    ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
                }

                mg.intMGIndexBuy = -1;
            }

            //添加出售物品
            if (mg.intSellProductID != -1)
            {
                //背包未出售 与 在售 没有关系
                bool booTemp = true;
                for (int i = 0; i < intProductSellIDs.Length; i++)
                {
                    if (intProductSellIDs[i] == -1)
                    {
                        booTemp = false;
                        intProductSellIDs[i] = mg.intSellProductID;
                        enumKnaspackItems[i] = mg.enumStockType;
                        intProductSellNums[i] = mg.intSellProductNum;
                        intProductSellPrices[i] = mg.intSellProductPrice;
                        intProductSellRipe[i] = mg.intSellProductTime;
                        intResidueTimes[i] = mg.intSellProductTime + intSellTime;
                        int intTemp = intResidueTimes[i];
                        if (!dicSellTime.ContainsKey(intTemp))
                        {
                            dicSellTime.Add(intTemp, new int[] { i });
                        }
                        else
                        {
                            List<int> listTemp = new List<int>();
                            for (int j = 0; j < dicSellTime[intTemp].Length; j++)
                            {
                                listTemp.Add(dicSellTime[intTemp][j]);
                            }
                            listTemp.Add(i);
                            dicSellTime[intTemp] = listTemp.ToArray();
                        }

                        //if (!UserValue.Instance.ProductNumReduce(mg.intSellProductID, mg.intSellProductNum))
                        //{
                        //    PropertiesProductRes resProduct = ManagerProduct.Instance.GetValue(mg.intSellProductID);
                        //    viewHint.strHint = resProduct.strChinaName + " 数量不足!";
                        //    ManagerView.Instance.Show(EnumView.ViewHint);
                        //    ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
                        //}

                        break;
                    }
                }
                if (booTemp)
                {
                    viewHint.strHint = "已经超过最大售卖数量!";
                    ManagerView.Instance.Show(EnumView.ViewHint);
                    ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
                }

                mg.intSellProductID = -1;
            }

            //下架出售中的产品
            if (mg.intProductIDIndex != -1)
            {
                int intIndex = mg.intProductIDIndex;
                //移除计时器中的下标
                List<int> listTemp = new List<int>();
                if (dicSellTime.ContainsKey(intResidueTimes[intIndex]))
                {
                    for (int i = 0; i < dicSellTime[intResidueTimes[intIndex]].Length; i++)
                    {
                        if (intIndex == dicSellTime[intResidueTimes[intIndex]][i])
                        {
                            continue;
                        }
                        listTemp.Add(dicSellTime[intResidueTimes[intIndex]][i]);
                    }
                    dicSellTime[intResidueTimes[intIndex]] = listTemp.ToArray();
                }

                intProductSellIDs[intIndex] = -1;

                mg.intProductIDIndex = -1;
            }

            mgViewBuildInfo.intIndexGround = GetIndexGround;
            ManagerValue.actionViewBuildShop(mgViewBuildInfo);
        }
    }

    void UpRankStore(int intRank)
    {
        int intLenght = intRank * 10;
        List<int> listRandom = ManagerProduct.Instance.GetRandomProductID(intLenght);
        intProductBuyIDs = new int[intLenght];
        intPorudctBuyNums = new int[intLenght];
        intProductBuyPrices = new int[intLenght];
        booProductBuys = new bool[intLenght];
        for (int i = 0; i < listRandom.Count; i++)
        {
            intProductBuyIDs[i] = listRandom[i];
            JsonValue.DataTableBackPackItem resTemp = ManagerProduct.Instance.GetProductTableItem(listRandom[i]);
            //随机产生商品数量
            int intTemp = Random.Range(1, 11);
            intPorudctBuyNums[i] = intTemp;
            intProductBuyPrices[i] = ManagerCompound.Instance.GetProductPrice(resTemp.intProductID) * intTemp;
            booProductBuys[i] = false;
        }
        mgViewBuildInfo.intProductBuyIDs = intProductBuyIDs;
        mgViewBuildInfo.intProductBuyNums = intPorudctBuyNums;
        mgViewBuildInfo.intProductBuyPrices = intProductBuyPrices;
        mgViewBuildInfo.booProductBuys = booProductBuys;

    }

    public override void ShowViewBuildInfo()
    {
        booBuildToView = true;

        ManagerView.Instance.Show(EnumView.ViewShop);

        mgViewBuildInfo.intIndexGround = GetIndexGround;
        mgViewBuildInfo.intRankStore = intRankStore;
        mgViewBuildInfo.intBuildID = proBuild.intBuildID;

        //mgViewBuildInfo.listBackProductData = UserValue.Instance.GetknapsaxkIDList();

        mgViewBuildInfo.intProductSellIDs = intProductSellIDs;
        mgViewBuildInfo.enumGridItems = enumKnaspackItems;
        mgViewBuildInfo.intProductSellNums = intProductSellNums;
        mgViewBuildInfo.intProductSellPrices = intProductSellPrices;
        mgViewBuildInfo.intProductSellRipe = intProductSellRipe;
        mgViewBuildInfo.intResidueTimes = intResidueTimes;

        ManagerValue.actionViewBuildShop(mgViewBuildInfo);
    }

    void ActionUpdateShopProduct()
    {
        if (booBuildToView)
        {
            //mgViewBuildInfo.intIndexGround = GetIndexGround;
            //mgViewBuildInfo.listBackProductData = UserValue.Instance.GetknapsaxkIDList();
            //ManagerValue.actionViewBuildShop(mgViewBuildInfo);
        }
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();

        ManagerValue.actionUpdateShopProduct -= ActionUpdateShopProduct;
    }
}
