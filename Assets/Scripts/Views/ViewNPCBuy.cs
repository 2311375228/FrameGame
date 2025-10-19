using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ViewNPCBuy : ViewBase
{
    public Text textTitle;
    public Text textGridName;
    public Text textGridCount;
    public Text textGridPrice;
    public Text textGridTotalPrice;
    public Text textContent;
    public Text textPage;
    public Text textRefreshTime;//下次进货时间

    public Text textCountTag;//购买数量
    public Text textPriceTag;//购买单价
    public Text textTotalPriceTag;//购买总价

    public Image imageGrid;
    public Image imageGridFrame;
    public Button btnClose;
    public Button btnBuy;
    public Button btnLeft;
    public Button btnRight;

    public Transform transList;

    List<View_PropertiesItem> listItem;
    BackpackGrid[] dataGrid;

    int intIndexSelect;
    int intPage;
    int intPageTotal;
    ViewMGToViewNPCBuy mgToViewNPCBuy;
    MGViewToBuildNPCBuy mgToBuildNPC = new MGViewToBuildNPCBuy();

    protected override void Start()
    {
        btnClose.onClick.AddListener(() => { SendClose(); });
        btnBuy.onClick.AddListener(() =>
        {
            BackpackGrid temp = dataGrid[intPage * listItem.Count + intIndexSelect];

            if (temp.enumStockType == EnumKnapsackStockType.Sword
            || temp.enumStockType == EnumKnapsackStockType.Bow
            || temp.enumStockType == EnumKnapsackStockType.Wand
            || temp.enumStockType == EnumKnapsackStockType.Armor
            || temp.enumStockType == EnumKnapsackStockType.Shoes)
            {
                JsonValue.TableEquipmentItem item = ManagerCombat.Instance.GetEquipmentItem(temp.intID);
                if (UserValue.Instance.SetCoinReduce(item.intPrice))
                {
                    SendBuy();
                }
            }
            else if (temp.enumStockType == EnumKnapsackStockType.Farm
            || temp.enumStockType == EnumKnapsackStockType.Fasture
            || temp.enumStockType == EnumKnapsackStockType.Factory)
            {
                JsonValue.DataTableBackPackItem itemProduct = ManagerProduct.Instance.GetProductTableItem(temp.intID);
                if (UserValue.Instance.SetCoinReduce(ManagerCompound.Instance.GetProductPrice(itemProduct.intProductID)))
                {
                    SendBuy();
                }
            }

        });
        btnLeft.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            if (intPage > 0)
            {
                intPage -= 1;
                intIndexSelect = 0;
                PageShowItem();
                SelectItemGrid(intIndexSelect);
            }

        });
        btnRight.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            if (intPage < intPageTotal)
            {
                intPage += 1;
                intIndexSelect = 0;
                PageShowItem();
                SelectItemGrid(intIndexSelect);
            }
        });

        textRefreshTime.text = "";

    }

    public override void Show()
    {
        base.Show();
        textCountTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.QuantityToPurchase);
        textPriceTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.UnitPrice);
        textTotalPriceTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.TotalPrice);
        btnBuy.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Purchase);

        if (listItem == null)
        {
            listItem = new List<View_PropertiesItem>();
            for (int i = 0; i < transList.childCount; i++)
            {
                listItem.Add(transList.GetChild(i).GetComponent<View_PropertiesItem>());
                listItem[i].GetComponent<Button>().onClick.AddListener(OnClickItem(i));
            }
        }

        HideItem();
    }

    public override void SetData(Message message)
    {
        mgToViewNPCBuy = message as ViewMGToViewNPCBuy;
        if (mgToViewNPCBuy != null)
        {
            dataGrid = mgToViewNPCBuy.equipmentScrolls;
            textTitle.text = mgToViewNPCBuy.strBuildName;
            textRefreshTime.text = mgToViewNPCBuy.strRefreshTime;

            if (mgToViewNPCBuy.booRefreshData)
            {
                intPage = 0;
                intPageTotal = dataGrid.Length / listItem.Count;
                intIndexSelect = 0;
            }
        }
        PageShowItem();
        SelectItemGrid(intIndexSelect);
        SelectItem(intIndexSelect);
    }
    /// <summary>
    /// 当前页面显示
    /// </summary>
    void PageShowItem()
    {
        int intTemp = 0;
        if (intPage != intPageTotal)
        {
            intTemp = listItem.Count;
        }
        else
        {
            intTemp = dataGrid.Length % listItem.Count;
        }
        for (int i = 0; i < listItem.Count; i++)
        {
            if (intTemp > i)
            {
                BackpackGrid item = dataGrid[intPage * listItem.Count + i];
                if (item == null)
                {
                    listItem[i].imageValueMain.gameObject.SetActive(false);
                    listItem[i].imageValue.sprite = ManagerResources.Instance.GetFrameRank("1");
                    continue;
                }

                if (item.enumStockType == EnumKnapsackStockType.Sword
                || item.enumStockType == EnumKnapsackStockType.Bow
                || item.enumStockType == EnumKnapsackStockType.Wand
                || item.enumStockType == EnumKnapsackStockType.Armor
                || item.enumStockType == EnumKnapsackStockType.Shoes)
                {
                    listItem[i].imageValueMain.sprite = ManagerResources.Instance.GetEquipmentSprite(item.icon);
                    listItem[i].textValueMain.text = "";
                    listItem[i].textValueMain.gameObject.SetActive(false);
                }
                else if (item.enumStockType == EnumKnapsackStockType.Farm
                || item.enumStockType == EnumKnapsackStockType.Fasture
                || item.enumStockType == EnumKnapsackStockType.Factory)
                {
                    listItem[i].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(item.icon);
                    listItem[i].textValueMain.text = item.intCount.ToString();
                    listItem[i].textValueMain.gameObject.SetActive(true);
                }

                listItem[i].imageValueMain.gameObject.SetActive(true);

                RectTransform rect = listItem[i].imageValueMain.GetComponent<RectTransform>();
                rect.sizeDelta = Tools.SetSpriteRectSize(rect.sizeDelta, listItem[i].imageValueMain.sprite);
                listItem[i].imageValue.sprite = ManagerResources.Instance.GetFrameRank(item.intRank.ToString());
            }
            else
            {
                listItem[i].imageValueMain.gameObject.SetActive(false);
                listItem[i].imageValue.sprite = ManagerResources.Instance.GetFrameRank("1");
            }
        }
    }

    void HideItem()
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            listItem[i].transform.GetChild(0).gameObject.SetActive(false);
            listItem[i].imageValueMain.gameObject.SetActive(false);
            listItem[i].textValueMain.gameObject.SetActive(false);
        }
    }

    UnityAction OnClickItem(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            SelectItem(intIndex);
        };
    }
    /// <summary>
    /// 处理单个框显示
    /// </summary>
    void SelectItem(int intIndex)
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            if (i == intIndex)
            {
                SelectItemGrid(i);
                listItem[i].transform.GetChild(0).gameObject.SetActive(true);
                int intIndexTemp = dataGrid.Length % listItem.Count;
                if (intIndexTemp > i && dataGrid[i] == null)
                {
                    listItem[i].textValueMain.gameObject.SetActive(false);
                    continue;
                }
                if (intIndexTemp > i)
                {

                    if (dataGrid[i].enumStockType == EnumKnapsackStockType.Sword
|| dataGrid[i].enumStockType == EnumKnapsackStockType.Bow
|| dataGrid[i].enumStockType == EnumKnapsackStockType.Wand
|| dataGrid[i].enumStockType == EnumKnapsackStockType.Armor
|| dataGrid[i].enumStockType == EnumKnapsackStockType.Shoes)
                    {
                        listItem[i].textValueMain.gameObject.SetActive(false);
                    }
                    else if (dataGrid[i].enumStockType == EnumKnapsackStockType.Farm
                    || dataGrid[i].enumStockType == EnumKnapsackStockType.Fasture
                    || dataGrid[i].enumStockType == EnumKnapsackStockType.Factory)
                    {
                        listItem[i].textValueMain.gameObject.SetActive(true);
                        listItem[i].textValueMain.text = dataGrid[i].intCount.ToString();
                    }
                }
                else
                {
                    listItem[i].textValueMain.gameObject.SetActive(false);
                }

                continue;
            }
            listItem[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void SelectItemGrid(int intIndex)
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            listItem[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        listItem[intIndex].transform.GetChild(0).gameObject.SetActive(true);

        textPage.text = (intPage + 1) + "/" + (intPageTotal + 1);

        intIndexSelect = intIndex;


        int intTemp = 0;
        if (intPage != intPageTotal)
        {
            intTemp = listItem.Count;
        }
        else
        {
            intTemp = dataGrid.Length % listItem.Count;
        }

        if (intIndex < intTemp)
        {
            BackpackGrid item = dataGrid[intPage * listItem.Count + intIndex];
            if (item == null)
            {
                btnBuy.gameObject.SetActive(false);
                imageGrid.gameObject.SetActive(false);
                imageGridFrame.sprite = ManagerResources.Instance.GetFrameRank("1");

                textGridName.text = "";
                textGridPrice.text = "";
                textGridTotalPrice.text = "";
                textGridCount.text = "";
                textContent.text = "";
                return;
            }
            btnBuy.gameObject.SetActive(true);
            imageGrid.gameObject.SetActive(true);
            imageGridFrame.sprite = ManagerResources.Instance.GetFrameRank(item.intRank.ToString());
            textGridName.text = item.strName;
            textGridPrice.text = item.intPrice.ToString();
            textGridTotalPrice.text = (item.intPrice * item.intCount).ToString();
            textGridCount.text = item.intCount.ToString();
            textContent.text = "";

            if (item.enumStockType == EnumKnapsackStockType.Sword
|| item.enumStockType == EnumKnapsackStockType.Bow
|| item.enumStockType == EnumKnapsackStockType.Wand
|| item.enumStockType == EnumKnapsackStockType.Armor
|| item.enumStockType == EnumKnapsackStockType.Shoes)
            {
                imageGrid.sprite = ManagerResources.Instance.GetEquipmentSprite(item.icon);
                if (item.intRank > 10)
                {
                    textGridName.text += "(" + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Scrolls) + ")";
                }
            }
            else if (item.enumStockType == EnumKnapsackStockType.Farm
            || item.enumStockType == EnumKnapsackStockType.Fasture
            || item.enumStockType == EnumKnapsackStockType.Factory)
            {
                imageGrid.sprite = ManagerResources.Instance.GetBackpackSprite(item.icon);
            }
        }
        else
        {
            btnBuy.gameObject.SetActive(false);
            imageGrid.gameObject.SetActive(false);
            imageGridFrame.sprite = ManagerResources.Instance.GetFrameRank("1");

            textGridName.text = "";
            textGridPrice.text = "";
            textGridTotalPrice.text = "";
            textGridCount.text = "";
            textContent.text = "";
        }
    }
    void SendBuy()
    {
        mgToBuildNPC.booShow = true;
        mgToBuildNPC.intBuyEquipmentID = intPage * listItem.Count + intIndexSelect;
        mgToBuildNPC.intIndexGround = mgToViewNPCBuy.intIndexGround;
        ManagerValue.actionGround(mgToBuildNPC.intIndexGround, mgToBuildNPC);
    }
    void SendClose()
    {
        mgToBuildNPC.booShow = false;
        mgToBuildNPC.intBuyEquipmentID = -1;
        mgToBuildNPC.intIndexGround = mgToViewNPCBuy.intIndexGround;
        ManagerValue.actionGround(mgToBuildNPC.intIndexGround, mgToBuildNPC);
        ManagerView.Instance.Hide(EnumView.ViewNPCBuy);
    }

    public enum EnumNPCBuy
    {
        None,
        Farm,
        Equipment,
    }
}
