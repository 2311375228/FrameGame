using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRoleShowData
{
    public EnumCombatDamageType damageType;
    public EnumCombatRoleState combatState;
    public int intIndex;
    public int intHP;
    public int intHPMax;
    public int intATK;
    public int intMP;
    public CombatStructValue harmValue;
    public Vector3 vecPosition;
    public Vector3 vecHarmShow;
    public EnumCombatAttackType combatType;
    public int intCombatTypeRank;
    public int intSkillLength;
    public int[] intSkillRanks;
    public PropertiesSkill[] proSkills;
}
