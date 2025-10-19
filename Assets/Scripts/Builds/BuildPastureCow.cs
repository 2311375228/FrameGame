using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPastureCow : BuildBasePasture
{
    public override void OnStart()
    {
        base.OnStart();

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Intellect, "减少生产时间");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Stamina, "增加产量");
    }

    //public override void PlayAudio()
    //{
    //    ManagerValue.actionAudio(EnumAudio.Cow);
    //}
}
