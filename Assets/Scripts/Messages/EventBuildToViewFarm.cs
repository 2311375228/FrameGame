using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 推送消息给ViewBuildFarmInfo
/// </summary>
public class EventBuildToViewFarm : EventBuildToViewBase
{
    public ViewBuild_Base.BuildTipsState buildState;
    public int intProductID;
    public int intProductCount;//产量
    public int intProductRipeDay;//成熟所需时间
    public int intCompoundID;
    public int intResidueTime;
    public int[] intEmployeeSizes;
    public Dictionary<EnumEmployeeProperties, string> dicPropertiesInfo;
    public EnumEmployeeProperties[] enumEmployeeProperties;
}
