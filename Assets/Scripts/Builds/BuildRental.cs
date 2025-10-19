using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 出租屋
/// </summary>
public class BuildRental : BuildBase
{
    int intBuildRank = 1;
    int intExpend;//消耗
    int intPersonCount = 2;
    int intMonthlyIncome;//租金
    int intPersonPrice = 50;
    int intPersonPriceNext = 50;//下个月的租金

    string strRentalContent;//出租屋主标题
    string strRentalContentNotice;//出租屋主要内容

    List<int> listEmployeeIDClear = new List<int>();
    Dictionary<int, PropertiesEmployee> dicEmployee = new Dictionary<int, PropertiesEmployee>();

    //增加房屋等级，则增加房间数量
    //增加房屋等级，则增加入住的冒险者等级
    //是否让他加入城堡
    //ViewMGToViewInfoHouse mgUpdateHouse = new ViewMGToViewInfoHouse();
    MGViewToBuildRental mgToBuild;

    ViewMGToViewInfoRental mgToInfo = new ViewMGToViewInfoRental();
    public override void ShowViewBuildInfo()
    {
        booBuildToView = true;
        SentContent();
    }
    protected override void UpdateDate(MessageDate mgData)
    {
        MessageDate date = mgData as MessageDate;
        if (date != null)
        {
            if (date.numDay == 1)
            {
                MonthlyIncome();
            }
        }
    }
    void MonthlyIncome()
    {
        if (dicEmployee.Count == 0)
        {
            PropertiesEmployee employee = UserValue.Instance.AddGuest();
            dicEmployee.Add(employee.intIndexID, employee);
        }

        //因为PropertiesEmployee.cs的引用无法清除,所以做状态改变处理
        listEmployeeIDClear.Clear();
        foreach (PropertiesEmployee temp in dicEmployee.Values)
        {
            if (temp.enumState == EnumEmployeeState.Delete)
            {
                listEmployeeIDClear.Add(temp.intIndexID);
            }
        }
        for (int i = 0; i < listEmployeeIDClear.Count; i++)
        {
            dicEmployee.Remove(listEmployeeIDClear[i]);
        }

        intPersonPrice = intPersonPriceNext;
        intMonthlyIncome = intPersonPrice * dicEmployee.Count;

        if (booBuildToView)
        {
            SentContent();
        }
    }

    public override void MGViewBuildInfo(MGViewToBuildBase toGround)
    {
        mgToBuild = toGround as MGViewToBuildRental;
        if (mgToBuild != null)
        {
            if (mgToBuild.intPrice != -1)
            {
                intPersonPriceNext = mgToBuild.intPrice;
            }
            booBuildToView = mgToBuild.booSend;
            if (booBuildToView)
            {
                SentContent();
            }
        }
    }

    void SentContent()
    {
        strRentalContent = "目前有<color=#ff8c00>" + dicEmployee.Count + "名租客</color>,目前每人租金为<color=#ff8c00>" + intPersonPrice + "金币</color>,下个月1号收取租金为每人<color=#ff8c00>" + intPersonPriceNext + "金币</color>";

        mgToInfo.intIndexGround = GetIndexGround;
        mgToInfo.intBuildID = GetBuildID;
        mgToInfo.intRentalRank = intBuildRank;
        mgToInfo.intPersonCount = intPersonCount;
        mgToInfo.intMonthlyIncome = intMonthlyIncome;
        mgToInfo.dicEmployee = dicEmployee;
        mgToInfo.intPersonPrice = intPersonPrice;

        mgToInfo.strRentalContent = strRentalContent;
        mgToInfo.strRentalContentNotice = strRentalContentNotice;

        ManagerView.Instance.Show(EnumView.ViewHouse);
        ManagerView.Instance.SetData(EnumView.ViewHouse, mgToInfo);
    }
}
