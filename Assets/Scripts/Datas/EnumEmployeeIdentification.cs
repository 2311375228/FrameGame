using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 以后增加的类型 临时工,需要临时工建筑,
/// 按任务收较高费用,为农场等建筑减少10倍的时间,可设定持续天数
/// 战斗时,只能参与一次副本
/// </summary>
public enum EnumEmployeeIdentification
{
    None = 0,
    /// <summary>
    /// 农民
    /// </summary>
    Farmer = 1,
    /// <summary>
    /// 租客 租期时间限制
    /// </summary>
    Renter = 2,
    /// <summary>
    /// 酒鬼
    /// 工作时间限制:为农场工作多少天;
    /// 战斗次数限制:根据副本开启状态改变悬赏状态,副本次数
    /// </summary>
    Toper = 3,
    /// <summary>
    /// 兰博,猛男,野外找到的
    /// </summary>
    Rambo = 4,
    /// <summary>
    /// 员工,招募来的
    /// </summary>
    Employee = 5,
}
