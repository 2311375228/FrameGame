using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 仓库
/// </summary>
public class BuildWarehouse : BuildBase
{
    public override void OnStart()
    {
        base.OnStart();
    }

    ViewBuild_Base.WarehouseToView messageWarehouse = new ViewBuild_Base.WarehouseToView();
    public override void ShowViewBuildInfo()
    {
        messageWarehouse.enumKeyUp = ViewBuild_Base.EnumViewUp.ViewBuild_Warehouse;

        ManagerView.Instance.Show(EnumView.ViewBuildMain);
        ManagerValue.actionViewBuildMain(messageWarehouse);
    }
}
