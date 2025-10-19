//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class ScrollCycleColumnChat : MonoBehaviour
//{
//    public ScrollRect scrollRect;
//    public RectTransform scrollRectContent;

//    float floItemHeight;
//    float floShowHeight;
//    List<ItemChat> listChat = new List<ItemChat>();
//    List<ItemRect> listItem = new List<ItemRect>();
//    public delegate int SetRectData(int numIndexItem, int numIndexData);
//    SetRectData setRectData;

//    private void Awake()
//    {
//        floShowHeight = scrollRectContent.sizeDelta.y;
//        scrollRect.onValueChanged.AddListener(OnRefresh);
//    }

//    public List<RectTransform> InitItem(SetRectData even)
//    {
//        List<RectTransform> listTemp = new List<RectTransform>();
//        setRectData = even;
//        for (int i = 0; i < scrollRectContent.childCount; i++)
//        {
//            ItemRect item = new ItemRect();
//            item.numIndex = i;
//            item.rectItem = scrollRectContent.GetChild(i).GetComponent<RectTransform>();
//            listItem.Add(item);
//            listItem[i].rectItem.gameObject.SetActive(false);
//            listTemp.Add(item.rectItem);
//        }
//        GridLayoutGroup grid = scrollRectContent.GetComponent<GridLayoutGroup>();
//        floItemHeight = grid.cellSize.y;// + grid.spacing.y;
//        return listTemp;
//    }

//    int numIndexItem = 0;
//    Vector2 vecItem = Vector2.zero;
//    public void AddItemData(bool booWho, string strInfo)
//    {
//        //�Զ��ϻ�����������ڵ�ҳ�����һ������������û���
//        //��� �б仯�¼�����
//        //�ı��ܸ߶�
//        //��ȡ��ǰ�߶ȼ���
//        ItemChat itemChat = new ItemChat();
//        int numColumn = -1;
//        if (numIndexItem < listItem.Count)
//        {
//            vecItem.x = listItem[numIndexItem].rectItem.localPosition.x;
//            listItem[numIndexItem].rectItem.localPosition = vecItem;

//            listItem[numIndexItem].rectItem.gameObject.SetActive(true);
//            numColumn = setRectData(numIndexItem, numIndexItem);

//            numColumnTotal += numColumn;
//            Vector2 vecPosition = listItem[numIndexItem].rectItem.localPosition;
//            vecPosition.y = -numColumnTotal * floItemHeight;
//            vecItem = vecPosition;
//            //listItem[numIndexItem].rectItem.localPosition = vecPosition;
//            Debug.Log("������" + numColumnTotal + "�߶ȣ�" + numColumn + ":" + vecPosition.y + "=" + floItemHeight);

//            Vector2 vecTemp = scrollRectContent.sizeDelta;
//            vecTemp.y = (numColumnTotal + 1) * floItemHeight + 20;
//            scrollRectContent.sizeDelta = vecTemp;
//            numIndexItem++;
//        }
//        else
//        {
//            if (numIndexItem == listItem.Count)
//            {

//            }
//        }
//        itemChat.numColumn = numColumn;
//        //���㵱ǰ�ܸ߶ȣ��߶�ֵ��仯���仯
//        listChat.Add(itemChat);
//    }


//    void OnRefresh(Vector2 vecPoint)
//    {
//        int numIndexData = (int)(scrollRectContent.anchoredPosition.y / floItemHeight);
//        if (numIndexData < 0) { return; }
//        if (scrollRectContent.anchoredPosition.y > scrollRectContent.sizeDelta.y - floShowHeight) { return; }
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
//    int numColumnTotal = 0;
//    int numDifferentUp = 0;
//    int numIndexUp = 0;
//    //int numIndexDwon = 0;
//    int numIndexChatUp = 0;
//    void ShowOrder(int numIndexData)
//    {
//        int numShowTemp = numTarget;
//        if (numTarget < numIndexData)
//        {
//            numDifferentUp--;
//            if (numDifferentUp == 0)
//            {
//                numDifferentUp = listChat[numIndexChatUp].numColumn;

//                numIndexUp++;
//                numIndexChatUp++;
//                Vector2 vecUp = Vector2.zero;
//                if (numIndexUp == listItem.Count)
//                {
//                    numIndexUp = 0;
//                    vecUp = listItem[listItem.Count - 1].rectItem.localPosition;
//                    vecUp.y -= numColumnTotal * floItemHeight;
//                    listItem[listItem.Count - 1].rectItem.localPosition = vecUp;
//                    numDifferentUp = listChat[numIndexChatUp].numColumn;

//                }
//                else
//                {
//                    vecUp = listItem[numIndexUp - 1].rectItem.localPosition;
//                    vecUp.y -= numColumnTotal * floItemHeight;
//                    listItem[numIndexUp - 1].rectItem.localPosition = vecUp;
//                }

//                setRectData(numShowTemp, numIndexData + listItem.Count - 1);
//            }
//        }
//        else if (numTarget > numIndexData)
//        {

//            setRectData(numShowTemp, numIndexData);
//        }
//        else
//        {
//            return;
//        }
//        numTarget = numIndexData;
//    }

//    class ItemRect
//    {
//        public int numIndex;
//        public int numIndexChat;
//        public RectTransform rectItem;
//    }
//    class ItemChat
//    {
//        public int numIndex;
//        public int numColumn;
//    }
//}
