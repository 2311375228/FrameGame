using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewHouse : ViewBase
{
    public ViewHouse_Menu menu;
    public ViewHouse_Employee employeeShow;
    public ViewHouse_Tower tower;
    public ViewHouse_Saloon saloon;
    public ViewHouse_Rental rental;

    HouseType houseType = HouseType.None;
    int intIndexGround;
    RectTransform[] rectItems = new RectTransform[] { };

    MGViewToBuildTower mgToBuildTower = new MGViewToBuildTower();
    MGViewToBuildRental mgToBuildRental = new MGViewToBuildRental();
    MGViewToBuildSaloon mgToBuildSaloon = new MGViewToBuildSaloon();

    ViewHintBar.MessageHintBar barMessage = new ViewHintBar.MessageHintBar();
    protected override void Start()
    {
        menu.btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            ManagerView.Instance.Hide(EnumView.ViewHouse);
            switch (houseType)
            {
                case HouseType.Tower:
                    break;
                case HouseType.Rental:
                    SendToBuildRental(false, -1);
                    break;
                case HouseType.Saloon:
                    SendToBuildSaloon(false, false, -1, -1);
                    break;
            }
        });

        menu.btnDeleteTower.onClick.AddListener(() => { DemolishBuildHouse(); });
        menu.btnDeleteRental.onClick.AddListener(() => { DemolishBuildHouse(); });
        menu.btnDeleteSaloon.onClick.AddListener(() => { DemolishBuildHouse(); });
        //设置员工能力
        menu.btnEmployeePower.onClick.AddListener(() =>
        {
            employeeShow.gameObject.SetActive(false);
            tower.gameObject.SetActive(true);
            rental.gameObject.SetActive(false);
            saloon.gameObject.SetActive(false);
        });
        //雇佣公告
        menu.btnEmployNotice.onClick.AddListener(() =>
        {
            employeeShow.gameObject.SetActive(false);
            tower.gameObject.SetActive(false);
            rental.gameObject.SetActive(false);
            saloon.gameObject.SetActive(true);
        });
        //房租设定
        menu.btnRentalSet.onClick.AddListener(() =>
        {
            employeeShow.gameObject.SetActive(false);
            tower.gameObject.SetActive(false);
            rental.gameObject.SetActive(true);
            saloon.gameObject.SetActive(false);
        });
        //查看
        menu.btnHouseSee.onClick.AddListener(() =>
        {
            SetDataTotal(employeeShow.listData.Count);
            employeeShow.gameObject.SetActive(true);
            tower.gameObject.SetActive(false);
            rental.gameObject.SetActive(false);
            saloon.gameObject.SetActive(false);
        });
        //升级建筑
        menu.btnHouseUpRank.onClick.AddListener(() =>
        {
        });
        //全部
        employeeShow.btnAll.onClick.AddListener(() =>
        {
            employeeShow.listTemp.Clear();
            for (int i = 0; i < employeeShow.listData.Count; i++)
            {
                employeeShow.listTemp.Add(employeeShow.listData[i]);
            }
            SetDataTotal(employeeShow.listTemp.Count);
        });
        //已经雇佣
        employeeShow.btnEmploy.onClick.AddListener(() =>
        {
        });
        //没有雇佣
        employeeShow.btnEmployNone.onClick.AddListener(() =>
        {
        });
        //工作中
        employeeShow.btnEmployWork.onClick.AddListener(() =>
        {
            employeeShow.listTemp.Clear();
            for (int i = 0; i < employeeShow.listData.Count; i++)
            {
                if (employeeShow.listData[i].enumState == EnumEmployeeState.Employ)
                {
                    employeeShow.listTemp.Add(employeeShow.listData[i]);
                }
            }
            SetDataTotal(employeeShow.listTemp.Count);
        });
        //闲置中
        employeeShow.btnEmployeeIdle.onClick.AddListener(() =>
        {
            employeeShow.listTemp.Clear();
            for (int i = 0; i < employeeShow.listData.Count; i++)
            {
                if (employeeShow.listData[i].enumState == EnumEmployeeState.NoHire)
                {
                    employeeShow.listTemp.Add(employeeShow.listData[i]);
                }
            }
            SetDataTotal(employeeShow.listTemp.Count);
        });
        //关闭
        employeeShow.btnClose.onClick.AddListener(() => { employeeShow.gameObject.SetActive(false); });

        rental.SendToBuildRental = SendToBuildRental;
        saloon.SendToBuildSaloon = SendToBuildSaloon;

        menu.goReatal.transform.position = menu.goTower.transform.position;
        menu.goSloon.transform.position = menu.goTower.transform.position;
        Vector3 vecTemp = rental.transform.position;
        vecTemp.y = employeeShow.transform.position.y;
        tower.transform.position = vecTemp;
        rental.transform.position = vecTemp;
        saloon.transform.position = vecTemp;
    }

    public override void Show()
    {
        if (!gameObject.activeSelf)
        {
            employeeShow.gameObject.SetActive(false);
            tower.gameObject.SetActive(false);
            rental.gameObject.SetActive(false);
            saloon.gameObject.SetActive(false);
        }

        base.Show();

        //if (employeeShow.listItem.Count == 0)
        //{
        //    

        //    tower.Show();

        //    saloon.Show();
        //}

        menu.textHousePersonTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.NumberOfPeople) + ":";
        menu.btnDeleteTower.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Demolish);
    }

    public override void SetData(Message message)
    {
        ViewMGToViewInfoTower mgTower = message as ViewMGToViewInfoTower;
        Dictionary<int, PropertiesEmployee> dicTempEmployee = null;
        if (mgTower != null)
        {
            houseType = HouseType.Tower;
            intIndexGround = mgTower.intIndexGround;
            employeeShow.gameObject.SetActive(false);
            tower.gameObject.SetActive(false);
            rental.gameObject.SetActive(false);
            saloon.gameObject.SetActive(false);
            menu.goTower.SetActive(true);
            menu.goReatal.SetActive(false);
            menu.goSloon.SetActive(false);

            menu.textTitle.text = mgTower.strBuildName;
            menu.textHouseRankTag.text = mgTower.strBuildName + "等级:";
            menu.textHouseRank.text = mgTower.intTowerRank.ToString();
            menu.textHouseRankTag.gameObject.SetActive(false);
            menu.textHouseRank.gameObject.SetActive(false);
            menu.textHousePerson.text = mgTower.dicEmployee.Count.ToString();
            dicTempEmployee = mgTower.dicEmployee;
            tower.intHouseRank = mgTower.intTowerRank;

            employeeShow.btnAll.gameObject.SetActive(false);
            employeeShow.btnEmploy.gameObject.SetActive(false);
            employeeShow.btnEmployNone.gameObject.SetActive(false);
            employeeShow.btnEmployWork.gameObject.SetActive(false);
            employeeShow.btnEmployeeIdle.gameObject.SetActive(false);
            for (int i = 0; i < rectItems.Length; i++)
            {
                ViewHouse_EmployeeItem itemEmployee = rectItems[i].GetComponent<ViewHouse_EmployeeItem>();
                itemEmployee.goEmployeeState.SetActive(true);
                itemEmployee.goMonthly.SetActive(false);
                itemEmployee.btnTakeOff.gameObject.SetActive(false);
                itemEmployee.btnEmploy.gameObject.SetActive(false);
            }

            //赶时间,放弃的东西
            menu.btnHouseSee.gameObject.SetActive(false);
            menu.btnHouseUpRank.gameObject.SetActive(false);
            menu.textTowerInfo.transform.parent.gameObject.SetActive(false);
            menu.btnEmployeePower.gameObject.SetActive(false);
        }
        ViewMGToViewInfoRental mgRental = message as ViewMGToViewInfoRental;
        if (mgRental != null)
        {
            if (houseType != HouseType.Rental || intIndexGround != mgRental.intIndexGround)
            {
                employeeShow.gameObject.SetActive(false);
                tower.gameObject.SetActive(false);
                rental.gameObject.SetActive(false);
                saloon.gameObject.SetActive(false);
                menu.goTower.SetActive(false);
                menu.goReatal.SetActive(true);
                menu.goSloon.SetActive(false);
            }
            houseType = HouseType.Rental;
            intIndexGround = mgRental.intIndexGround;

            menu.textTitle.text = ManagerBuild.Instance.GetBuildName(mgRental.intBuildID);
            menu.textHouseRankTag.text = menu.textTitle.text + "等级:";
            menu.textHouseRank.text = mgRental.intRentalRank.ToString();
            menu.textHousePerson.text = mgRental.dicEmployee.Count + "/" + mgRental.intPersonCount;
            dicTempEmployee = mgRental.dicEmployee;
            rental.intIncome = mgRental.intMonthlyIncome;
            rental.intPrice = mgRental.intPersonPrice;
            rental.intPriceChange = mgRental.intPersonPrice;
            rental.intHouseRank = mgRental.intRentalRank;
            rental.inputRent.text = mgRental.intPersonPrice.ToString();
            rental.textContent.text = mgRental.strRentalContent;
            rental.textContentInfo.text = mgRental.strRentalContentNotice;
            menu.textRentalMoney.text = mgRental.intMonthlyIncome.ToString();

            employeeShow.btnAll.gameObject.SetActive(true);
            employeeShow.btnEmploy.gameObject.SetActive(true);
            employeeShow.btnEmployNone.gameObject.SetActive(true);
            employeeShow.btnEmployWork.gameObject.SetActive(false);
            employeeShow.btnEmployeeIdle.gameObject.SetActive(false);
            for (int i = 0; i < rectItems.Length; i++)
            {
                ViewHouse_EmployeeItem itemEmployee = rectItems[i].GetComponent<ViewHouse_EmployeeItem>();
                itemEmployee.goEmployeeState.SetActive(true);
                itemEmployee.goMonthly.SetActive(false);
                itemEmployee.btnTakeOff.gameObject.SetActive(false);
                itemEmployee.btnEmploy.gameObject.SetActive(false);
            }
        }
        ViewMGToViewInfoSaloon mgSaloon = message as ViewMGToViewInfoSaloon;
        if (mgSaloon != null)
        {
            if (houseType != HouseType.Saloon || saloon.intIndexGround != mgSaloon.intIndexGround)
            {
                employeeShow.gameObject.SetActive(false);
                tower.gameObject.SetActive(false);
                rental.gameObject.SetActive(false);
                saloon.gameObject.SetActive(false);
                menu.goTower.SetActive(false);
                menu.goReatal.SetActive(false);
                menu.goSloon.SetActive(true);
            }
            houseType = HouseType.Saloon;
            saloon.intIndexGround = mgSaloon.intIndexGround;

            menu.textTitle.text = ManagerBuild.Instance.GetBuildName(mgSaloon.intBuildID);
            menu.textHouseRankTag.text = menu.textTitle.text + "等级:";
            menu.textHouseRank.text = mgSaloon.intSaloonRank.ToString();
            menu.textHousePersonTag.text = "人数:";
            menu.textHousePerson.text = mgSaloon.dicEmployee.Count + "/" + mgSaloon.intPersonCount;
            dicTempEmployee = mgSaloon.dicEmployee;
            saloon.intIncome = mgSaloon.intDailyIncome;
            saloon.intHouseRank = mgSaloon.intSaloonRank;
            saloon.intPrice = mgSaloon.intEmployPrice;
            saloon.intPriceChange = mgSaloon.intEmployPrice;
            saloon.inputSaloon.text = mgSaloon.intEmployPrice.ToString();
            saloon.textContent.text = mgSaloon.strContentTitle;
            saloon.textEmployContent.text = mgSaloon.strContentNotice;

            if (mgSaloon.booEmploying)
            {
                saloon.goEmploying.SetActive(true);
                saloon.goSelectEmployeeContent.SetActive(false);
                saloon.goSelectEmployeeType.SetActive(false);
            }
            else
            {
                saloon.goEmploying.SetActive(false);
                saloon.goSelectEmployeeContent.SetActive(true);
                saloon.goSelectEmployeeType.SetActive(true);
            }

            saloon.listData.Clear();
            saloon.columnItem.SetDataTotal(0);
            foreach (BuildSaloon.EmployingWork temp in mgSaloon.dicEmploying.Values)
            {
                saloon.listData.Add(temp);
            }
            saloon.columnItem.SetDataTotal(saloon.listData.Count);

            employeeShow.btnAll.gameObject.SetActive(true);
            employeeShow.btnEmploy.gameObject.SetActive(true);
            employeeShow.btnEmployNone.gameObject.SetActive(true);
            employeeShow.btnEmployWork.gameObject.SetActive(false);
            employeeShow.btnEmployeeIdle.gameObject.SetActive(false);
            for (int i = 0; i < rectItems.Length; i++)
            {
                ViewHouse_EmployeeItem itemEmployee = rectItems[i].GetComponent<ViewHouse_EmployeeItem>();
                itemEmployee.goEmployeeState.SetActive(true);
                itemEmployee.goMonthly.SetActive(false);
                itemEmployee.btnTakeOff.gameObject.SetActive(false);
                itemEmployee.btnEmploy.gameObject.SetActive(false);
            }
        }

        employeeShow.listData.Clear();
        foreach (PropertiesEmployee temp in dicTempEmployee.Values)
        {
            employeeShow.listData.Add(temp);
            employeeShow.listTemp.Add(temp);
        }

        SetDataTotal(employeeShow.listData.Count);
    }

    /// <summary>
    /// 查看当前员工
    /// </summary>
    void SetDataTotal(int intCount)
    {
        rectItems = employeeShow.columnItem.SetDataTotal(intCount);
        for (int i = 0; i < rectItems.Length; i++)
        {
            ViewHouse_EmployeeItem itemEmployee = rectItems[i].GetComponent<ViewHouse_EmployeeItem>();
            itemEmployee.numIndexItem = i;
            itemEmployee.numIndexData = i;
            itemEmployee.actionBase = ActionEventItem;
            itemEmployee.actionEmploy = ActionEventEmploy;
            itemEmployee.actionTakeOff = ActionEventTakeOff;

            itemEmployee.btnSee.gameObject.SetActive(false);
            itemEmployee.btnTakeOff.gameObject.SetActive(false);
            itemEmployee.textMonthly.transform.parent.parent.gameObject.SetActive(false);

            RefreshData(itemEmployee, i, i);
        }
    }

    /// <summary>
    /// 查看员工 刷新事件
    /// </summary>
    void RefreshData(ViewHouse_EmployeeItem itemTemp, int intIndexItem, int intIndexData)
    {
        if (intIndexData >= employeeShow.listTemp.Count)
        {
            itemTemp.gameObject.SetActive(false);
        }
        else
        {
            itemTemp.gameObject.SetActive(true);
            itemTemp.numIndexItem = intIndexItem;
            itemTemp.numIndexData = intIndexData;

            PropertiesEmployee employee = employeeShow.listTemp[intIndexData];
            EmoloyeeShowItem(itemTemp, employee);
        }
    }
    /// <summary>
    /// 产看员工 产看按钮
    /// </summary>
    void ActionEventItem(int intIndexItem, int intIndexData)
    {

    }
    /// <summary>
    /// 产看员工 雇佣按钮
    /// </summary>
    void ActionEventEmploy(int intIndexItem, int intIndexData)
    { }
    /// <summary>
    /// 产看员工 解雇按钮
    /// </summary>
    void ActionEventTakeOff(int intIndexItem, int intIndexData)
    { }

    void EmoloyeeShowItem(ViewHouse_EmployeeItem itemTemp, PropertiesEmployee employee)
    {
        itemTemp.textEmployeeName.text = employee.strEmployeeName;
        itemTemp.textMonthly.text = "0";
        itemTemp.textEmployeeContent.text = "???";
        itemTemp.textEmployeeContent.gameObject.SetActive(false);
        itemTemp.imageEmployeeHead.sprite = employee.spriteHead;
        itemTemp.proCombatType.textValueMain.text = employee.intCombatTypeRank.ToString();
        itemTemp.proCombatType.imageValueMain.sprite = ManagerSkill.Instance.GetCombatType(employee.combatAttackType);
        int intTemp = 0;
        foreach (KeyValuePair<EnumEmployeeProperties, int> temp in employee.dicEmployeeProperties)
        {
            itemTemp.employeeProperties.items[intTemp].textValueMain.text = temp.Value.ToString();
            itemTemp.employeeProperties.items[intTemp].imageValueMain.sprite = ManagerResources.Instance.GetEmployeeProperties(temp.Key);
            itemTemp.employeeProperties.items[intTemp].gameObject.SetActive(false);
            intTemp++;
        }
        itemTemp.combatValue.items[0].textValueMain.text = employee.intHP.ToString();
        itemTemp.combatValue.items[1].textValueMain.text = employee.intATK.ToString();
        itemTemp.combatValue.items[2].textValueMain.text = employee.intMP.ToString();
        itemTemp.combatValue.items[3].textValueMain.text = employee.floSpeed.ToString();
        itemTemp.combatValue.items[3].gameObject.SetActive(false);
    }

    void DemolishBuildHouse()
    {
        ManagerValue.actionAudio(EnumAudio.Ground);
        List<int> listEmployWork = new List<int>();
        foreach (PropertiesEmployee temp in employeeShow.listData)
        {
            if (temp.intIndexGroundWork != -1)
            {
                listEmployWork.Add(temp.intIndexID);
            }
        }
        switch (houseType)
        {
            case HouseType.Tower:
                if (listEmployWork.Count > 0)
                {
                    viewHint.strHint = "有" + listEmployWork.Count + "名员工在工作中,拆除建筑后,员工将离开岗位,并失去这些员工。";
                }
                else
                {
                    viewHint.strHint = "拆除建筑后,将失去这些员工。";
                }
                BuildBase build = UserValue.Instance.GetBuildValue(intIndexGround);
                viewHint.strHint += "\n" + ManagerValue.DemolishBuildRecycleCoin(build.GetBuildID);
                break;
            case HouseType.Rental:
                if (listEmployWork.Count > 0)
                {
                    viewHint.strHint = "有" + listEmployWork.Count + "名在工作中,拆除建筑,将返还1个月的租金,并失去" + listEmployWork.Count + "名员工";
                }
                else
                {
                    viewHint.strHint = "拆除建筑后,将返每名租客一个月的租金";
                }
                break;
            case HouseType.Saloon:
                saloon.DeleteBuild(listEmployWork, viewHint);
                break;
        }
        viewHint.actionConfirm = () =>
        {
            if (houseType == HouseType.Tower)
            {
                foreach (PropertiesEmployee temp in employeeShow.listData)
                {
                    UserValue.Instance.RecyclingStationEmployee(temp.intIndexID);
                }
            }
            else if (houseType == HouseType.Rental)
            {
                BuildRental buildRental = UserValue.Instance.GetBuildValue(intIndexGround) as BuildRental;
                if (rental.intIncome > UserValue.Instance.GetCoin)
                {
                    viewHint.strHint = "金币不够返还租客租金,所以不能拆除该建筑";
                    viewHint.actionConfirm = null;
                    ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
                    return;
                }
                else
                {
                    //扣除租金
                }
                foreach (PropertiesEmployee temp in employeeShow.listData)
                {
                    UserValue.Instance.RecyclingStationEmployee(temp.intIndexID);
                    UserValue.Instance.RecyclingStationGuest(temp.intIndexID);
                }
            }
            else if (houseType == HouseType.Saloon)
            {
                saloon.DelectEmployee(employeeShow.listData, listEmployWork);
            }
            MessageDemolishBuild mgDemolishBuild = new MessageDemolishBuild();
            mgDemolishBuild.intIndexGround = intIndexGround;
            ManagerMessage.Instance.PostEvent(EnumMessage.DemolishBuild, mgDemolishBuild);
            ManagerView.Instance.Hide(EnumView.ViewHouse);
        };
        ManagerView.Instance.Show(EnumView.ViewHint);
        ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
    }
    void SendToBuildRental(bool booSend, int intPrice)
    {
        mgToBuildRental.intIndexGround = intIndexGround;
        mgToBuildRental.intPrice = intPrice;
        mgToBuildRental.booSend = booSend;
        ManagerValue.actionGround(intIndexGround, mgToBuildRental);
    }
    void SendToBuildSaloon(bool booSend, bool booEmploying, int intEmployPrice, int intEmployType)
    {
        mgToBuildSaloon.intIndexGround = intIndexGround;
        mgToBuildSaloon.booSend = booSend;
        mgToBuildSaloon.booEmploying = booEmploying;
        mgToBuildSaloon.intEmployPrice = intEmployPrice;
        mgToBuildSaloon.intEmployType = intEmployType;
        ManagerValue.actionGround(intIndexGround, mgToBuildSaloon);
    }
    enum HouseType
    {
        None,
        Tower,
        Rental,
        Saloon,
    }
}