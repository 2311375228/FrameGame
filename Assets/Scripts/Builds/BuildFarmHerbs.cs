using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 药草农场
/// </summary>
public class BuildFarmHerbs : BuildBaseFarm
{
    public GameObject[] goHerbs;
    public override void OnStart()
    {
        base.OnStart();

        goHerbs[0].SetActive(true);
        goHerbs[1].SetActive(false);

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Intellect, "增加产量");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Stamina, "增加产量");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Versatility, "增加产量");
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        base.UpdateDate(mgData);

        if (floResidueDay > itemCompound.intRipeDay * 0.5f)
        {
            goHerbs[0].SetActive(true);
            goHerbs[1].SetActive(false);
        }
        else
        {
            goHerbs[0].SetActive(false);
            goHerbs[1].SetActive(true);
        }

    }
}
