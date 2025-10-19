using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBuildToViewPasture : EventBuildToViewBase
{
    public ViewBuild_Base.BuildTipsState buildState;
    public int intResidueTime;
    public int intIndexProduct;
    public int intProductCount;
    public int intIndexProductExpend;
    public int intProductExpendNum;
    public int[] intCompoundIDs;
    public int[] intPorductIDs;
    public int[] intProductIDExpends;
    public int[] intProductExpendNums;
    public int[] intEmployeeSizes;
    public Dictionary<EnumEmployeeProperties, string> dicPropertiesInfo;
    public EnumEmployeeProperties[] enumEmployeeProperties;

}
