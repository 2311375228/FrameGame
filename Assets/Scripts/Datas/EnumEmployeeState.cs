using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 员工状态
/// </summary>
public enum EnumEmployeeState
{
    None,
    /// <summary>
    /// 雇佣
    /// </summary>
    Employ,
    /// <summary>
    /// 有效工作时长
    /// </summary>
    EmployTime,
    /// <summary>
    /// 雇佣战斗次数
    /// </summary>
    EmployCombat,
    /// <summary>
    /// 没有雇佣
    /// </summary>
    NoHire,
    /// <summary>
    /// 已经删除
    /// </summary>
    Delete,
}
