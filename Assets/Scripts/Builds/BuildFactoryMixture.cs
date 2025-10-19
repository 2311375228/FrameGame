using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 合剂工厂
/// </summary>
public class BuildFactoryMixture : BuildBaseFactory
{
    public override void OnStart()
    {
        base.OnStart();

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Intellect, "增加产量");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Versatility, "增加产量");
    }
}
