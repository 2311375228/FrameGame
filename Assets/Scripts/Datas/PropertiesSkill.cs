using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesSkill
{
    public int intSkillID;
    public EnumSkill enumSkill;
    public EnumCombatAttackType combatType;
    public string strName;
    public string strICON;
    public string strEffect;
    public int intValue;//基础伤害值
    public int intRoleCount;//最大作用角色数
    public bool booRandom;//是否随机

    /// <summary>
    /// 技能只能当前回合有效
    /// 分为单体,随机单体,随机全体,百分比最少血量加血,嘲讽
    /// 防御,增加血量,减少伤害
    /// </summary>
    public enum EnumSkill
    {
        None,
        Attack = 1,//攻击力伤害
        AttackUp_self = 2,//提升攻击伤害
        AttackDown_Enemy = 3,//降低攻击伤害
        DefenseAttack_Self = 4,//减少受到攻击伤害
        DefenseAttack_Enemy = 5,//增加敌人受到攻击伤害
        DefenseMagic_Self = 6,//减少自己的魔法伤害
        DefenseMagic_Enemy = 7,//增加敌人的魔法伤害
        DefenseHP_Self = 8,//增加自己血量
        DefenseHP_Enemy = 9,//减少敌人血量
        Magic = 12,//魔法伤害
        Dizziness_Self = 13,//解除队伍眩晕
        Dizziness_Enemy = 14,//眩晕敌人
    }
}
