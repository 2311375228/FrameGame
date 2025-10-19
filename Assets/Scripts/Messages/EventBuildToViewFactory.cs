using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBuildToViewFactory : EventBuildToViewBase
{
    public int[] intCompoundIDs;//可生产产品ID

    public int[] intCompoundingIDs;//正在生产的产品
    public int[] intCompoundResidueTimes;//正在生产的产品剩余时间
    public int[] intCompoundingCoins;//正在生产的产品回收金币

    public int[] intEmployeeSizes;
    public Dictionary<EnumEmployeeProperties, string> dicPropertiesInfo;
    public EnumEmployeeProperties[] enumEmployeeProperties;

}
