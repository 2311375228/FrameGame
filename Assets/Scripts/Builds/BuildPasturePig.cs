using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPasturePig : BuildBasePasture
{
    public override void OnStart()
    {
        base.OnStart();

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Versatility, "增加产量");
    }

    //public override void PlayAudio()
    //{
    //    ManagerValue.actionAudio(EnumAudio.Pig);
    //}
}
