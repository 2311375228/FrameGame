using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_BuildConstruction : ViewBuild_Base
{
    public Text textBuildName;
    public Text textBuildPriceTag;
    public Text textBuildPrice;
    public Text textScrollbarFinishTime;
    public Text textFinishTime;
    public Image imageBuild;
    public Scrollbar scrollbar;
    public Button btnCancel;

    float floTotalBuildTime;
    ConstructionCancelToBuild messageCancel = new ConstructionCancelToBuild();
    protected override void Start()
    {
        btnCancel.onClick.AddListener(() =>
        {
            SendToGround(messageCancel);
        });
    }

    public override void Show()
    {
        base.Show();
        textBuildPriceTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.ConstructionCost);
        btnCancel.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.CancelConstruction);
    }

    public override void SetDate(ViewGroundToBuildMainDateBase message)
    {
        ContructionToView_Date messageDate = message as ContructionToView_Date;
        if (messageDate != null)
        {
            textFinishTime.text = ((int)messageDate.floResidueDay).ToString();
            textScrollbarFinishTime.text = (int)messageDate.floResidueDay + "/" + (int)floTotalBuildTime;
            scrollbar.size = messageDate.floResidueDay / floTotalBuildTime;
        }
    }

    public override void BuildMessage(EventBuildToViewBase message)
    {
        ConstructionToView messageView = message as ConstructionToView;
        if (messageView != null)
        {
            JsonValue.DataTableBuildingItem itemBuild = ManagerBuild.Instance.GetBuildItem(messageView.intTargetBuildID);
            textBuildName.text = ManagerBuild.Instance.GetBuildName(messageView.intTargetBuildID);
            textBuildPrice.text = itemBuild.numPrice.ToString("N0");
            floTotalBuildTime = messageView.floTotalTime;
            Sprite[] s = ManagerResources.Instance.GetBuildSprite(itemBuild.strModelName);
            imageBuild.sprite = s[s.Length - 1];
        }
    }
}
