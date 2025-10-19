using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesEmployee
{
    public EnumEmployeeState enumState;
    public EnumEmployeeIdentification enumIdentification;
    public EnumEmployeeLocation enumLocation;
    public EnumCombatAttackType combatAttackType;
    public int intTableEmployeeID;//配置表ID
    /// <summary>
    /// 通过增量来标记唯一员工,因为原始信息可重复
    /// </summary>
    public int intIndexID;
    public int intRank;//员工等级
    public int intIndexGroundWork;
    public int intIndexGround;//创建员工的地方
    public int intMonthlyMoney;//员工月薪
    public string strState;
    public Dictionary<EnumEmployeeProperties, int> dicEmployeeProperties;

    public int intIndexCombat;//战斗位置序号
    public int[] intSkillRanks;
    public PropertiesSkill[] proSkills;
    public int intHP;
    public int intATK;
    public int intMP;
    public int intCombatTypeRank;
    public float floSpeed;
    public string strEmployeeName;
    public string strHeadICONPath;
    public Sprite spriteHead;//员工头像
    public EmployeeBody body;//主要是减少模型资源在内存中的占用,之记录关键数据即可

    /// <summary>
    /// 0:武器 1:护甲 2:鞋子
    /// </summary>
    public int[] intEquipmentIDs = new int[] { -1, -1, -1 };

    //农场,牧草,工坊 的增益
    public EnumEmployeeAddition enumAddition;//员工额外提升ID
    public int intAdditionBuildID;
    public int intAdditionProductID;
    public int intAdditionValue;
    public string strAddtionContent;//内容

    //工作时长或战斗次数
    public int intEmployWorkValue;
}
