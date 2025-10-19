using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPastureChook : BuildBasePasture
{
    public override void OnStart()
    {
        base.OnStart();

        dicEmployeePropertiesInfo.Add( EnumEmployeeProperties.Stamina,"缩短生产时间");
    }

    //public override void PlayAudio()
    //{
    //    ManagerValue.actionAudio(EnumAudio.Chicken);
    //}
}
