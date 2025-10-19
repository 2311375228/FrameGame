using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCombatBase : MonoBehaviour
{
    [SerializeField]
    protected GameObject[] goPositionEnemy;
    [SerializeField]
    protected GameObject[] goPositionEmployee;//

    [SerializeField]
    protected Camera cameraPicture;

    protected bool booCombat = false;
    protected int intIndexAttackEnemy;//攻击者
    protected int intIndexAttackHero;//攻击者
    protected int intRoleTotal;//有效角色个数
    protected EnumCombatAttackState enumOrder;

    //需要战胜所有敌人队伍
    protected bool booAttack;//攻击队伍切换
    protected CombatStructValue harmValue;
    protected CombatStructValue harmBeValue;
    protected CombatRole roleAttack;
    protected CombatRole[] roleAttacks;
    protected CombatRole roleBeAttack;
    protected CombatRole[] roleBeAttacks;

    protected int intIndexTeam;//被攻击的敌人队伍下标,切换队伍用
    protected CombatRole[] roleEnemys;
    protected CombatRole[] roleHeros;
    protected PropertiesDungeon.DungeonPoint dungeonPoint;
    Vector3[] vecEnemyPositions = new Vector3[5];
    Vector3[] vecEnemyAngles = new Vector3[5];
    Vector3[] vecEnemyScales = new Vector3[5];

    protected CombatOver over = new CombatOver();

    //显示Role数值
    protected CombatRoleShowData roleShowData = new CombatRoleShowData();

    protected virtual void Start()
    {
        //初始化 敌人 员工 信息
        roleEnemys = new CombatRole[goPositionEnemy.Length];
        roleHeros = new CombatRole[goPositionEmployee.Length];
        int intIndexShowData = 0;
        for (int i = 0; i < roleEnemys.Length; i++)
        {
            roleEnemys[i] = new CombatRole();
            roleEnemys[i].intIndex = i;
            roleEnemys[i].intIndexShowData = intIndexShowData++;
            roleEnemys[i].roleState = EnumCombatRoleState.Idle;
            roleEnemys[i].goRole = goPositionEnemy[i];
            roleEnemys[i].intSkillRanks = new int[4];
            roleEnemys[i].proSkills = new PropertiesSkill[4];

            roleEnemys[i].vecPosition = goPositionEnemy[i].transform.position;
            roleEnemys[i].vecPositionBeCombat = roleEnemys[i].vecPosition;
            roleEnemys[i].vecPositionBeCombat.z -= 1;
            roleEnemys[i].vecPositionBack = roleEnemys[i].vecPosition;
            roleEnemys[i].vecPositionBack.z -= 3;

            vecEnemyPositions[i] = goPositionEnemy[i].transform.position;
            vecEnemyAngles[i] = goPositionEnemy[i].transform.eulerAngles;
            vecEnemyScales[i] = goPositionEnemy[i].transform.localScale;
        }
        for (int i = 0; i < roleHeros.Length; i++)
        {
            roleHeros[i] = new CombatRole();
            roleHeros[i].intIndex = i;
            roleHeros[i].intIndexShowData = intIndexShowData++;
            roleHeros[i].roleState = EnumCombatRoleState.Idle;
            roleHeros[i].goRole = goPositionEmployee[i];

            roleHeros[i].vecPosition = goPositionEmployee[i].transform.position;
            roleHeros[i].vecPositionBeCombat = roleHeros[i].vecPosition;
            roleHeros[i].vecPositionBeCombat.z += 1;
            roleHeros[i].vecPositionBack = roleHeros[i].vecPosition;
            roleHeros[i].vecPositionBack.z += 3;
        }
    }

    protected virtual void Update()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void ActionInitCombat(int[] intEmployeeIDs, int intDungeonPointIndex, int intDungeonID)
    {
        booAttack = true;
        intIndexAttackEnemy = 0;
        intIndexAttackHero = 0;

        intIndexTeam = 0;
        booCombat = true;
        enumOrder = EnumCombatAttackState.Ready;

        over.booOver = false;
        over.booRound = false;
        over.booTeam = true;
        over.intRoundCount = 0;
        over.intRoleDeath = 0;
        over.intIndexTeam = 0;

        PropertiesDungeon itemDungeon = UserValue.Instance.dicDungeon[intDungeonID];
        over.intDungeonID = intDungeonID;
        over.intDungeonIndex = intDungeonPointIndex;
        ManagerView.Instance.SetData(EnumView.ViewCombat, over);

        dungeonPoint = itemDungeon.points[intDungeonPointIndex];
        SetEnemyTeam(dungeonPoint, intIndexTeam);

        for (int i = 0; i < intEmployeeIDs.Length; i++)
        {
            if (intEmployeeIDs[i] != -1)
            {
                PropertiesEmployee item = UserValue.Instance.GetEmployeeValue(intEmployeeIDs[i]);
                roleHeros[i].roleState = EnumCombatRoleState.Idle;
                roleHeros[i].intHP = item.intHP;
                roleHeros[i].intHPMax = item.intHP;
                roleHeros[i].intATK = item.intATK;
                roleHeros[i].intMP = item.intMP;
                roleHeros[i].combatType = item.combatAttackType;
                roleHeros[i].intSkillRanks = item.intSkillRanks;
                roleHeros[i].proSkills = item.proSkills;
                roleHeros[i].goRole.transform.position = roleHeros[i].vecPosition;
                roleHeros[i].goRole.SetActive(true);

                roleHeros[i].SetAnim();
            }
            else
            {
                roleHeros[i].roleState = EnumCombatRoleState.None;
                roleHeros[i].goRole.SetActive(false);
            }
            roleHeros[i].booTeam = true;
            ShowRoleInfo(roleHeros[i], harmValue.GetHarmZero());
            roleHeros[i].goRole.transform.localEulerAngles = Vector3.zero;
        }

        StartCoroutine(IEWaitRole());
    }

    IEnumerator IEWaitRole()
    {
        yield return 0;
        for (int i = 0; i < roleEnemys.Length; i++)
        {
            ShowRoleInfo(roleEnemys[i], harmBeValue.GetHarmZero());

            if (roleEnemys[i].roleState != EnumCombatRoleState.None)
            {
                roleEnemys[i].SetPlayAction(CombatRole.EnumPlayAction.Attack, false);
                roleEnemys[i].SetPlayAction(CombatRole.EnumPlayAction.BeAttack, false);
                roleEnemys[i].SetPlayAction(CombatRole.EnumPlayAction.Death, false);
                roleEnemys[i].SetPlayAction(CombatRole.EnumPlayAction.Run, false);
                roleEnemys[i].SetPlayAction(CombatRole.EnumPlayAction.Skill, false);
            }
        }
        for (int i = 0; i < roleHeros.Length; i++)
        {
            ShowRoleInfo(roleHeros[i], harmBeValue.GetHarmZero());

            if (roleHeros[i].roleState != EnumCombatRoleState.None)
            {
                roleHeros[i].SetPlayAction(CombatRole.EnumPlayAction.Attack, false);
                roleHeros[i].SetPlayAction(CombatRole.EnumPlayAction.BeAttack, false);
                roleHeros[i].SetPlayAction(CombatRole.EnumPlayAction.Death, false);
                roleHeros[i].SetPlayAction(CombatRole.EnumPlayAction.Run, false);
                roleHeros[i].SetPlayAction(CombatRole.EnumPlayAction.Skill, false);
            }
        }
    }

    protected void SetEnemyTeam(PropertiesDungeon.DungeonPoint point, int intIndexTeam)
    {
        for (int i = 0; i < roleEnemys.Length; i++)
        {
            roleEnemys[i].roleState = EnumCombatRoleState.None;
            Destroy(roleEnemys[i].goRole);
        }

        if (point.teams[intIndexTeam].intIDs.Length > roleEnemys.Length)
        {
            return;
        }
        for (int i = 0; i < point.teams[intIndexTeam].intIDs.Length; i++)
        {
            JsonValue.DataTableEnemyItem enemy = ManagerCombat.Instance.GetEnemyItem(point.teams[intIndexTeam].intIDs[i]);
            roleEnemys[i].goRole = Instantiate(ManagerResources.Instance.GetEnemyModel(enemy.strModelName));
            roleEnemys[i].goRole.transform.position = roleEnemys[i].vecPosition;
            roleEnemys[i].goRole.transform.eulerAngles = vecEnemyAngles[i];
            roleEnemys[i].goRole.transform.localScale = Vector3.one;// vecEnemyScales[i];
            roleEnemys[i].roleState = EnumCombatRoleState.Idle;
            roleEnemys[i].intHP = ManagerValue.EnemyHP(point.intWinCount, enemy.intHP); //enemy.intHP;
            roleEnemys[i].intHPMax = ManagerValue.EnemyHP(point.intWinCount, enemy.intHP);//enemy.intHP;
            roleEnemys[i].intATK = enemy.intAttack;
            roleEnemys[i].intMP = enemy.intMagic;
            roleEnemys[i].combatType = (EnumCombatAttackType)enemy.intCombatType;
            if (roleEnemys[i].goRole != null)
            {
                roleEnemys[i].goRole.transform.position = roleEnemys[i].vecPosition;
            }

            int intSkillCount = RandomSkillCount(roleEnemys[i].proSkills.Length);
            for (int j = 0; j < roleEnemys[i].proSkills.Length; j++)
            {
                if (j < intSkillCount)
                {
                    int intSkillIDTemp = ManagerSkill.Instance.GetRandomSkillID();
                    roleEnemys[i].proSkills[j] = ManagerSkill.Instance.GetSkillValue(intSkillIDTemp);
                    roleEnemys[i].intSkillRanks[j] = RandomSkillRank();
                }
                else
                {
                    roleEnemys[i].proSkills[j] = null;
                    roleEnemys[i].intSkillRanks[j] = -1;
                }
            }
            roleEnemys[i].booTeam = false;
            ShowRoleInfo(roleEnemys[i], harmBeValue.GetHarmZero());
            if (roleEnemys[i].goRole != null)
            {
                roleEnemys[i].SetAnim();
                roleEnemys[i].goRole.transform.localEulerAngles = new Vector3(0, 180, 0);
            }
        }
    }

    protected void ShowRoleInfo(CombatRole role, CombatStructValue harmValue)
    {
        roleShowData.combatState = role.roleState;
        roleShowData.intIndex = role.intIndexShowData;
        roleShowData.damageType = EnumCombatDamageType.None;
        roleShowData.intHP = role.intHP;
        roleShowData.intHPMax = role.intHPMax;
        roleShowData.intATK = role.intATK;
        roleShowData.intMP = role.intMP;
        roleShowData.combatType = role.combatType;
        roleShowData.intSkillRanks = role.intSkillRanks;
        roleShowData.vecPosition = role.vecPosition;
        roleShowData.vecHarmShow = role.vecPositionBeCombat;

        roleShowData.harmValue = harmValue;

        if (role.proSkills != null)
        {
            roleShowData.intSkillLength = role.proSkills.Length;
        }
        roleShowData.proSkills = role.proSkills;

        ManagerValue.actionCombatShow(roleShowData);
    }

    int RandomSkillCount(int intSkillCount)
    {
        int intTemp = Random.Range(0, 100);
        if (intTemp < 10)
        {
            intTemp = 0;
        }
        else if (intTemp >= 10 && intTemp < 50)
        {
            intTemp = 1;
        }
        else if (intTemp >= 50 && intTemp < 90)
        {
            intTemp = 2;
        }
        else if (intTemp >= 90 && intTemp < 99)
        {
            intTemp = 3;
        }
        else if (intTemp >= 98)
        {
            intTemp = 4;
        }
        else
        {
            intTemp = 0;
        }
        return intTemp;
    }
    int RandomSkillRank()
    {
        int intTemp = Random.Range(0, 10000);
        if (intTemp < 10)
        {
            intTemp = 0;
        }
        else if (intTemp >= 10 && intTemp < 5000)
        {
            intTemp = Random.Range(1, 9);
        }
        else if (intTemp >= 5000 && intTemp < 8000)
        {
            intTemp = Random.Range(9, 12);
        }
        else if (intTemp >= 8000 && intTemp < 9998)
        {
            intTemp = Random.Range(12, 15);
        }
        else if (intTemp >= 9998)
        {
            intTemp = 15;
        }
        else
        {
            intTemp = 0;
        }
        return intTemp;
    }

    protected virtual void OnDestroy()
    {
    }
}
