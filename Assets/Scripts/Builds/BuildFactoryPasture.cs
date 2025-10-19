using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 畜牧业工厂
/// </summary>
public class BuildFactoryPasture : BuildBaseFactory
{
    public override void OnStart()
    {
        base.OnStart();

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Strengt, "增加产量");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Intellect, "缩短生产时间");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Versatility, "缩短生产时间");
    }
}
