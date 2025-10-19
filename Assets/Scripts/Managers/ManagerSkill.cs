using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerSkill
{
    Dictionary<int, PropertiesSkill> dicSkill;
    List<int> listRandom;
    static ManagerSkill _instance;
    public static ManagerSkill Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ManagerSkill();
                string strData = ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableSkill);
                JsonValue.DataSkillBase json = JsonUtility.FromJson<JsonValue.DataSkillBase>(strData);
                _instance.dicSkill = new Dictionary<int, PropertiesSkill>();
                _instance.listRandom = new List<int>();
                JsonValue.DataSkillItem item = null;
                for (int i = 0; i < json.listItems.Count; i++)
                {
                    item = json.listItems[i];
                    PropertiesSkill skill = new PropertiesSkill();
                    skill.intSkillID = item.intSkillID;
                    skill.enumSkill = (PropertiesSkill.EnumSkill)item.intSkillType;
                    skill.combatType = (EnumCombatAttackType)item.intCombatType;
                    skill.strName = item.strNameChina;
                    skill.strICON = item.strICON;
                    skill.strEffect = item.strNameEffect;
                    skill.intValue = item.intValue;
                    skill.intRoleCount = item.intRoleCount;
                    skill.booRandom = item.intRandom > 0 ? true : false;
                    _instance.dicSkill.Add(skill.intSkillID, skill);
                    _instance.listRandom.Add(skill.intSkillID);
                }
            }
            return _instance;
        }
    }
    public PropertiesSkill GetSkillValue(int intSkillID)
    {
        return dicSkill[intSkillID];
    }
    public int GetRandomSkillID()
    {
        return listRandom[Random.Range(0, listRandom.Count)];
    }

    public Sprite GetCombatType(EnumCombatAttackType combatType)
    {
        string strName = "ICONSkills/";
        switch (combatType)
        {
            case EnumCombatAttackType.Attack:
                strName += "1_Attack";
                break;
            case EnumCombatAttackType.Magic:
                strName += "2_Magic";
                break;
            case EnumCombatAttackType.Defense:
                strName += "3_Defense";
                break;
        }
        return ManagerResources.Instance.GetCombatTypeSprite(strName);
    }
}
