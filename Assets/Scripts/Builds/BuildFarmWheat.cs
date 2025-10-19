using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 小麦农场
/// </summary>
public class BuildFarmWheat : BuildBaseFarm
{
    public GameObject[] goWheats;
    public override void OnStart()
    {
        base.OnStart();

        goWheats[0].SetActive(true);
        goWheats[1].SetActive(false);
        goWheats[2].SetActive(false);

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Strengt, "增加产量");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Versatility, "减少成熟时间");
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        base.UpdateDate(mgData);

        if (floResidueDay > itemCompound.intRipeDay * 0.6f)
        {
            goWheats[0].SetActive(true);
            goWheats[1].SetActive(false);
            goWheats[2].SetActive(false);
        }
        else if (floResidueDay > itemCompound.intRipeDay * 0.3f && floResidueDay < itemCompound.intRipeDay * 0.6f)
        {
            goWheats[0].SetActive(false);
            goWheats[1].SetActive(true);
            goWheats[2].SetActive(false);
        }
        else if (floResidueDay < itemCompound.intRipeDay * 0.3f)
        {
            goWheats[0].SetActive(false);
            goWheats[1].SetActive(true);
            goWheats[2].SetActive(true);
        }

    }
}
