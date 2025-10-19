using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色战斗中的状态
/// </summary>
public enum EnumCombatRoleState
{
    None,
    Idle,//闲置,休闲
    Walk,//移动
    Attack,//攻击
    death,//死亡
}
