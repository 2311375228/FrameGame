using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewTask : ViewBase
{
    public Text textTitle;
    public Text textTaskName;
    public Text textTaskInfo;
    public Text textTaskCion;
    public Text textPenaltyCoinTag;
    public Text textPenaltyCoin;

    public Text textTaskTarget;
    public Text textAward;

    public Button btnClose;
    public Button btnGetAward;
    public ScrollRect scrollRect;
    public RectTransform rectScroll;
    public GameObject goCopyItemRoot;
    public GameObject goCopyItem;
    public View_PropertiesBase itemTaskGoods;
    public View_PropertiesBase itemAward;

    //获取当前任务最深层,每次改变最深层的时候,更新任务和副本显示
    //任务状态
    //未开启,任务还未开启,隐藏
    //等待完成,显示,任务领取到了还没有完成,等待完成
    //等待交付,显示,任务领取到并完成了,但是还没有点击交付,
    //已完成,隐藏,任务完成了,并且点击了交付按钮

    //限时任务

    int intIndexItem;
    List<PropertiesTask> listTask;
    List<RectTransform> listItemScrollRoot = new List<RectTransform>();
    List<RectTransform> listItemScroll = new List<RectTransform>();
    List<ViewTask_SubItem> listItem = new List<ViewTask_SubItem>();
    Vector2 vecScorllDis;
    bool booScorllMouse;
    protected override void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            ManagerView.Instance.Hide(EnumView.ViewTask);
        });
        btnGetAward.onClick.AddListener(() =>
        {
            FinishTask(intIndexItem);
        });


        scrollRect.onValueChanged.AddListener((value) =>
        {
            if (booScorllMouse)
            {
                for (int i = 0; i < listItemScroll.Count; i++)
                {
                    listItem[i].GetComponent<RectTransform>().position = listItem[i].rectItemRoot.position;
                }
            }
        });

        goCopyItem.SetActive(false);
        goCopyItemRoot.SetActive(false);

        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Task, MessageTask);
    }

    public override void Show()
    {
        base.Show();
        UpdateTaskData();
        StartCoroutine(Wait());

        if (UserValue.Instance.listTask.Count > 0)
        {
            ItemTaskInfo(0);
        }
        else
        {
            ItemTaskInfo(-1);
        }

        textTitle.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Tasks);
        textTaskTarget.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.PleaseCTFT);
        textAward.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Rewards);
        textPenaltyCoinTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Penalty);
        btnGetAward.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.ReceiveReward);
    }

    IEnumerator Wait()
    {
        yield return 0;
        for (int i = 0; i < listItemScroll.Count; i++)
        {
            listItem[i].GetComponent<RectTransform>().position = listItem[i].rectItemRoot.position;
        }
    }

    protected override void Update()
    {
        booScorllMouse = false;
        if (Vector2.Distance(Input.mousePosition, vecScorllDis) > 0.1f)
        {
            booScorllMouse = true;
        }
        vecScorllDis = Input.mousePosition;
    }

    void UpdateTaskData()
    {
        listTask = UserValue.Instance.listTask;
        int intTaskCount = 0;
        for (int i = 0; i < listTask.Count; i++)
        {
            intTaskCount++;
        }
        GetScrollColumn(intTaskCount, goCopyItemRoot, rectScroll, listItemScrollRoot);
        GetColumnItem(intTaskCount, goCopyItem, listItemScroll);
        for (int i = 0; i < listItemScroll.Count; i++)
        {
            if (listItem.Count <= i)
            {
                listItem.Add(listItemScroll[i].GetComponent<ViewTask_SubItem>());
                listItem[i].intIndex = i;
                listItem[i].rectItemRoot = listItemScrollRoot[i];
                listItem[i].GetComponent<Button>().onClick.AddListener(OnClickItemColumn(i));
                listItem[i].btnFinish.onClick.AddListener(OnClickTaskFinish(i));
            }
            listItem[i].btnFinish.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.ReceiveReward);
        }
        for (int i = 0; i < listItemScroll.Count; i++)
        {
            if (listTask.Count > i)
            {
                bool booAwardProduct = true;
                if (listTask[i].enumTask == EnumTaskType.Farm || listTask[i].enumTask == EnumTaskType.Pasture || listTask[i].enumTask == EnumTaskType.Factory)
                {
                    listItem[i].textTaskName.text = ManagerTask.Instance.GetTaskProductName(listTask[i].intID);
                    listItem[i].textInfo.text = ManagerTask.Instance.GetTaskProductExplain(listTask[i].intID);
                    for (int j = 0; j < listTask[i].intGoodsIDs.Length; j++)
                    {
                        JsonValue.DataTableBackPackItem backpack = ManagerProduct.Instance.GetProductTableItem(listTask[i].intGoodsIDs[j]);
                        itemTaskGoods.items[j].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(backpack.strIconName);
                        int intCount = UserValue.Instance.GetKnapsackProductCount(listTask[i].intGoodsIDs[j]);
                        if (intCount < listTask[i].intGoodsCounts[j])
                        {
                            booAwardProduct = false;
                        }
                    }
                }
                else if (listTask[i].enumTask == EnumTaskType.Dungeon)
                {
                    listItem[i].textTaskName.text = ManagerTask.Instance.GetDungeonName(listTask[i].intID);
                    listItem[i].textInfo.text = ManagerTask.Instance.GetDungeonExplain(listTask[i].intID);

                    if (!listTask[i].booDown)
                    {
                        booAwardProduct = false;
                    }
                }
                listItem[i].btnFinish.gameObject.SetActive(booAwardProduct);
                listItemScrollRoot[i].gameObject.SetActive(true);
                listItemScroll[i].gameObject.SetActive(true);
            }
            else
            {
                listItemScrollRoot[i].gameObject.SetActive(false);
                listItemScroll[i].gameObject.SetActive(false);
            }
        }
    }

    UnityEngine.Events.UnityAction OnClickItemColumn(int intIndex)
    {
        return () =>
        {
            intIndexItem = intIndex;
            ItemTaskInfo(intIndex);
        };

    }

    /// <summary>
    /// 完成任务中所有任务,才会显示领取奖励按钮
    /// </summary>
    /// <param name="intIndex"></param>
    UnityEngine.Events.UnityAction OnClickTaskFinish(int intIndex)
    {
        return () =>
        {
            FinishTask(intIndex);
        };
    }

    void FinishTask(int intIndex)
    {
        ManagerValue.actionAudio(EnumAudio.CoinBuy);
        if (listTask[intIndex].enumTask == EnumTaskType.Farm || listTask[intIndex].enumTask == EnumTaskType.Pasture || listTask[intIndex].enumTask == EnumTaskType.Factory)
        {
            for (int i = 0; i < listTask[intIndex].intGoodsCounts.Length; i++)
            {
                UserValue.Instance.KnapsackProductReduce(listTask[intIndex].intGoodsIDs[i], listTask[intIndex].intGoodsCounts[i]);
            }
            UserValue.Instance.SetCoinAdd = listTask[intIndex].intAwardCion;
        }
        else if (listTask[intIndex].enumTask == EnumTaskType.Dungeon)
        {
            JsonValue.DataTaskDungeonItem itemDungeon = ManagerTask.Instance.GetDungeonItem(listTask[intIndex].intID);
            if (!UserValue.Instance.KnapsackCheckGridCount(itemDungeon.awards.Length))
            {
                ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();
                hintBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.TheBDNHESPSIO, null);//"背包空间不足,请整理!";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
                return;
            }
            for (int i = 0; i < itemDungeon.awards.Length; i++)
            {
                BackpackGrid itemGrid = new BackpackGrid();
                ManagerValue.SetProductItem(itemDungeon.awards[i].intID, itemGrid);
                itemGrid.intCount = itemDungeon.awards[i].intCount;
                UserValue.Instance.KnapsackProductAddGrid(itemGrid);
            }
            UserValue.Instance.SetCoinAdd = listTask[intIndex].intAwardCion;
        }

        listTask.RemoveAt(intIndex);
        UpdateTaskData();
        if (intIndex < listTask.Count)
        {
            ItemTaskInfo(intIndex);
            Vector3 vecDis = Vector3.zero;
            if (listItem.Count > 1)
            {
                vecDis = listItem[1].rectItemRoot.position - listItem[0].rectItemRoot.position;
            }
            for (int i = intIndex; i < listItem.Count; i++)
            {
                listItem[i].GetComponent<RectTransform>().position = listItem[i].rectItemRoot.position + vecDis;
            }
        }
        else
        {
            if (intIndex > 0)
            {
                intIndex--;
                intIndexItem = intIndex;
                ItemTaskInfo(intIndex);
            }
            else
            {
                ItemTaskInfo(-1);
            }
        }

        ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);
    }

    void ItemTaskInfo(int intIndex)
    {
        if (intIndex == -1)
        {
            textTaskName.text = "-";
            textTaskInfo.text = "-";
            textTaskCion.text = "0";
            textPenaltyCoin.text = "0";
            btnGetAward.gameObject.SetActive(false);
            for (int i = 0; i < itemAward.items.Length; i++)
            {
                itemAward.items[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < itemTaskGoods.items.Length; i++)
            {
                itemTaskGoods.items[i].gameObject.SetActive(false);
            }
            return;
        }
        for (int i = 0; i < listItem.Count; i++)
        {
            if (intIndex == i)
            {
                listItem[i].goSelect.SetActive(true);
                continue;
            }
            listItem[i].goSelect.SetActive(false);
        }
        int intCount = 0;
        GameObject goFinish = null;
        if (listTask[intIndex].enumTask == EnumTaskType.Farm || listTask[intIndex].enumTask == EnumTaskType.Pasture || listTask[intIndex].enumTask == EnumTaskType.Factory)
        {
            itemTaskGoods.items[0].imageValue.gameObject.SetActive(true);
            itemTaskGoods.items[0].imageValueMain.gameObject.SetActive(true);
            JsonValue.DataTaskProductItem itemProduct = ManagerTask.Instance.GetProductItem(listTask[intIndex].intID);
            textTaskName.text = ManagerTask.Instance.GetTaskProductName(itemProduct.intID);
            textTaskInfo.text = ManagerTask.Instance.GetTaskProductExplain(itemProduct.intID);
            bool booProductDone = true;
            for (int j = 0; j < itemTaskGoods.items.Length; j++)
            {
                if (j < listTask[intIndex].intGoodsCounts.Length)
                {
                    itemTaskGoods.items[j].textValue.text = listTask[intIndex].intGoodsCounts[j].ToString();
                    JsonValue.DataTableBackPackItem backpack = ManagerProduct.Instance.GetProductTableItem(listTask[intIndex].intGoodsIDs[j]);
                    itemTaskGoods.items[j].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(backpack.strIconName);
                    intCount = itemTaskGoods.items[j].transform.childCount;
                    goFinish = itemTaskGoods.items[j].transform.GetChild(intCount - 1).gameObject;
                    intCount = UserValue.Instance.GetKnapsackProductCount(listTask[intIndex].intGoodsIDs[j]);
                    if (intCount >= listTask[intIndex].intGoodsCounts[j])
                    {
                        goFinish.transform.GetChild(0).gameObject.SetActive(true);
                        itemTaskGoods.items[j].textValueMain.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Completed);//"已完成!";
                    }
                    else
                    {
                        goFinish.transform.GetChild(0).gameObject.SetActive(false);
                        itemTaskGoods.items[j].textValueMain.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.TheIINITB);//"背包中,没有该物品!";
                        booProductDone = false;
                    }

                    itemTaskGoods.items[j].gameObject.SetActive(true);
                    continue;
                }
                itemTaskGoods.items[j].gameObject.SetActive(false);
            }
            btnGetAward.gameObject.SetActive(booProductDone);

            textTaskCion.text = listTask[intIndex].intAwardCion.ToString();
            textPenaltyCoin.text = listTask[intIndex].intPenaltyCoin.ToString();
            for (int j = 0; j < itemAward.items.Length; j++)
            {
                //if (j < itemProduct.awards.Length)
                //{
                //    itemAward.items[j].textValueMain.text = itemProduct.awards[j].intCount.ToString();
                //    JsonValue.DataTableBackPackItem backpack = ManagerProduct.Instance.GetProductTableItem(itemProduct.awards[j].intID);
                //    itemAward.items[j].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(backpack.strIconName);

                //    itemAward.items[j].gameObject.SetActive(true);
                //    continue;
                //}
                itemAward.items[j].gameObject.SetActive(false);
            }

        }
        else if (listTask[intIndex].enumTask == EnumTaskType.Dungeon)
        {
            JsonValue.DataTaskDungeonItem itemDungeon = ManagerTask.Instance.GetDungeonItem(listTask[intIndex].intID);
            textTaskName.text = ManagerTask.Instance.GetDungeonName(itemDungeon.intID);
            textTaskInfo.text = ManagerTask.Instance.GetDungeonExplain(itemDungeon.intID);
            for (int j = 0; j < itemTaskGoods.items.Length; j++)
            {
                itemTaskGoods.items[j].gameObject.SetActive(false);
            }
            itemTaskGoods.items[0].gameObject.SetActive(true);
            itemTaskGoods.items[0].imageValue.gameObject.SetActive(false);
            itemTaskGoods.items[0].imageValueMain.gameObject.SetActive(false);
            itemTaskGoods.items[0].textValue.text = "";
            intCount = itemTaskGoods.items[0].transform.childCount;
            goFinish = itemTaskGoods.items[0].transform.GetChild(intCount - 1).gameObject;
            if (listTask[intIndex].booDown)
            {
                goFinish.transform.GetChild(0).gameObject.SetActive(true);
                itemTaskGoods.items[0].textValueMain.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Completed);//"已完成!";
                btnGetAward.gameObject.SetActive(true);
            }
            else
            {
                goFinish.transform.GetChild(0).gameObject.SetActive(false);
                itemTaskGoods.items[0].textValueMain.text = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.HeadTLMPACT, new string[] { ManagerCombat.Instance.GetGameDungeonName(listTask[intIndex].intDungeonID) });//"前往" + dungeon.GetName + "，寻找任务点，并通关任务点。";
                btnGetAward.gameObject.SetActive(false);
            }

            textTaskCion.text = listTask[intIndex].intAwardCion.ToString();
            textPenaltyCoin.text = listTask[intIndex].intPenaltyCoin.ToString();
            for (int j = 0; j < itemAward.items.Length; j++)
            {
                if (j < itemDungeon.awards.Length)
                {
                    itemAward.items[j].textValueMain.text = itemDungeon.awards[j].intCount.ToString();
                    JsonValue.DataTableBackPackItem backpack = ManagerProduct.Instance.GetProductTableItem(itemDungeon.awards[j].intID);
                    itemAward.items[j].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(backpack.strIconName);

                    itemAward.items[j].gameObject.SetActive(true);
                    continue;
                }
                itemAward.items[j].gameObject.SetActive(false);
            }
        }
    }

    void MessageTask(ManagerMessage.MessageBase message)
    {
        UpdateTaskData();
        if (listTask.Count > 0)
        {
            ItemTaskInfo(0);
        }
        else
        {
            ItemTaskInfo(-1);
        }
    }

    private void OnDestroy()
    {
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Update_Task, MessageTask);
    }
}
