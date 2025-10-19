using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewShop : ViewBase
{
    public Text textTitle;
    public Text textTitleRank;
    public Text textSellInfo;

    public Button btnShop;
    public Button btnSell;
    public Button btnBuy;
    public Button btnBackpack;
    public Button btnDelete;
    public Button btnClose;

    public ViewShop_Store store;//商店首页
    public ViewShop_Buy buy;//购买商品
    public ViewShop_Sell sell;//正在出售的商品
    public ViewShop_Backpack backpack;//背包

    int intSellTime;
    MGViewToBuildShop mgShop = new MGViewToBuildShop();
    EventBuildToViewShop mgViewInfo;
    ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();

    //利益链的价格每个月会随机变动一次
    protected override void Start()
    {
        base.Start();

        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            SendCloseView();
        });
        btnDelete.onClick.AddListener(() =>
        {

            viewHint.actionConfirm = () =>
            {
                MessageDemolishBuild mgDemolishBuild = new MessageDemolishBuild();
                mgDemolishBuild.intIndexGround = mgViewInfo.intIndexGround;
                //for (int i = 0; i < mgBuildInfo.intEmployeeSizes.Length; i++)
                //{
                //    if (mgBuildInfo.intEmployeeSizes[i] != -1)
                //    {
                //        PropertiesRole.Instance.GetEmployeeValue(mgBuildInfo.intEmployeeSizes[i]).intIndexGroundWork = -1;
                //    }
                //}
                ManagerMessage.Instance.PostEvent(EnumMessage.DemolishBuild, mgDemolishBuild);
                ManagerView.Instance.Hide(EnumView.ViewShop);
            };
            viewHint.strHint = "是否拆除建筑!";
            ManagerView.Instance.Show(EnumView.ViewHint);
            ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
        });
        btnShop.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            store.gameObject.SetActive(true);
            buy.gameObject.SetActive(false);
            sell.gameObject.SetActive(false);
            backpack.gameObject.SetActive(false);
            //JsonValue.DataTableBuildingItem item = ManagerBuild.Instance.GetBuildItem(mgViewInfo.intBuildID);
            //textListTitle.text = item.strChinaName;
        });
        btnBuy.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            store.gameObject.SetActive(false);
            buy.gameObject.SetActive(true);
            sell.gameObject.SetActive(false);
            backpack.gameObject.SetActive(false);

            //textListTitle.text = "购买产品";
        });
        btnSell.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            store.gameObject.SetActive(false);
            sell.gameObject.SetActive(true);
            buy.gameObject.SetActive(false);
            backpack.gameObject.SetActive(false);
            //textListTitle.text = "出售产品";
        });
        btnBackpack.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            store.gameObject.SetActive(false);
            sell.gameObject.SetActive(false);
            buy.gameObject.SetActive(false);
            backpack.gameObject.SetActive(true);
            //textListTitle.text = "背包";
        });
        buy.btnBuy.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            SendBuyItem(buy.intIndexBuy);
        });
        for (int i = 0; i < backpack.soldOuts.Length; i++)
        {
            backpack.soldOuts[i].btnSoldOut.onClick.AddListener(OnClickSellPriceType(i));
        }
        backpack.SendSellProduct = SendSellProduct;
        sell.SendSellProductDown = SendSellProductDown;
        buy.actionSendBuyItem = SendBuyItem;

        buy.GetComponent<RectTransform>().position = store.GetComponent<RectTransform>().position;
        sell.GetComponent<RectTransform>().position = store.GetComponent<RectTransform>().position;
        backpack.GetComponent<RectTransform>().position = store.GetComponent<RectTransform>().position;
    }

    public override void SetData(Message message)
    {
        ViewMGToViewInfoShop mg = message as ViewMGToViewInfoShop;
        if (mg != null)
        {
            intSellTime = mg.intSellTime;
            sell.intSellTime = mg.intSellTime;
            sell.SetShowList();
        }
    }

    public override void Show()
    {
        base.Show();

        if (ManagerValue.actionViewBuildShop == null)
        {
            ManagerValue.actionViewBuildShop = MGBuildBaseShop;
        }

        sell.Show();
        backpack.Show();

        store.gameObject.SetActive(true);
        sell.gameObject.SetActive(false);
        buy.gameObject.SetActive(false);
        backpack.gameObject.SetActive(false);
    }

    void MGBuildBaseShop(EventBuildToViewBase mgBuild)
    {

        mgViewInfo = mgBuild as EventBuildToViewShop;
        if (mgViewInfo != null)
        {
            sell.intSellTime = mgViewInfo.intSellTime;
            sell.intProductSellIDs = mgViewInfo.intProductSellIDs;
            sell.enumGridItems = mgViewInfo.enumGridItems;
            sell.intProductSellPrices = mgViewInfo.intProductSellPrices;
            sell.intProductSellNums = mgViewInfo.intProductSellNums;
            sell.intProductSellRipe = mgViewInfo.intProductSellRipe;
            sell.intResidueTimes = mgViewInfo.intResidueTimes;
            sell.SetShowList();

            buy.intProductBuyIDs = mgViewInfo.intProductBuyIDs;
            buy.intProductBuyNums = mgViewInfo.intProductBuyNums;
            buy.intProductBuyPrices = mgViewInfo.intProductBuyPrices;
            buy.booProductBuys = mgViewInfo.booProductBuys;
            mgShop.intMGIndexBuy = buy.intIndexBuy;

            buy.Show();
            buy.ShowPage();

            backpack.Show();
        }
    }

    UnityEngine.Events.UnityAction OnClickSellPriceType(int intIndex)
    {
        return delegate
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            if (backpack.SelectSell(intIndex))
            {
                SendSellProduct();
            }
            else
            {
                hintBar.strHintBar = "数量不足,无法出售";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
            }
        };
    }

    void SendCloseView()
    {
        mgShop.booSendInfoToViewBuildInfo = false;
        mgShop.intIndexGround = mgViewInfo.intIndexGround;

        buy.intIndexBuy = -1;
        mgShop.intMGIndexBuy = -1;
        mgShop.intSellProductID = -1;
        mgShop.intProductIDIndex = -1;

        ManagerValue.actionGround(mgViewInfo.intIndexGround, mgShop);
        ManagerView.Instance.Hide(EnumView.ViewShop);
    }

    void SendBuyItem(int intIndex)
    {
        mgShop.booSendInfoToViewBuildInfo = true;
        mgShop.intIndexGround = mgViewInfo.intIndexGround;

        mgShop.intMGIndexBuy = intIndex;

        mgShop.intSellProductID = -1;
        mgShop.intProductIDIndex = -1;

        ManagerValue.actionGround(mgViewInfo.intIndexGround, mgShop);
    }

    void SendSellProduct()
    {
        mgShop.booSendInfoToViewBuildInfo = true;
        mgShop.intIndexGround = mgViewInfo.intIndexGround;

        mgShop.intMGIndexBuy = -1;

        mgShop.intSellProductID = backpack.intProductID;
        mgShop.intProductIDIndex = -1;
        mgShop.enumStockType = backpack.gridItem;
        mgShop.intSellProductNum = backpack.intProductCount;
        mgShop.intSellProductPrice = backpack.intProductPrice;
        mgShop.intSellProductTime = backpack.intProductTime;
        ManagerValue.actionGround(mgViewInfo.intIndexGround, mgShop);
    }

    void SendSellProductDown()
    {
        mgShop.booSendInfoToViewBuildInfo = true;
        mgShop.intIndexGround = mgViewInfo.intIndexGround;

        mgShop.intMGIndexBuy = -1;

        mgShop.intSellProductID = -1;
        mgShop.intProductIDIndex = sell.intProductIDIndex;
        mgShop.intSellProductNum = backpack.intProductCount;
        mgShop.intSellProductPrice = backpack.intProductPrice;
        mgShop.intSellProductTime = backpack.intProductTime;
        ManagerValue.actionGround(mgViewInfo.intIndexGround, mgShop);
    }

    private void OnDestroy()
    {
        ManagerValue.actionViewBuildShop -= MGBuildBaseShop;
        ManagerValue.actionViewBuildShop = null;
    }
}