using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewProductionSee_MenuItem : ColumnItemBase
{
    public Text textBuildName;
    public Text textEmployeeCount;
    public Text textPage;
    public Button btnPageLeft;
    public Button btnPageRight;
    public View_PropertiesBase employType;
    public View_PropertiesBase item;
    public Button[] btnChecks;

    public System.Action<int, int> actionPageLeft;
    public System.Action<int, int> actionPageRight;

    public System.Action<int, int> actionProductSee_1;
    public System.Action<int, int> actionProductSee_2;
    public System.Action<int, int> actionProductSee_3;
    public System.Action<int, int> actionProductSee_4;
    // Start is called before the first frame update
    void Start()
    {
        btnPageLeft.onClick.AddListener(() => { actionPageLeft(numIndexItem, numIndexData); });
        btnPageRight.onClick.AddListener(() => { actionPageRight(numIndexItem, numIndexData); });

        btnChecks[0].onClick.AddListener(() => { actionProductSee_1(numIndexItem, numIndexData); });
        btnChecks[1].onClick.AddListener(() => { actionProductSee_2(numIndexItem, numIndexData); });
        btnChecks[2].onClick.AddListener(() => { actionProductSee_3(numIndexItem, numIndexData); });
        btnChecks[3].onClick.AddListener(() => { actionProductSee_4(numIndexItem, numIndexData); });
    }
}
