using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumGroudState
{
    None,
    /// <summary>
    /// 无效土地
    /// </summary>
    Nullity,
    /// <summary>
    /// 不能建筑的土地
    /// </summary>
    Obstacle,
    /// <summary>
    /// 有障碍物体
    /// </summary>
    Hinder,
    /// <summary>
    /// 未购买
    /// </summary>
    Unpurchased,
    /// <summary>
    /// 已经购买
    /// </summary>
    Purchased,
    /// <summary>
    /// 没有建筑
    /// </summary>
    BuildingNon,
    /// <summary>
    /// 已经有建筑
    /// </summary>
    BuildingPruchased,
    /// <summary>
    /// 工地
    /// </summary>
    BuildConstruction,
}
