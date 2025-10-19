using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewProductionSee : ViewBase
{
    public Button btnClose;
    public ScrollCycleColumn columnItem;

    List<ViewProductionSee_MenuItem> listItem = new List<ViewProductionSee_MenuItem>();
    List<int> listBuildID = new List<int>();
    List<List<int>> listBuildGround = new List<List<int>>();
    protected override void Start()
    {
        btnClose.onClick.AddListener(() => { ManagerView.Instance.Hide(EnumView.ViewProductionSee); });

        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Date, MessageBuildTypeUpdateDate);
        //ManagerMessage.Instance.AddEvent(EnumMessage.Update_Date, MessageBuildInfoDate);
    }

    public override void Show()
    {
        base.Show();

        listBuildID.Clear();
        Dictionary<int, List<int>> itemTemp = UserValue.Instance.GetBuildProductSeeAll();
        int intIndexTemp = 0;
        foreach (KeyValuePair<int, List<int>> temp in itemTemp)
        {
            listBuildID.Add(temp.Key);
            if (intIndexTemp >= listBuildGround.Count)
            {
                listBuildGround.Add(null);
            }
            listBuildGround[intIndexTemp] = temp.Value;
            intIndexTemp++;

        }

        listItem.Clear();
        RectTransform[] rectItems = columnItem.SetDataTotal(listBuildID.Count);
        for (int i = 0; i < rectItems.Length; i++)
        {
            ViewProductionSee_MenuItem itemMenu = rectItems[i].GetComponent<ViewProductionSee_MenuItem>();
            itemMenu.numIndexItem = i;
            itemMenu.numIndexData = i;
            itemMenu.actionProductSee_1 = EventProductSee_1;
            itemMenu.actionProductSee_2 = EventProductSee_2;
            itemMenu.actionProductSee_3 = EventProductSee_3;
            itemMenu.actionProductSee_4 = EventProductSee_4;

            itemMenu.actionPageLeft = EventProductLeft;
            itemMenu.actionPageRight = EventProductRight;
            RefreshData(itemMenu, i, i);
            listItem.Add(itemMenu);
        }

        DateRefreshItemShow();
    }

    void RefreshData(ViewProductionSee_MenuItem itemTemp,int numIndexItem, int numIndexData)
    {
        if (numIndexData >= listBuildID.Count)
        {
            itemTemp.gameObject.SetActive(false);
        }
        else
        {
            itemTemp.gameObject.SetActive(true);
            itemTemp.numIndexItem = numIndexItem;
            itemTemp.numIndexData = numIndexData;

            JsonValue.DataTableBuildingItem item = ManagerBuild.Instance.GetBuildItem(listBuildID[numIndexData]);
            string strTemp = listBuildGround[numIndexData].Count.ToString();
            strTemp = "  " + UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, strTemp);
            itemTemp.textBuildName.text = ManagerBuild.Instance.GetBuildName(listBuildID[numIndexData]) + strTemp;
        }
    }

    void EventProductSee_1(int intIndexItem, int intIndexData)
    {

    }
    void EventProductSee_2(int intIndexItem, int intIndexData)
    {

    }
    void EventProductSee_3(int intIndexItem, int intIndexData)
    {

    }
    void EventProductSee_4(int intIndexItem, int intIndexData)
    {

    }
    void EventProductLeft(int intIndexItem, int intIndexData)
    { }

    void EventProductRight(int intIndexItem, int intIndexData)
    { }

    /// <summary>
    /// 这个是更新当前建筑类型
    /// </summary>
    void MessageBuildTypeUpdateDate(ManagerMessage.MessageBase message)
    {
        MessageDate date = message as MessageDate;
        if (date != null && gameObject.activeSelf)
        {
            DateRefreshItemShow();
        }
    }
    void DateRefreshItemShow()
    {
        string strOutput = "";
        string strExpend = "";
        string strStock = "";
        float floOutput = 0;
        float floExpend = 0;
        float floStock = 0;
        Image imageTemp = null;
        List<int> listTemp = new List<int>();
        for (int i = 0; i < listItem.Count; i++)
        {
            listTemp.Clear();
            if (i >= listBuildGround.Count)
            {
                break;
            }
            //获取所有同类型建筑的 产品生产种类
            List<int> ground = listBuildGround[listItem[i].numIndexData];
            for (int j = 0; j < ground.Count; j++)
            {
                BuildBase build = UserValue.Instance.GetBuildValue(ground[j]);
                if (!listTemp.Contains(build.intFarmProductID))
                {
                    listTemp.Add(build.intFarmProductID);
                }
            }
            for (int j = 0; j < listItem[i].item.items.Length; j++)
            {
                if (j < listTemp.Count)
                {
                    //FarmClass.StockItem stock = UserValue.Instance.GetStock()[listTemp[j]];
                    //float floDay = 0;
                    //float floCount = 0;
                    //floExpend = 0;
                    //foreach (FarmClass.StockProduct temp in stock.dicProduct.Values)
                    //{
                    //    floDay += temp.intDay;
                    //    floCount += temp.intProductCount;
                    //    floExpend += temp.intDayExpend;
                    //}
                    //if (floDay != 0)
                    //{
                    //    floOutput = floCount / floDay;
                    //}
                    //else
                    //{
                    //    floOutput = 0;
                    //}
                    strOutput = floOutput.ToString("f4");
                    //floStock = stock.intStockCount;
                    strOutput = "产率:  " + strOutput;
                    listItem[i].item.items[j].textValueMain.text = strOutput;
                    imageTemp = listItem[i].item.items[j].GetComponent<Image>();
                    listItem[i].item.items[j].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(ManagerProduct.Instance.GetProductTableItem(listTemp[j]).strIconName);
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
                    else if (floOutput > floExpend && floStock > 0)
                    {
                        imageTemp.color = UserValue.Instance.GetImageColor(UserValue.EnumColorType.Green);
                    }
                    listItem[i].item.items[j].gameObject.SetActive(true);
                }
                else
                {
                    listItem[i].item.items[j].gameObject.SetActive(false);
                }
            }

        }
    }
    /// <summary>
    /// 更新具体建筑 更新的是每个建筑在生产的指定 intProductID 的数值
    /// </summary>
    void MessageBuildInfoDate(ManagerMessage.MessageBase message)
    {

    }

    private void OnDestroy()
    {
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Update_Date, MessageBuildTypeUpdateDate);
    }
}
