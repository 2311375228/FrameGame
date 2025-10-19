using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerResources : TSingleton<ManagerResources>
{
    Dictionary<string, Sprite> dicBackpackSprite = new Dictionary<string, Sprite>();
    Dictionary<string, GameObject> dicGo = new Dictionary<string, GameObject>();
    Dictionary<string, string> dicString = new Dictionary<string, string>();
    Dictionary<EnumEmployeeProperties, Sprite> dicEmployeeProperties = new Dictionary<EnumEmployeeProperties, Sprite>();
    Dictionary<string, Sprite> dicRoleCombatTypeSprite = new Dictionary<string, Sprite>();
    Dictionary<string, Sprite> dicSkillSprite = new Dictionary<string, Sprite>();
    Dictionary<string, Sprite> dicEquipmentSprite = new Dictionary<string, Sprite>();
    Dictionary<string, Sprite> dicIconFrameRank = new Dictionary<string, Sprite>();
    Dictionary<string, Sprite[]> dicBuildSprite = new Dictionary<string, Sprite[]>();
    Dictionary<string, Material> dicOtherMaterial = new Dictionary<string, Material>();
    Dictionary<string, Texture2D> dicTexture2D = new Dictionary<string, Texture2D>();
    public Sprite GetBackpackSprite(string strICON)
    {
        if (!dicBackpackSprite.ContainsKey(strICON))
        {
            dicBackpackSprite.Add(strICON, Resources.Load<Sprite>("ICONS/" + strICON));
        }
        return dicBackpackSprite[strICON];
    }
    public GameObject GetBuildGameObject(string strGo)
    {
        if (!dicGo.ContainsKey(strGo))
        {
            dicGo.Add(strGo, Resources.Load<GameObject>("BuyBuild/" + strGo));
        }
        return dicGo[strGo];
    }
    public string GetTextAssetString(string strPath, EnumTableName tableName)
    {
        string strKey = strPath + (int)tableName;
        if (!dicString.ContainsKey(strKey))
        {
            string str = Tools.GetFileString(Resources.Load<TextAsset>(strKey).bytes);
            dicString.Add(strKey, str);
        }
        return dicString[strKey];
    }
    public Sprite GetEmployeeProperties(EnumEmployeeProperties enumKey)
    {
        if (!dicEmployeeProperties.ContainsKey(enumKey))
        {
            string strPath = "ICONS/EmployeeProperties" + (int)enumKey;
            dicEmployeeProperties.Add(enumKey, Resources.Load<Sprite>(strPath));
        }
        return dicEmployeeProperties[enumKey];
    }
    public Sprite GetCombatTypeSprite(string strPath)
    {
        if (!dicRoleCombatTypeSprite.ContainsKey(strPath))
        {
            dicRoleCombatTypeSprite.Add(strPath, Resources.Load<Sprite>(strPath));
        }
        return dicRoleCombatTypeSprite[strPath];
    }
    public Sprite GetSkillSprite(string strIconName)
    {
        if (!dicSkillSprite.ContainsKey(strIconName))
        {
            dicSkillSprite.Add(strIconName, Resources.Load<Sprite>("ICONSkills/" + strIconName));
        }
        return dicSkillSprite[strIconName];
    }
    public Sprite GetEquipmentSprite(string strICON)
    {
        if (!dicEquipmentSprite.ContainsKey(strICON))
        {
            dicEquipmentSprite.Add(strICON, Resources.Load<Sprite>("ICONEquipment/" + strICON));
        }
        return dicEquipmentSprite[strICON];
    }
    public GameObject GetEnemyModel(string strModelName)
    {
        return Resources.Load<GameObject>("Enemy/" + strModelName);
    }
    public GameObject GetMapModel(string strModelMap)
    {
        return Resources.Load<GameObject>("Map/" + strModelMap);
    }
    public Sprite GetFrameRank(string strRank)
    {
        if (!dicIconFrameRank.ContainsKey(strRank))
        {
            dicIconFrameRank.Add(strRank, Resources.Load<Sprite>("IconRank/" + strRank));
        }
        return dicIconFrameRank[strRank];
    }
    public Sprite[] GetBuildSprite(string strModelName)
    {
        if (!dicBuildSprite.ContainsKey(strModelName))
        {
            List<Sprite> listTemp = new List<Sprite>();
            for (int i = 0; i < 10; i++)
            {
                Sprite s = Resources.Load<Sprite>("BuildSprite/" + strModelName + i);
                if (s == null)
                {
                    break;
                }
                listTemp.Add(s);
            }
            dicBuildSprite.Add(strModelName, listTemp.ToArray());
        }
        return dicBuildSprite[strModelName];
    }
    public Material GetMaterial(string strName)
    {
        if (!dicOtherMaterial.ContainsKey(strName))
        {
            dicOtherMaterial.Add(strName, Resources.Load<Material>("Other/productSaleShop"));
        }
        return dicOtherMaterial[strName];
    }
    public Texture2D GetTexture2D(string strPath)
    {
        if (!dicTexture2D.ContainsKey(strPath))
        {
            dicTexture2D.Add(strPath, Resources.Load<Texture2D>("ICONS/" + strPath));
        }
        return dicTexture2D[strPath];
    }
}
