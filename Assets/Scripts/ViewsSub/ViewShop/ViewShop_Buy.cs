using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewShop_Buy : MonoBehaviour
{
    public Button btnArrowLeft;
    public Button btnArrowRight;
    public Button btnBuy;

    public Text textResidueBuyCount;
    public Text textPrice;
    public Text textPage;

    public View_PropertiesItem itemProductPerperties;
    public Transform transGridParent;

    int intPageNow = 1;
    int intPageTotal = 0;
    List<ViewShop_BuyItem> listBuyItem = new List<ViewShop_BuyItem>();

    [System.NonSerialized]
    public int intIndexBuy;
    [System.NonSerialized]
    public int[] intProductBuyIDs;//购买列表
    [System.NonSerialized]
    public int[] intProductBuyNums;
    [System.NonSerialized]
    public int[] intProductBuyPrices;
    [System.NonSerialized]
    public bool[] booProductBuys;//是否已经买过

    public System.Action<int> actionSendBuyItem;

    private void Start()
    {
        btnArrowLeft.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            if (intPageNow > 1)
            {
                intPageNow--;
                ShowProductBuy(intPageNow);
                intIndexBuy = 0;
                SelectItem(intIndexBuy % listBuyItem.Count);
            }
            textPage.text = intPageNow + "/" + intPageTotal;
        });
        btnArrowRight.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            if (intPageNow < intPageTotal)
            {
                intPageNow++;
                intIndexBuy = 0;
                SelectItem(intIndexBuy % listBuyItem.Count);
            }
            textPage.text = intPageNow + "/" + intPageTotal;
        });
    }

    public void Show()
    {

        if (listBuyItem.Count == 0)
        {
            for (int i = 0; i < transGridParent.childCount; i++)
            {
                listBuyItem.Add(transGridParent.GetChild(i).GetComponent<ViewShop_BuyItem>());
                listBuyItem[i].btnBuy.onClick.AddListener(OnClickBuyProduct(i));
                listBuyItem[i].GetComponent<Button>().onClick.AddListener(OnClickSelectProduct(i));
            }
        }
    }

    public void ShowPage()
    {

        intPageTotal = intProductBuyIDs.Length / 10;
        if (intPageTotal < intPageNow)
        {
            intPageNow = 1;
        }
        textPage.text = intPageNow + "/" + intPageTotal;
        ShowProductBuy(intPageNow);
        if (intIndexBuy != -1)
        {
            BuyItem(intIndexBuy);
            SelectItem(intIndexBuy % listBuyItem.Count);
        }
        else
        {
            SelectItem(0);
            BuyItem(0);
        }
    }

    /// <summary>
    /// 购买选择
    /// </summary>
    UnityEngine.Events.UnityAction OnClickSelectProduct(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);

            int intPage = (intPageNow - 1) * 10;
            intPage += intIndex;
            BuyItem(intPage);
            SelectItem(intIndex);
        };
    }

    /// <summary>
    /// 购买
    /// </summary>
    UnityEngine.Events.UnityAction OnClickBuyProduct(int intIndex)
    {
        return delegate
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            int intPage = (intPageNow - 1) * 10;
            intPage += intIndex;
            BuyItem(intPage);
            actionSendBuyItem(intPage);
        };
    }

    void ShowProductBuy(int intPage)
    {
        intPage = (intPage - 1) * 10;
        int intIndex = 0;
        for (int i = intPage; i < intPage + listBuyItem.Count; i++)
        {
            if (i < intProductBuyIDs.Length)
            {
                listBuyItem[intIndex].gameObject.SetActive(true);
                JsonValue.DataTableBackPackItem itemProduct = ManagerProduct.Instance.GetProductTableItem(intProductBuyIDs[i]);
                listBuyItem[intIndex].itemProductProperties.imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(itemProduct.strIconName);
                listBuyItem[intIndex].itemProductProperties.textValueMain.text = ManagerProduct.Instance.GetName(itemProduct.intProductID, false);//itemProduct.GetName(false);
                listBuyItem[intIndex].itemProductProperties.textValue.text = intProductBuyNums[i].ToString();
                if (booProductBuys[i])
                {
                    listBuyItem[intIndex].textBuyHint.gameObject.SetActive(true);
                }
                else
                {
                    listBuyItem[intIndex].textBuyHint.gameObject.SetActive(false);
                }
            }
            else
            {
                listBuyItem[intIndex].gameObject.SetActive(false);
            }
            intIndex++;
        }
    }

    void SelectItem(int intIndex)
    {
        for (int i = 0; i < listBuyItem.Count; i++)
        {
            if (i == intIndex)
            {
                listBuyItem[i].goSelect.SetActive(true);
                continue;
            }
            listBuyItem[i].goSelect.SetActive(false);
        }
    }

    /// <summary>
    /// 显示详情
    /// </summary>
    void BuyItem(int intIndex)
    {
        intIndexBuy = intIndex;
        btnBuy.gameObject.SetActive(!booProductBuys[intIndexBuy]);
        JsonValue.DataTableBackPackItem itemProduct = ManagerProduct.Instance.GetProductTableItem(intProductBuyIDs[intIndexBuy]);
        itemProductPerperties.textValueMain.text = ManagerProduct.Instance.GetName(itemProduct.intProductID, false);//itemProduct.GetName(false);
        itemProductPerperties.imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(itemProduct.strIconName);
        itemProductPerperties.textValue.text = intProductBuyNums[intIndexBuy].ToString();
        textPrice.text = intProductBuyPrices[intIndexBuy].ToString();
    }
}