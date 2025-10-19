using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ViewKnapsack : ViewBase
{
    public Text textTitle;
    public Text textProductName;
    public Text textProductCount;
    public Text textProductMoveCount;
    public Text textProductMovePage;
    public Text textProductMoveInfo;
    public Image imageProductFrame;
    public Image imageProduct;
    public Image imageProductFrameMove;
    public Image imageProductMove;

    public Button btnClose;
    public Button btnTidy;
    public Button btnToStock;
    public Button btnProductMove;
    public Button btnKnapsack1;
    public Button btnKnapsack2;

    public View_PropertiesBase itemProductInfo;
    public ScrollCycleColumn columnItem;

    UserValue.EnumKnapsackType knapsackType = UserValue.EnumKnapsackType.Backpack_1;
    List<ViewKnapsack_SubItem> listItem = new List<ViewKnapsack_SubItem>();
    ViewKnapsack_SubItem knapsackItem;

    int intIndexItem_1;
    int intIndexItem_2;
    float floBackpack_1 = 1;
    float floBackpack_2 = 1;
    ViewHintBar.MessageHintBar barMessage = new ViewHintBar.MessageHintBar();
    protected override void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            ManagerView.Instance.Hide(EnumView.ViewKnapsack);
        });
        btnTidy.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            UserValue.Instance.KnapasckTidy(knapsackType);
            switch (knapsackType)
            {
                case UserValue.EnumKnapsackType.Backpack_1:
                    floBackpack_1 = 1;
                    intIndexItem_1 = 0;
                    break;
                case UserValue.EnumKnapsackType.Backpack_2:
                    floBackpack_2 = 1;
                    intIndexItem_2 = 0;
                    break;
            }
            RefreshKnapsackData();
        });
        btnProductMove.onClick.AddListener(() =>
        {
            int intIndex = 0;
            switch (knapsackType)
            {
                case UserValue.EnumKnapsackType.Backpack_1:
                    intIndex = intIndexItem_1;
                    break;
                case UserValue.EnumKnapsackType.Backpack_2:
                    intIndex = intIndexItem_2;
                    break;
            }
            BackpackGrid[] grids = UserValue.Instance.GetKnapsackGrids(knapsackType);
            BackpackGrid item = grids[intIndex];
            UserValue.EnumKnapsackType key = knapsackType == UserValue.EnumKnapsackType.Backpack_1 ? UserValue.EnumKnapsackType.Backpack_2 : UserValue.EnumKnapsackType.Backpack_1;
            if (UserValue.Instance.KnapsackProductAddGrid(item, key))
            {
                if (item.enumStockType == EnumKnapsackStockType.None)
                {
                    ManagerValue.actionAudio(EnumAudio.Unable);
                }
                else
                {
                    ManagerValue.actionAudio(EnumAudio.Ground);
                }
                UserValue.Instance.KnapsackProductReduce(knapsackType, intIndex, item.intCount);
                RefreshKnapsackData();
            }
            else
            {
                ManagerValue.actionAudio(EnumAudio.Unable);
                barMessage.strHintBar = "背包空间不足,请整理!";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, barMessage);
            }

        });
        btnToStock.onClick.AddListener(() =>
        {
            //if (intPageMove != -1)
            //{

            //}
        });
        btnToStock.gameObject.SetActive(false);

        btnKnapsack1.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            if (knapsackType != UserValue.EnumKnapsackType.Backpack_1)
            {
                knapsackType = UserValue.EnumKnapsackType.Backpack_1;
                ProductFrameColor(UserValue.EnumColorType.Gray);
                columnItem.scrollRect.verticalScrollbar.value = floBackpack_1;
                RefreshKnapsackData();
                ProductItem(intIndexItem_1);
            }
        });

        btnKnapsack2.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            if (knapsackType != UserValue.EnumKnapsackType.Backpack_2)
            {
                knapsackType = UserValue.EnumKnapsackType.Backpack_2;
                ProductFrameColor(UserValue.EnumColorType.Black);
                columnItem.scrollRect.verticalScrollbar.value = floBackpack_2;
                RefreshKnapsackData();
                ProductItem(intIndexItem_2);
            }
        });

        columnItem.scrollRect.onValueChanged.AddListener((value) =>
        {
            switch (knapsackType)
            {
                case UserValue.EnumKnapsackType.Backpack_1:
                    floBackpack_1 = columnItem.scrollRect.verticalScrollbar.value;
                    break;
                case UserValue.EnumKnapsackType.Backpack_2:
                    floBackpack_2 = columnItem.scrollRect.verticalScrollbar.value;
                    break;
            }
        });
    }

    public override void Show()
    {
        base.Show();

        ManagerValue.booMoveCamera = false;

        RefreshKnapsackData();
        switch (knapsackType)
        {
            case UserValue.EnumKnapsackType.Backpack_1:
                ProductFrameColor(UserValue.EnumColorType.Gray);
                break;
            case UserValue.EnumKnapsackType.Backpack_2:
                ProductFrameColor(UserValue.EnumColorType.Black);
                break;
        }
        textTitle.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Backpack);
        btnTidy.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Organize);
        btnKnapsack1.transform.GetChild(0).gameObject.SetActive(false);
        btnKnapsack2.transform.GetChild(0).gameObject.SetActive(false);
        btnKnapsack1.transform.GetChild(1).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Backpack) + " 1";
        btnKnapsack2.transform.GetChild(1).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Backpack) + " 2";
        btnProductMove.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Move);
    }
    public override void SetData(Message message)
    {
        RefreshKnapsackData();
    }
    public override void Hide()
    {
        base.Hide();

        ManagerValue.booMoveCamera = true;
    }

    void RefreshKnapsackData()
    {
        BackpackGrid[] items = UserValue.Instance.GetKnapsackGrids(knapsackType);
        if (knapsackItem == null)
        {
            knapsackItem = columnItem.scrollRectContent.transform.GetChild(0).GetComponent<ViewKnapsack_SubItem>();
        }

        if (listItem.Count == 0)
        {
            RectTransform[] rectItems = columnItem.SetDataTotal(items.Length / knapsackItem.items.Length);
            for (int i = 0; i < rectItems.Length; i++)
            {
                ViewKnapsack_SubItem itemKnapsack = rectItems[i].GetComponent<ViewKnapsack_SubItem>();
                for (int j = 0; j < itemKnapsack.items.Length; j++)
                {
                    itemKnapsack.numIndexItem = i;
                    itemKnapsack.numIndexData = j;
                    itemKnapsack.actionData = ActionShowProduct;
                }
                listItem.Add(itemKnapsack);
            }
        }
        for (int i = 0; i < listItem.Count; i++)
        {
            for (int j = 0; j < listItem[i].items.Length; j++)
            {
                RefreshData(listItem[i].items[j], items[i * knapsackItem.items.Length + j]);
            }

        }
        switch (knapsackType)
        {
            case UserValue.EnumKnapsackType.Backpack_1:
                ProductItem(intIndexItem_1);
                break;
            case UserValue.EnumKnapsackType.Backpack_2:
                ProductItem(intIndexItem_2);
                break;
        }
    }
    void ProductItem(int intIndexData)
    {
        int intTemp = 0;
        int intCount = listItem[0].items[0].transform.childCount - 1;
        for (int i = 0; i < listItem.Count; i++)
        {
            for (int j = 0; j < listItem[i].items.Length; j++)
            {
                if (intTemp == intIndexData)
                {
                    listItem[i].items[j].transform.GetChild(intCount).gameObject.SetActive(true);
                }
                else
                {
                    listItem[i].items[j].transform.GetChild(intCount).gameObject.SetActive(false);
                }
                intTemp++;
            }
        }

        if (intIndexData == -1)
        {
            textProductName.text = "";
            textProductCount.text = "";
            imageProductFrame.sprite = ManagerResources.Instance.GetFrameRank("1");
            imageProduct.sprite = null;
            imageProduct.gameObject.SetActive(false);

            textProductMoveCount.text = "";
            textProductMovePage.text = "";
            textProductMoveInfo.text = "";
            imageProductFrameMove.sprite = ManagerResources.Instance.GetFrameRank("1");
            imageProductMove.gameObject.SetActive(false);

            for (int i = 0; i < itemProductInfo.items.Length; i++)
            {
                itemProductInfo.items[i].gameObject.SetActive(false);
            }
            return;
        }

        BackpackGrid item = UserValue.Instance.GetKnapsackGrids(knapsackType)[intIndexData];
        if (item.enumStockType == EnumKnapsackStockType.None)
        {
            textProductName.text = "";
            textProductCount.text = "";
            imageProductFrame.sprite = ManagerResources.Instance.GetFrameRank("1");
            imageProduct.sprite = null;
            imageProduct.gameObject.SetActive(false);

            textProductMoveCount.text = "";
            textProductMovePage.text = "";
            textProductMoveInfo.text = "";
            imageProductFrameMove.sprite = ManagerResources.Instance.GetFrameRank("1");
            imageProductMove.gameObject.SetActive(false);

            for (int i = 0; i < itemProductInfo.items.Length; i++)
            {
                itemProductInfo.items[i].gameObject.SetActive(false);
            }
            return;
        }
        textProductMoveCount.text = item.intCount.ToString();
        textProductMovePage.text = (intIndexData + 1).ToString();
        textProductMoveInfo.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.MoveTo) + " " + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Backpack) + " " + ((int)(knapsackType == UserValue.EnumKnapsackType.Backpack_1 ? UserValue.EnumKnapsackType.Backpack_2 : UserValue.EnumKnapsackType.Backpack_1)).ToString();
        imageProductFrameMove.sprite = ManagerResources.Instance.GetFrameRank(item.intRank.ToString());
        imageProductMove.gameObject.SetActive(true);

        imageProduct.gameObject.SetActive(true);
        textProductCount.text = item.intCount.ToString();
        imageProductFrame.sprite = ManagerResources.Instance.GetFrameRank(item.intRank.ToString());

        if (item.enumStockType == EnumKnapsackStockType.Sword
            || item.enumStockType == EnumKnapsackStockType.Bow
            || item.enumStockType == EnumKnapsackStockType.Wand
            || item.enumStockType == EnumKnapsackStockType.Armor
            || item.enumStockType == EnumKnapsackStockType.Shoes)
        {
            textProductName.text = ManagerCombat.Instance.GetEquipmentItem(item.intID).strNameChina;
            imageProductMove.sprite = ManagerResources.Instance.GetEquipmentSprite(item.icon);
            imageProduct.sprite = ManagerResources.Instance.GetEquipmentSprite(item.icon);
        }
        else if (item.enumStockType == EnumKnapsackStockType.Farm
            || item.enumStockType == EnumKnapsackStockType.Fasture
            || item.enumStockType == EnumKnapsackStockType.Factory)
        {
            textProductName.text = ManagerProduct.Instance.GetName(item.intID, false);
            imageProductMove.sprite = ManagerResources.Instance.GetBackpackSprite(item.icon);
            imageProduct.sprite = ManagerResources.Instance.GetBackpackSprite(item.icon);
        }


        for (int i = 0; i < itemProductInfo.items.Length; i++)
        {
            itemProductInfo.items[i].gameObject.SetActive(false);
        }
    }

    void ActionShowProduct(int numIndex)
    {
        ManagerValue.actionAudio(EnumAudio.Ground);
        switch (knapsackType)
        {
            case UserValue.EnumKnapsackType.Backpack_1:
                intIndexItem_1 = numIndex;
                break;
            case UserValue.EnumKnapsackType.Backpack_2:
                intIndexItem_2 = numIndex;
                break;
        }

        ProductItem(numIndex);
    }
    void ProductFrameColor(UserValue.EnumColorType key)
    {
        Color32 color = UserValue.Instance.GetImageColor(key);
        for (int i = 0; i < listItem.Count; i++)
        {
            for (int j = 0; j < listItem[i].items.Length; j++)
            {
                listItem[i].items[j].transform.GetChild(0).GetComponent<Image>().color = color;
            }
        }
    }

    void RefreshData(View_PropertiesItem itemTemp, BackpackGrid itemGrid)
    {
        if (itemGrid.enumStockType != EnumKnapsackStockType.None)
        {
            itemTemp.textValueMain.gameObject.SetActive(true);
            itemTemp.imageValueMain.gameObject.SetActive(true);
            itemTemp.textValueMain.text = itemGrid.intCount.ToString();

            if (itemGrid.enumStockType == EnumKnapsackStockType.Sword
                || itemGrid.enumStockType == EnumKnapsackStockType.Bow
                || itemGrid.enumStockType == EnumKnapsackStockType.Wand
                || itemGrid.enumStockType == EnumKnapsackStockType.Armor
                || itemGrid.enumStockType == EnumKnapsackStockType.Shoes)
            {
                itemTemp.imageValueMain.sprite = ManagerResources.Instance.GetEquipmentSprite(itemGrid.icon);
            }
            else if (itemGrid.enumStockType == EnumKnapsackStockType.Farm
                || itemGrid.enumStockType == EnumKnapsackStockType.Fasture
                || itemGrid.enumStockType == EnumKnapsackStockType.Factory)
            {
                itemTemp.imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(itemGrid.icon);
            }

            itemTemp.imageValue.sprite = ManagerResources.Instance.GetFrameRank(itemGrid.intRank.ToString());
        }
        else
        {
            itemTemp.textValueMain.gameObject.SetActive(false);
            itemTemp.imageValueMain.gameObject.SetActive(false);
            itemTemp.imageValue.sprite = ManagerResources.Instance.GetFrameRank("1");
        }
        itemTemp.textValue.text = (itemGrid.intIndex + 1).ToString();
    }

    private void OnDestroy()
    {

    }
}
