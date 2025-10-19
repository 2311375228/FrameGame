using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCombatLogic
{
    float floTime;
    float floTimeLimit;
    bool booPlay;
    public void SetRolePlayValue(float floTime, float floTimeLimit, bool booPlay)
    {
        this.floTime = floTime;
        this.floTimeLimit = floTimeLimit;
        this.booPlay = booPlay;
    }

    /// <summary>
    /// 查找role
    /// </summary>
    public CombatRole FindRole(ref int intIndex, CombatRole[] roles, int intRival = -1)
    {
        if (intRival != -1 && !(roles[intRival].roleState == EnumCombatRoleState.None || roles[intRival].roleState == EnumCombatRoleState.death))
        {
            return roles[intRival];
        }

        //循环的目的是防止该位置role已失效
        //这里必然要找到role
        if (intIndex == roles.Length)
        {
            intIndex = 0;
        }
        for (int i = 0; i < roles.Length; i++)
        {
            if (roles[intIndex].roleState == EnumCombatRoleState.Idle)
            {
                return roles[intIndex++];
            }
            intIndex++;
            if (intIndex == roles.Length)
            {
                intIndex = 0;
            }
        }
        return null;
    }

    /// <summary>
    /// 检查攻击队伍是否是最后一位攻击
    /// </summary>
    public bool RoleEndAttack(int intIndex, CombatRole[] roles)
    {
        intIndex += 1;
        for (int i = intIndex; i < roles.Length; i++)
        {
            if (roles[i].roleState == EnumCombatRoleState.Idle)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 准备阶段 等待时间
    /// </summary>
    public bool ReadyWaiting()
    {
        floTime += Time.deltaTime;
        if (floTime > floTimeLimit)
        {
            floTime = 0;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 设置攻击者的攻击信息
    /// </summary>
    public void SetAttackValue(ref CombatStructValue harmValue, ref CombatStructValue harmBeValue, CombatRole role)
    {
        //harmBeValue 代表被攻击方需要承受的伤害
        //harmValue 代表攻击方需要承受的伤害

        harmValue.intATK = 0;
        harmValue.intMP = 0;
        harmValue.intHP = 0;
        harmValue.intAttackNum = 0;
        harmValue.attackType = EnumCombatAttackType.None;

        harmBeValue.attackType = (EnumCombatAttackType)Random.Range(1, 4);
        harmBeValue.intAttackNum = Random.Range(1, 4);
        harmBeValue.intATK = role.intATK;
        harmBeValue.intMP = role.intMP;
        int intTemp = Random.Range(0, role.proSkills.Length);
        if (role.proSkills[intTemp] != null && role.proSkills[intTemp].intSkillID != 0)
        {
            if (harmBeValue.attackType == role.proSkills[intTemp].combatType)
            {
                switch (harmBeValue.attackType)
                {
                    case EnumCombatAttackType.Attack:
                        harmBeValue.intAttackNum = 1;
                        harmBeValue.intATK = role.intATK * Random.Range(1, 3); //role.intATK + role.intATK * role.intSkillRanks[intTemp];
                        harmBeValue.intMP = 0;
                        harmBeValue.intHP = 0;
                        break;
                    case EnumCombatAttackType.Magic:
                        harmBeValue.intAttackNum = 1;
                        harmBeValue.intATK = role.intATK * Random.Range(1, 3);
                        harmBeValue.intMP = 0;//role.intATK + role.intMP * role.intSkillRanks[intTemp];
                        harmBeValue.intHP = 0;
                        break;
                    case EnumCombatAttackType.Defense:
                        harmBeValue.intAttackNum = 1;
                        harmBeValue.intATK = role.intATK * Random.Range(1, 3);//role.intATK;
                        harmBeValue.intMP = 0;
                        harmBeValue.intHP = 0;

                        harmValue.intHP = 2;//role.intSkillRanks[intTemp] * Random.Range(1, 3);//(int)(role.intHPMax * Random.Range(5.0f, 10.0f) / 100.0f);
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            switch (harmBeValue.attackType)
            {
                case EnumCombatAttackType.Attack:
                    harmBeValue.intAttackNum = 1;
                    harmBeValue.intATK = role.intATK * Random.Range(1, 3);
                    harmBeValue.intMP = 0;
                    harmBeValue.intHP = 0;
                    break;
                case EnumCombatAttackType.Magic:
                    harmBeValue.intAttackNum = 1;
                    harmBeValue.intATK = role.intATK * Random.Range(1, 3);
                    harmBeValue.intMP = 0;//role.intMP;
                    harmBeValue.intHP = 0;
                    break;
                case EnumCombatAttackType.Defense:
                    harmBeValue.intAttackNum = 1;
                    harmBeValue.intATK = role.intATK * Random.Range(1, 3);
                    harmBeValue.intMP = 0;
                    harmBeValue.intHP = 0;

                    harmValue.intHP = 2;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 攻击与伤害
    /// </summary>
    public bool RoleAttack(CombatStructValue harmValue, CombatStructValue harmBeValue, CombatRole roleAttack, CombatRole roleBeAttack)
    {
        floTime += Time.deltaTime;
        if (booPlay)
        {
            booPlay = false;
            AttackPlay(true, harmValue.attackType, roleAttack);
            roleBeAttack.SetPlayAction(CombatRole.EnumPlayAction.BeAttack, true);
            switch (roleAttack.combatType)
            {
                case EnumCombatAttackType.Attack:
                    roleAttack.SetPlayAction(CombatRole.EnumPlayAction.Attack, true);
                    break;
                case EnumCombatAttackType.Magic:
                    roleAttack.SetPlayAction(CombatRole.EnumPlayAction.Attack, true);
                    break;
                case EnumCombatAttackType.Defense:
                    roleAttack.SetPlayAction(CombatRole.EnumPlayAction.Attack, true);
                    break;
                default:
                    break;
            }
        }
        if (floTime > floTimeLimit)
        {
            floTime = 0;

            roleBeAttack.intHP = roleBeAttack.intHP - harmBeValue.intATK;
            roleBeAttack.intHP = roleBeAttack.intHP - harmBeValue.intMP;
            roleBeAttack.intHP = roleBeAttack.intHP + harmBeValue.intHP;
            if (roleBeAttack.intHP <= 0)
            {
                roleBeAttack.intHP = 0;
                roleBeAttack.roleState = EnumCombatRoleState.death;
                roleBeAttack.SetPlayAction(CombatRole.EnumPlayAction.Death, true);
                ManagerValue.actionAudioCombat(EnumAudioCombat.cartoon_squirt_04);
            }
            if (roleBeAttack.intHP > roleBeAttack.intHPMax)
            {
                roleBeAttack.intHP = roleBeAttack.intHPMax;
            }

            roleAttack.intHP = roleAttack.intHP - harmValue.intATK;
            roleAttack.intHP = roleAttack.intHP - harmValue.intMP;
            roleAttack.intHP = roleAttack.intHP += harmValue.intHP;
            if (roleAttack.intHP <= 0)
            {
                roleAttack.intHP = 0;
                roleAttack.roleState = EnumCombatRoleState.death;
                roleBeAttack.SetPlayAction(CombatRole.EnumPlayAction.Death, true);
            }
            if (roleAttack.intHP > roleAttack.intHPMax)
            {
                roleAttack.intHP = roleAttack.intHPMax;
            }

            AttackPlay(false, harmValue.attackType, roleAttack);
            roleBeAttack.SetPlayAction(CombatRole.EnumPlayAction.BeAttack, false);
            roleAttack.SetPlayAction(CombatRole.EnumPlayAction.Attack, false);
            switch (roleAttack.combatType)
            {
                case EnumCombatAttackType.Attack:
                    roleAttack.SetPlayAction(CombatRole.EnumPlayAction.Attack, false);
                    break;
                case EnumCombatAttackType.Magic:
                    roleAttack.SetPlayAction(CombatRole.EnumPlayAction.Attack, false);
                    break;
                case EnumCombatAttackType.Defense:
                    roleAttack.SetPlayAction(CombatRole.EnumPlayAction.Attack, false);
                    break;
                default:
                    break;
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// 模型动画类型
    /// </summary>
    void AttackPlay(bool booPlay, EnumCombatAttackType enumAttackType, CombatRole role)
    {
        switch (enumAttackType)
        {
            case EnumCombatAttackType.Attack:
                role.SetPlayAction(CombatRole.EnumPlayAction.Attack, booPlay);
                break;
            case EnumCombatAttackType.Magic:
                role.SetPlayAction(CombatRole.EnumPlayAction.Skill, booPlay);
                break;
            case EnumCombatAttackType.Defense:
                role.SetPlayAction(CombatRole.EnumPlayAction.Attack, booPlay);
                break;
            default:
                break;
        }
    }
    public bool RoleMove(Vector3 vecTarget, CombatRole role)
    {
        role.goRole.transform.position = Vector3.MoveTowards(role.goRole.transform.position, vecTarget, 10 * Time.deltaTime);
        if (Vector3.Distance(role.goRole.transform.position, vecTarget) < 0.1f)
        {
            role.goRole.transform.position = vecTarget;
            return true;
        }
        return false;
    }

    public bool AttackRoleAgain(CombatRole roleAttack, CombatRole roleBeAttack)
    {

        return false;
    }

    /// <summary>
    /// 下一回合 倒计时
    /// </summary>
    public bool RoundCountDown(ref float flo)
    {
        floTime += Time.deltaTime;
        flo = floTime;
        if (floTime > floTimeLimit)
        {
            return true;
        }
        return false;
    }

}
