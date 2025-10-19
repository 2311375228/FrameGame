using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildFarmWood : BuildBaseFarm
{
    public override void OnStart()
    {
        base.OnStart();

        dicEmployeePropertiesInfo.Add( EnumEmployeeProperties.Strengt,"增加产量");
        dicEmployeePropertiesInfo.Add( EnumEmployeeProperties.Stamina,"缩短生产时间");
        dicEmployeePropertiesInfo.Add( EnumEmployeeProperties.Versatility,"增加产量");
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        base.UpdateDate(mgData);
    }
}
