using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 为了方便写法,避免混淆,方便调用,所以用消息机制
/// </summary>
public enum EnumMessage
{
    None,
    Update_Coin,
    Update_Ground,
    Update_Date,
    Update_Task,

    Hide_GroundChoice,

    Mail,//邮件
    DemolishBuild,//拆除建筑
    StopCombat,//停止战斗
}
