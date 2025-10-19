//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class ScrollCycleColumn : MonoBehaviour
//{
//    int numDataTotal;
//    float floItemHeight;
//    float floShowHieght;
//    Vector3[] vecPoints = new Vector3[4];
//    Vector3[] vecPositionItem;
//    List<RectTransform> listRectItem = new List<RectTransform>();
//    public ScrollRect scrollRect;
//    public RectTransform scrollRectContent;
//    Action<int, int> actionData;
//    void Awake()
//    {
//        floShowHieght = scrollRectContent.sizeDelta.y;
//        //Debug.LogError(floShowHieght);
//        scrollRectContent.GetWorldCorners(vecPoints);
//        //for (int i = 0; i < vecPoints.Length; i++)
//        //{
//        //    Debug.Log(vecPoints[i]);
//        //}
//        scrollRect.onValueChanged.AddListener(OnRefresh);
//    }

//    public List<RectTransform> InitItem(Action<int, int> action, float floHeight)
//    {
//        actionData = action;

//        for (int i = 0; i < scrollRectContent.childCount; i++)
//        {
//            listRectItem.Add(scrollRectContent.GetChild(i).GetComponent<RectTransform>());
//        }
//        floItemHeight = floHeight;
//        numIndexDwon = listRectItem.Count - 1;

//        return listRectItem;
//    }
//    public void SetItemDatas(int numData)
//    {
//        for (int i = 0; i < listRectItem.Count; i++)
//        {
//            if (i >= numData)
//            {
//                listRectItem[i].gameObject.SetActive(false);
//                continue;
//            }
//            actionData(i, i);
//        }
//        Vector2 vecTemp = scrollRectContent.sizeDelta;
//        vecTemp.y = numData * floItemHeight + 20;
//        scrollRectContent.sizeDelta = vecTemp;
//    }

//    public void SetDataTotal(int numData)
//    {
//        numDataTotal = numData;
//        if (numData <= listRectItem.Count)
//        {
//            for (int i = 0; i < listRectItem.Count; i++)
//            {
//                if (i >= numData)
//                {
//                    listRectItem[i].gameObject.SetActive(false);
//                }
//                else
//                {
//                    listRectItem[i].gameObject.SetActive(true);
//                    actionData(i, i);
//                }
//            }
//        }
//        else
//        {
//            for (int i = 0; i < listRectItem.Count; i++)
//            {
//                listRectItem[i].gameObject.SetActive(true);
//                actionData(i, i);
//            }
//        }

//        Vector2 vecTemp = scrollRectContent.sizeDelta;
//        vecTemp.y = (numData + 1) * floItemHeight + 20;
//        scrollRectContent.sizeDelta = vecTemp;
//    }

//    public void SetScrollValue(float flo, System.Action actionTemp)
//    {
//        scrollRect.verticalScrollbar.value = flo;
//        StartCoroutine(RefreshWait(flo, actionTemp));
//    }
//    IEnumerator RefreshWait(float flo, Action actionTemp)
//    {
//        float floTemp = 1.0f / numDataTotal * 2;
//        int intTemp = numDataTotal * 2;
//        float floAdd = 1 - floTemp * 4;
//        for (int i = 0; i < intTemp; i++)
//        {
//            yield return 0;
//            floAdd -= floTemp;

//            if (floAdd < flo)
//            {
//                scrollRect.verticalScrollbar.value = flo;
//                actionTemp();
//                break;
//            }
//            scrollRect.verticalScrollbar.value = floAdd;
//        }

//    }

//    public float GetScrollValue()
//    {
//        return scrollRect.verticalScrollbar.value;
//    }
//    public void RemoveItemDatas(int numIndexData)
//    {
//        //需要验证
//        if (numIndexData >= numTarget && numIndexData < numTarget + listRectItem.Count)
//        {
//            for (int i = numIndexData; i < numTarget + listRectItem.Count; i++)
//            {
//                actionData(numTarget + listRectItem.Count - i, i + 1);
//            }
//        }
//    }

//    void OnRefresh(Vector2 vecPoint)
//    {
//        int numIndexData = (int)(scrollRectContent.anchoredPosition.y / floItemHeight);
//        if (numIndexData < 0) { return; }
//        if (scrollRectContent.anchoredPosition.y > scrollRectContent.sizeDelta.y - floShowHieght) { return; }
//        if (numIndexData >= numDataTotal - listRectItem.Count + 1)
//        {
//            return;
//        }
//        CheckOut(numIndexData);
//    }

//    int numCheck;

//    void CheckOut(int numIndexData)
//    {
//        if (numCheck < numIndexData)
//        {
//            for (int i = numCheck; i < numIndexData; i++)
//            {
//                ShowOrder(i + 1);
//            }
//        }
//        else if (numCheck > numIndexData)
//        {
//            for (int i = numCheck; i > numIndexData; i--)
//            {
//                ShowOrder(i - 1);
//            }
//        }
//        numCheck = numIndexData;
//    }

//    int numTarget = 0;
//    int numIndexUp = 0;
//    int numIndexDwon = 0;
//    void ShowOrder(int numIndexData)
//    {
//        int numShowTemp = numTarget;
//        if (numTarget < numIndexData)
//        {
//            numIndexDwon += 1;
//            if (numIndexDwon == listRectItem.Count)
//            {
//                numIndexDwon = 0;
//            }

//            numIndexUp += 1;
//            Vector2 vecUp = Vector2.zero;
//            if (numIndexUp == listRectItem.Count)
//            {
//                numIndexUp = 0;
//                vecUp = listRectItem[listRectItem.Count - 1].localPosition;
//                vecUp.y -= listRectItem.Count * floItemHeight;
//                listRectItem[listRectItem.Count - 1].localPosition = vecUp;
//                numShowTemp = listRectItem.Count - 1;
//            }
//            else
//            {
//                vecUp = listRectItem[numIndexUp - 1].localPosition;
//                vecUp.y -= listRectItem.Count * floItemHeight;
//                listRectItem[numIndexUp - 1].localPosition = vecUp;
//                numShowTemp = numIndexUp - 1;
//            }
//            actionData(numShowTemp, numIndexData + listRectItem.Count - 1);
//        }
//        else if (numTarget > numIndexData)
//        {
//            numIndexUp -= 1;
//            if (numIndexUp == -1)
//            {
//                numIndexUp = listRectItem.Count - 1;
//            }

//            numIndexDwon -= 1;
//            Vector2 vecDown = Vector2.zero;
//            if (numIndexDwon == -1)
//            {
//                numIndexDwon = listRectItem.Count - 1;
//                vecDown = listRectItem[0].localPosition;
//                vecDown.y += listRectItem.Count * floItemHeight;
//                listRectItem[0].localPosition = vecDown;
//                numShowTemp = 0;
//            }
//            else
//            {
//                vecDown = listRectItem[numIndexDwon + 1].localPosition;
//                vecDown.y += listRectItem.Count * floItemHeight;
//                listRectItem[numIndexDwon + 1].localPosition = vecDown;
//                numShowTemp = numIndexDwon + 1;
//            }
//            actionData(numShowTemp, numIndexData);
//        }
//        else
//        {
//            return;
//        }
//        numTarget = numIndexData;
//    }
//}
