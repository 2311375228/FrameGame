using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 酒吧
/// </summary>
public class BuildSaloon : BuildBase
{
    //雇佣公告每随机15-60天,刷新一次来的员工,平时驻留小角色
    bool booEmploying;//是否正在雇佣

    int intBuildRank = 1;
    int intDailyIncome;//日收益
    int intEmployPrice = 500;//雇佣金额度
    int intWaitTime;

    int intEmployingRefrush;
    int intEmployingWaitDay;
    int intEmployingPersonCount = 30;
    int intEmployingIncomeTime;//自增量
    int intEmployingIncomeWait;//等待多久
    /// <summary>
    /// 雇佣类型
    /// -1=不用更改
    /// 0=没有雇佣
    /// 1=养殖
    /// 2=工厂
    /// 3=战斗
    /// </summary>
    int intEmployType;

    string strContentTitle;//标题提醒
    string strContentNotice;//雇佣内容
    string strContentEmploying;//正在雇佣

    //因为要有空闲位置
    public int[] intSaloonPserion = new int[6];
    //员工可以工作的天数
    int[] intEmployDays = new int[] { 30, 50, 80, 150, 280, 365, 730, 1095 };
    //需要清理的员工
    List<int> listEmployeeIDClear = new List<int>();
    Dictionary<int, PropertiesEmployee> dicEmployee = new Dictionary<int, PropertiesEmployee>();
    //雇佣公告人员
    Dictionary<int, EmployingWork> dicEmploying = new Dictionary<int, EmployingWork>();
    ViewMGToViewInfoSaloon mgToInfo = new ViewMGToViewInfoSaloon();
    MGViewToBuildSaloon mgToBuild;
    public override void ShowViewBuildInfo()
    {
        booBuildToView = true;
        SendContent();
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        MessageDate date = mgData as MessageDate;
        if (date != null)
        {
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

            GuestComeIn();
            EmployComeIn();
        }
    }

    public override void MGViewBuildInfo(MGViewToBuildBase toGround)
    {
        mgToBuild = toGround as MGViewToBuildSaloon;
        if (mgToBuild != null)
        {
            booBuildToView = mgToBuild.booSend;

            if (booBuildToView)
            {
                //为了防止关闭是还在触发事件
                booEmploying = mgToBuild.booEmploying;
                if (mgToBuild.intEmployPrice != -1)
                {
                    intEmployPrice = mgToBuild.intEmployPrice;
                }
                if (mgToBuild.intEmployType != -1)
                {
                    intEmployType = mgToBuild.intEmployType;
                    switch (intEmployType)
                    {
                        case 0://取消雇佣
                            break;
                        case 1://养殖
                            strContentNotice = "为牧草场,苹果场,蔬菜场,牧牛场,养猪场等招募工人.";
                            break;
                        case 2://工厂
                            strContentNotice = "为工坊提供合适的工人";
                            break;
                        case 3://战斗
                            strContentNotice = "寻找需要的战斗人员";
                            break;
                    }
                }
                SendContent();
            }
        }
    }

    /// <summary>
    /// 客人进入酒吧
    /// 逗留5天
    /// </summary>
    void GuestComeIn()
    {
        intWaitTime += 1;
        if (intWaitTime == 5)
        {
            intWaitTime = 0;

            //不能超过酒吧 最大空闲数
            int intRandomPerson = Random.Range(0, intSaloonPserion.Length - dicEmployee.Count);
            foreach (PropertiesEmployee temp in dicEmployee.Values)
            {
                UserValue.Instance.RecyclingStationGuest(temp.intIndexID);
                temp.enumState = EnumEmployeeState.Delete;
            }
            dicEmployee.Clear();

            for (int i = 0; i < intRandomPerson; i++)
            {
                PropertiesEmployee temp = UserValue.Instance.AddGuest();
                temp.enumIdentification = EnumEmployeeIdentification.Toper;
                dicEmployee.Add(temp.intIndexID, temp);
            }

            if (booBuildToView)
            {
                SendContent();
            }
        }
    }
    /// <summary>
    /// 悬赏 人员进入
    /// </summary>
    void EmployComeIn()
    {
        if (intEmployingWaitDay < 90)//大于则不再招募
        {
            intEmployingRefrush = 0;
            intEmployingWaitDay += 1;
            //当佣金人员满了就不再进人
            if (dicEmploying.Count != intEmployingPersonCount)
            {
                intEmployingIncomeTime += 1;
                if (intEmployingIncomeTime > intEmployingIncomeWait)
                {
                    intEmployingIncomeTime = 0;
                    intEmployingIncomeWait = Random.Range(7, 20);

                    int intRandomCount = Random.Range(1, 4);
                    RandomEmployProperties(intRandomCount);
                    if (booBuildToView)
                    {
                        SendContent();
                    }
                }
            }
        }
        if (intEmployingRefrush > 365)//刷新一次
        {
            intEmployingRefrush = 0;
            foreach (EmployingWork temp in dicEmploying.Values)
            {
                temp.employee.enumState = EnumEmployeeState.Delete;
                UserValue.Instance.RecyclingStationGuest(temp.employee.intIndexID);
            }
            int intTempCount = dicEmploying.Count;
            dicEmploying.Clear();
            RandomEmployProperties(intTempCount);
            if (booBuildToView)
            {
                SendContent();
            }
        }
    }

    void RandomEmployProperties(int intPersonCount)
    {
        for (int i = 0; i < intPersonCount; i++)
        {
            if (dicEmploying.Count == intEmployingPersonCount)
            {
                break;
            }
            PropertiesEmployee guest = UserValue.Instance.AddGuest();
            EmployingWork work = new EmployingWork();
            work.employee = guest;
            //养殖:根据养殖场数量,消耗量,佣金,来判断
            //工坊:根据工坊需要的属性来做调整,佣金是参考调剂
            //战斗:打败的最高关卡,通关数量,通关次数
            switch (intEmployType)
            {
                case 1://养殖
                    ManagerEmployee.Instance.GetRandomExtraBuildID(out guest.intAdditionBuildID, out guest.intAdditionProductID);
                    guest.enumAddition = (EnumEmployeeAddition)Random.Range(0, 3);
                    guest.intEmployWorkValue = Random.Range(0, 100) < 98 ? intEmployDays[Random.Range(0, 5)] : intEmployDays[Random.Range(5, 8)];
                    guest.enumState = EnumEmployeeState.EmployTime;
                    //这里设置增益
                    guest.intAdditionValue = ManagerEmployee.Instance.GetRandomExtraValue(guest.enumAddition);
                    work.strContent = ManagerEmployee.Instance.GetExtraContent(guest.intAdditionBuildID, guest.intAdditionProductID, guest.enumAddition, guest.intAdditionValue);
                    work.employee.strAddtionContent = work.strContent;
                    work.strTime = "工作" + guest.intEmployWorkValue + "天";
                    break;
                case 2://工坊
                    //增加工坊非装备的生产量,提高装备的属性,攻击,魔法,速度,血量
                    guest.enumState = EnumEmployeeState.EmployTime;
                    guest.enumAddition = EnumEmployeeAddition.Time;
                    guest.intEmployWorkValue = 30;
                    guest.strAddtionContent = "生产过程中,该增益需提前添加,且全程不可中断,否则不会附加到物品中";
                    work.strTime = "工作" + guest.intAdditionValue + "天";
                    break;
                case 3://战斗
                    guest.intEmployWorkValue = 2;
                    guest.enumState = EnumEmployeeState.EmployCombat;
                    work.strTime = "参数副本战斗" + guest.intEmployWorkValue + "次";

                    //只能是单个属性增加,提高2-10倍的攻击力或法力值,提高全队2-10倍血量值
                    guest.intHP = 500;
                    guest.intATK = 20;
                    guest.intMP = 40;
                    guest.floSpeed = 4;
                    break;
            }
            dicEmploying.Add(guest.intIndexID, work);
        }
    }

    void SendContent()
    {
        mgToInfo.intIndexGround = GetIndexGround;
        mgToInfo.intBuildID = GetBuildID;
        mgToInfo.intSaloonRank = intBuildRank;
        mgToInfo.intPersonCount = intSaloonPserion.Length;
        mgToInfo.intDailyIncome = intDailyIncome;
        mgToInfo.dicEmployee = dicEmployee;
        mgToInfo.dicEmploying = dicEmploying;
        mgToInfo.intEmployPrice = intEmployPrice;
        mgToInfo.booEmploying = booEmploying;
        mgToInfo.strContentTitle = strContentTitle;
        mgToInfo.strContentNotice = strContentNotice;
        mgToInfo.strContentEmploying = strContentEmploying;

        ManagerView.Instance.Show(EnumView.ViewHouse);
        ManagerView.Instance.SetData(EnumView.ViewHouse, mgToInfo);
    }

    /// <summary>
    /// 佣金工人 解释
    /// </summary>
    public class EmployingWork
    {
        //自身优势解释
        public string strContent;
        //工作时长
        public string strTime;
        public PropertiesEmployee employee;
    }
}
