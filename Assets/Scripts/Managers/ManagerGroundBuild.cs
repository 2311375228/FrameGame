using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理建筑
/// 生成，销毁，查询，改变，位置移动
/// 只给出每个格子的当前状态
/// </summary>
public class ManagerGroundBuild
{
    List<int> intOres = new List<int>() { 1008, 1009, 1010, 1011 };
    /// <summary>
    /// 地性质的改变 dicGround可建造地ID与dicBuild的ID一致
    /// </summary>
    Dictionary<int, PropertiesGround> dicGround = new Dictionary<int, PropertiesGround>(10100);
    /// <summary>
    /// dicGround可建造地ID与dicBuild的ID一致
    /// 管理实例化的类，建筑类型，是否增长金币，是否停止增长金币
    /// </summary>
    Dictionary<int, GroundBuildBase> dicBuild = new Dictionary<int, GroundBuildBase>(10000);

    System.Action<int, int, int> actionDate = (value, value2, value3) => { };

    public void Initialize()
    {
        for (int i = 0; i < dicGround.Count; i++)
        {
            dicGround.Add(i, new PropertiesGround());
            dicGround[i].SetIntGround = i;
        }
        for (int i = 0; i < dicBuild.Count; i++)
        {
            dicBuild.Add(i, null);
            dicBuild[i].intIndexGround = i;
        }

        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Date, MessageUpdateDate);
    }

    public void GetDicGround(int[] intGrounds, PropertiesGround[] properties)
    {
        properties[0] = dicGround[intGrounds[0]];
        properties[1] = dicGround[intGrounds[1]];
        properties[2] = dicGround[intGrounds[2]];
        properties[3] = dicGround[intGrounds[3]];
        properties[4] = dicGround[intGrounds[4]];
        properties[5] = dicGround[intGrounds[5]];
        properties[6] = dicGround[intGrounds[6]];
        properties[7] = dicGround[intGrounds[7]];
        properties[8] = dicGround[intGrounds[8]];
        properties[9] = dicGround[intGrounds[9]];
    }

    /// <summary>
    /// 重置所有地的状态
    /// </summary>
    public void ResetGroundState()
    {
        foreach (PropertiesGround temp in dicGround.Values)
        {
            temp.SetState = EnumGroudState.Unpurchased;
            temp.intBuildID = -1;
            temp.intObstacleMat = -1;
        }
    }

    public PropertiesGround GetGround(int intIndexGround)
    {
        return dicGround[intIndexGround];
    }
    public GroundBuildBase GetBuild(int intIndexGround)
    {
        return dicBuild[intIndexGround];
    }
    public void BuildAdd(int intIndetGround, GroundBuildBase build)
    {
        dicBuild[intIndetGround] = build;
        actionDate += dicBuild[intIndetGround].Date;
    }
    public void BuildReduce(int intIndexGround)
    {
        actionDate -= dicBuild[intIndexGround].Date;
        dicBuild[intIndexGround] = null;
    }

    void MessageUpdateDate(ManagerMessage.MessageBase message)
    {
        MessageDate date = message as MessageDate;
        if (date != null)
        {
            actionDate(date.numYear, date.numMonth, date.numDay);
        }
    }

    public void OnDestroy()
    {
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Update_Date, MessageUpdateDate);
    }
}
