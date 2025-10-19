using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCombat : ControllerCombatBase
{
    //1.每回合开始时制作一个队列,速度最快的排在最前,最慢的排在最后
    //2.速度最快的优先出手,打对家,如果对方队伍有嘲讽技能,优先攻击嘲讽,如果对家死了
    //3.当队列成员数量=0,开始下一回合(回到回合开始所做的操作) 每次轮到下一个角色,(提前预判队列成员数量是否大于1) 队列的0号位置攻击
    //4.技能至少一个被动
    //5.角色的主要技能,攻击系,坦克系列,恢复系,加强系
    //6.吸血,暴击,溅射,连击技能,反伤
    //7.队伍驻扎防守

    //员工类型:攻击,坦克,治疗

    //攻速 基础是1,每一回合打一次,每次叠加量大于1,
    //可以攻击两次,每一回合两队比拼时,按攻速排列攻击顺序

    float floTimeRun = 1;
    float floTimeAttack = 1;
    float floTimeHurt = 1;
    EnumAudioCombat[] enumAduioRuns = new EnumAudioCombat[] {
         EnumAudioCombat.footstep_grass_run_01,
          EnumAudioCombat.footstep_grass_run_02,
           EnumAudioCombat.footstep_grass_run_03,
            EnumAudioCombat.footstep_grass_run_04,
             EnumAudioCombat.footstep_grass_run_05,
              EnumAudioCombat.footstep_grass_run_06,
               EnumAudioCombat.footstep_grass_run_07,
                EnumAudioCombat.footstep_grass_run_08,
                 EnumAudioCombat.footstep_grass_run_09,
                  EnumAudioCombat.footstep_grass_run_10,
    };
    EnumAudioCombat[] enumAudioHurts = new EnumAudioCombat[] {

         EnumAudioCombat.GRUNT_Male_Hurt_01_mono,
          EnumAudioCombat.GRUNT_Male_Hurt_02_mono,
           EnumAudioCombat.GRUNT_Male_Hurt_03_mono,
            EnumAudioCombat.GRUNT_Male_Hurt_04_mono,
    };
    EnumAudioCombat[] enumAudioAttacks = new EnumAudioCombat[] {
         EnumAudioCombat.IMPACT_ShootEmUp_Medium_01_RR01_mono,
          EnumAudioCombat.IMPACT_ShootEmUp_Medium_01_RR02_mono,
           EnumAudioCombat.IMPACT_ShootEmUp_Medium_01_RR03_mono,
            EnumAudioCombat.IMPACT_ShootEmUp_Medium_02_RR01_mono,
             EnumAudioCombat.IMPACT_ShootEmUp_Medium_02_RR02_mono,
              EnumAudioCombat.IMPACT_ShootEmUp_Medium_02_RR03_mono,
               EnumAudioCombat.IMPACT_ShootEmUp_Medium_03_RR01_mono,
                EnumAudioCombat.IMPACT_ShootEmUp_Medium_03_RR02_mono,
                 EnumAudioCombat.IMPACT_ShootEmUp_Medium_03_RR03_mono,
                  EnumAudioCombat.IMPACT_ShootEmUp_Medium_04_RR01_mono,
                   EnumAudioCombat.IMPACT_ShootEmUp_Medium_04_RR02_mono,
                    EnumAudioCombat.IMPACT_ShootEmUp_Medium_04_RR03_mono,
                     EnumAudioCombat.IMPACT_ShootEmUp_Medium_05_RR01_mono,
                      EnumAudioCombat.IMPACT_ShootEmUp_Medium_05_RR02_mono,
                       EnumAudioCombat.IMPACT_ShootEmUp_Medium_05_RR03_mono,
                        EnumAudioCombat.IMPACT_ShootEmUp_Medium_06_RR01_mono,
                         EnumAudioCombat.IMPACT_ShootEmUp_Medium_06_RR02_mono,
                          EnumAudioCombat.IMPACT_ShootEmUp_Medium_06_RR03_mono,
                           EnumAudioCombat.IMPACT_ShootEmUp_Medium_07_RR01_mono,
                            EnumAudioCombat.IMPACT_ShootEmUp_Medium_07_RR02_mono,
                             EnumAudioCombat.IMPACT_ShootEmUp_Medium_07_RR03_mono,
    };

    ControllerCombatLogic logicCombat = new ControllerCombatLogic();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        ManagerValue.actionInitCombat = ActionInitCombat;
        ManagerMessage.Instance.AddEvent(EnumMessage.StopCombat, MessageStopCombat);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (booCombat)
        {
            switch (enumOrder)
            {
                case EnumCombatAttackState.Ready:
                    if (logicCombat.ReadyWaiting())
                    {
                        RoleReady();
                        //booCombat是在RoleReady判断执行,防止导致战斗结束roleAttack可能为空
                        if (!(!booCombat || enumOrder == EnumCombatAttackState.Over))
                        {
                            //设置攻击动作的时间
                            logicCombat.SetRolePlayValue(0, 2, true);
                            //准备阶段将攻击者 攻击信息设置好
                            logicCombat.SetAttackValue(ref harmValue, ref harmBeValue, roleAttack);
                            roleAttack.SetPlayAction(CombatRole.EnumPlayAction.Run, true);
                        }
                    }
                    break;
                case EnumCombatAttackState.ForwardMove:
                    if (logicCombat.RoleMove(roleBeAttack.vecPositionBeCombat, roleAttack))
                    {
                        enumOrder = EnumCombatAttackState.AttackAction;
                        roleAttack.SetPlayAction(CombatRole.EnumPlayAction.Run, false);
                        floTimeAttack = Random.Range(0f, 0.25f);
                        floTimeHurt = Random.Range(0f, 0.25f);
                    }
                    floTimeRun += Time.deltaTime;
                    if (floTimeRun > 0.2f)
                    {
                        floTimeRun = 0;
                        ManagerValue.actionAudioCombat(enumAduioRuns[Random.Range(0, enumAduioRuns.Length)]);
                    }
                    break;
                case EnumCombatAttackState.AttackAction:
                    if (logicCombat.RoleAttack(harmValue, harmBeValue, roleAttack, roleBeAttack))
                    {
                        //伤害提示
                        ShowRoleInfo(roleAttack, harmValue);
                        ShowRoleInfo(roleBeAttack, harmBeValue);
                        logicCombat.SetRolePlayValue(0, 1f, true);
                        Vector3 vecTempAngle = roleAttack.goRole.transform.localEulerAngles;
                        vecTempAngle.y += 180;
                        roleAttack.goRole.transform.localEulerAngles = vecTempAngle;
                        enumOrder = EnumCombatAttackState.BackMove;
                    }
                    floTimeAttack += Time.deltaTime;
                    if (floTimeAttack > 0.5f)
                    {
                        floTimeAttack = 0;
                        ManagerValue.actionAudioCombat(enumAudioAttacks[Random.Range(0, enumAudioAttacks.Length)]);
                    }
                    floTimeHurt += Time.deltaTime;
                    if (floTimeHurt > 0.5f)
                    {
                        floTimeHurt = 0;
                        ManagerValue.actionAudioCombat(enumAudioHurts[Random.Range(0, enumAudioHurts.Length)]);
                    }
                    break;
                case EnumCombatAttackState.BackMoveAgain:

                    break;
                case EnumCombatAttackState.BackMove:
                    if (logicCombat.RoleMove(roleAttack.vecPosition, roleAttack))
                    {
                        enumOrder = EnumCombatAttackState.Ready;
                        Vector3 vecTempAngle = roleAttack.goRole.transform.localEulerAngles;
                        vecTempAngle.y += 180;
                        roleAttack.goRole.transform.localEulerAngles = vecTempAngle;
                        if (logicCombat.RoleEndAttack(roleAttack.intIndex, roleAttacks))
                        {
                            booAttack = !booAttack;
                            intIndexAttackEnemy = 0;
                            intIndexAttackHero = 0;
                            over.intRoundCount += 1;
                        }
                    }
                    floTimeRun += Time.deltaTime;
                    if (floTimeRun > 0.2f)
                    {
                        floTimeRun = 0;
                        ManagerValue.actionAudioCombat(enumAduioRuns[Random.Range(0, enumAduioRuns.Length)]);
                    }
                    break;
                case EnumCombatAttackState.Over:
                    //倒计时
                    if (logicCombat.RoundCountDown(ref over.floTime))
                    {
                        enumOrder = EnumCombatAttackState.Ready;
                        booAttack = !booAttack;
                        intIndexAttackEnemy = 0;
                        intIndexAttackHero = 0;
                        over.booRound = false;
                    }
                    over.floTime = 5 - over.floTime;
                    ManagerView.Instance.SetData(EnumView.ViewCombat, over);
                    break;
            }
        }
    }
    /// <summary>
    /// 每一回合的准备
    /// </summary>
    void RoleReady()
    {
        for (int i = 0; i < roleEnemys.Length; i++)
        {
            ShowRoleInfo(roleEnemys[i], harmBeValue.GetHarmZero());
        }
        for (int i = 0; i < roleHeros.Length; i++)
        {
            ShowRoleInfo(roleHeros[i], harmBeValue.GetHarmZero());
        }
        //攻击者 与 被攻击者 每一回合都在切换
        int intTempAttack = -1;
        if (booAttack)
        {
            roleAttack = logicCombat.FindRole(ref intIndexAttackEnemy, roleEnemys);
            roleAttacks = roleEnemys;
            if (roleAttack != null)
            {
                intTempAttack = roleAttack.intIndex;
            }
            roleBeAttack = logicCombat.FindRole(ref intIndexAttackHero, roleHeros, intTempAttack);
        }
        else
        {
            roleAttack = logicCombat.FindRole(ref intIndexAttackHero, roleHeros);
            roleAttacks = roleHeros;
            if (roleAttack != null)
            {
                intTempAttack = roleAttack.intIndex;
            }
            roleBeAttack = logicCombat.FindRole(ref intIndexAttackEnemy, roleEnemys, intTempAttack);
        }
        if (roleBeAttack == null || roleAttack == null)
        {
            //其中一方全部阵亡,进行结算
            enumOrder = EnumCombatAttackState.Over;
            logicCombat.SetRolePlayValue(0, 5, true);

            if (roleAttack != null)
            {
                over.booTeam = roleAttack.booTeam;
            }
            else if (roleBeAttack != null)
            {
                over.booTeam = roleBeAttack.booTeam;
            }

            over.intRoleDeath = 0;
            for (int i = 0; i < roleHeros.Length; i++)
            {
                if (roleHeros[i].roleState == EnumCombatRoleState.death)
                {
                    over.intRoleDeath += 1;
                }
            }
            if (intIndexTeam == dungeonPoint.teams.Length - 1)
            {
                //与所有敌人队伍战斗完毕
                over.booOver = true;
                over.booRound = false;
                booCombat = false;
            }
            else
            {
                //还有剩余敌人队伍
                over.booOver = false;
                over.booRound = true;
                intIndexTeam++;
                over.intIndexTeam = intIndexTeam;
                if (over.booTeam)
                {
                    //模型加载 并没有卡住刷新帧率的执行
                    SetEnemyTeam(dungeonPoint, intIndexTeam);
                    for (int i = 0; i < roleEnemys.Length; i++)
                    {
                        ShowRoleInfo(roleEnemys[i], harmBeValue.GetHarmZero());
                    }
                }
                else
                {
                    //敌人获胜
                    booCombat = false;
                }
            }
            ManagerView.Instance.SetData(EnumView.ViewCombat, over);
        }
        else
        {
            enumOrder = EnumCombatAttackState.ForwardMove;
        }
    }

    protected override void ActionInitCombat(int[] intEmployeeIDs, int intDungeonPointIndex, int intDungeonID)
    {
        base.ActionInitCombat(intEmployeeIDs, intDungeonPointIndex, intDungeonID);
        //处理第一次攻击没有伤害的问题
        logicCombat.SetRolePlayValue(0, 2, false);
    }

    void MessageStopCombat(ManagerMessage.MessageBase message)
    {
        booCombat = false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        ManagerValue.actionInitCombat -= ActionInitCombat;
        ManagerMessage.Instance.RemoveEvent(EnumMessage.StopCombat, MessageStopCombat);
    }
}
