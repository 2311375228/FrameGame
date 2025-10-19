using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRole
{
    public int intIndex;
    public int intIndexShowData;//界面消息下标
    public int intHP;
    public int intHPMax;
    public int intATK;
    public int intMP;
    public EnumCombatAttackType combatType;
    public float floSpeed;//攻击速度
    public GameObject goRole;
    public Vector3 vecPosition;
    public Vector3 vecPositionBeCombat;//敌方的攻击位置
    public Vector3 vecPositionBack;//敌方后退的位置

    public bool booTeam;
    public EnumCombatRoleState roleState;
    public int[] intSkillRanks;
    public int[] intSkillCounts;//技能作用角色数量
    public PropertiesSkill[] proSkills;

    public Animator anim;
    Dictionary<EnumPlayAction, string> dicPlayAction = new Dictionary<EnumPlayAction, string>();
    public void SetAnim()
    {
        anim = goRole.GetComponent<Animator>();
        if (dicPlayAction.Count == 0)
        {
            dicPlayAction.Add(EnumPlayAction.Run, "run");
            dicPlayAction.Add(EnumPlayAction.Attack, "attack");
            dicPlayAction.Add(EnumPlayAction.BeAttack, "be_attack");
            dicPlayAction.Add(EnumPlayAction.Skill, "skill");
            dicPlayAction.Add(EnumPlayAction.Death, "death");
        }
    }
    public void SetPlayAction(EnumPlayAction key, bool boo)
    {
        anim.SetBool(dicPlayAction[key], boo);
    }
    public enum EnumPlayAction
    {
        Run,
        Attack,
        BeAttack,
        Skill,
        Death,
    }
}
