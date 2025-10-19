using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMGToViewInfoSaloon : ViewBase.Message
{
    public bool booEmploying;
    public int intIndexGround;
    public int intBuildID;
    public int intSaloonRank;
    public int intPersonCount;
    public int intDailyIncome;
    public int intEmployPrice;
    public string strContentTitle;
    public string strContentNotice;
    public string strContentEmploying;
    //酒吧员工
    public Dictionary<int, PropertiesEmployee> dicEmployee;
    //雇佣公告人员
    public Dictionary<int, BuildSaloon.EmployingWork> dicEmploying;
}
