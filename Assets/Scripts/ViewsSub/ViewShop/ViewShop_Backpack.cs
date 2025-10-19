using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewShop_Backpack : MonoBehaviour
{
    public Text textSellProductTypeTag;
    public Text textSellProductTypeNum;
    public View_PropertiesItem itemProductProperties;
    public VIewShop_BackpackSoldOut[] soldOuts;

    public ScrollCycleColumn columnItemBackpack;

    [System.NonSerialized]
    public int intProductID;
    [System.NonSerialized]
    public EnumKnapsackStockType gridItem;
    [System.NonSerialized]
    public int intProductCount;//产品总量
    [System.NonSerialized]
    public int intProductPrice;//产品总价
    [System.NonSerialized]
    public int intProductTime;//出售时间
    public System.Action SendSellProduct;

    int[] intPrices = new int[4];
    int[] intCounts = new int[4];
    int[] intTimes = new int[4];
    BackpackGrid[] backpackGrids;
    List<ViewShop_BackpackItem> listBackpackItem = new List<ViewShop_BackpackItem>();

    public void Show()
    {
        GetBackpackData();
        if (backpackGrids.Length == 0)
        {
            SelectProduct(-1);
        }
        else
        {
            SelectProduct(0);
            for (int i = 0; i < listBackpackItem.Count; i++)
            {
                if (i == 0)
                {
                    listBackpackItem[i].goSelect.SetActive(true);
                    continue;
                }
                listBackpackItem[i].goSelect.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 选择出售价格并出售
    /// </summary>
    public bool SelectSell(int intIndex)
    {
        switch (intIndex)
        {
            case 0:
                intProductPrice = intPrices[0];
                intProductCount = intCounts[0];
                intProductTime = intTimes[0];
                break;
            case 1:
                intProductPrice = intPrices[1];
                intProductCount = intCounts[1];
                intProductTime = intTimes[1];
                break;
            case 2:
                intProductPrice = intPrices[2];
                intProductCount = intCounts[2];
                intProductTime = intTimes[2];
                break;
        }

        if (UserValue.Instance.GetKnapsackProductCount(intProductID) >= intProductCount)
        {
            UserValue.Instance.KnapsackProductReduce(intProductID, intProductCount);
            return true;
        }
        return false;
    }

    public void GetBackpackData()
    {
        backpackGrids = UserValue.Instance.GetKnapsackItems();

        RectTransform[] rectItems = columnItemBackpack.SetDataTotal(backpackGrids.Length);
        for (int i = 0; i < rectItems.Length; i++)
        {
            ViewShop_BackpackItem itemBackpack = rectItems[i].GetComponent<ViewShop_BackpackItem>();
            itemBackpack.numIndexItem = i;
            itemBackpack.numIndexData = i;
            itemBackpack.actionBase = ActionEventBackpackItem;
            itemBackpack.actionSell = ActionEventBackpackSell;
            RefreshPackbackData(itemBackpack, i, i);
        }

        if (backpackGrids.Length == 0)
        {
            itemProductProperties.imageValueMain.gameObject.SetActive(false);
            itemProductProperties.textValueMain.text = "";
            soldOuts[0].gameObject.SetActive(false);
            soldOuts[1].gameObject.SetActive(false);
            soldOuts[2].gameObject.SetActive(false);
        }
        else
        {
            itemProductProperties.imageValueMain.gameObject.SetActive(true);

            if (backpackGrids[0].enumStockType == EnumKnapsackStockType.Sword
                || backpackGrids[0].enumStockType == EnumKnapsackStockType.Bow
                || backpackGrids[0].enumStockType == EnumKnapsackStockType.Wand
                || backpackGrids[0].enumStockType == EnumKnapsackStockType.Armor
                || backpackGrids[0].enumStockType == EnumKnapsackStockType.Shoes)
            {
                JsonValue.TableEquipmentItem itemEquipment = ManagerCombat.Instance.GetEquipmentItem(backpackGrids[0].intID);
                itemProductProperties.imageValueMain.sprite = ManagerResources.Instance.GetEquipmentSprite(itemEquipment.strICON);
                itemProductProperties.textValueMain.text = itemEquipment.strNameChina;
            }
            else if (backpackGrids[0].enumStockType == EnumKnapsackStockType.Farm
                || backpackGrids[0].enumStockType == EnumKnapsackStockType.Fasture
                || backpackGrids[0].enumStockType == EnumKnapsackStockType.Factory)
            {
                JsonValue.DataTableBackPackItem itemProduct = ManagerProduct.Instance.GetProductTableItem(backpackGrids[0].intID);
                itemProductProperties.imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(itemProduct.strIconName);
                itemProductProperties.textValueMain.text = ManagerProduct.Instance.GetName(itemProduct.intProductID, false);//itemProduct.GetName(false);
            }
        }
    }

    /// <summary>
    /// 选择背包物品
    /// </summary>
    void SelectProduct(int intIndex)
    {
        if (intIndex == -1)
        {
            itemProductProperties.textValueMain.text = "-";
            itemProductProperties.textValue.text = "0";
            itemProductProperties.imageValueMain.gameObject.SetActive(false);

            soldOuts[0].gameObject.SetActive(false);
            soldOuts[1].gameObject.SetActive(false);
            soldOuts[2].gameObject.SetActive(false);
            return;
        }
        intProductID = backpackGrids[intIndex].intID;
        gridItem = backpackGrids[intIndex].enumStockType;

        int intPrice = 0;

        if (backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Sword
            || backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Bow
            || backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Wand
            || backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Armor
            || backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Shoes)
        {
            JsonValue.TableEquipmentItem itemEquipment = ManagerCombat.Instance.GetEquipmentItem(intProductID);
            intPrice = itemEquipment.intPrice;
        }
        else if (backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Farm
            || backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Fasture
            || backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Factory)
        {
            JsonValue.DataTableBackPackItem itemProduct = ManagerProduct.Instance.GetProductTableItem(intProductID);
            intPrice = ManagerCompound.Instance.GetProductPrice(itemProduct.intProductID);
        }

        intPrices[0] = intPrice;
        intPrices[1] = intPrice;
        intPrices[2] = intPrice;

        if (intPrices[0] > 10)
        {
            intPrices[0] = (int)(intPrices[0] * 0.6f);
        }
        else
        {
            intPrices[0] = 1;
        }
        intPrices[0] = intPrices[0];
        intCounts[0] = 1;
        intTimes[0] = 5;
        soldOuts[0].textProductDay.text = intTimes[0].ToString();
        soldOuts[0].textProductPrice.text = intPrices[0].ToString();

        intPrices[1] = (int)(intPrices[1] * 50 * 0.65f);
        intCounts[1] = 50;
        intTimes[1] = 45;

        intPrices[2] = (int)(intPrices[2] * 500 * 0.7f);
        intCounts[2] = 500;
        intTimes[2] = 380;

        soldOuts[0].textProductDay.text = intTimes[0].ToString();
        soldOuts[1].textProductDay.text = intTimes[1].ToString();
        soldOuts[2].textProductDay.text = intTimes[2].ToString();

        soldOuts[0].textProductPrice.text = intPrices[0].ToString();
        soldOuts[1].textProductPrice.text = intPrices[1].ToString();
        soldOuts[2].textProductPrice.text = intPrices[2].ToString();

        soldOuts[0].textProductNum.text = intCounts[0].ToString();
        soldOuts[1].textProductNum.text = intCounts[1].ToString();
        soldOuts[2].textProductNum.text = intCounts[2].ToString();

        soldOuts[0].textProductNum.text = "1";
        soldOuts[1].textProductNum.text = "50";
        soldOuts[2].textProductNum.text = "500";

        if (backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Sword
            || backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Bow
            || backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Wand
            || backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Armor
            || backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Shoes)
        {
            JsonValue.TableEquipmentItem itemEquipment = ManagerCombat.Instance.GetEquipmentItem(backpackGrids[intIndex].intID);
            itemProductProperties.imageValueMain.sprite = ManagerResources.Instance.GetEquipmentSprite(itemEquipment.strICON);
            itemProductProperties.textValueMain.text = itemEquipment.strNameChina;
            soldOuts[0].gameObject.SetActive(true);
            soldOuts[1].gameObject.SetActive(false);
            soldOuts[2].gameObject.SetActive(false);
        }
        else if (backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Farm
            || backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Fasture
            || backpackGrids[intIndex].enumStockType == EnumKnapsackStockType.Factory)
        {
            JsonValue.DataTableBackPackItem itemProduct = ManagerProduct.Instance.GetProductTableItem(backpackGrids[intIndex].intID);
            itemProductProperties.imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(itemProduct.strIconName);
            itemProductProperties.textValueMain.text = ManagerProduct.Instance.GetName(itemProduct.intProductID, false);//itemProduct.GetName(false);
            soldOuts[0].gameObject.SetActive(true);
            soldOuts[1].gameObject.SetActive(true);
            soldOuts[2].gameObject.SetActive(true);
        }

        itemProductProperties.textValue.text = backpackGrids[intIndex].intCount.ToString();
    }

    void RefreshPackbackData(ViewShop_BackpackItem itemTemp, int numIndexItem, int numIndexData)
    {
        if (numIndexData >= backpackGrids.Length)
        {
            itemTemp.gameObject.SetActive(false);
        }
        else
        {
            itemTemp.gameObject.SetActive(true);
            itemTemp.numIndexItem = numIndexItem;
            itemTemp.numIndexData = numIndexData;

            if (backpackGrids[numIndexData].enumStockType == EnumKnapsackStockType.Sword
                || backpackGrids[numIndexData].enumStockType == EnumKnapsackStockType.Bow
                || backpackGrids[numIndexData].enumStockType == EnumKnapsackStockType.Wand
                || backpackGrids[numIndexData].enumStockType == EnumKnapsackStockType.Armor
                || backpackGrids[numIndexData].enumStockType == EnumKnapsackStockType.Shoes)
            {
                JsonValue.TableEquipmentItem itemEquipment = ManagerCombat.Instance.GetEquipmentItem(backpackGrids[numIndexData].intID);
                itemTemp.itemProduct.imageValueMain.sprite = ManagerResources.Instance.GetEquipmentSprite(itemEquipment.strICON);
                itemTemp.itemProduct.textValueMain.text = itemEquipment.strNameChina;
            }
            else if (backpackGrids[numIndexData].enumStockType == EnumKnapsackStockType.Farm
                || backpackGrids[numIndexData].enumStockType == EnumKnapsackStockType.Fasture
                || backpackGrids[numIndexData].enumStockType == EnumKnapsackStockType.Factory)
            {
                JsonValue.DataTableBackPackItem itemProduct = ManagerProduct.Instance.GetProductTableItem(backpackGrids[numIndexData].intID);
                itemTemp.itemProduct.imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(itemProduct.strIconName);
                itemTemp.itemProduct.textValueMain.text = ManagerProduct.Instance.GetName(itemProduct.intProductID, false);//itemProduct.GetName(false);
            }

            SellBackpackGridPrice(numIndexData);
            itemTemp.textPrice.text = intPrices[3].ToString();
            itemTemp.textDay.text = intTimes[3] + "天";
            itemTemp.itemProduct.textValue.text = backpackGrids[numIndexData].intCount.ToString();
        }
    }

    /// <summary>
    /// 选择背包物品
    /// </summary>
    void ActionEventBackpackItem(int intIndexItem, int intIndexData)
    {
        ManagerValue.actionAudio(EnumAudio.Ground);
        SelectProduct(intIndexData);
        for (int i = 0; i < listBackpackItem.Count; i++)
        {
            if (intIndexItem == i)
            {
                listBackpackItem[i].goSelect.SetActive(true);
                continue;
            }
            listBackpackItem[i].goSelect.SetActive(false);
        }
    }

    /// <summary>
    /// 出售背包物品
    /// </summary>
    void ActionEventBackpackSell(int intIndexItem, int intIndexData)
    {
        ManagerValue.actionAudio(EnumAudio.Ground);
        intProductID = backpackGrids[intIndexData].intID;
        gridItem = backpackGrids[intIndexData].enumStockType;
        SellBackpackGridPrice(intIndexData);
        UserValue.Instance.KnapsackProductReduce(intProductID, backpackGrids[intIndexData].intCount);
        SendSellProduct();
    }

    void SellBackpackGridPrice(int intIndexData)
    {
        intPrices[3] = backpackGrids[intIndexData].intPrice;
        intCounts[3] = backpackGrids[intIndexData].intCount;
        if (intCounts[3] * intPrices[3] > 10)
        {
            intPrices[3] = (int)(intPrices[3] * intCounts[3] * 0.6f);
        }
        else
        {
            intPrices[3] = intCounts[3];
        }
        intTimes[3] = 600;
        intProductCount = intCounts[3];
        intProductPrice = intPrices[3];
        intProductTime = intTimes[3];
    }
}
