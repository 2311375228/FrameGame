using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewTower : ViewBase
{
    public Button btnHelp;
    public Button btnClose;

    public Text textTowerName;
    public Text textTowerRank;
    public Text textEmployeeNum;

    public Text textEmployeeName;
    public Text textEmployeeRank;

    public Text textStrengt;
    public Text textAgility;
    public Text textIntellect;
    public Text textStamina;
    public Text textVersatility;
    public Text textHP;
    public Text textMP;

    public Image imageHead;
    public Image imageProperties;
    public Button btnLeavefor;//前往土地

    public Text textEmployeeGround;

    public ScrollCycleColumn columnItem;

    List<PropertiesEmployee> listData = new List<PropertiesEmployee>();
    List<ViewTower_subItem> listItemSub = new List<ViewTower_subItem>();


    protected override void Start()
    {
        base.Start();
        btnHelp.onClick.AddListener(() =>
        {
        });
        //btnClose.onClick.AddListener(() => { ManagerView.Instance.Hide(EnumView.ViewTower); });
    }

    public override void Show()
    {
        base.Show();
    }

    public override void SetData(Message message)
    {
        ViewMGToViewInfoTower mg = message as ViewMGToViewInfoTower;
        if (mg != null)
        {
            listData.Clear();
            foreach (PropertiesEmployee temp in mg.dicEmployee.Values)
            {
                listData.Add(temp);
            }
            columnItem.SetDataTotal(listData.Count);
            ShowEmployeeInfo(listData[0]);
        }

    }

    void RefreshData(int numIndexItem, int numIndexData)
    {
        ViewTower_subItem itemTemp = listItemSub[numIndexItem];
        if (numIndexData >= listData.Count)
        {
            itemTemp.gameObject.SetActive(false);
        }
        else
        {
            itemTemp.gameObject.SetActive(true);
            itemTemp.numIndexItem = numIndexItem;
            itemTemp.numIndexData = numIndexData;

            itemTemp.textEmployeeRank.text = listData[numIndexData].intRank.ToString();
            itemTemp.textEmployeeName.text = listData[numIndexData].strEmployeeName;
            string strBuildName = "";
            int indexWork = listData[numIndexData].intIndexGroundWork;
            if (listData[numIndexData].enumLocation == EnumEmployeeLocation.Ground)
            {
                if (indexWork != -1)
                {
                    strBuildName = ManagerBuild.Instance.GetBuildName(UserValue.Instance.GetDicGround[indexWork].intBuildID);
                }
                else
                {
                    strBuildName = ManagerBuild.Instance.GetBuildName(UserValue.Instance.GetDicGround[listData[numIndexData].intIndexGround].intBuildID);
                }
            }
            else if (listData[numIndexData].enumLocation == EnumEmployeeLocation.Risk)
            {
                strBuildName = "冒险岛";
            }
            itemTemp.textEmployeeGround.text = strBuildName;
            itemTemp.imageHead.sprite = listData[numIndexData].spriteHead;
            itemTemp.imageProperties.sprite = null;
        }
    }
    void ActionEventItemSub(int intIndexItem, int intIndexData)
    {
        ShowEmployeeInfo(listData[intIndexData]);
    }
    void ShowEmployeeInfo(PropertiesEmployee employee)
    {
        textEmployeeRank.text = employee.intRank.ToString();
        textEmployeeName.text = employee.strEmployeeName;
        textStrengt.text = employee.dicEmployeeProperties[EnumEmployeeProperties.Strengt].ToString();
        textAgility.text = employee.dicEmployeeProperties[EnumEmployeeProperties.Agility].ToString();
        textIntellect.text = employee.dicEmployeeProperties[EnumEmployeeProperties.Intellect].ToString();
        textStamina.text = employee.dicEmployeeProperties[EnumEmployeeProperties.Stamina].ToString();
        textVersatility.text = employee.dicEmployeeProperties[EnumEmployeeProperties.Versatility].ToString();
        textHP.text = employee.intHP.ToString();
        textMP.text = employee.intMP.ToString();
        imageHead.sprite = employee.spriteHead;
    }
}
