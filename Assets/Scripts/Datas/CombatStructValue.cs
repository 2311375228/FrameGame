using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CombatStructValue
{
    public int intATK;
    public int intMP;
    public int intHP;
    public int intAttackNum;
    public EnumCombatAttackType attackType;

    public CombatStructValue GetHarmZero()
    {
        intATK = 0;
        intMP = 0;
        intHP = 0;
        intAttackNum = 0;
        attackType = EnumCombatAttackType.None;
        return this;
    }
}
