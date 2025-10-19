using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSaleShop : BuildBase
{
    public Transform transSellProduct;
    List<MeshRenderer> listProductRender = new List<MeshRenderer>();

    int intRank = 1;//就两个等级，1级代表随机农园的产品，2级代表随机牧园的产品
    int[] intSellProdects = new int[1];//当前正在售卖的商品
    int[] intsellProductCounts = new int[1];
    int[] intSellPrices = new int[1];
    int[] intSellState = new int[1] { 1 };//商品售卖状态

    List<int[]> listRank = new List<int[]>();
    //销售的商品数量每年都会涨价，并且销售数量也会涨
    int intDay = 1;
    int intMonth = 1;
    int intSellMaxCount = 1;
    int intCoinAnnualRevenue;//每年的收益
    string[] strDatas;
    ViewBuild_Base.SaleShopToView messageSaleShop = new ViewBuild_Base.SaleShopToView();
    ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();

    float floTime = 0;
    bool booExpend;
    public override void OnStart()
    {
        base.OnStart();

        for (int i = 0; i < transSellProduct.childCount; i++)
        {
            listProductRender.Add(transSellProduct.GetChild(i).GetComponent<MeshRenderer>());
            listProductRender[i].material = Instantiate(ManagerResources.Instance.GetMaterial("productSaleShop"));
        }

        listRank.Add(new int[] { 10100, 10101, 10102, 10103, 10104, 10105, 10106 });
        listRank.Add(new int[] { 10200, 10201, 10202, 10203 });
        listRank.Add(new int[] { 20001, 20002, 20003, 20004, 20005, 20006, 20007, 20008, 20009 });
        listRank.Add(new int[] { 20010, 20011, 20012, 20013 });

        if (strDatas == null)
        {
            intDay = ManagerValue.intDay;
            intMonth = ManagerValue.intMonth;
            RandomProduct();
        }
        ProductPrice();
        ProductRender();

        ChnageBuildExpendProduct();
    }

    protected override void UpdateDate(MessageDate mgData)
    {

        if (intDay == mgData.numDay && intMonth == mgData.numMonth)
        {
            if (intSellMaxCount < 5)
            {
                intSellMaxCount += 1;
                ProductPrice();
                ProductRender();
                ChnageBuildExpendProduct();
            }
            UserValue.Instance.SetCoinAdd = intCoinAnnualRevenue;
            ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);
            intCoinAnnualRevenue = 0;
        }

        if (intSellState[0] == 0 && listProductRender[0].gameObject.activeSelf)
        {
            for (int i = 0; i < listProductRender.Count; i++)
            {
                listProductRender[i].gameObject.SetActive(false);
            }
        }
        else if (intSellState[0] == 1 && !listProductRender[0].gameObject.activeSelf)
        {
            ProductRender();
        }
        if (intSellState[0] == 0)
        {
            booExpend = false;
            return;
        }

        Dictionary<int, FarmClass.StockCount> dicStockCount = UserValue.Instance.GetStockCountAll();
        if (dicStockCount.ContainsKey(intPastureExpendProductID) && UserValue.Instance.StockCountReduce(intPastureExpendProductID, intSellMaxCount))
        {
            intCoinAnnualRevenue += intSellPrices[0];//已经是总价了

            if (booExpend)
            {
                ProductRender();
            }
            booExpend = false;
        }
        else
        {
            booExpend = true;
        }
        if (booBuildToView)
        {
            messageSaleShop.intCoinAnnualRevenue = intCoinAnnualRevenue;
            ManagerValue.actionViewBuildMain(messageSaleShop);
        }
    }

    private void Update()
    {
        if (booExpend)
        {
            floTime += Time.deltaTime;
            if (floTime > 0.5f)
            {
                floTime = 0;
                for (int i = 0; i < listProductRender.Count; i++)
                {
                    if (i < intSellMaxCount)
                    {
                        listProductRender[i].gameObject.SetActive(!listProductRender[i].gameObject.activeSelf);
                    }
                }
            }
        }
    }

    public override string GameSaveData()
    {
        string strTemp = intRank + "_";
        strTemp += intDay + "_";
        strTemp += intMonth + "_";
        for (int i = 0; i < intSellProdects.Length; i++)
        {
            strTemp += intSellProdects[i] + "_";
            strTemp += intSellState[i] + "_";
        }
        strTemp += intSellMaxCount + "_";
        strTemp += intCoinAnnualRevenue;
        return strTemp;
    }
    public override void GameReadData(string strData)
    {
        int intIndexTemp = 0;
        strDatas = strData.Split("_");
        intRank = int.Parse(strDatas[intIndexTemp++]);
        intDay = int.Parse(strDatas[intIndexTemp++]);
        intMonth = int.Parse(strDatas[intIndexTemp++]);
        for (int i = 0; i < intSellProdects.Length; i++)
        {
            intSellProdects[i] = int.Parse(strDatas[intIndexTemp++]);
            intSellState[i] = int.Parse(strDatas[intIndexTemp++]);
        }
        intSellMaxCount = int.Parse(strDatas[intIndexTemp++]);
        intCoinAnnualRevenue = int.Parse(strDatas[intIndexTemp++]);
    }

    public override void MGViewBuildInfo(MGViewToBuildBase toGround)
    {
        ViewBuild_Base.SaleShopRandomToBuild saleShopRandom = toGround as ViewBuild_Base.SaleShopRandomToBuild;
        if (saleShopRandom != null)
        {
            RandomProduct();
            ProductPrice();
            ProductRender();

            UserValue.Instance.SetCoinReduce(100);
            hintBar.strHintBar = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Consume) + ": 100 " + ManagerLanguage.Instance.GetWord(EnumLanguageWords.GoldCoins);
            ManagerView.Instance.Show(EnumView.ViewHintBar);
            ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
            ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);

            ManagerValue.actionViewBuildMain(messageSaleShop);
        }
        ViewBuild_Base.SaleShopSellStateBoBuild messageSellState = toGround as ViewBuild_Base.SaleShopSellStateBoBuild;
        if (messageSellState != null)
        {
            //因为是引用的关系，所以在这里只要执行就可以了; 需要的是事件推动
            ManagerValue.actionViewBuildMain(messageSaleShop);
        }
        ViewBuild_Base.SaleShopUpgradeToBuild messageUpgrage = toGround as ViewBuild_Base.SaleShopUpgradeToBuild;
        if (messageUpgrage != null)
        {
            intRank += 1;
            if (intRank > listRank.Count)
            {
                intRank = 1;
            }
            RandomProduct();
            ProductPrice();
            ProductRender();

            UserValue.Instance.SetCoinReduce(100);
            hintBar.strHintBar = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Consume) + ": 100 " + ManagerLanguage.Instance.GetWord(EnumLanguageWords.GoldCoins);
            ManagerView.Instance.Show(EnumView.ViewHintBar);
            ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
            ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);

            messageSaleShop.intRank = intRank;
            ManagerValue.actionViewBuildMain(messageSaleShop);
        }
        ViewBuild_Base.CloseMessage messageClose = toGround as ViewBuild_Base.CloseMessage;
        if (messageClose != null)
        {
            booBuildToView = false;
        }
    }
    /// <summary>
    /// 随机商品，并计算售价
    /// </summary>
    void RandomProduct()
    {
        intSellMaxCount = 1;
        for (int i = 0; i < intSellPrices.Length; i++)
        {
            intSellProdects[i] = listRank[intRank - 1][Random.Range(0, listRank[intRank - 1].Length)];
            intSellState[i] = 1;
            intsellProductCounts[i] = intSellMaxCount;
        }

        ChnageBuildExpendProduct();
    }

    void ProductPrice()
    {
        for (int i = 0; i < intsellProductCounts.Length; i++)
        {
            intsellProductCounts[i] = intSellMaxCount;
            intSellPrices[i] = intsellProductCounts[i] * ManagerCompound.Instance.GetProductPrice(intSellProdects[i]);
        }
    }

    /// <summary>
    /// 改变仓库的消耗产品
    /// </summary>
    void ChnageBuildExpendProduct()
    {
        intPastureExpendProductID = intSellProdects[0];
        intPastureExpendProductCount = intSellMaxCount;
        UserValue.Instance.BuildProductSeeAdd(GetBuildID, GetIndexGround);
        UserValue.Instance.UpdateStock();
    }

    void ProductRender()
    {
        for (int i = 0; i < listProductRender.Count; i++)
        {
            if (i < intSellMaxCount)
            {
                listProductRender[i].gameObject.SetActive(true);
                listProductRender[i].material.mainTexture = ManagerResources.Instance.GetTexture2D(ManagerProduct.Instance.GetProductTableItem(intSellProdects[0]).strIconName);
                continue;
            }
            listProductRender[i].gameObject.SetActive(false);
        }
    }

    public override void ShowViewBuildInfo()
    {
        booBuildToView = true;

        messageSaleShop.intBuildID = GetBuildID;
        messageSaleShop.intIndexGround = GetIndexGround;
        messageSaleShop.enumKeyUp = ViewBuild_Base.EnumViewUp.ViewBuild_SaleShopUp;
        messageSaleShop.enumKeyDown = ViewBuild_Base.EnumViewDown.ViewBuild_SaleShopDown;

        messageSaleShop.intSellMaxCount = 5;
        messageSaleShop.intRank = intRank;
        messageSaleShop.intSellProdects = intSellProdects;
        messageSaleShop.intSellPrices = intSellPrices;
        messageSaleShop.intsellProductCounts = intsellProductCounts;
        messageSaleShop.intSellState = intSellState;

        messageSaleShop.intDay = intDay;
        messageSaleShop.intMonth = intMonth;

        ManagerView.Instance.Show(EnumView.ViewBuildMain);
        ManagerValue.actionViewBuildMain(messageSaleShop);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        ChnageBuildExpendProduct();
    }
}
