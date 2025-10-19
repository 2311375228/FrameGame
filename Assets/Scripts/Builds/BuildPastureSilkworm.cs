using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPastureSilkworm : BuildBasePasture
{
    public override void OnStart()
    {
        base.OnStart();

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Intellect, "增加产量");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Stamina, "增加产量");
    }
}
