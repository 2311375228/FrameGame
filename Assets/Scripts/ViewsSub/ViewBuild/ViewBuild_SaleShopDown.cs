using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_SaleShopDown : ViewBuild_Base
{
    public Text textEarningsTag;
    public Text textEarnings;
    public Text textInfo;

    public RectTransform rectSellRoot;

    List<View_PropertiesItem> listItem = new List<View_PropertiesItem>();
    SaleShopSellStateBoBuild messageSellState = new SaleShopSellStateBoBuild();
    int[] intSellStates;
    string[] strStatementSell = new string[2];
    string[] strStatementEarnGCA = new string[1];
    string[] strStatementTheQOII = new string[1];

    public override void Show()
    {
        base.Show();

        if (listItem.Count == 0)
        {
            for (int i = 0; i < rectSellRoot.childCount; i++)
            {
                listItem.Add(rectSellRoot.GetChild(i).GetComponent<View_PropertiesItem>());
                listItem[i].imageValue.transform.parent.GetComponent<Button>().onClick.AddListener(OnClickSell(i));
            }
        }

        textEarningsTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.AnnualRevenue);
    }

    public override void BuildMessage(EventBuildToViewBase message)
    {
        SaleShopToView messageSaleShop = message as SaleShopToView;
        if (messageSaleShop != null)
        {
            intSellStates = messageSaleShop.intSellState;

            int intProductTotal = 0;
            int intProductTotalPrice = 0;
            for (int i = 0; i < listItem.Count; i++)
            {
                if (i < messageSaleShop.intSellProdects.Length)
                {
                    intProductTotal += messageSaleShop.intsellProductCounts[i];
                    intProductTotalPrice += messageSaleShop.intSellPrices[i];

                    listItem[i].textValue.text = messageSaleShop.intsellProductCounts[i].ToString();
                    listItem[i].textValueMain.text = messageSaleShop.intSellPrices[i].ToString();
                    listItem[i].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(ManagerProduct.Instance.GetProductTableItem(messageSaleShop.intSellProdects[i]).strIconName);
                    listItem[i].imageValue.gameObject.SetActive(messageSaleShop.intSellState[i] == 0 ? false : true);
                    listItem[i].gameObject.SetActive(true);
                    continue;
                }
                listItem[i].gameObject.SetActive(false);
            }
            textEarnings.text = messageSaleShop.intCoinAnnualRevenue.ToString("N0");

            strStatementSell[0] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, intProductTotal.ToString());
            strStatementSell[1] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, intProductTotalPrice.ToString());
            textInfo.text = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.SellIPDTEGC, strStatementSell);
            strStatementEarnGCA[0] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, (365 * intProductTotalPrice).ToString("N0"));
            textInfo.text += " " + ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.EarnGCA, strStatementEarnGCA);
            if (false)
            {
                textInfo.text += ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.TheQOII, null);
            }
        }
    }

    /// <summary>
    /// 是否出售按钮
    /// </summary>
    UnityEngine.Events.UnityAction OnClickSell(int intIndex)
    {
        return () =>
        {
            intSellStates[intIndex] = intSellStates[intIndex] == 0 ? 1 : 0;
            SendToGround(messageSellState);
        };
    }
}
