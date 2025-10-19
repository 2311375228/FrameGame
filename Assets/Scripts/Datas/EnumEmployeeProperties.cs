using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单个属性最大是10
/// 属性影响 农场产出
/// 属性影响 关卡任务,CombatRole.EnumCombatType 的值增加与减少 单属性x2,总量的60%
/// 
/// </summary>
public enum EnumEmployeeProperties
{
    None,
    /// <summary>
    /// 力量
    /// </summary>
    Strengt,
    /// <summary>
    /// 敏捷
    /// </summary>
    Agility,
    /// <summary>
    /// 智力
    /// </summary>
    Intellect,
    /// <summary>
    /// 耐力
    /// </summary>
    Stamina,
    /// <summary>
    /// 全能
    /// </summary>
    Versatility,
}
