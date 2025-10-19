using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildFarmMeadow : BuildBaseFarm
{
    public GameObject[] goMeadows;
    public override void OnStart()
    {
        base.OnStart();

        goMeadows[0].SetActive(true);
        goMeadows[1].SetActive(false);
        goMeadows[2].SetActive(false);

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Strengt, "增加产量");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Agility, "缩短生产时间");
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        base.UpdateDate(mgData);
        if (floResidueDay > itemCompound.intRipeDay * 0.6f)
        {
            goMeadows[0].SetActive(true);
            goMeadows[1].SetActive(false);
            goMeadows[2].SetActive(false);
        }
        else if (floResidueDay > itemCompound.intRipeDay * 0.3f && floResidueDay < itemCompound.intRipeDay * 0.6f)
        {
            goMeadows[0].SetActive(true);
            goMeadows[1].SetActive(true);
            goMeadows[2].SetActive(false);
        }
        else if (floResidueDay < itemCompound.intRipeDay * 0.3f)
        {
            goMeadows[0].SetActive(true);
            goMeadows[1].SetActive(true);
            goMeadows[2].SetActive(true);
        }
    }

}
