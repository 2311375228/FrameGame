using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewShop_Sell : MonoBehaviour
{
    public Text textSellTag;
    public Text textSellNum;

    public ScrollCycleColumn columnItem;
    int intSellData;
    List<ViewShop_SubItem> listItemSub = new List<ViewShop_SubItem>();
    /// <summary>
    /// 存入的是下标
    /// </summary>
    List<int> listIndexSellData = new List<int>();

    [System.NonSerialized]
    public int intProductIDIndex;
    [System.NonSerialized]
    public int intSellTime;
    [System.NonSerialized]
    public int[] intProductSellIDs;
    [System.NonSerialized]
    public EnumKnapsackStockType[] enumGridItems;
    [System.NonSerialized]
    public int[] intProductSellPrices;
    [System.NonSerialized]
    public int[] intProductSellNums;
    [System.NonSerialized]
    public int[] intProductSellRipe;
    [System.NonSerialized]
    public int[] intResidueTimes;

    public System.Action SendSellProductDown;

    public void Show()
    {
    }

    public void SetShowList()
    {
        listIndexSellData.Clear();
        for (int i = 0; i < intProductSellIDs.Length; i++)
        {
            if (intProductSellIDs[i] != -1)
            {
                listIndexSellData.Add(i);
            }
        }

        int intTemp = listIndexSellData.Count / 2;
        if (listIndexSellData.Count % 2 == 1)
        {
            intTemp += 1;
        }
        intSellData = intTemp;

        RectTransform[] rectItems = columnItem.SetDataTotal(intTemp);
        for (int i = 0; i < rectItems.Length; i++)
        {
            ViewShop_SubItem itemShop = rectItems[i].GetComponent<ViewShop_SubItem>();
            itemShop.numIndexItem = i;
            itemShop.numIndexData = i;
            itemShop.actionSellDown = ActionEventSellDown;
            RefreshData(itemShop, i, i);
        }
    }


    void RefreshData(ViewShop_SubItem itemShop, int numIndexItem, int numIndexData)
    {
        ViewShop_SubItemSell[] itemTemp = itemShop.itemSells;
        if (numIndexData >= intSellData)
        {
            listItemSub[numIndexItem].gameObject.SetActive(false);
        }
        else
        {
            itemShop.gameObject.SetActive(true);
            itemShop.numIndexItem = numIndexItem;
            itemShop.numIndexData = numIndexData;

            int numIndexTemp = numIndexData * 2;
            itemTemp[0].itemProduct.imageValueMain.sprite = null;
            itemTemp[1].itemProduct.imageValueMain.sprite = null;

            int intIndexTemp = listIndexSellData[numIndexTemp];
            JsonValue.DataTableBackPackItem itemProduct = null;
            JsonValue.TableEquipmentItem itemEquipment = null;
            if (enumGridItems[intIndexTemp] == EnumKnapsackStockType.Sword
                || enumGridItems[intIndexTemp] == EnumKnapsackStockType.Bow
                || enumGridItems[intIndexTemp] == EnumKnapsackStockType.Wand
                || enumGridItems[intIndexTemp] == EnumKnapsackStockType.Armor
                || enumGridItems[intIndexTemp] == EnumKnapsackStockType.Shoes)
            {
                itemEquipment = ManagerCombat.Instance.GetEquipmentItem(intProductSellIDs[intIndexTemp]);
                itemTemp[0].itemProduct.textValueMain.text = itemEquipment.strNameChina;
                itemTemp[0].itemProduct.imageValueMain.sprite = ManagerResources.Instance.GetEquipmentSprite(itemEquipment.strICON);
            }
            else if (enumGridItems[intIndexTemp] == EnumKnapsackStockType.Farm
                || enumGridItems[intIndexTemp] == EnumKnapsackStockType.Fasture
                || enumGridItems[intIndexTemp] == EnumKnapsackStockType.Factory)
            {
                itemProduct = ManagerProduct.Instance.GetProductTableItem(intProductSellIDs[intIndexTemp]);
                itemTemp[0].itemProduct.textValueMain.text = ManagerProduct.Instance.GetName(itemProduct.intProductID, false);//itemProduct.GetName(false);
                itemTemp[0].itemProduct.imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(itemProduct.strIconName);
            }

            itemTemp[0].itemProduct.textValue.text = intProductSellNums[intIndexTemp].ToString();
            itemTemp[0].textPrice.text = intProductSellPrices[intIndexTemp].ToString();
            itemTemp[0].textBtnItem.text = "下架";
            itemTemp[0].sliderDay.maxValue = intProductSellRipe[intIndexTemp];
            itemTemp[0].sliderDay.value = intResidueTimes[intIndexTemp] - intSellTime;
            itemTemp[0].textTime.text = (intResidueTimes[intIndexTemp] - intSellTime) + "/" + intProductSellRipe[intIndexTemp];

            if (numIndexTemp < listIndexSellData.Count - 1 || numIndexTemp == listIndexSellData.Count - 2)
            {
                intIndexTemp = listIndexSellData[numIndexTemp + 1];
                itemTemp[1].gameObject.SetActive(true);

                if (enumGridItems[intIndexTemp] == EnumKnapsackStockType.Sword
                    || enumGridItems[intIndexTemp] == EnumKnapsackStockType.Bow
                    || enumGridItems[intIndexTemp] == EnumKnapsackStockType.Wand
                    || enumGridItems[intIndexTemp] == EnumKnapsackStockType.Armor
                    || enumGridItems[intIndexTemp] == EnumKnapsackStockType.Shoes)
                {
                    itemEquipment = ManagerCombat.Instance.GetEquipmentItem(intProductSellIDs[intIndexTemp]);
                    itemTemp[1].itemProduct.textValueMain.text = itemEquipment.strNameChina;
                    itemTemp[1].itemProduct.imageValueMain.sprite = ManagerResources.Instance.GetEquipmentSprite(itemEquipment.strICON);
                }
                else if (enumGridItems[intIndexTemp] == EnumKnapsackStockType.Farm
                    || enumGridItems[intIndexTemp] == EnumKnapsackStockType.Fasture
                    || enumGridItems[intIndexTemp] == EnumKnapsackStockType.Factory)
                {
                    itemProduct = ManagerProduct.Instance.GetProductTableItem(intProductSellIDs[intIndexTemp]);
                    itemTemp[1].itemProduct.textValueMain.text = ManagerProduct.Instance.GetName(itemProduct.intProductID, false);//itemProduct.GetName(false);
                    itemTemp[1].itemProduct.imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(itemProduct.strIconName);
                }

                itemTemp[1].itemProduct.textValue.text = intProductSellNums[intIndexTemp].ToString();
                itemTemp[1].textPrice.text = intProductSellPrices[intIndexTemp].ToString();
                itemTemp[1].sliderDay.maxValue = intProductSellRipe[intIndexTemp];
                itemTemp[1].sliderDay.value = intResidueTimes[intIndexTemp] - intSellTime;
                itemTemp[1].textTime.text = (intResidueTimes[intIndexTemp] - intSellTime) + "/" + intProductSellRipe[intIndexTemp];
            }
            else
            {
                itemTemp[1].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 下架按钮
    /// </summary>
    void ActionEventSellDown(int intIndexItem, int intIndexData)
    {
        BackpackGrid item = new BackpackGrid();
        if (enumGridItems[intIndexData] == EnumKnapsackStockType.Sword
            || enumGridItems[intIndexData] == EnumKnapsackStockType.Bow
            || enumGridItems[intIndexData] == EnumKnapsackStockType.Wand
            || enumGridItems[intIndexData] == EnumKnapsackStockType.Armor
            || enumGridItems[intIndexData] == EnumKnapsackStockType.Shoes)
        {
            ManagerValue.SetEquipmentItem(intProductSellIDs[listIndexSellData[intIndexData]], item);
        }
        else if (enumGridItems[intIndexData] == EnumKnapsackStockType.Farm
            || enumGridItems[intIndexData] == EnumKnapsackStockType.Fasture
            || enumGridItems[intIndexData] == EnumKnapsackStockType.Factory)
        {
            ManagerValue.SetProductItem(intProductSellIDs[listIndexSellData[intIndexData]], item);
        }

        item.intCount = intProductSellNums[listIndexSellData[intIndexData]];
        intProductIDIndex = listIndexSellData[intIndexData];

        UserValue.Instance.KnapsackProductAddGrid(item);

        SendSellProductDown();
    }

}
