using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 棉花农场
/// </summary>
public class BuildFarmCotton : BuildBaseFarm
{
    public GameObject[] goCottons;
    public override void OnStart()
    {
        base.OnStart();
        goCottons[0].SetActive(true);
        goCottons[1].SetActive(false);
        goCottons[2].SetActive(false);
        goCottons[3].SetActive(false);

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Agility, "缩短成熟时间");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Stamina, "增加产量");
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        base.UpdateDate(mgData);

        if (floResidueDay > itemCompound.intRipeDay * 0.8f)
        {
            goCottons[0].SetActive(true);
            goCottons[1].SetActive(false);
            goCottons[2].SetActive(false);
            goCottons[3].SetActive(false);
        }
        else if (floResidueDay > itemCompound.intRipeDay * 0.6f
            && floResidueDay < itemCompound.intRipeDay * 0.8f)
        {
            goCottons[0].SetActive(false);
            goCottons[1].SetActive(true);
            goCottons[2].SetActive(false);
            goCottons[3].SetActive(false);
        }
        else if (floResidueDay > itemCompound.intRipeDay * 0.3f
            && floResidueDay < itemCompound.intRipeDay * 0.6f)
        {
            goCottons[0].SetActive(false);
            goCottons[1].SetActive(false);
            goCottons[2].SetActive(true);
            goCottons[3].SetActive(false);
        }
        else if (floResidueDay < itemCompound.intRipeDay * 0.3f)
        {
            goCottons[0].SetActive(false);
            goCottons[1].SetActive(false);
            goCottons[2].SetActive(true);
            goCottons[3].SetActive(true);
        }

    }
}
