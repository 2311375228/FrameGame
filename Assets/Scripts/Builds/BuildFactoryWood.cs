using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 木材加工厂
/// </summary>
public class BuildFactoryWood :BuildBaseFactory
{
    //木板=1木头

    public override void OnStart()
    {
        base.OnStart();
        
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Strengt, "减少生产时间");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Agility, "减少生产时间");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Stamina, "减少生产时间");
    }
}
