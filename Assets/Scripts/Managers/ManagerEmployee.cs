using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ManagerEmployee
{
    public delegate void HeadSprite(PropertiesEmployee pro);
    public delegate void Body(PropertiesEmployee pro);
    public HeadSprite GetSprite;
    public Body GetBody;
    public GameObject goEmployee;
    public string[] strEmployeeTypes = new string[] { "", "农民", "租客", "酒鬼", "独行侠", "员工" };

    //员工的模型数据
    Dictionary<int, EmployeeBody> dicEmployeeBody = new Dictionary<int, EmployeeBody>();

    //员工表数据
    Dictionary<int, JsonValue.DataTableEmployeeItem> dicEmployee;
    List<int> listEmployee = new List<int>();
    static ManagerEmployee _instance;
    public static ManagerEmployee Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ManagerEmployee();
                //员工表
                _instance.dicEmployee = new Dictionary<int, JsonValue.DataTableEmployeeItem>();
                JsonValue.DataTableEmployeeBase tempEmployee = JsonUtility.FromJson<JsonValue.DataTableEmployeeBase>(ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableEmployee));
                for (int i = 0; i < tempEmployee.listItem.Count; i++)
                {
                    JsonValue.DataTableEmployeeItem item = tempEmployee.listItem[i];
                    _instance.dicEmployee.Add(item.intEmployeeID, item);
                    _instance.listEmployee.Add(item.intEmployeeID);
                }
            }
            return _instance;
        }
    }

    public Dictionary<int, JsonValue.DataTableEmployeeItem> GetAllEmployee()
    {
        return dicEmployee;
    }

    public JsonValue.DataTableEmployeeItem GetEmployeeValue()
    {
        //这里是临时随机产生员工信息
        //可重复利用,员工名字,图片
        int intIndex = Random.Range(0, listEmployee.Count);
        return dicEmployee[listEmployee[intIndex]];
    }
    public EnumEmployeeAddition GetRandomExtra()
    {
        int intRandom = Random.Range(0, 100);
        if (intRandom < 90)
        {
            intRandom = 0;
        }
        else
        {
            intRandom = Random.Range(1, 3);
        }
        return (EnumEmployeeAddition)intRandom;
    }
    public int GetRandomExtraValue(EnumEmployeeAddition key)
    {
        int intValue = 0;
        int[] intTemp = new int[] { 20, 40, 60, 80, 100, 150, 300, 350 };
        if (key == EnumEmployeeAddition.Time)
        {
            intValue = Random.Range(0, 10000);
            if (intValue < 8500)
            {
                intValue = intTemp[0];
            }
            else if (intValue >= 8500 && intValue < 9950)
            {
                intValue = intTemp[1];
            }
            else if (intValue >= 9950 && intValue < 9995)
            {
                intValue = intTemp[2];
            }
            else
            {
                intValue = intTemp[3];
            }
        }
        if (key == EnumEmployeeAddition.Count)
        {
            intValue = Random.Range(0, 10000);
            if (intValue < 5000)
            {
                intValue = intTemp[0];
            }
            else if (intValue >= 5000 && intValue < 7000)
            {
                intValue = intTemp[1];
            }
            else if (intValue >= 7000 && intValue < 8500)
            {
                intValue = intTemp[2];
            }
            else if (intValue >= 8500 && intValue < 9000)
            {
                intValue = intTemp[3];
            }
            else if (intValue >= 9000 && intValue < 9400)
            {
                intValue = intTemp[4];
            }
            else if (intValue >= 9400 && intValue < 9700)
            {
                intValue = intTemp[5];
            }
            else if (intValue >= 9700 && intValue < 9950)
            {
                intValue = intTemp[6];
            }
            else
            {
                intValue = intTemp[7];
            }
        }
        return intValue;
    }

    Dictionary<int, int[]> dicEmployeeAddtion;
    List<int> listEmployeeAddtion;
    /// <summary>
    /// 随机可以员工与对应的额外属性建筑ID
    /// </summary>
    public void GetRandomExtraBuildID(out int intBuildID, out int intProductID)
    {
        if (dicEmployeeAddtion == null)
        {
            dicEmployeeAddtion = new Dictionary<int, int[]>();
            dicEmployeeAddtion.Add(1001, new int[] { 101001 });//牧草场
            dicEmployeeAddtion.Add(1002, new int[] { 101002 });//蔬菜场
            dicEmployeeAddtion.Add(1003, new int[] { 101003 });//棉花场
            dicEmployeeAddtion.Add(1004, new int[] { 101004 });//小麦农场
            dicEmployeeAddtion.Add(1005, new int[] { 101006 });//药草农场
            dicEmployeeAddtion.Add(1006, new int[] { 101005 });//苹果农场
            dicEmployeeAddtion.Add(1007, new int[] { 101007 });//木材农场

            dicEmployeeAddtion.Add(2001, new int[] { 201007, 201008 });//养鸡场
            dicEmployeeAddtion.Add(2002, new int[] { 201005, 201004, 201003 });//绵羊场
            dicEmployeeAddtion.Add(2003, new int[] { 201001, 201002, 201003 });//奶牛场
            dicEmployeeAddtion.Add(2004, new int[] { 201009 });//养蚕场
            dicEmployeeAddtion.Add(2005, new int[] { 201006, 201003 });//养猪场
        }
        if (listEmployeeAddtion == null)
        {
            listEmployeeAddtion = new List<int>();
            foreach (int temp in dicEmployeeAddtion.Keys)
            {
                listEmployeeAddtion.Add(temp);
            }
        }

        int intIndexRandom = Random.Range(0, listEmployeeAddtion.Count);

        intBuildID = listEmployeeAddtion[intIndexRandom];
        intIndexRandom = Random.Range(0, dicEmployeeAddtion[listEmployeeAddtion[intIndexRandom]].Length);
        intIndexRandom = dicEmployeeAddtion[intBuildID][intIndexRandom];
        intProductID = ManagerCompound.Instance.GetValue(intIndexRandom).intProductID;
    }

    public string GetExtraContent(int intBuildID, int intProductID, EnumEmployeeAddition key, int intValue)
    {
        string strTemp = null;
        if (key == EnumEmployeeAddition.None)
        {
            strTemp = ExtraEmployeeCommon();
        }
        else if (key == EnumEmployeeAddition.Time)
        {
            strTemp = ExtraEmployeeTime(intBuildID, intProductID, intValue);
        }
        else if (key == EnumEmployeeAddition.Count)
        {
            strTemp = ExtraEmployeeCount(intBuildID, intProductID, intValue);
        }
        return strTemp;
    }

    string[] strExtraCommon = new string[]
    {
        "我是一个普通员工",
        "我是谋求工作的普通员工",
    };

    string ExtraEmployeeCommon()
    {
        return strExtraCommon[Random.Range(0, strExtraCommon.Length)];
    }
    string[] strExtraTime = new string[]
    {
        "我曾经在<color=white>@1</color>打过工,有一些工作经验,能减少<color=cyan>@2%</color>的<color=magenta>@3</color>生产时间",
        "我有丰富的<color=white>@1</color>工作经验,可以减少<color=cyan>@2%</color>的color=magenta>@3</color>生产时间",
        "在<color=white>@1</color>非常丰富的工作经验,能减少<color=cyan>@2%</color>的color=magenta>@3</color>的生产时间"
    };
    string ExtraEmployeeTime(int intBuildID, int intProductID, int intValue)
    {
        string strTemp = null;
        string strBuildName = ManagerBuild.Instance.GetBuildName(intBuildID);
        string strProductName = ManagerProduct.Instance.GetName(intProductID, false);
        int intIndex = 0;
        if (intValue == 60)
        {
            intIndex = 1;
        }
        else if (intValue == 80)
        {
            intIndex = 2;
        }
        strTemp = strExtraTime[intIndex].Replace("@1", strBuildName);
        strTemp = strTemp.Replace("@3", strProductName);
        strTemp = strTemp.Replace("@2", intValue.ToString());
        return strTemp;
    }
    string[] strExtraCount = new string[]
    {
        "我曾经在<color=white>@1</color>工作过,有工作经验,可以增加<color=cyan>@2%</color>的<color=magenta>@3</color>产量",
        "丰富的<color=white>@1</color>工作经验,可以增加<color=cyan>@2%</color>的<color=magenta>@3</color>产量",
        "很高的<color=white>@1</color>工作经验,能增加<color=magenta>@3</color>的<color=cyan>@2%</color>产量",
        "可以让<color=white>@1</color>的<color=magenta>@3</color>增加<color=cyan>@2%</color>产量",
        "<color=white>@1</color>的<color=magenta>@3</color>提高让你难以拒绝的<color=cyan>@2%</color>",
        "<color=white>@1</color>的<color=magenta>@3</color>提升到最大<color=cyan>@2%</color>"
    };
    string ExtraEmployeeCount(int intBuildID, int intProductID, int intValue)
    {

        string strTemp = null;
        string strBuildName = ManagerBuild.Instance.GetBuildName(intBuildID);
        string strProductName = ManagerProduct.Instance.GetName(intProductID, false);
        int intIndex = 0;
        if (intValue == 60)
        {
            intIndex = 1;
        }
        else if (intValue == 80)
        {
            intIndex = 2;
        }
        else if (intValue == 100)
        {
            intIndex = 3;
        }
        else if (intValue > 100 && intValue < 300)
        {
            intIndex = 4;
        }
        else if (intValue >= 300)
        {
            intIndex = 5;
        }
        strTemp = strExtraCount[intIndex].Replace("@1", strBuildName);
        strTemp = strTemp.Replace("@3", strProductName);
        strTemp = strTemp.Replace("@2", intValue.ToString());
        return strTemp;
    }
}
