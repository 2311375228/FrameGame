using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBuildSell : BuildBase
{
    ViewMGToViewNPCSell mgToNPCSell = new ViewMGToViewNPCSell();

    public override void OnStart()
    {
        base.OnStart();
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        if (booBuildToView)
        {
            ManagerView.Instance.SetData(EnumView.ViewNPCSell, mgToNPCSell);
        }
    }

    public override void ShowViewBuildInfo()
    {
        mgToNPCSell.strTitle = ManagerLanguage.Instance.GetWord(EnumLanguageWords.PurchasingMerchant);//"收购商人";
        mgToNPCSell.intGround = GetIndexGround;

        booBuildToView = true;
        ManagerView.Instance.Show(EnumView.ViewNPCSell);
        ManagerView.Instance.SetData(EnumView.ViewNPCSell, mgToNPCSell);
    }

    public override void MGViewBuildInfo(MGViewToBuildBase toGround)
    {
        MGViewToBuildNPCSell mg = toGround as MGViewToBuildNPCSell;
        if (mg != null)
        {
            booBuildToView = mg.booShow;
        }
    }
}
