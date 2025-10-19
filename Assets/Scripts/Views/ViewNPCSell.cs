using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ViewNPCSell : ViewBase
{
    public Text textTitle;
    public Text textSelectType;
    public Text textSliderCount;
    public Text textSellCount;
    public Text textTotalPrice;
    public Text textPage;
    public Text textLimitSellCount;

    public Text textPriceTag;//单价
    public Text textProductCountTag;//当前数量
    public Text textSellCountTag;//售出数量
    public Text textTotalPriceTag;//出售总价

    public Button btnPageLeft;
    public Button btnPageRight;
    public Button btnClose;
    public Button btnSell;
    public Button btnStock;
    public Button btnBackpack1;
    public Button btnBackpack2;
    public Button btnSellReduce;
    public Button btnSellAdd;

    //
    public Slider slider;
    public View_PropertiesItem product;

    public Transform transImageGrid;

    EnumSell enumSell;
    int intSellPrice;//出售价格
    int intSellCount;//出售数量
    List<View_PropertiesItem> listItem = new List<View_PropertiesItem>();
    List<BackpackGrid> listGrid = new List<BackpackGrid>();

    int intPage;
    int intPageTotal;
    int intSelectGrid;//选中的格子
    int intGround;
    int intLimitCount = 500;
    MGViewToBuildNPCSell mgToNPCSell = new MGViewToBuildNPCSell();
    ViewHintBar.MessageHintBar messageHintBar = new ViewHintBar.MessageHintBar();
    protected override void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            ManagerView.Instance.Hide(EnumView.ViewNPCSell);

            mgToNPCSell.booShow = false;
            ManagerValue.actionGround(intGround, mgToNPCSell);
        });
        btnPageLeft.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            if (intPage > 0)
            {
                intPage -= 1;
                intSelectGrid = 0;
                SelectType(enumSell);
                SelectGrid(intSelectGrid);
            }
        });
        btnPageRight.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            if (intPage < intPageTotal)
            {
                intPage += 1;
                intSelectGrid = 0;
                SelectType(enumSell);
                SelectGrid(intSelectGrid);
            }
        });
        btnSell.onClick.AddListener(() =>
        {
            BackpackGrid item = listGrid[intSelectGrid];
            switch (enumSell)
            {
                case EnumSell.Stock:
                    Dictionary<int, FarmClass.StockCount> dicStockCount = UserValue.Instance.GetStockCountAll();
                    if (dicStockCount[listGrid[intSelectGrid].intID].intStockCount < intSellCount)
                    {
                        intSellCount = dicStockCount[listGrid[intSelectGrid].intID].intStockCount;
                    }
                    if (!UserValue.Instance.StockCountReduce(item.intID, intSellCount))
                    {
                        ManagerValue.actionAudio(EnumAudio.Unable);
                        messageHintBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.QuantityCPTA, null);//"数量变动,请重试!";
                        ManagerView.Instance.Show(EnumView.ViewHintBar);
                        ManagerView.Instance.SetData(EnumView.ViewHintBar, messageHintBar);
                        return;
                    }
                    break;
                case EnumSell.Backpack1:
                    UserValue.Instance.KnapsackProductReduce(UserValue.EnumKnapsackType.Backpack_1, item.intIndex, intSellCount);
                    break;
                case EnumSell.Backpack2:
                    UserValue.Instance.KnapsackProductReduce(UserValue.EnumKnapsackType.Backpack_2, item.intIndex, intSellCount);
                    break;
            }
            if (intSellCount != 0)
            {
                ManagerValue.actionAudio(EnumAudio.CoinBuy);
            }
            else
            {
                ManagerValue.actionAudio(EnumAudio.Ground);
            }
            int intPrice = intSellPrice * intSellCount; ;
            UserValue.Instance.SetCoinAdd = intPrice;
            ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);

            if (enumSell == EnumSell.Backpack1 || enumSell == EnumSell.Backpack2)
            {
                ManagerValue.intNPCSellProductCount += intSellCount;
                //"今年可出售数量：" + (ManagerValue.intNPCSellProductCount - intLimitCount > 0 ? 0 : intLimitCount - ManagerValue.intNPCSellProductCount);
                int intSellLimit = ManagerValue.intNPCSellProductCount - intLimitCount > 0 ? 0 : intLimitCount - ManagerValue.intNPCSellProductCount;
                textLimitSellCount.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.QuantityAFSTY) + UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, intSellLimit.ToString("N0"));
                //背包超过出货限制，则按数量进行税收
                if (ManagerValue.intNPCSellProductCount > intLimitCount)
                {
                    int intTemp = ManagerValue.intNPCSellProductCount - intLimitCount;
                    switch (intTemp / 5000)
                    {
                        case 0:
                            if (intSellPrice >= 10)
                            {
                                intPrice = (int)(intPrice * 0.2f);
                            }
                            else
                            {
                                intPrice = 0;
                            }
                            break;
                        case 1:
                            if (intSellPrice >= 10)
                            {
                                intPrice = (int)(intPrice * 0.25f);
                            }
                            else
                            {
                                intPrice = intSellCount;
                            }
                            break;
                        default:
                            if (intSellPrice >= 100)
                            {
                                intPrice = (int)(intPrice * 0.35f);
                            }
                            else
                            {
                                intPrice = intSellCount;
                            }
                            break;
                    }
                    if (intPrice > 0)
                    {
                        UserValue.Instance.SetCoinReduce(intPrice);
                        ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);

                        ManagerValue.actionAudio(EnumAudio.Unable);
                        messageHintBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.SellingIETAQLIAT, new string[] { UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, intPrice.ToString("N0")) });//"出售物品超过每年的数量限制,支付：" + intPrice + "金币的税金。";
                        ManagerView.Instance.Show(EnumView.ViewHintBar);
                        ManagerView.Instance.SetData(EnumView.ViewHintBar, messageHintBar);
                    }
                    else
                    {
                        ManagerValue.actionAudio(EnumAudio.Ground);
                        //"出售的物品单价没有超过10金币，且没有超过5000的数量限制，不用交税金。";
                        string[] strItemPrices = new string[2];
                        strItemPrices[0] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, "10");
                        strItemPrices[1] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, "5,500");
                        messageHintBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.ItemsSWAUPBGCAQ, strItemPrices);
                        ManagerView.Instance.Show(EnumView.ViewHintBar);
                        ManagerView.Instance.SetData(EnumView.ViewHintBar, messageHintBar);
                    }
                }
            }

            SelectType(enumSell);
            SelectGrid(intSelectGrid);
            intSellCount = 0;
            slider.value = 0;
        });
        btnStock.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            intPage = 0;
            intSelectGrid = 0;
            SelectType(EnumSell.Stock);
            SelectGrid(intSelectGrid);
        });
        btnBackpack1.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            intPage = 0;
            intSelectGrid = 0;
            SelectType(EnumSell.Backpack1);
            SelectGrid(intSelectGrid);
        });
        btnBackpack2.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            intPage = 0;
            intSelectGrid = 0;
            SelectType(EnumSell.Backpack2);
            SelectGrid(intSelectGrid);
        });

        btnSellReduce.onClick.AddListener(() =>
        {
            if (listGrid.Count <= intSelectGrid)
            {
                intSelectGrid = 0;
            }
            if (listGrid.Count == 0)
            {
                return;
            }
            if (intSellCount > 0)
            {
                intSellCount--;
                slider.value = intSellCount;
                textSellCount.text = intSellCount + "/" + listGrid[intSelectGrid].intCount;
                textSliderCount.text = (listGrid[intSelectGrid].intCount - intSellCount).ToString();
                textTotalPrice.text = (intSellCount * intSellPrice).ToString();
            }
        });
        btnSellAdd.onClick.AddListener(() =>
        {
            if (listGrid.Count <= intSelectGrid)
            {
                intSelectGrid = 0;
            }
            if (listGrid.Count == 0)
            {
                return;
            }
            if (enumSell == EnumSell.Stock)
            {
                Dictionary<int, FarmClass.StockCount> dicStockCount = UserValue.Instance.GetStockCountAll();
                slider.maxValue = dicStockCount[listGrid[intSelectGrid].intID].intStockCount;
            }
            else
            {
                slider.maxValue = listGrid[intSelectGrid].intCount;
            }

            if (slider.maxValue > intSellCount)
            {
                intSellCount++;
            }
            slider.value = intSellCount;
            textSellCount.text = intSellCount + "/" + listGrid[intSelectGrid].intCount;
            textSliderCount.text = (listGrid[intSelectGrid].intCount - intSellCount).ToString();
            textTotalPrice.text = (intSellCount * intSellPrice).ToString();
        });

        slider.onValueChanged.AddListener((value) =>
        {
            if (listGrid.Count <= intSelectGrid)
            {
                intSelectGrid = 0;
            }
            if (listGrid.Count == 0)
            {
                return;
            }
            if (enumSell == EnumSell.Stock)
            {
                Dictionary<int, FarmClass.StockCount> dicStockCount = UserValue.Instance.GetStockCountAll();
                slider.maxValue = dicStockCount[listGrid[intSelectGrid].intID].intStockCount;
            }
            textSellCount.text = value + "/" + listGrid[intSelectGrid].intCount;
            textSliderCount.text = (listGrid[intSelectGrid].intCount - value).ToString();
            textTotalPrice.text = (value * intSellPrice).ToString();
            intSellCount = (int)value;
        });

        for (int i = 0; i < transImageGrid.childCount; i++)
        {
            listItem.Add(transImageGrid.GetChild(i).GetComponent<View_PropertiesItem>());
            transImageGrid.GetChild(i).GetComponent<Button>().onClick.AddListener(OnClickItem(i));
        }

        intPage = 0;
        intSelectGrid = 0;
        SelectType(EnumSell.Stock);
        SelectGrid(intSelectGrid);

        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Date, MessageUpdateDate);
    }

    public override void Show()
    {
        base.Show();
        if (listItem.Count != 0)
        {
            intSellCount = 0;
            slider.value = 0;

            intPage = 0;
            intSelectGrid = 0;
            SelectType(EnumSell.Stock);
            SelectGrid(intSelectGrid);
        }

        textPriceTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.UnitPrice);
        textProductCountTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.CurrentQuantity) + ":";
        textSellCountTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.QuantityForSale);
        textTotalPriceTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.EarnedCoins);

        btnSell.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Sell);
        btnStock.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Warehouse);
        btnBackpack1.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Backpack) + " 1";
        btnBackpack2.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Backpack) + " 2";

        //"今年可出售数量：" + (ManagerValue.intNPCSellProductCount - intLimitCount > 0 ? 0 : intLimitCount - ManagerValue.intNPCSellProductCount);
        int intSellLimit = ManagerValue.intNPCSellProductCount - intLimitCount > 0 ? 0 : intLimitCount - ManagerValue.intNPCSellProductCount;
        textLimitSellCount.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.QuantityAFSTY) + UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, intSellLimit.ToString("N0"));
    }

    public override void SetData(Message message)
    {
        ViewMGToViewNPCSell mg = message as ViewMGToViewNPCSell;
        if (mg != null)
        {
            textTitle.text = mg.strTitle;
            intGround = mg.intGround;

            if (enumSell == EnumSell.Stock)
            {
                SelectType(enumSell);
                SelectGrid(intSelectGrid);
            }
        }
    }

    UnityAction OnClickItem(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            intSellCount = 0;
            slider.value = 0;
            intSelectGrid = intIndex;
            SelectGrid(intIndex);
        };
    }

    /// <summary>
    /// 选中的物品
    /// </summary>
    void SelectGrid(int intIndex)
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            listItem[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        int intItemCount = intIndex % listItem.Count;
        listItem[intItemCount].transform.GetChild(0).gameObject.SetActive(true);

        if (intItemCount + intPage * listItem.Count < listGrid.Count)
        {
            intSelectGrid = intItemCount + intPage * listItem.Count;

            product.imageValueMain.gameObject.SetActive(true);

            if (listGrid[intSelectGrid].enumStockType == EnumKnapsackStockType.Sword
                || listGrid[intSelectGrid].enumStockType == EnumKnapsackStockType.Bow
                || listGrid[intSelectGrid].enumStockType == EnumKnapsackStockType.Wand
                || listGrid[intSelectGrid].enumStockType == EnumKnapsackStockType.Armor
                || listGrid[intSelectGrid].enumStockType == EnumKnapsackStockType.Shoes)
            {
                product.imageValueMain.sprite = ManagerResources.Instance.GetEquipmentSprite(listGrid[intSelectGrid].icon);
                product.imageValue.sprite = ManagerResources.Instance.GetFrameRank(listGrid[intSelectGrid].intRank.ToString());
                product.textValueMain.text = listGrid[intSelectGrid].strName;
                intSellCount = 1;
                intSellPrice = listGrid[intSelectGrid].intPrice;
                slider.gameObject.SetActive(false);
            }
            else if (listGrid[intSelectGrid].enumStockType == EnumKnapsackStockType.Farm
                || listGrid[intSelectGrid].enumStockType == EnumKnapsackStockType.Fasture
                || listGrid[intSelectGrid].enumStockType == EnumKnapsackStockType.Factory)
            {
                product.imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(listGrid[intSelectGrid].icon);
                product.imageValue.sprite = ManagerResources.Instance.GetFrameRank(listGrid[intSelectGrid].intRank.ToString());
                product.textValueMain.text = ManagerProduct.Instance.GetName(listGrid[intSelectGrid].intID, false);
                intSellPrice = ManagerCompound.Instance.GetProductPrice(listGrid[intSelectGrid].intID);
                slider.gameObject.SetActive(true);
            }

            //定义收购价格
            intSellPrice = (int)(intSellPrice * 0.5f);
            if (intSellPrice <= 1)
            {
                intSellPrice = 1;
            }
            product.textValue.text = intSellPrice.ToString();

            textSliderCount.text = (listGrid[intSelectGrid].intCount - intSellCount).ToString();
            textTotalPrice.text = (intSellCount * intSellPrice).ToString();
            slider.maxValue = listGrid[intSelectGrid].intCount;
            textSellCount.text = intSellCount + "/" + listGrid[intSelectGrid].intCount;
        }
        else
        {
            product.textValue.text = "";
            product.imageValueMain.gameObject.SetActive(false);
            product.imageValue.sprite = ManagerResources.Instance.GetFrameRank("1");
            product.textValueMain.text = "";

            slider.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 获取选择的类型
    /// </summary>
    void SelectType(EnumSell key)
    {
        enumSell = key;
        listGrid.Clear();

        switch (key)
        {
            case EnumSell.Stock:
                textSelectType.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Warehouse);//"仓库";
                FarmClass.StockProduction[] itemStocks = UserValue.Instance.GetStockProductionOrder();
                Dictionary<int, FarmClass.StockCount> dicStockCount = UserValue.Instance.GetStockCountAll();
                for (int i = 0; i < itemStocks.Length; i++)
                {
                    BackpackGrid item = new BackpackGrid();
                    ManagerValue.SetProductItem(itemStocks[i].intProductID, item);
                    item.intRank = 1;
                    item.intCount = dicStockCount.ContainsKey(item.intID) ? dicStockCount[item.intID].intStockCount : 0;
                    listGrid.Add(item);
                }
                break;
            case EnumSell.Backpack1:
                textSelectType.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Backpack) + " 1";//"背包1";
                BackpackGrid[] grid1 = UserValue.Instance.GetKnapsackItems(UserValue.EnumKnapsackType.Backpack_1);
                for (int i = 0; i < grid1.Length; i++)
                {
                    listGrid.Add(grid1[i]);
                }
                break;
            case EnumSell.Backpack2:
                textSelectType.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Backpack) + " 2";//"背包2";
                BackpackGrid[] grid2 = UserValue.Instance.GetKnapsackItems(UserValue.EnumKnapsackType.Backpack_2);
                for (int i = 0; i < grid2.Length; i++)
                {
                    listGrid.Add(grid2[i]);
                }
                break;
        }

        intPageTotal = listGrid.Count / listItem.Count;
        if (intPage > intPageTotal)
        {
            intPage = intPageTotal;
        }
        textPage.text = (intPage + 1) + "/" + (intPageTotal + 1);

        //这里实现选择的页数显示,因为不缺定物品是否实时改变,所以每次执行都会重新获取
        int intIndexTemp = intPage * listItem.Count;

        for (int i = 0; i < listItem.Count; i++)
        {
            if (i + intIndexTemp < listGrid.Count)
            {
                listItem[i].imageValueMain.gameObject.SetActive(true);

                if (listGrid[i + intIndexTemp].enumStockType == EnumKnapsackStockType.Sword
                    || listGrid[i + intIndexTemp].enumStockType == EnumKnapsackStockType.Bow
                    || listGrid[i + intIndexTemp].enumStockType == EnumKnapsackStockType.Wand
                    || listGrid[i + intIndexTemp].enumStockType == EnumKnapsackStockType.Armor
                    || listGrid[i + intIndexTemp].enumStockType == EnumKnapsackStockType.Shoes)
                {
                    listItem[i].imageValueMain.sprite = ManagerResources.Instance.GetEquipmentSprite(listGrid[i + intIndexTemp].icon);
                    listItem[i].imageValue.sprite = ManagerResources.Instance.GetFrameRank(listGrid[i + intIndexTemp].intRank.ToString());
                    listItem[i].textValueMain.text = listGrid[i + intIndexTemp].intCount.ToString();
                }
                else if (listGrid[i + intIndexTemp].enumStockType == EnumKnapsackStockType.Farm
                    || listGrid[i + intIndexTemp].enumStockType == EnumKnapsackStockType.Fasture
                    || listGrid[i + intIndexTemp].enumStockType == EnumKnapsackStockType.Factory)
                {
                    listItem[i].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(listGrid[i + intIndexTemp].icon);
                    listItem[i].imageValue.sprite = ManagerResources.Instance.GetFrameRank("1");
                    listItem[i].textValueMain.text = listGrid[i + intIndexTemp].intCount.ToString();

                }
                continue;
            }
            listItem[i].imageValueMain.gameObject.SetActive(false);
            listItem[i].imageValue.sprite = ManagerResources.Instance.GetFrameRank("1");
            listItem[i].textValueMain.text = "";
        }
    }

    void MessageUpdateDate(ManagerMessage.MessageBase message)
    {
        MessageDate date = message as MessageDate;
        if (date != null)
        {
            if (date.numMonth == 1 && date.numDay == 1)
            {
                ManagerValue.intNPCSellProductCount = 0;
                //"今年可出售数量：" + (ManagerValue.intNPCSellProductCount - intLimitCount > 0 ? 0 : intLimitCount - ManagerValue.intNPCSellProductCount);
                int intSellLimit = ManagerValue.intNPCSellProductCount - intLimitCount > 0 ? 0 : intLimitCount - ManagerValue.intNPCSellProductCount;
                textLimitSellCount.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.QuantityAFSTY) + UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, intSellLimit.ToString("N0"));
            }
        }
    }

    private void OnDestroy()
    {
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Update_Date, MessageUpdateDate);
    }

    /// <summary>
    /// 选择的标记类型
    /// </summary>
    enum EnumSell
    {
        None,
        Stock,
        Backpack1,
        Backpack2,
    }
}
