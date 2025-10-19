using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_ProductFactorySelect : ViewBuild_Base
{
    public Text textProductCountTag;
    public Text textNeedMat;

    public Text textProductName;
    public Text textProductGetNum;
    public Text textPrice;
    public Text textProductRipeDay;
    public Image imageProduct;
    public Button btnConfirmProduct;
    public Button btnClose;

    public View_PropertiesItem[] expendItems;

    public ScrollCycleColumn columnItem;

    bool booFactoryChangeProduct;
    int intIndexFactoryProductSelect;
    int[] intCompoundResidueTimes;
    int[] intCompoundIDs;
    int[] intCompoundingIDs;
    int[] intCompoundingCoins;
    //工厂可以生产的产品列表
    List<JsonValue.DataTableCompoundItem> listCompoundData = new List<JsonValue.DataTableCompoundItem>();
    ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();
    List<ViewFactoryInfo_SubItem> listItem = new List<ViewFactoryInfo_SubItem>();

    MGViewToBuildFactory mgFactory = new MGViewToBuildFactory();

    protected override void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            gameObject.SetActive(false);
        });

        btnConfirmProduct.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            SendMGFactoryCompound();
        });
    }
    public override void Show()
    {
        base.Show();

        btnConfirmProduct.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Confirm);
        textProductCountTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Yield) + ":";
        textNeedMat.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.RequiredMaterials) + ":";
    }

    public override void BuildMessage(EventBuildToViewBase message)
    {
        EventBuildToViewFactory mgToInfoFactory = message as EventBuildToViewFactory;
        intCompoundResidueTimes = mgToInfoFactory.intCompoundResidueTimes;
        intCompoundIDs = mgToInfoFactory.intCompoundIDs;
        intCompoundingIDs = mgToInfoFactory.intCompoundingIDs;
        intCompoundingCoins = mgToInfoFactory.intCompoundingCoins;

        listCompoundData.Clear();
        for (int i = 0; i < mgToInfoFactory.intCompoundIDs.Length; i++)
        {
            JsonValue.DataTableCompoundItem itemCompound = ManagerCompound.Instance.GetValue(mgToInfoFactory.intCompoundIDs[i]);
            listCompoundData.Add(itemCompound);
        }

        UpdateFactoryListItem();

        SelectInfo(intIndexFactoryProductSelect, intIndexFactoryProductSelect);
    }

    void UpdateFactoryListItem()
    {
        listItem.Clear();
        RectTransform[] rectItems = columnItem.SetDataTotal(listCompoundData.Count);
        for (int i = 0; i < rectItems.Length; i++)
        {
            ViewFactoryInfo_SubItem itemInfoSub = rectItems[i].GetComponent<ViewFactoryInfo_SubItem>();
            itemInfoSub.numIndexItem = i;
            itemInfoSub.numIndexData = i;
            itemInfoSub.actionBase = ActionOnClickFactoryItem;
            RefreshFactoryData(itemInfoSub, i, i);
            listItem.Add(itemInfoSub);
        }
    }
    void ActionOnClickFactoryItem(int intIndexItem, int intIndexData)
    {
        ManagerValue.actionAudio(EnumAudio.Ground);
        SelectInfo(intIndexItem, intIndexData);
    }
    void RefreshFactoryData(ViewFactoryInfo_SubItem itemTemp, int numIndexItem, int numIndexData)
    {
        if (numIndexData >= listCompoundData.Count)
        {
            itemTemp.gameObject.SetActive(false);
        }
        else
        {
            itemTemp.gameObject.SetActive(true);
            itemTemp.numIndexItem = numIndexItem;
            itemTemp.numIndexData = numIndexData;

            JsonValue.DataTableBackPackItem resProduct = ManagerProduct.Instance.GetProductTableItem(listCompoundData[numIndexData].intProductID);
            itemTemp.itemProduct.textValueMain.text = ManagerProduct.Instance.GetName(resProduct.intProductID, false);
            itemTemp.itemProduct.textValue.text = listCompoundData[numIndexData].intProductCount.ToString();
            itemTemp.itemProduct.imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(resProduct.strIconName);
            for (int i = 0; i < itemTemp.itemExpend.items.Length; i++)
            {
                if (i < listCompoundData[numIndexData].intPorductIDStuff.Length)
                {
                    itemTemp.itemExpend.items[i].gameObject.SetActive(true);

                    resProduct = ManagerProduct.Instance.GetProductTableItem(listCompoundData[numIndexData].intPorductIDStuff[i]);
                    itemTemp.itemExpend.items[i].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(resProduct.strIconName);
                    itemTemp.itemExpend.items[i].textValueMain.text = listCompoundData[numIndexData].intPorductIDnum[i].ToString();

                    //判断背包物品是否充足,并改变数字的颜色
                    if (UserValue.Instance.KnapsackProductChectCount(listCompoundData[numIndexData].intPorductIDStuff[i], listCompoundData[numIndexData].intPorductIDnum[i]))
                    {
                        itemTemp.itemExpend.items[i].textValueMain.color = new Color32(25, 255, 0, 255);
                    }
                    else
                    {
                        itemTemp.itemExpend.items[i].textValueMain.color = new Color32(255, 0, 0, 255);
                    }

                    continue;
                }
                itemTemp.itemExpend.items[i].gameObject.SetActive(false);
            }

        }
    }


    void SelectInfo(int intIndexItem, int intIndexData)
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            if (i == intIndexItem)
            {
                listItem[i].goFrame.SetActive(true);
                continue;
            }
            listItem[i].goFrame.SetActive(false);
        }

        //因为发生发生过一次错误,所以做一个预防
        if (listCompoundData.Count <= intIndexData)
        {
            intIndexData = 0;
        }

        intIndexFactoryProductSelect = intIndexData;
        mgFactory.intIndexCompound = intIndexData;

        JsonValue.DataTableBackPackItem resProduct = ManagerProduct.Instance.GetProductTableItem(listCompoundData[intIndexData].intProductID);
        textProductName.text = ManagerProduct.Instance.GetName(resProduct.intProductID, false);
        textProductGetNum.text = listCompoundData[intIndexData].intProductCount.ToString();//可获得的数量
        textPrice.text = listCompoundData[intIndexData].intCoinConsume.ToString();
        string[] strProductions = null;
        EnumLanguageStatement enumStatement = EnumLanguageStatement.None;
        if (listCompoundData[intIndexData].intRipeDay > 1)
        {
            enumStatement = EnumLanguageStatement.RequiresFinish;
            strProductions = new string[] { listCompoundData[intIndexData].intRipeDay.ToString() };
        }
        else
        {
            enumStatement = EnumLanguageStatement.TakesComplete;

        }
        textProductRipeDay.text = ManagerLanguage.Instance.GetStatement(enumStatement, strProductions);//"需要" + listCompoundData[intIndexData].intRipeDay + "天完成";
        imageProduct.sprite = ManagerResources.Instance.GetBackpackSprite(resProduct.strIconName);
        for (int i = 0; i < expendItems.Length; i++)
        {
            if (i < listCompoundData[intIndexData].intProductKind)
            {
                expendItems[i].gameObject.SetActive(true);
                JsonValue.DataTableBackPackItem resTemp = ManagerProduct.Instance.GetProductTableItem(listCompoundData[intIndexData].intPorductIDStuff[i]);
                expendItems[i].textValueMain.text = ManagerProduct.Instance.GetName(resTemp.intProductID, false);
                expendItems[i].textValue.text = listCompoundData[intIndexData].intPorductIDnum[i].ToString();
                expendItems[i].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(resTemp.strIconName);

                if (UserValue.Instance.KnapsackProductChectCount(listCompoundData[intIndexData].intPorductIDStuff[i], listCompoundData[intIndexData].intPorductIDnum[i]))
                {
                    expendItems[i].textValue.color = new Color32(25, 255, 0, 255);
                }
                else
                {
                    expendItems[i].textValue.color = new Color32(255, 0, 0, 255);
                }
            }
            else
            {
                expendItems[i].gameObject.SetActive(false);
            }
        }
    }

    void SendMGFactoryCompound()
    {
        JsonValue.DataTableCompoundItem itemCompound = ManagerCompound.Instance.GetValue(intCompoundIDs[mgFactory.intIndexCompound]);

        bool boo = false;
        string strTempInfo = "";
        int intCount = 0;
        for (int i = 0; i < itemCompound.intPorductIDStuff.Length; i++)
        {
            intCount = UserValue.Instance.GetKnapsackProductCount(itemCompound.intPorductIDStuff[i]);
            if (intCount < itemCompound.intPorductIDnum[i])
            {
                boo = true;
                strTempInfo += "(" + ManagerProduct.Instance.GetName(itemCompound.intPorductIDStuff[i], false) + ")";
            }
        }
        if (UserValue.Instance.GetCoin < listCompoundData[mgFactory.intIndexCompound].intCoinConsume)
        {
            boo = true;
            strTempInfo += "(" + ManagerLanguage.Instance.GetWord(EnumLanguageWords.InsufficientCoins) + ")";
        }

        if (boo)
        {
            ManagerView.Instance.Show(EnumView.ViewHintBar);
            //"背包中,以下物品数量不足:" + strTempInfo;
            hintBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.InsufficientQOTFIITI, null);
            hintBar.strHintBar += strTempInfo;
            ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
            return;
        }

        boo = true;
        JsonValue.DataTableCompoundItem itemTemp = ManagerCompound.Instance.GetValue(intCompoundIDs[mgFactory.intIndexCompound]);
        for (int i = 0; i < intCompoundResidueTimes.Length; i++)
        {
            if (intCompoundResidueTimes[i] == -1)
            {
                boo = false;
                intCompoundingIDs[i] = intCompoundIDs[mgFactory.intIndexCompound];
                intCompoundingCoins[i] = -1;
                intCompoundResidueTimes[i] = itemTemp.intRipeDay;
                break;
            }
        }
        if (boo)
        {

            //已经达到最大合成个数
            hintBar.strHintBar = ManagerLanguage.Instance.GetWord(EnumLanguageWords.MaximumSQR);
            ManagerView.Instance.Show(EnumView.ViewHintBar);
            ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
            return;
        }

        for (int i = 0; i < itemCompound.intPorductIDStuff.Length; i++)
        {
            if (!UserValue.Instance.KnapsackProductReduce(itemCompound.intPorductIDStuff[i], itemCompound.intPorductIDnum[i]))
            {
                ManagerView.Instance.Show(EnumView.ViewHint);
                //数值出错，这里是指有bug，且物品已经扣除
                //viewHint.strHint = ManagerLanguage.Instance.GetWord(EnumLanguageWords.NumericalError);
                //ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
                return;
            }
        }
        UserValue.Instance.SetCoinReduce(listCompoundData[mgFactory.intIndexCompound].intCoinConsume);
        ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);

        UpdateFactoryListItem();
        SendToGround(mgFactory);
    }
}
