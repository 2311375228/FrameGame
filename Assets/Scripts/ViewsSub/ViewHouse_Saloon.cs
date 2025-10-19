using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ViewHouse_Saloon : MonoBehaviour
{
    public Text textContent;
    public Text textEmployContent;
    public Text textEmployMoney;
    public Text textEmployingContent;

    public Button btnFarm;//农场,畜牧 类型
    public Button btnFactory;//工坊类型
    public Button btnCombat;//战斗类型
    public Button btnEmployAdd;//雇佣金 +100
    public Button btnEmployReduce;//雇佣金 -100
    public Button btnEmployMoneyCancel;//消除雇佣金
    public Button btnEmployConfirm;//确定发布雇佣信息
    public Button btnEmployCancel;//取消雇佣公告
    public Button btnClose;

    public InputField inputSaloon;

    public RectTransform rectLoadingRotate;

    public GameObject goSelectEmployeeType;//招募类型
    public GameObject goSelectEmployeeContent;//招募内容
    public GameObject goEmploying;//正在招募员工

    public ScrollCycleColumn columnItem;

    [System.NonSerialized]
    public List<ViewHouse_SaloonItem> listItem = new List<ViewHouse_SaloonItem>();
    public List<BuildSaloon.EmployingWork> listData = new List<BuildSaloon.EmployingWork>();
    public GameObject[] goLoadRotate;

    int intIndexRotate;
    float floTime = 0;

    public int intPriceChange;
    public int intHouseRank;
    public int intPrice;
    public int intIncome;
    public int intIndexGround;
    public System.Action<bool, bool, int, int> SendToBuildSaloon;
    ViewHintBar.MessageHintBar barMessage = new ViewHintBar.MessageHintBar();
    // Start is called before the first frame update
    void Start()
    {
        //养殖
        btnFarm.onClick.AddListener(() =>
         {
             SendToBuildSaloon(true, false, -1, 1);
         });
        //工厂
        btnFactory.onClick.AddListener(() =>
        {
            SendToBuildSaloon(true, false, -1, 2);
        });
        //战斗
        btnCombat.onClick.AddListener(() =>
        {
            SendToBuildSaloon(true, false, -1, 3);
        });
        //悬赏金 +100
        btnEmployAdd.onClick.AddListener(() =>
        {
            if (intPriceChange + 100 > intHouseRank * 5000)
            {
                intPriceChange = intHouseRank * 5000;
                barMessage.strHintBar = "已达到该酒吧的佣金限制,若要提高限制,请升级酒吧";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, barMessage);
            }
            else
            {
                intPriceChange += 100;
            }
            inputSaloon.text = intPriceChange.ToString();
        });
        //悬赏金 -100
        btnEmployReduce.onClick.AddListener(() =>
        {
            if (intPriceChange - 100 < 500)
            {
                intPriceChange = 500;
                barMessage.strHintBar = "不能再减了,最低金额是500";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, barMessage);
            }
            else
            {
                intPriceChange -= 100;
            }
            inputSaloon.text = intPriceChange.ToString();
        });
        inputSaloon.onValueChanged.AddListener((value) =>
        {
            if (value.Length > 0)
            {
                intPriceChange = int.Parse(value.ToString());
                if (intPriceChange > intHouseRank * 5000)
                {
                    intPriceChange = intHouseRank * 5000;
                    inputSaloon.text = intPriceChange.ToString();
                    barMessage.strHintBar = "已达到该酒吧的佣金限制,若要提高限制,请升级酒吧";
                    ManagerView.Instance.Show(EnumView.ViewHintBar);
                    ManagerView.Instance.SetData(EnumView.ViewHintBar, barMessage);
                }
            }
        });
        //重设雇佣金,最低1000
        btnEmployMoneyCancel.onClick.AddListener(() =>
        {
            intPriceChange = intPrice;
            inputSaloon.text = intPrice.ToString();
        });
        //确认雇佣金
        btnEmployConfirm.onClick.AddListener(() =>
        {
            intPrice = intPriceChange;
            SendToBuildSaloon(true, true, intPrice, -1);
        });
        //取消雇佣公告
        btnEmployCancel.onClick.AddListener(() =>
        {
            SendToBuildSaloon(true, false, -1, 0);
        });
        //关闭
        btnClose.onClick.AddListener(() => { gameObject.SetActive(false); });

        goLoadRotate = new GameObject[rectLoadingRotate.childCount];
        for (int i = 0; i < rectLoadingRotate.childCount; i++)
        {
            goLoadRotate[i] = rectLoadingRotate.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rectLoadingRotate.gameObject.activeSelf)
        {
            floTime += Time.deltaTime;
            if (floTime > 0.1f)
            {
                floTime = 0;

                intIndexRotate++;
                goLoadRotate[intIndexRotate == goLoadRotate.Length ? 0 : intIndexRotate].SetActive(false);
                if (intIndexRotate == goLoadRotate.Length) { intIndexRotate = 0; }
                goLoadRotate[intIndexRotate + 1 == goLoadRotate.Length ? 0 : intIndexRotate + 1].SetActive(true);

            }
        }
    }

    public void Show()
    {
        //酒吧
        //GridLayoutGroup temp = columnItem.scrollRectContent.GetComponent<GridLayoutGroup>();
        //float floHeight = temp.cellSize.y + temp.spacing.y;
        //List<RectTransform> listTemp = columnItem.InitItem(RefreshDataSaloon, floHeight);
        //for (int i = 0; i < listTemp.Count; i++)
        //{
        //    listItem.Add(listTemp[i].GetComponent<ViewHouse_SaloonItem>());
        //    listItem[i].actionBase = ActionEventSeeSaloon;
        //    listItem[i].actionEmploy = ActionEventEmploySaloon;
        //}
        //columnItem.SetDataTotal(0);
    }

    void RefreshDataSaloon(int intIndexItem, int intIndexData)
    {
        ViewHouse_SaloonItem itemTemp = listItem[intIndexItem];
        if (intIndexData >= listData.Count)
        {
            itemTemp.gameObject.SetActive(false);
        }
        else
        {
            itemTemp.gameObject.SetActive(true);
            itemTemp.numIndexItem = intIndexItem;
            itemTemp.numIndexData = intIndexData;

            BuildSaloon.EmployingWork work = listData[intIndexData];
            itemTemp.textContent.text = work.strContent;
            itemTemp.textWork.text = work.strTime;
        }
    }
    /// <summary>
    /// 酒吧 查看按钮
    /// </summary>
    void ActionEventSeeSaloon(int intIndexItem, int intIndexData)
    {

    }
    /// <summary>
    /// 酒吧 雇佣按钮
    /// </summary>
    void ActionEventEmploySaloon(int intIndexItem, int intIndexData)
    {

    }

    public void DeleteBuild(List<int> listEmployWork, ViewHint.MessageHint viewHint)
    {
        if (listEmployWork.Count > 0)
        {
            int intPersion = listEmployWork.Count;
            List<int> listGroundID = UserValue.Instance.GetBuildTypeIntGound(4003);
            for (int i = 0; i < listGroundID.Count; i++)
            {
                BuildSaloon saloon = UserValue.Instance.GetBuildValue(listGroundID[i]) as BuildSaloon;
                if (intIndexGround != listGroundID[i])
                {
                    for (int j = 0; j < saloon.intSaloonPserion.Length; j++)
                    {
                        if (saloon.intSaloonPserion[j] == -1)
                        {
                            intPersion--;
                        }
                    }
                }
                if (intPersion <= 0)
                {
                    break;
                }
            }
            if (intPersion <= 0)
            {
                viewHint.strHint = "有" + listEmployWork.Count + "在工作中,拆除建筑,这些员工将去其他酒吧";
            }
            else if (intPersion > 0)
            {
                viewHint.strHint = listEmployWork.Count + "名员工中,有" + (listEmployWork.Count - intPersion) + "名员工没有去处,拆除建筑,将失去" + (listEmployWork.Count - intPersion) + "名员工";
            }
        }
        else
        {
            viewHint.strHint = "是否拆除建筑";
        }
    }

    public void DelectEmployee(List<PropertiesEmployee> listData,List<int> listEmployWork)
    {
        #region 移除酒吧员工
        foreach (PropertiesEmployee temp in listData)
        {
            if (!listEmployWork.Contains(temp.intIndexID))
            {
                UserValue.Instance.RecyclingStationGuest(temp.intIndexID);
            }
        }
        int intPersion = listEmployWork.Count;
        List<int> listGroundID = UserValue.Instance.GetBuildTypeIntGound(4003);
        //酒吧里 不移除站空位没有雇佣的员工
        for (int i = 0; i < listGroundID.Count; i++)
        {
            BuildSaloon saloon = UserValue.Instance.GetBuildValue(listGroundID[i]) as BuildSaloon;
            if (intIndexGround != listGroundID[i])
            {
                for (int j = 0; j < saloon.intSaloonPserion.Length; j++)
                {
                    //-1 表示该位置空出 其他代表有人
                    if (saloon.intSaloonPserion[j] == -1)
                    {
                        saloon.intSaloonPserion[j] = listEmployWork[intPersion - 1];
                        listEmployWork.RemoveAt(intPersion - 1);
                        if (intPersion == 0)
                        {
                            break;
                        }
                        intPersion--;
                    }
                }
                //内部找完了,避免进入下一个无效循环
                if (intPersion == 0)
                {
                    break;
                }
            }
        }
        //酒吧里 位置不够 移除站空位没有雇佣的员工
        for (int i = 0; i < listGroundID.Count; i++)
        {
            BuildSaloon saloon = UserValue.Instance.GetBuildValue(listGroundID[i]) as BuildSaloon;
            if (intIndexGround != listGroundID[i])
            {
                for (int j = 0; j < saloon.intSaloonPserion.Length; j++)
                {
                    //-1 表示该位置空出 其他代表有人
                    if (saloon.intSaloonPserion[j] != -1)
                    {
                        PropertiesEmployee temployeeTemp = UserValue.Instance.GetEmployeeValue(saloon.intSaloonPserion[j]);
                        if (temployeeTemp.enumState == EnumEmployeeState.Employ)
                        {
                            //将没有雇佣的员工,删除掉
                            UserValue.Instance.RecyclingStationGuest(saloon.intSaloonPserion[j]);
                            saloon.intSaloonPserion[j] = listEmployWork[intPersion - 1];
                            listEmployWork.RemoveAt(intPersion - 1);
                            if (intPersion == 0)
                            {
                                break;
                            }
                            intPersion--;
                        }
                    }
                }
                //内部找完了,避免进入下一个无效循环
                if (intPersion == 0)
                {
                    break;
                }
            }
        }

        //将不能安排位置的员工移除
        for (int i = 0; i < listEmployWork.Count; i++)
        {
            UserValue.Instance.RecyclingStationEmployee(listEmployWork[i]);
        }
        #endregion
    }
}
