using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMGToViewInfoRental : ViewBase.Message
{
    public int intIndexGround;
    public int intBuildID;
    public int intRentalRank;
    public int intPersonCount;
    public int intMonthlyIncome;
    public int intPersonPrice;

    public string strRentalContent;
    public string strRentalContentNotice;

    public Dictionary<int, PropertiesEmployee> dicEmployee;
}
