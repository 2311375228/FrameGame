using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 农作物工厂
/// </summary>
public class BuildFactoryFarm : BuildBaseFactory
{
    public override void OnStart()
    {
        base.OnStart();

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Strengt, "增加产量");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Stamina, "缩短生产时间");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Versatility, "缩短生产时间");
    }
}
