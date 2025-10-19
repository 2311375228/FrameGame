  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 蔬菜农场
/// </summary>
public class BuildFarmVegetable : BuildBaseFarm
{
    public GameObject[] goVegetables;
    public override void OnStart()
    {
        base.OnStart();

        goVegetables[0].SetActive(true);
        goVegetables[1].SetActive(false);
        goVegetables[2].SetActive(false);
        goVegetables[3].SetActive(false);

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Agility, "增加产量");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Stamina, "增加产量");
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        base.UpdateDate(mgData);

        if (floResidueDay > itemCompound.intRipeDay * 0.8f)
        {
            goVegetables[0].SetActive(true);
            goVegetables[1].SetActive(false);
            goVegetables[2].SetActive(false);
            goVegetables[3].SetActive(false);
        }
        else if (floResidueDay > itemCompound.intRipeDay * 0.5f && floResidueDay < itemCompound.intRipeDay * 0.8f)
        {
            goVegetables[0].SetActive(false);
            goVegetables[1].SetActive(true);
            goVegetables[2].SetActive(false);
            goVegetables[3].SetActive(false);
        }
        else if (floResidueDay < itemCompound.intRipeDay * 0.5f && floResidueDay > itemCompound.intRipeDay * 0.3f)
        {
            goVegetables[0].SetActive(false);
            goVegetables[1].SetActive(false);
            goVegetables[2].SetActive(true);
            goVegetables[3].SetActive(false);
        }
        else if (floResidueDay < itemCompound.intRipeDay * 0.3f)
        {
            goVegetables[0].SetActive(false);
            goVegetables[1].SetActive(false);
            goVegetables[2].SetActive(false);
            goVegetables[3].SetActive(true);
        }

    }
}