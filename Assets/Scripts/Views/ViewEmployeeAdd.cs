using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewEmployeeAdd : ViewBase
{
    public Text textTitle;
    public View_PropertiesBase EmployeeProperties;

    public Button btnClose;

    public ScrollCycleColumn columnItem;

    List<PropertiesEmployee> listData = new List<PropertiesEmployee>();

    ViewMGToEmployeeAdd mgViewInfo;
    ViewMGEmployeeAddTo mgViewTo = new ViewMGEmployeeAddTo();

    protected override void Start()
    {
        base.Start();
        textTitle.gameObject.SetActive(false);

        btnClose.onClick.AddListener(
            () =>
            {
                ManagerValue.actionAudio(EnumAudio.Close);
                ManagerView.Instance.Hide(EnumView.ViewEmployeeAdd);
            });
    }

    public override void Show()
    {
        base.Show();
        SetDataTotal(0);
    }

    public override void SetData(Message message)
    {
        mgViewInfo = message as ViewMGToEmployeeAdd;

        textTitle.text = "为‘" + mgViewInfo.strBuildName + "'添加员工";
        for (int i = 0; i < EmployeeProperties.items.Length; i++)
        {
            //if (i < mgViewInfo.enumEmployeeProperties.Length)
            //{
            //    EmployeeProperties.items[i].gameObject.SetActive(true);
            //    EmployeeProperties.items[i].textValueMain.text = mgViewInfo.dicPropertiesInfo[mgViewInfo.enumEmployeeProperties[i]];
            //    EmployeeProperties.items[i].imageValueMain.sprite = ManagerResources.Instance.GetEmployeeProperties(mgViewInfo.enumEmployeeProperties[i]);
            //}
            //else
            //{
            //    EmployeeProperties.items[i].gameObject.SetActive(false);
            //}
            EmployeeProperties.items[i].gameObject.SetActive(false);
        }

        //筛选闲置的员工
        listData.Clear();
        Dictionary<int, PropertiesEmployee> dicTemp = UserValue.Instance.GetAllEmployee();
        foreach (PropertiesEmployee temp in dicTemp.Values)
        {
            if (temp.intIndexGroundWork == -1)
            {
                listData.Add(temp);
            }
        }
        SetDataTotal(listData.Count);
    }

    void SetDataTotal(int intCount)
    {

        RectTransform[] rectItems = columnItem.SetDataTotal(intCount);
        for (int i = 0; i < rectItems.Length; i++)
        {
            ViewEmployeeAdd_SubItem itemEmployeeAdd = rectItems[i].GetComponent<ViewEmployeeAdd_SubItem>();
            itemEmployeeAdd.btnEmployeeAdd.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Employ);
            itemEmployeeAdd.textEmployeeName.gameObject.SetActive(false);
            itemEmployeeAdd.numIndexItem = i;
            itemEmployeeAdd.numIndexData = i;
            itemEmployeeAdd.actionBase = ActionEmployeeAdd;
            itemEmployeeAdd.actionEmployeeSee = ActionEmployeeSee;
            RefreshData(itemEmployeeAdd, i, i);
        }
    }

    void RefreshData(ViewEmployeeAdd_SubItem itemTemp, int numIndexItem, int numIndexData)
    {
        if (numIndexData >= listData.Count)
        {
            itemTemp.gameObject.SetActive(false);
        }
        else
        {
            itemTemp.gameObject.SetActive(true);
            itemTemp.numIndexItem = numIndexItem;
            itemTemp.numIndexData = numIndexData;

            itemTemp.textEmployeeName.text = listData[numIndexData].strEmployeeName;
            itemTemp.textEmployeeRank.text = listData[numIndexData].intRank.ToString();
            itemTemp.textEmployeeType.text = listData[numIndexData].enumIdentification.ToString();
            itemTemp.textEmployeeType.transform.parent.gameObject.SetActive(false);
            itemTemp.imageHead.sprite = listData[numIndexData].spriteHead;

            for (int i = 0; i < itemTemp.employeeProperties.items.Length; i++)
            {
                //if (mgViewInfo != null && i < mgViewInfo.enumEmployeeProperties.Length)
                //{
                //    itemTemp.employeeProperties.items[i].gameObject.SetActive(true);
                //    itemTemp.employeeProperties.items[i].imageValueMain.sprite = ManagerResources.Instance.GetEmployeeProperties(mgViewInfo.enumEmployeeProperties[i]);
                //    itemTemp.employeeProperties.items[i].textValueMain.text = listData[numIndexData].dicEmployeeProperties[mgViewInfo.enumEmployeeProperties[i]].ToString();
                //}
                //else
                //{
                //    itemTemp.employeeProperties.items[i].gameObject.SetActive(false);
                //}
                itemTemp.employeeProperties.items[i].gameObject.SetActive(false);
            }
            itemTemp.btnEmployeeSee.gameObject.SetActive(false);
        }
    }

    void ActionEmployeeSee(int intIndexItem, int intIndexData)
    {

    }
    void ActionEmployeeAdd(int intIndexItem, int intIndexData)
    {
        ManagerValue.actionAudio(EnumAudio.Ground);
        ManagerView.Instance.Hide(EnumView.ViewEmployeeAdd);

        mgViewTo.intEmployeeID = listData[intIndexData].intIndexID;
        ManagerView.Instance.SetData(mgViewInfo.enumView, mgViewTo);
    }

}
