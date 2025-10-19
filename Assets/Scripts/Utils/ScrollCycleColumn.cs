using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollCycleColumn : MonoBehaviour
{
    Vector3[] vecPoints = new Vector3[4];
    List<GameObject> listRectItem = new List<GameObject>();
    public ScrollRect scrollRect;
    public RectTransform scrollRectContent;
    Action<int, int> actionData;
    void Awake()
    {
        scrollRectContent.GetChild(0).gameObject.SetActive(false);
        //scrollRectContent.GetWorldCorners(vecPoints);//获得UI锚点坐标值
    }

    //public List<RectTransform> InitItem(Action<int, int> action, float floHeight)
    //{
    //    actionData = action;

    //    for (int i = 0; i < scrollRectContent.childCount; i++)
    //    {
    //        listRectItem.Add(scrollRectContent.GetChild(i).GetComponent<RectTransform>());
    //    }
    //    floItemHeight = floHeight;
    //    //numIndexDwon = listRectItem.Count - 1;

    //    return listRectItem;
    //}

    public RectTransform[] SetDataTotal(int numData)
    {
        for (int i = 0; i < listRectItem.Count; i++)
        {
            Destroy(listRectItem[i].gameObject);
        }
        listRectItem.Clear();
        List<RectTransform> listTemp = new List<RectTransform>();
        GridLayoutGroup temp = scrollRectContent.GetComponent<GridLayoutGroup>();
        float floItemHeight = temp.cellSize.y + temp.spacing.y;
        for (int i = 0; i < numData; i++)
        {
            Transform transTemp = scrollRectContent.GetChild(0);
            transTemp.gameObject.SetActive(false);
            GameObject goTemp = Instantiate(transTemp.gameObject, transTemp.parent, false);
            listRectItem.Add(goTemp);

            listTemp.Add(listRectItem[i].GetComponent<RectTransform>());
            listTemp[i].gameObject.SetActive(true);
        }

        Vector2 vecTemp = scrollRectContent.sizeDelta;
        vecTemp.y = (numData + 1) * floItemHeight + 20;
        scrollRectContent.sizeDelta = vecTemp;

        return listTemp.ToArray();
    }
}
