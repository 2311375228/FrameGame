using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 苹果农场
/// </summary>
public class BuildFarmApple : BuildBaseFarm
{
    int intInfancyDay = 365;
    int intInfancyDayInit = 365;
    public GameObject[] goApples;
    public override void OnStart()
    {
        base.OnStart();
        floResidueDay = 0;
        goApples[0].SetActive(true);
        goApples[1].SetActive(false);
        goApples[2].SetActive(false);
        goApples[3].SetActive(false);
        goApples[4].SetActive(false);

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Agility, "缩短收获时间");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Stamina, "缩短收获时间");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Versatility, "增加产量");
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        if (intInfancyDay <= 0)
        {
            if (floResidueDay > itemCompound.intRipeDay * 0.5f)
            {
                goApples[0].SetActive(false);
                goApples[1].SetActive(false);
                goApples[2].SetActive(false);
                goApples[3].SetActive(true);
                goApples[4].SetActive(true);
            }
            else
            {
                goApples[0].SetActive(false);
                goApples[1].SetActive(false);
                goApples[2].SetActive(false);
                goApples[3].SetActive(true);
                goApples[4].SetActive(false);
            }

            floResidueDay -= 1;
            if (floResidueDay <= 0)
            {
                floResidueDay = itemCompound.intRipeDay;

                //if (ManagerValue.booOpenBackpack)
                //{
                //    ManagerValue.actionBackpackAdd(proProduct.numProductID, itemCompound.intRipeDay);
                //}
                //else
                //{
                //    UserValue.Instance.ProductNumAdd(proProduct.numProductID, itemCompound.intRipeDay);
                //}
            }
        }
        else
        {
            intInfancyDay -= 1;
            if (intInfancyDay > intInfancyDayInit * 0.8f)
            {
                goApples[0].SetActive(true);
                goApples[1].SetActive(false);
                goApples[2].SetActive(false);
                goApples[3].SetActive(false);
                goApples[4].SetActive(false);
            }
            else if (intInfancyDay < intInfancyDayInit * 0.8f
                && intInfancyDay > intInfancyDayInit * 0.5f)
            {
                goApples[0].SetActive(false);
                goApples[1].SetActive(true);
                goApples[2].SetActive(false);
                goApples[3].SetActive(false);
                goApples[4].SetActive(false);
            }
            else if (intInfancyDay < intInfancyDayInit * 0.5f
                && intInfancyDay > intInfancyDayInit * 0.3f)
            {
                goApples[0].SetActive(false);
                goApples[1].SetActive(false);
                goApples[2].SetActive(true);
                goApples[3].SetActive(false);
                goApples[4].SetActive(false);
            }
            else if (intInfancyDay < intInfancyDayInit * 0.3f)
            {
                goApples[0].SetActive(false);
                goApples[1].SetActive(false);
                goApples[2].SetActive(false);
                goApples[3].SetActive(true);
                goApples[4].SetActive(true);
            }
        }

        if (booBuildToView)
        {
            if (intInfancyDay >= 0)
            {
                mgGroundToViewBuildInfo.intResidueTime = intInfancyDay;
            }
            else
            {
                mgGroundToViewBuildInfo.intResidueTime = (int)floResidueDay;
            }
        }

        base.UpdateDate(mgData);
    }
}
