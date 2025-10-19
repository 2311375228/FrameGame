using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPastureSheep : BuildBasePasture
{
    public override void OnStart()
    {
        base.OnStart();

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Strengt, "缩短生产时间");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Agility, "缩短生产时间");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Intellect, "增加产量");
    }

    //public override void PlayAudio()
    //{
    //    ManagerValue.actionAudio(EnumAudio.Sheep);
    //}
}
