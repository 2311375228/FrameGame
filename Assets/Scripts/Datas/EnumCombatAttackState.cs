using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻击步骤
/// </summary>
public enum EnumCombatAttackState
{
    None,
    /// <summary>
    /// 获取攻击者与被攻击者
    /// </summary>
    Ready,
    /// <summary>
    /// 前进
    /// </summary>
    ForwardMove,
    /// <summary>
    /// 攻击
    /// </summary>
    AttackAction,
    /// <summary>
    /// 再次攻击的后退
    /// </summary>
    BackMoveAgain,
    /// <summary>
    /// 再一次攻击的准备准备
    /// </summary>
    ReadyAgain,
    /// <summary>
    /// 返回
    /// </summary>
    BackMove,
    /// <summary>
    /// 结算
    /// </summary>
    Over,
}
