using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ViewCombatTeam : ViewBase
{
    public Text textTitle;

    public Button btnClose;

    public ViewCombatTeam_Team[] combatTeam;

    int intIndexSize;
    int[] intEmployeeIDs;

    ViewMGEmployeeAddTo mgEmployeeAdd;
    EmployeeData employeeData = new EmployeeData();
    protected override void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            ManagerView.Instance.Hide(EnumView.ViewCombatTeam);
        });

        if (combatTeam != null)
        {
            for (int i = 0; i < combatTeam.Length; i++)
            {
                combatTeam[i].btnAddEmployee.onClick.AddListener(OnClickEmployeeAdd(i));
                combatTeam[i].btnAddEmployee.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.EmployEmployees);
                combatTeam[i].btnRandomSkill.onClick.AddListener(OnClickRandomSkill(i));
                combatTeam[i].btnEmployeeSee.onClick.AddListener(OnClickEmployeeSee(i));
                combatTeam[i].btnEmployeeRemove.onClick.AddListener(OnClickEmployeeRemove(i));
                combatTeam[i].btnEmployeeRemove.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Dismiss);
                for (int j = 0; j < combatTeam[i].equipment.items.Length; j++)
                {
                    Transform transTemp = combatTeam[i].equipment.items[j].imageValueMain.transform;
                    transTemp.parent.GetComponent<Button>().onClick.AddListener(OnClickEquipment(i, j));
                }
                combatTeam[i].equipment.gameObject.SetActive(false);
                combatTeam[i].btnEmployeeSee.gameObject.SetActive(false);
                combatTeam[i].employeeCombatValue.items[2].gameObject.SetActive(false);
                combatTeam[i].employeeCombatValue.items[3].gameObject.SetActive(false);
                combatTeam[i].employeeType.gameObject.SetActive(false);
                combatTeam[i].textEmployeeName.gameObject.SetActive(false);
                combatTeam[i].btnRandomSkill.gameObject.SetActive(false);
                combatTeam[i].employeeSkill.gameObject.SetActive(false);
                combatTeam[i].imageCombatType.transform.parent.gameObject.SetActive(false);
            }
        }

        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Date, EventRefreshDate);
    }

    public override void Show()
    {
        base.Show();

        intEmployeeIDs = new int[5] { -1, -1, -1, -1, -1 };
        Dictionary<int, PropertiesEmployee> dicEmployee = null;
        //查找 农民员工 查找游客员工
        for (int i = 0; i < 2; i++)
        {
            if (i == 0)
            {
                dicEmployee = UserValue.Instance.GetEmployeeAll();
            }
            else if (i == 1)
            {
                dicEmployee = UserValue.Instance.GetEmployeeGuestAll();
            }
            foreach (PropertiesEmployee temp in dicEmployee.Values)
            {
                if (temp.enumLocation == EnumEmployeeLocation.CombatTeam)
                {
                    intEmployeeIDs[temp.intIndexCombat] = temp.intIndexID;
                }
            }
        }
        for (int i = 0; i < intEmployeeIDs.Length; i++)
        {
            if (intEmployeeIDs[i] != -1)
            {
                combatTeam[i].btnAddEmployee.gameObject.SetActive(false);
                combatTeam[i].goImageEmployeeCombatInfo.SetActive(true);

                PropertiesEmployee employee = UserValue.Instance.GetEmployeeValue(intEmployeeIDs[i]);
                for (int j = 0; j < employee.proSkills.Length; j++)
                {
                    if (employee.proSkills[j] != null)
                    {
                        combatTeam[i].employeeSkill.items[j].gameObject.SetActive(true);
                        combatTeam[i].employeeSkill.items[j].imageValueMain.sprite = ManagerResources.Instance.GetSkillSprite(employee.proSkills[j].strICON);
                        combatTeam[i].employeeSkill.items[j].imageValue.sprite = ManagerSkill.Instance.GetCombatType(employee.proSkills[j].combatType);
                        combatTeam[i].employeeSkill.items[j].textValueMain.text = employee.intSkillRanks[j].ToString();
                    }
                    else
                    {
                        combatTeam[i].employeeSkill.items[j].gameObject.SetActive(false);
                    }
                }
                ShowEmployeeInfo(combatTeam[i], employee);
            }
            else
            {
                combatTeam[i].btnAddEmployee.gameObject.SetActive(true);
                combatTeam[i].goImageEmployeeCombatInfo.SetActive(false);
            }
        }

        textTitle.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.BattleQueue);
    }

    public override void SetData(Message message)
    {
        mgEmployeeAdd = message as ViewMGEmployeeAddTo;
        if (mgEmployeeAdd != null)
        {
            //添加员工
            PropertiesEmployee itemEmployee = UserValue.Instance.GetEmployeeValue(mgEmployeeAdd.intEmployeeID);
            itemEmployee.intIndexGroundWork = 10000;
            itemEmployee.enumLocation = EnumEmployeeLocation.CombatTeam;
            itemEmployee.intIndexCombat = intIndexSize;


            intEmployeeIDs[intIndexSize] = itemEmployee.intIndexID;

            combatTeam[intIndexSize].btnAddEmployee.gameObject.SetActive(false);
            combatTeam[intIndexSize].goImageEmployeeCombatInfo.SetActive(true);

            ShowEquipment(combatTeam[intIndexSize], itemEmployee);

            //生成战斗数据
            combatTeam[intIndexSize].imageCombatType.sprite = ManagerSkill.Instance.GetCombatType(itemEmployee.combatAttackType);
            combatTeam[intIndexSize].textCombatRank.text = itemEmployee.intCombatTypeRank.ToString();

            SetHeroSkill(itemEmployee, combatTeam[intIndexSize]);
            ShowEmployeeInfo(combatTeam[intIndexSize], itemEmployee);
        }
    }

    void ShowEquipment(ViewCombatTeam_Team employeeInfo, PropertiesEmployee employee)
    {
        int[] intEquipmentIDs = employee.intEquipmentIDs;
        View_PropertiesItem[] items = employeeInfo.equipment.items;
        for (int i = 0; i < intEquipmentIDs.Length; i++)
        {
            if (intEquipmentIDs[i] != -1)
            {
                //PropertiesEquipment item = UserValue.Instance.GetEquipmentItem(intEquipmentIDs[i]);
                //items[i].imageValueMain.sprite = ManagerResources.Instance.GetEquipmentSprite(item.strICON);

                //hero.intHP += item.dicEquipment[PropertiesEquipment.EnumCombat.HP];
                //hero.intHPMax += item.dicEquipment[PropertiesEquipment.EnumCombat.HP];
                //hero.intAttack += item.dicEquipment[PropertiesEquipment.EnumCombat.ATK];
                //hero.intMagic += item.dicEquipment[PropertiesEquipment.EnumCombat.MP];
                //hero.floAttackSpeed += item.dicEquipment[PropertiesEquipment.EnumCombat.Speed];
            }
        }
    }

    void ShowEmployeeInfo(ViewCombatTeam_Team employeeInfo, PropertiesEmployee employee)
    {
        employeeInfo.textEmployeeName.text = employee.strEmployeeName;
        employeeInfo.employeeCombatValue.items[0].textValueMain.text = employee.intHP.ToString();
        employeeInfo.employeeCombatValue.items[1].textValueMain.text = employee.intATK.ToString();
        employeeInfo.employeeCombatValue.items[2].textValueMain.text = employee.intMP.ToString();
        employeeInfo.employeeCombatValue.items[3].textValueMain.text = employee.floSpeed.ToString();

        employeeInfo.imageCombatType.sprite = ManagerSkill.Instance.GetCombatType(employee.combatAttackType);
        employeeInfo.textCombatRank.text = employee.intCombatTypeRank.ToString();
        employeeInfo.imageEmployeeHead.sprite = employee.spriteHead;
    }

    void SetHeroSkill(PropertiesEmployee employee, ViewCombatTeam_Team employeeInfo)
    {
        //是否让相同的技能存在
        if (employee.proSkills == null || employee.proSkills.Length != 4)
        {
            employee.intSkillRanks = new int[4];
            employee.proSkills = new PropertiesSkill[4];
        }
        int intSkillCount = RandomSkillCount(employee.proSkills.Length);
        for (int i = 0; i < employee.proSkills.Length; i++)
        {
            if (i < intSkillCount)
            {
                int intSkillIDTemp = ManagerSkill.Instance.GetRandomSkillID();
                employee.proSkills[i] = ManagerSkill.Instance.GetSkillValue(intSkillIDTemp);
                employee.intSkillRanks[i] = RandomSkillRank();
                employeeInfo.employeeSkill.items[i].gameObject.SetActive(true);
                employeeInfo.employeeSkill.items[i].imageValueMain.sprite = ManagerResources.Instance.GetSkillSprite(employee.proSkills[i].strICON);
                employeeInfo.employeeSkill.items[i].imageValue.sprite = ManagerSkill.Instance.GetCombatType(employee.proSkills[i].combatType);
                employeeInfo.employeeSkill.items[i].textValueMain.text = employee.intSkillRanks[i].ToString();
            }
            else
            {
                employeeInfo.employeeSkill.items[i].gameObject.SetActive(false);
                employee.proSkills[i] = null;
                employee.intSkillRanks[i] = -1;
            }
        }
    }

    void EventRefreshDate(ManagerMessage.MessageBase message)
    {
        MessageDate date = message as MessageDate;
        if (date != null && gameObject.activeSelf)
        {
            for (int i = 0; i < intEmployeeIDs.Length; i++)
            {
                if (intEmployeeIDs[i] != -1)
                {
                    combatTeam[i].btnAddEmployee.gameObject.SetActive(false);
                    combatTeam[i].goImageEmployeeCombatInfo.SetActive(true);
                }
                else
                {
                    combatTeam[i].btnAddEmployee.gameObject.SetActive(true);
                    combatTeam[i].goImageEmployeeCombatInfo.SetActive(false);
                }
            }
        }
    }
    int RandomSkillCount(int intSkillCount)
    {
        int intTemp = Random.Range(0, 100);
        if (intTemp < 10)
        {
            intTemp = 0;
        }
        else if (intTemp >= 10 && intTemp < 50)
        {
            intTemp = 1;
        }
        else if (intTemp >= 50 && intTemp < 90)
        {
            intTemp = 2;
        }
        else if (intTemp >= 90 && intTemp < 99)
        {
            intTemp = 3;
        }
        else if (intTemp >= 98)
        {
            intTemp = 4;
        }
        else
        {
            intTemp = 0;
        }
        return intTemp;
    }

    int RandomSkillRank()
    {
        int intTemp = Random.Range(0, 10000);
        if (intTemp < 10)
        {
            intTemp = 0;
        }
        else if (intTemp >= 10 && intTemp < 5000)
        {
            intTemp = Random.Range(1, 9);
        }
        else if (intTemp >= 5000 && intTemp < 8000)
        {
            intTemp = Random.Range(9, 12);
        }
        else if (intTemp >= 8000 && intTemp < 9998)
        {
            intTemp = Random.Range(12, 15);
        }
        else if (intTemp >= 9998)
        {
            intTemp = 15;
        }
        else
        {
            intTemp = 0;
        }
        return intTemp;
    }

    /// <summary>
    /// 添加员工
    /// </summary>
    UnityAction OnClickEmployeeAdd(int intIndex)
    {
        return delegate
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            intIndexSize = intIndex;

            employeeData.booEmployee = combatTeam[intIndex].btnAddEmployee.gameObject.activeSelf;
            employeeData.intIndex = intIndex;
            employeeData.enumView = EnumView.ViewCombatTeam;
            employeeData.enumEmployeeProperties = new EnumEmployeeProperties[] { };
            employeeData.dicPropertiesInfo = new Dictionary<EnumEmployeeProperties, string>();

            EmployeeAdd(employeeData);
        };
    }

    /// <summary>
    /// 员工随机技能
    /// </summary>
    UnityAction OnClickRandomSkill(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            SetHeroSkill(UserValue.Instance.GetEmployeeValue(intEmployeeIDs[intIndex]), combatTeam[intIndex]);
        };
    }

    /// <summary>
    /// 产看员工
    /// </summary>
    UnityAction OnClickEmployeeSee(int intIndex)
    {
        return delegate
        {
            int intEmployeeID = intEmployeeIDs[intIndex];
        };
    }

    /// <summary>
    /// 移除员工
    /// </summary>
    UnityAction OnClickEmployeeRemove(int intIndex)
    {
        return delegate
        {
            ManagerValue.actionAudio(EnumAudio.Ground);

            PropertiesEmployee item = UserValue.Instance.GetEmployeeValue(intEmployeeIDs[intIndex]);
            item.intIndexGroundWork = -1;
            item.intIndexCombat = -1;
            item.enumLocation = EnumEmployeeLocation.Ground;
            item.intSkillRanks = null;
            item.proSkills = null;

            intEmployeeIDs[intIndex] = -1;
            combatTeam[intIndex].goImageEmployeeCombatInfo.SetActive(false);
            combatTeam[intIndex].btnAddEmployee.gameObject.SetActive(true);
        };
    }

    /// <summary>
    /// 装备按钮
    /// </summary>
    UnityAction OnClickEquipment(int intIndexHero, int intIndexEquipment)
    {
        return () =>
        {
            //当装备界面更换好

            //CombatRole.Hero hero = heros[intIndexHero];
            //PropertiesEmployee itemEmployee = hero.proEmployee;

            //ShowEquipment(combatTeam[intIndexSize], hero);

            //hero.intHP += itemEmployee.intHP;
            //hero.intHPMax += itemEmployee.intHP;
            //hero.intAttack += itemEmployee.intATK;
            //hero.intMagic += itemEmployee.intMP;
            //hero.floAttackSpeed += itemEmployee.floSpeed;

            //ShowEmployeeInfo(combatTeam[intIndexHero], hero);
        };
    }

    private void OnDestroy()
    {
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Update_Date, EventRefreshDate);
    }
}
