using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMGToViewInfoTower : ViewBase.Message
{
    public int intBuildID;
    public int intTowerRank;
    public int intEmployeeMaxNum;
    public int intIndexGround;
    public int intMonth;
    public int intDay;
    public string strBuildName;

    public Dictionary<int, PropertiesEmployee> dicEmployee;
}
