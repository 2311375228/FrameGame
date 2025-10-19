using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBarTop : ViewBase
{
    public Text textMailCount;
    public Text textMailInfo;

    public Text textCoinTag;
    public Text textCoin;
    public Text textCoin_;

    public Text textCoinMaxTag;
    public Text textCoinMax;
    public Text textCoinMax_;
    public GameObject goShowCoin;

    public Button btnRecruit;//招募员工
    public Button btnEmployeeSee;//查看员工
    public Button btnCombatTeam;//战斗队列
    public Button btnProductionSee;//查看生产
    public Button btnProductStockSee;//查看库存
    public Button btnTask;//任务

    public Button btnCloseMail;
    public Button btnLand;
    public Button[] btnLands;

    public GameObject goLand;
    public GameObject goMail;//邮件
    public ScrollRect scrollRect;
    public RectTransform rectScrollMail;
    public GameObject goCopyMailRoot;
    public GameObject goCopyMail;

    public ViewBarTop_TopBar topBar;


    BackpackGrid backpackItem = new BackpackGrid();
    List<ViewBarTop_ItemMail.Mail> listMail = new List<ViewBarTop_ItemMail.Mail>();
    List<RectTransform> listItemScrollRoot = new List<RectTransform>();
    List<RectTransform> listItemScroll = new List<RectTransform>();
    List<ViewBarTop_ItemMail> listItemMail = new List<ViewBarTop_ItemMail>();
    Vector2 vecScrollMailDis;
    bool booScrollMailMouse;

    Vector3[] vecMapPoints = new Vector3[4];
    //Vector3 vecPositionMap = new Vector3(4.98f, 9.71f, 4.82f);//10*10正方形格子地图
    Vector3 vecPositionMap = new Vector3(15.9f, 30.51f, 10.33f);//32*20的格子地图
    Vector3 vecTargetLand;
    float floTargetLand = 70;

    //邮件数量
    int IntMailCount
    {
        set
        {
            ManagerValue.intNewMailCount = value;
            if (value < 100)
            {
                textMailCount.text = ManagerValue.intNewMailCount.ToString();
            }
            else
            {
                textMailCount.text = "+99";
            }
        }
        get { return ManagerValue.intNewMailCount; }
    }

    protected override void Start()
    {
        base.Start();

        btnLand.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            goLand.SetActive(!goLand.activeSelf);
        });
        goLand.SetActive(false);
        goMail.SetActive(false);

        topBar.btnHint.gameObject.SetActive(false);

        btnCloseMail.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            goMail.SetActive(false);
        });
        //领地
        btnLands[0].onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerValue.actionCameraLocation(ControllerCamera.EnumCameraPosition.Ground);
        });
        //集市
        btnLands[1].onClick.AddListener(() =>
        {
            ManagerView.Instance.Hide(EnumView.ViewHouse);
            ManagerView.Instance.Hide(EnumView.ViewBuildMain);
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerValue.actionCameraLocation(ControllerCamera.EnumCameraPosition.Market);
        });
        //冒险
        btnLands[2].onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);

            ManagerView.Instance.HideAll();
            ManagerView.Instance.Show(EnumView.ViewMap);
            ManagerValue.actionCameraLocation(ControllerCamera.EnumCameraPosition.GameDungeon);
        });

        //招募员工
        btnRecruit.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerView.Instance.Show(EnumView.ViewRecruit);
        });
        //查看员工
        btnEmployeeSee.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerView.Instance.Show(EnumView.ViewEmployeeList);
        });
        //战斗队列
        btnCombatTeam.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerView.Instance.Show(EnumView.ViewCombatTeam);
        });

        //查看生产
        btnProductionSee.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerView.Instance.Show(EnumView.ViewProductionSee);
        });
        //查看库存
        btnProductStockSee.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerView.Instance.Show(EnumView.ViewProductStockSee);
        });
        //任务
        btnTask.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerView.Instance.Show(EnumView.ViewTask);
        });

        topBar.btnCoin.onClick.AddListener(() =>
        {
            goShowCoin.SetActive(!goShowCoin.activeSelf);

            textCoinMax.text = UserValue.Instance.GetCoinMax.ToString("N0");
            textCoinMax_.text = ManagerValue.GetCoin(UserValue.Instance.GetCoinMax);

            textCoin.text = UserValue.Instance.GetCoin.ToString("N0");
            textCoin_.text = ManagerValue.GetCoin(UserValue.Instance.GetCoin);
        });
        //设置
        topBar.btnSetting.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerView.Instance.Show(EnumView.ViewSetting);
        });
        //提醒
        topBar.btnHint.onClick.AddListener(() =>
        {

        });
        //背包
        topBar.btnBackpack.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Open);

            ManagerView.Instance.Show(EnumView.ViewKnapsack);
        });

        //地图
        topBar.btnMap.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);

            topBar.goMap.SetActive(!topBar.goMap.activeSelf);
            if (topBar.goMap.activeSelf)
            {
                //3D地图与2DUI之间的转换值
                float width = 32 / Vector3.Distance(vecMapPoints[1], vecMapPoints[2]);
                float hight = 20 / Vector3.Distance(vecMapPoints[0], vecMapPoints[1]);

                vecTargetLand = ControllerCamera.Instance.transCamera.position;
                vecTargetLand.y = vecTargetLand.z;
                vecTargetLand.z = 0;
                vecTargetLand = vecTargetLand * (floTargetLand * Screen.width / 3840);
                //vecTargetLand.x = vecTargetLand.x - 260;
                //vecTargetLand.y = vecTargetLand.y - 200;
                vecTargetLand.x = (vecMapPoints[0].x + Vector3.Distance(vecMapPoints[1], vecMapPoints[2]) * 0.05f) + vecTargetLand.x;
                vecTargetLand.y = (vecMapPoints[0].y + Vector3.Distance(vecMapPoints[0], vecMapPoints[1]) * 0.05f) + vecTargetLand.y;
                topBar.rectTargetLand.position = vecTargetLand;
            }
            goMail.SetActive(false);
        });
        //邮件
        topBar.btnMail.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);

            IntMailCount = 0;
            textMailCount.text = "";

            goMail.SetActive(!goMail.activeSelf);
            topBar.goMap.SetActive(false);

            for (int i = 0; i < ManagerValue.listMail.Count; i++)
            {
                if (ManagerValue.listMail[i].booGet)
                {
                    ManagerValue.listMail.RemoveAt(i);
                    i--;
                }
            }
            ShowsItemMail();
            StartCoroutine(Wait());
        });
        scrollRect.onValueChanged.AddListener((value) =>
        {
            if (booScrollMailMouse)
            {
                for (int i = 0; i < listItemScroll.Count; i++)
                {
                    listItemMail[i].GetComponent<RectTransform>().position = listItemMail[i].rectItemRoot.position;
                }
            }


        });
        //前往目的地
        topBar.btnGoto.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            topBar.goMap.SetActive(false);

            Vector3 vecTemp = vecTargetLand;
            //vecTargetLand.x = vecTargetLand.x + 260;
            //vecTargetLand.y = vecTargetLand.y + 200;
            vecTargetLand.x = vecTargetLand.x - (vecMapPoints[0].x + Vector3.Distance(vecMapPoints[1], vecMapPoints[2]) * 0.05f);
            vecTargetLand.y = vecTargetLand.y - (vecMapPoints[0].y + Vector3.Distance(vecMapPoints[0], vecMapPoints[1]) * 0.05f);
            vecTargetLand = vecTargetLand * (1 / (floTargetLand * Screen.width / 3840));
            vecTargetLand.z = vecTargetLand.y;
            vecTargetLand.y = 2;
            ControllerCamera.Instance.transCamera.position = vecTargetLand;
            ControllerCamera.Instance.vecRemenberPosition = vecTargetLand;
            vecTargetLand = vecTemp;

            ManagerValue.actionCameraLocation(ControllerCamera.EnumCameraPosition.Ground);
        });
        topBar.btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            topBar.goMap.SetActive(false);
        });

        topBar.rawImageMap.transform.parent.GetComponent<RectTransform>().GetWorldCorners(vecMapPoints);
        //for (int i = 0; i < vecMapPoints.Length; i++)
        //{
        //    Debug.Log(vecMapPoints[i]);
        //}

        goCopyMail.SetActive(false);
        goCopyMailRoot.SetActive(false);
        goShowCoin.SetActive(false);

        topBar.rawImageMap.texture = ManagerValue.cameraMap.targetTexture;
        topBar.goMap.SetActive(false);
        goMail.transform.position = topBar.goMap.transform.position;

        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Coin, MessageUpdateCoin);
        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Date, MessageUpdateDate);
        ManagerMessage.Instance.AddEvent(EnumMessage.Mail, MessageMail);
        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Task, MessageTask);

        ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);
    }

    public override void Show()
    {
        base.Show();

        //检查金币
        UserValue.Instance.SetCoinReduce(0);

        IntMailCount = ManagerValue.intNewMailCount;

        topBar.textGameMode.text = UserValue.Instance.GetGameModeName(ManagerValue.enumGameMode);
        topBar.textGameMode.gameObject.SetActive(false);
        topBar.textUserNickname.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Nickname) + ":" + UserValue.Instance.GetNickname;

        topBar.btnGoto.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Go);
        //战斗队列
        btnCombatTeam.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.BattleQueue);
        //查看库存
        btnProductStockSee.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Warehouse);
        //任务
        btnTask.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Tasks);
        textMailInfo.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.CurrentMailCount);
        topBar.textMapTitle.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Map);

        //金币显示
        textCoinMaxTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.MaximumCOFC);
        textCoinTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.GoldCoins);
    }

    IEnumerator Wait()
    {
        yield return 0;
        for (int i = 0; i < listItemScroll.Count; i++)
        {
            listItemMail[i].GetComponent<RectTransform>().position = listItemMail[i].rectItemRoot.position;
        }
    }
    protected override void Update()
    {
        if (Input.GetMouseButtonUp(0) && topBar.goMap.activeSelf)
        {
            Vector3 vecTemp = Input.mousePosition;
            //左下角,右上角
            if (vecMapPoints[0].x + Vector3.Distance(vecMapPoints[1], vecMapPoints[2]) * 0.06f < vecTemp.x
                && vecMapPoints[0].y + Vector3.Distance(vecMapPoints[0], vecMapPoints[1]) * 0.03f < vecTemp.y
                && vecMapPoints[2].x > vecTemp.x + Vector3.Distance(vecMapPoints[1], vecMapPoints[2]) * 0.06f
                && vecMapPoints[2].y > vecTemp.y + Vector3.Distance(vecMapPoints[0], vecMapPoints[1]) * 0.05f
             )
            {
                topBar.rectTargetLand.position = vecTemp;
                vecTargetLand = topBar.rectTargetLand.position;
            }
        }

        if (goMail.activeSelf)
        {
            booScrollMailMouse = false;
            if (Vector2.Distance(Input.mousePosition, vecScrollMailDis) > 0.1f)
            {
                booScrollMailMouse = true;
            }
            vecScrollMailDis = Input.mousePosition;
        }
    }

    void ShowsItemMail()
    {
        listMail = ManagerValue.listMail;
        int intMailCount = 0;
        for (int i = 0; i < listMail.Count; i++)
        {
            intMailCount++;
        }
        GetScrollColumn(intMailCount, goCopyMailRoot, rectScrollMail, listItemScrollRoot);
        GetColumnItem(intMailCount, goCopyMail, listItemScroll);
        for (int i = 0; i < listItemScrollRoot.Count; i++)
        {
            if (listItemMail.Count <= i)
            {
                listItemMail.Add(listItemScroll[i].GetComponent<ViewBarTop_ItemMail>());
                listItemMail[i].intIndex = i;
                listItemMail[i].rectItemRoot = listItemScrollRoot[i];
                listItemMail[i].btnSee.onClick.AddListener(OnClickMailSee(i));
                listItemMail[i].btnGet.onClick.AddListener(OnClickMailGet(i));
                listItemMail[i].textReceive.transform.position = listItemMail[i].btnGet.transform.position;
            }
            listItemMail[i].gameObject.SetActive(true);
            listItemMail[i].btnGet.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Receive);
            listItemMail[i].btnSee.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.View);
            listItemMail[i].btnSee.gameObject.SetActive(false);
        }

        for (int i = 0; i < listItemScroll.Count; i++)
        {
            if (listMail.Count > i)
            {
                listItemMail[i].textContent.text = listMail[i].strContent;
                listItemMail[i].imageProduct.sprite = ManagerResources.Instance.GetFrameRank(listMail[i].strIconName);

                Sprite spriteTemp = null;
                for (int j = 0; j < listItemMail[i].itemAwards.Length; j++)
                {
                    if (j < listMail[i].gridItems.Length)
                    {
                        listItemMail[i].itemAwards[j].gameObject.SetActive(true);
                        listItemMail[i].itemAwards[j].textValueMain.text = listMail[i].intIndexCounts[j].ToString();

                        if (ManagerValue.listMail[i].gridItems[j] == EnumKnapsackStockType.Sword
                            || listMail[i].gridItems[j] == EnumKnapsackStockType.Bow
                            || listMail[i].gridItems[j] == EnumKnapsackStockType.Wand
                            || listMail[i].gridItems[j] == EnumKnapsackStockType.Armor
                            || listMail[i].gridItems[j] == EnumKnapsackStockType.Shoes)
                        {
                            JsonValue.TableEquipmentItem equipment = ManagerCombat.Instance.GetEquipmentItem(listMail[i].intIndexIDs[j]);
                            spriteTemp = ManagerResources.Instance.GetEquipmentSprite(equipment.strICON);
                            listItemMail[i].itemAwards[j].imageValueMain.sprite = spriteTemp;
                            listItemMail[i].itemAwards[j].imageValue.sprite = ManagerResources.Instance.GetFrameRank(listMail[i].intRanks[j].ToString());
                        }
                        else if (listMail[i].gridItems[j] == EnumKnapsackStockType.Farm
                            || listMail[i].gridItems[j] == EnumKnapsackStockType.Fasture
                            || listMail[i].gridItems[j] == EnumKnapsackStockType.Factory)
                        {
                            JsonValue.DataTableBackPackItem product = ManagerProduct.Instance.GetProductTableItem(listMail[i].intIndexIDs[j]);
                            spriteTemp = ManagerResources.Instance.GetBackpackSprite(product.strIconName);
                            listItemMail[i].itemAwards[j].imageValueMain.sprite = spriteTemp;
                            listItemMail[i].itemAwards[j].imageValue.sprite = ManagerResources.Instance.GetFrameRank(listMail[i].intRanks[j].ToString());
                        }

                        continue;
                    }

                    listItemMail[i].itemAwards[j].gameObject.SetActive(false);
                }
                listItemScroll[i].gameObject.SetActive(true);
                listItemScrollRoot[i].gameObject.SetActive(true);
            }
            else
            {
                listItemScroll[i].gameObject.SetActive(false);
                listItemScrollRoot[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 查看邮件
    /// </summary>
    UnityEngine.Events.UnityAction OnClickMailSee(int intIndex)
    {
        return () =>
        {

        };
    }
    /// <summary>
    /// 点击确定获取奖励，或已收到
    /// </summary>
    UnityEngine.Events.UnityAction OnClickMailGet(int intIndex)
    {
        return () =>
        {
            ViewBarTop_ItemMail.Mail item = listMail[intIndex];
            if (!UserValue.Instance.KnapsackCheckGridCount(item.gridItems.Length))
            {
                ManagerValue.actionAudio(EnumAudio.Unable);

                ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();
                hintBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.LimitedBSWYLTOI, null);//"背包空间不足,请整理背包空间";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
                return;
            }

            ManagerValue.actionAudio(EnumAudio.Ground);

            for (int i = 0; i < item.gridItems.Length; i++)
            {
                if (item.gridItems[i] == EnumKnapsackStockType.Coin)
                {
                    if (item.intIndexIDs[i] == 0)
                    {
                        UserValue.Instance.SetCoinAdd = item.intIndexCounts[i];
                        ManagerView.Instance.Show(EnumView.ViewBarTop);
                    }
                }
                else if (item.gridItems[i] == EnumKnapsackStockType.Sword
                    || item.gridItems[i] == EnumKnapsackStockType.Bow
                    || item.gridItems[i] == EnumKnapsackStockType.Wand
                    || item.gridItems[i] == EnumKnapsackStockType.Armor
                    || item.gridItems[i] == EnumKnapsackStockType.Shoes)
                {
                    ManagerValue.SetEquipmentItem(item.intIndexIDs[i], backpackItem);
                    backpackItem.intRank = item.intRanks[i];
                    backpackItem.intCount = item.intIndexCounts[i];
                    UserValue.Instance.KnapsackProductAddGrid(backpackItem);
                }
                else if (item.gridItems[i] == EnumKnapsackStockType.Farm
                    || item.gridItems[i] == EnumKnapsackStockType.Fasture
                    || item.gridItems[i] == EnumKnapsackStockType.Factory)
                {
                    ManagerValue.SetProductItem(item.intIndexIDs[i], backpackItem);
                    backpackItem.intRank = item.intRanks[i];
                    backpackItem.intCount = item.intIndexCounts[i];
                    UserValue.Instance.KnapsackProductAddGrid(backpackItem);
                }
            }
            ManagerValue.listMail.RemoveAt(intIndex);
            ShowsItemMail();
            if (intIndex < ManagerValue.listMail.Count)
            {
                Vector3 vecDis = Vector3.zero;
                if (ManagerValue.listMail.Count > 1)
                {
                    vecDis = listItemMail[1].rectItemRoot.position - listItemMail[0].rectItemRoot.position;
                }
                for (int i = intIndex; i < listItemMail.Count; i++)
                {
                    listItemMail[i].GetComponent<RectTransform>().position = listItemMail[i].rectItemRoot.position + vecDis;
                }
            }
        };
    }

    void MessageUpdateCoin(ManagerMessage.MessageBase message)
    {
        long longCoin = UserValue.Instance.GetCoin;
        float floCoin = longCoin;
        float floMax = UserValue.Instance.GetCoinMax;
        float floTemp = floCoin / floMax;
        topBar.textCoin.text = longCoin.ToString("N0");
        topBar.sliderCoin.value = floTemp;

        if (goShowCoin.activeSelf)
        {
            textCoinMax.text = floMax.ToString("N0");
            textCoinMax_.text = ManagerValue.GetCoin(floMax);

            textCoin.text = floCoin.ToString("N0");
            textCoin_.text = ManagerValue.GetCoin(floCoin);
        }
    }

    void MessageUpdateDate(ManagerMessage.MessageBase message)
    {
        MessageDate mgDate = message as MessageDate;
        if (mgDate != null)
        {
            topBar.textDate.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.GameDate) + ":" + mgDate.numYear + "-" + mgDate.numMonth + "-" + mgDate.numDay;
            topBar.textNowTime.text = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.DayOfWeek + "  " + DateTime.Now.Hour + ":" + DateTime.Now.Minute;

            topBar.textTotalTime.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.GameDuration) + ":" + ManagerValue.intTotalDay / 3600 + ":" + (ManagerValue.intTotalDay % 3600) / 60;
        }
    }

    void MessageMail(ManagerMessage.MessageBase message)
    {
        MessageMail mail = message as MessageMail;
        if (mail != null)
        {
            ViewBarTop_ItemMail.Mail item = new ViewBarTop_ItemMail.Mail();
            item.booGet = false;
            item.booSee = false;
            item.enumMail = mail.enumMail;
            item.gridItems = mail.gridItems.Clone() as EnumKnapsackStockType[];
            item.intIndexIDs = mail.intIndexIDs.Clone() as int[];
            item.intRanks = mail.intRanks.Clone() as int[];
            item.intIndexCounts = mail.intIndexCounts.Clone() as int[];
            item.strContent = mail.strContent;
            item.strIconName = mail.enumMail.ToString();
            ManagerValue.listMail.Add(item);
            ShowsItemMail();
            if (!goMail.activeSelf)
            {
                IntMailCount++;
            }
        }
    }

    void MessageTask(ManagerMessage.MessageBase message)
    {
        //List<PropertiesTask> listTemp = UserValue.Instance.listTask;
        //if (listTemp.Count > 0)
        //{
        //    btnTask.transform.GetChild(0).GetComponent<Text>().text = "任务(" + UserValue.Instance.listTask.Count + ")";
        //}
        //else
        //{
        //    btnTask.transform.GetChild(0).GetComponent<Text>().text = "任务";
        //}
    }

    private void OnDestroy()
    {
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Update_Coin, MessageUpdateCoin);
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Update_Date, MessageUpdateDate);
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Mail, MessageMail);
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Update_Task, MessageTask);
    }
}
