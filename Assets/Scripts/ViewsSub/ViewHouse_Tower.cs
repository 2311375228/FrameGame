using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewHouse_Tower : MonoBehaviour
{
    public Text textContent;
    public Button btnClose;

    public ScrollCycleColumn columnItem;

    [System.NonSerialized]
    public List<ViewHouse_TowerItem> listItem = new List<ViewHouse_TowerItem>();

    [System.NonSerialized]
    public int intHouseRank;

    public void Show()
    {
        //城堡

        RectTransform[] rectItems = columnItem.SetDataTotal(0);
        for (int i = 0; i < rectItems.Length; i++)
        {
            ViewHouse_TowerItem itemTower = rectItems[i].GetComponent<ViewHouse_TowerItem>();
            itemTower.numIndexItem = i;
            itemTower.numIndexData = i;
            itemTower.actionBase = ActionEventItemTower;
            RefreshDataTower(itemTower, i, i);
        }
    }

    void RefreshDataTower(ViewHouse_TowerItem itemTemp,int intIndexItem, int intIndexData)
    {
        //if (intIndexData >= employeeShow.listData.Count)
        //{
        //    itemTemp.gameObject.SetActive(false);
        //}
        //else
        //{
        //    itemTemp.gameObject.SetActive(true);
        //    itemTemp.numIndexItem = intIndexItem;
        //    itemTemp.numIndexData = intIndexData;

        //    PropertiesEmployee employee = employeeShow.listData[intIndexData];
        //    EmoloyeeShowItem(itemTemp, employee);
        //}
    }
    void ActionEventItemTower(int intIndexItem, int intIndexData)
    {

    }
}
