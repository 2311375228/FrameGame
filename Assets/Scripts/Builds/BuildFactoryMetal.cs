using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 炼金工厂
/// </summary>
public class BuildFactoryMetal : BuildBasePasture
{
    //体力增强药剂=5晶石+3蘑菇
    //力量增强药=5晶石+3牛奶
    //速度药剂=5晶石+3鸡蛋
    //铁块=20物质转换液体+2鸡蛋
    //铜块=20物质转换液体+铁块
    //银块=20物质转换液体+铜块
    //金块=20物质转换液体+银块

    public Animator anim;

    public override void OnStart()
    {
        base.OnStart();

        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Strengt, "缩短生产时间时间");
        dicEmployeePropertiesInfo.Add(EnumEmployeeProperties.Versatility, "增加产量");
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        CheckEmployeeState();
        //检查库存
        if (ProdectExpend())
        {
            //仓库库存充足
            mgGroundToViewBuildInfo.buildState = ViewBuild_Base.BuildTipsState.Smelt;
        }
        else
        {
            //仓库库存不足
            mgGroundToViewBuildInfo.buildState = ViewBuild_Base.BuildTipsState.NotExpend;
        }

        if (booBuildToView)
        {
            mgGroundToViewBuildInfo.intResidueTime = (int)floResidueDay;
            mgGroundToViewBuildInfo.intRipeDay = itemCompounds[intIndexProduct].intRipeDay;
            ManagerView.Instance.SetData(EnumView.ViewBuildMain, mgGroundToViewBuildInfo);
        }

    }
}
