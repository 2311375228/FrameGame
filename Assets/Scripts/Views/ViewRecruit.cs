using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewRecruit : ViewBase
{
    public Text textTitle;
    public Button btnClose;
    public Button btnRefreshRecruitCommon;
    public Button btnRefreshRecruitAdvance;
    public Button btnRefreshRecruitMast;

    public ViewRecruit_EmployeeItem[] employeeItems;

    //最多一次招聘4个员工
    protected override void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            ManagerView.Instance.Hide(EnumView.ViewRecruit);
        });

        //普通招聘
        btnRefreshRecruitCommon.onClick.AddListener(() =>
        {
            int Temp = RandomEmployeeCommon();
            for (int i = 0; i < Temp; i++)
            {
                if (i < Temp)
                {
                    employeeItems[i].gameObject.SetActive(true);
                }
                else
                {
                    employeeItems[i].gameObject.SetActive(false);
                }
            }
        });
        //高级招聘
        btnRefreshRecruitAdvance.onClick.AddListener(() =>
        {
        });
        //重金招聘
        btnRefreshRecruitMast.onClick.AddListener(() =>
        {
        });
    }

    /// <summary>
    /// 没有战斗属性:98%的概率
    /// 有战斗属性:2%的概率
    ///
    /// 战斗属性值:1-15级
    /// 1-5:9500%
    /// 6-10:400%
    /// 11-14:98%
    /// 15:2%
    ///
    /// 对农场效率提升:1.普通工人,2.高级工人,3.普通农场主,4.高级店主,5.普通牧场主,6.高级牧场主,7.普通店主,8.高级店主
    /// 普通概率:99%
    /// 高级概率:1%
    /// 1-5级别:
    /// 1-3:95%
    /// 4:94%
    /// 5:1%
    /// </summary>
    int RandomEmployeeCommon()
    {
        int intRandomCount = Random.Range(1, 5);

        return intRandomCount;
    }

    /// <summary>
    /// 没有战斗属性:50%的概率
    /// 有战斗属性的概率:50%
    ///
    /// 战斗属性值:1-15级
    /// 1-5:6000%
    /// 6-10:3900%
    /// 11-14:98%
    /// 15:2%
    ///
    /// 对农场效率提升:1.普通工人,2.高级工人,3.普通农场主,4.高级店主,5.普通牧场主,6.高级牧场主,7.普通店主,8.高级店主
    /// 普通概率:70%
    /// 高级概率:30%
    /// 1-5级别:
    /// 1-3:95%
    /// 4:94%
    /// 5:1%
    /// </summary>
    int RandomEmployeeAdvance()
    {
        int intRandomCount = Random.Range(1, 5);

        return intRandomCount;
    }

    /// <summary>
    /// 没有战斗属性:5%
    /// 有战斗属性:95%
    ///
    /// 战斗属性值:1-15
    /// 1-5:95%
    /// 6-10:6900%
    /// 11-14:3000%
    /// 15:5%
    ///
    /// 对农场效率提升:1.普通工人,2.高级工人,3.普通农场主,4.高级店主,5.普通牧场主,6.高级牧场主,7.普通店主,8.高级店主
    /// 普通概率:5%
    /// 高级概率:95%
    /// 1-5级别:
    /// 1-3:95%
    /// 4:94%
    /// 5:1%
    /// </summary>
    int RandomEmployeeMast()
    {
        int intRandomCount = Random.Range(1, 5);

        return intRandomCount;
    }
}
