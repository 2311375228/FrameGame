using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewProductStockSee : ViewBase
{
    //查看 农园,牧场,产品的动态库存
    public Text textTitle;
    public Text textProductName;
    public Text textProductCount;
    public Image imageProduct;

    public Button btnAll;
    public Button btnFarm;
    public Button btnPasture;
    public Button btnFactory;
    public Button btnClose;

    //转入 转出
    public Text textMoveTitle;
    public Text textSliderCount;
    public Slider slider;
    public Button btnConfirm;
    public Button btnStockClose;
    public GameObject goStockMove;

    public ScrollCycleColumn columnItem;

    EnumMove enumMove;
    int intProductID;
    int intProductCount;
    List<int> listProductSee = new List<int>();
    List<ViewProductStockSee_Item> listItem = new List<ViewProductStockSee_Item>();
    ViewHintBar.MessageHintBar barMessage = new ViewHintBar.MessageHintBar();
    ViewHint.MessageHint hintMessage = new ViewHint.MessageHint();
    protected override void Start()
    {
        btnAll.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            FarmClass.StockProduction[] itemStocks = UserValue.Instance.GetStockProductionOrder();
            for (int i = 0; i < itemStocks.Length; i++)
            {
                listProductSee.Add(itemStocks[i].intProductID);
            }
            SetDataTotal(listProductSee.Count);
        });
        btnFarm.onClick.AddListener(() =>
        {
        });
        btnPasture.onClick.AddListener(() =>
        {
        });
        btnFactory.onClick.AddListener(() =>
        {
        });
        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            ManagerView.Instance.Hide(EnumView.ViewProductStockSee);
        });

        //仓库 转入 转出的确认
        btnConfirm.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            if (enumMove == EnumMove.Backpack)
            {
                #region 转出
                float floTemp = 0;
                FarmClass.StockCount item = UserValue.Instance.GetStockCountAll()[intProductID];
                if (item != null)
                {
                    floTemp = item.intStockCount;
                }
                textProductCount.text = floTemp.ToString();
                if (floTemp >= intProductCount && intProductCount > 0)
                {
                    BackpackGrid itemGrid = new BackpackGrid();
                    ManagerValue.SetProductItem(intProductID, itemGrid);
                    itemGrid.intRank = 1;
                    itemGrid.intCount = intProductCount;

                    if (UserValue.Instance.KnapsackProductAddGrid(itemGrid))
                    {
                        ManagerView.Instance.SetData(EnumView.ViewKnapsack, null);
                        UserValue.Instance.GetStockCountAll()[intProductID].intStockCount -= intProductCount;
                        goStockMove.SetActive(false);
                    }
                    else
                    {
                        //"背包空间不足,是否整理?";
                        hintMessage.strHint = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.LimitedBSWYLTOI, null);
                        hintMessage.actionConfirm = () => { ManagerView.Instance.Show(EnumView.ViewKnapsack); };
                        ManagerView.Instance.Show(EnumView.ViewHint);
                        ManagerView.Instance.SetData(EnumView.ViewHint, hintMessage);
                    }
                }
                else
                {
                    // "输出数量为0,请重新输入";
                    barMessage.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.OutputQIPEA, null);
                    ManagerView.Instance.Show(EnumView.ViewHintBar);
                    ManagerView.Instance.SetData(EnumView.ViewHintBar, barMessage);
                }
                #endregion
            }
            else if (enumMove == EnumMove.Stock)
            {
                Dictionary<int, FarmClass.StockCount> dicStockCount = UserValue.Instance.GetStockCountAll();
                FarmClass.StockCount item = dicStockCount.ContainsKey(intProductID) ? dicStockCount[intProductID] : null;
                if (item == null)
                {
                    return;
                }
                if (Mathf.Pow(10, 6) < item.intStockCount + intProductCount)
                {
                    barMessage.strHintBar = ManagerLanguage.Instance.GetWord(EnumLanguageWords.TheMWCHB);
                    ManagerView.Instance.Show(EnumView.ViewHintBar);
                    ManagerView.Instance.SetData(EnumView.ViewHintBar, barMessage);
                    return;
                }
                if (UserValue.Instance.KnapsackProductChectCount(intProductID, intProductCount))
                {
                    item.intStockCount += intProductCount;
                    UserValue.Instance.KnapsackProductReduce(intProductID, intProductCount);
                    SetDataTotal(listProductSee.Count);
                    goStockMove.SetActive(false);
                }
            }
        });
        btnStockClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            goStockMove.SetActive(false);
        });

        slider.onValueChanged.AddListener((value) =>
        {
            intProductCount = (int)value;
            textSliderCount.text = value + "/" + slider.maxValue;
        });

        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Date, MessageDate);
    }

    public override void Show()
    {
        base.Show();

        listProductSee.Clear();
        UserValue.Instance.UpdateStock();
        FarmClass.StockProduction[] itemStocks = UserValue.Instance.GetStockProductionOrder();
        for (int i = 0; i < itemStocks.Length; i++)
        {
            listProductSee.Add(itemStocks[i].intProductID);
        }
        SetDataTotal(listProductSee.Count);

        goStockMove.SetActive(false);

        textTitle.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Warehouse);
        btnConfirm.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Confirm);
    }

    void SetDataTotal(int intCount)
    {

        listItem.Clear();
        RectTransform[] rectItems = columnItem.SetDataTotal(intCount);
        for (int i = 0; i < rectItems.Length; i++)
        {
            ViewProductStockSee_Item itemStockSee = rectItems[i].GetComponent<ViewProductStockSee_Item>();
            itemStockSee.textBtnStockEnterTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.IncomingWarehouse);
            itemStockSee.textBtnStockOutTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.OutgoingWarehouse);
            itemStockSee.textProductionTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.AnnualOIA) + ":";
            itemStockSee.textExpentTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.AnnualCIA) + ":";
            itemStockSee.numIndexItem = i;
            itemStockSee.numIndexData = i;
            itemStockSee.actionEnter = ActionStockEnter;
            itemStockSee.actionOut = ActionStockOut;
            RefreshData(itemStockSee, i, i);
            listItem.Add(itemStockSee);
        }
    }

    void RefreshData(ViewProductStockSee_Item itemTemp, int numIndexItem, int numIndexData)
    {
        if (numIndexData >= listProductSee.Count)
        {
            itemTemp.gameObject.SetActive(false);
        }
        else
        {
            itemTemp.gameObject.SetActive(true);
            itemTemp.numIndexItem = numIndexItem;
            itemTemp.numIndexData = numIndexData;

            MenuItemShow(itemTemp);
        }
    }

    /// <summary>
    /// 打开 转入仓库
    /// </summary>
    void ActionStockEnter(int intIndexItem, int intIndexData)
    {
        ManagerValue.actionAudio(EnumAudio.Ground);
        enumMove = EnumMove.Stock;

        goStockMove.SetActive(true);
        textMoveTitle.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.MoveTo) + " " + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Warehouse);//"转入仓库";
        intProductCount = 0;
        intProductID = listProductSee[intIndexData];
        JsonValue.DataTableBackPackItem item = ManagerProduct.Instance.GetProductTableItem(intProductID);
        textProductName.text = ManagerProduct.Instance.GetName(intProductID, false);
        imageProduct.sprite = ManagerResources.Instance.GetBackpackSprite(item.strIconName);
        int intTemp = UserValue.Instance.GetKnapsackProductCount(intProductID);
        textProductCount.text = intTemp.ToString();
        slider.value = 0;
        slider.maxValue = intTemp;
        textSliderCount.text = intProductCount + "/" + slider.maxValue;
    }

    /// <summary>
    /// 打开 转出仓库
    /// </summary>
    void ActionStockOut(int intIndexItem, int intIndexData)
    {
        ManagerValue.actionAudio(EnumAudio.Ground);
        enumMove = EnumMove.Backpack;

        goStockMove.SetActive(true);
        textMoveTitle.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.MoveTo) + " " + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Backpack);//"转入背包";
        intProductCount = 0;
        intProductID = listProductSee[intIndexData];
        JsonValue.DataTableBackPackItem item = ManagerProduct.Instance.GetProductTableItem(intProductID);
        textProductName.text = ManagerProduct.Instance.GetName(intProductID, false);
        imageProduct.sprite = ManagerResources.Instance.GetBackpackSprite(item.strIconName);
        int intTemp = UserValue.Instance.GetStockCountAll()[listProductSee[intIndexData]].intStockCount;
        textProductCount.text = intTemp.ToString();
        slider.value = 0;
        slider.maxValue = intTemp;
        textSliderCount.text = intProductCount + "/" + slider.maxValue;
    }

    void MessageDate(ManagerMessage.MessageBase message)
    {
        MessageDate date = message as MessageDate;
        if (date != null && gameObject.activeSelf && !goStockMove.activeSelf)
        {
            for (int i = 0; i < listItem.Count; i++)
            {
                MenuItemShow(listItem[i]);
            }
        }
        else if (goStockMove.activeSelf && enumMove == EnumMove.Backpack)
        {
            int intTemp = UserValue.Instance.GetStockCountAll()[intProductID].intStockCount;
            slider.maxValue = intTemp;
            textProductCount.text = intTemp.ToString();
            textSliderCount.text = intProductCount + "/" + slider.maxValue;
        }
    }

    void MenuItemShow(ViewProductStockSee_Item itemTemp)
    {
        Dictionary<int, FarmClass.StockProduction> dicStock = UserValue.Instance.GetStockProduction();
        if (dicStock.Count == 0)
        {
            return;
        }
        FarmClass.StockProduction stock = dicStock[listProductSee[itemTemp.numIndexData]];
        Dictionary<int, FarmClass.StockCount> dicStockCount = UserValue.Instance.GetStockCountAll();
        Dictionary<int, FarmClass.StockExpend> dicExpend = UserValue.Instance.GetStockExpend();
        float floOutput = dicStock[stock.intProductID].intTotalRipeCount;
        float floDay = dicStock[stock.intProductID].intTotalRipeDay;
        float floExpend = dicExpend.ContainsKey(stock.intProductID) ? dicExpend[stock.intProductID].intDayExpend : 0;
        float floStock = dicStockCount.ContainsKey(stock.intProductID) ? dicStockCount[stock.intProductID].intStockCount : 0;
        float floStockMax = stock.intStockMax;

        if (floDay != 0)
        {
            floOutput = floOutput / floDay;//每天的产量
            floOutput = floOutput * 365;//每年的产量
            floOutput = floOutput * stock.intBuildCount;
        }
        else
        {
            floOutput = 0;
        }
        floExpend = floExpend * 365;//产品每年的消耗量

        Image imageTemp = itemTemp.GetComponent<Image>();
        JsonValue.DataTableBackPackItem item = ManagerProduct.Instance.GetProductTableItem(listProductSee[itemTemp.numIndexData]);
        itemTemp.textProductName.text = ManagerProduct.Instance.GetName(listProductSee[itemTemp.numIndexData], false);
        itemTemp.imageProduct.sprite = ManagerResources.Instance.GetBackpackSprite(item.strIconName);
        itemTemp.textProductionCount.text = floOutput.ToString("f0");
        itemTemp.textExpentCount.text = floExpend.ToString("f0");
        itemTemp.textStockCount.text = floStock.ToString("N0") + " / " + floStockMax.ToString("N0");
        if (floOutput < floExpend && floStock < 0)
        {
            imageTemp.color = UserValue.Instance.GetImageColor(UserValue.EnumColorType.Red);
        }
        else if (floOutput < floExpend && floStock > 0)
        {
            imageTemp.color = UserValue.Instance.GetImageColor(UserValue.EnumColorType.Yellow);
        }
        else if (floOutput > floExpend && floStock < 0)
        {
            imageTemp.color = UserValue.Instance.GetImageColor(UserValue.EnumColorType.Yellow);
        }
        else if (floOutput >= floExpend && floStock >= 0)
        {
            imageTemp.color = UserValue.Instance.GetImageColor(UserValue.EnumColorType.Green);
        }
    }

    private void OnDestroy()
    {
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Update_Date, MessageDate);
    }

    enum EnumMove
    {
        None,
        /// <summary>
        /// 移动到仓库
        /// </summary>
        Stock,
        /// <summary>
        /// 移动到背包
        /// </summary>
        Backpack,
    }
}
