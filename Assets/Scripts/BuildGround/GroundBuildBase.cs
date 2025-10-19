using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 处理3D物品刷新，性能消耗
/// </summary>
public class GroundBuildBase
{
    public int intIndexGround;
    public EnumBuildType buildType;

    public virtual void Date(int intYear, int intMonty, int intDay) { }

    public enum EnumBuildType
    {
        None,
        Farm,
        Pasture,
        Factory,
    }
}
