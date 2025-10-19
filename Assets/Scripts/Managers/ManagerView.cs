using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerView : TSingleton<ManagerView>
{
    Transform rootView;
    Dictionary<EnumView, ViewBase> dicView = new Dictionary<EnumView, ViewBase>();

    public void Show(EnumView enumKey)
    {
        if (dicView.ContainsKey(enumKey) && dicView[enumKey] == null)
        {
            if (rootView == null)
            {
                rootView = GameObject.Find("Canvas").transform;
            }

            GameObject goView = Resources.Load<GameObject>("ViewPrefabs/" + enumKey.ToString());
            goView = GameObject.Instantiate(goView, rootView, false);
            dicView[enumKey] = goView.GetComponent<ViewBase>();
        }
        else if (!dicView.ContainsKey(enumKey))
        {
            if (rootView == null)
            {
                rootView = GameObject.Find("Canvas").transform;
            }

            GameObject goView = Resources.Load<GameObject>("ViewPrefabs/" + enumKey.ToString());
            goView = GameObject.Instantiate(goView, rootView, false);
            dicView.Add(enumKey, goView.GetComponent<ViewBase>());
        }

        dicView[enumKey].Show();

        //将显示条置与顶层
        if (dicView.ContainsKey(EnumView.ViewHintBar))
        {
            dicView[EnumView.ViewHintBar].transform.SetSiblingIndex(dicView[EnumView.ViewHintBar].transform.parent.childCount - 1);
        }
    }

    public void ChangeLanguage()
    {
        foreach (ViewBase temp in dicView.Values)
        {
            if (temp.gameObject.activeSelf)
            {
                temp.Show();
            }
        }
    }

    public ViewBase GetAndShow(EnumView enumKey)
    {
        Show(enumKey);
        return dicView[enumKey];
    }

    public ViewBase GetView(EnumView enumKey)
    {
        if (dicView.ContainsKey(enumKey))
        {
            return dicView[enumKey];
        }
        return null;
    }

    public void Hide(EnumView enumKey)
    {
        if (dicView.ContainsKey(enumKey) && dicView[enumKey] != null)
        {
            dicView[enumKey].Hide();
        }
    }
    public void HideAll()
    {
        foreach (KeyValuePair<EnumView, ViewBase> temp in dicView)
        {
            temp.Value.Hide();
        }
    }
    public void SetData(EnumView enumKey, ViewBase.Message message)
    {
        if (dicView.ContainsKey(enumKey) && dicView[enumKey] != null)
        {
            dicView[enumKey].SetData(message);
        }
    }

    public void Remove(EnumView enumKey)
    {
        if (dicView.ContainsKey(enumKey))
        {
            GameObject.Destroy(dicView[enumKey].gameObject);
            dicView.Remove(enumKey);
        }
    }
    public void RemoveAll()
    {
        List<ViewBase> listTemp = new List<ViewBase>();
        foreach (ViewBase temp in dicView.Values)
        {
            listTemp.Add(temp);
        }
        for (int i = 0; i < listTemp.Count; i++)
        {
            GameObject.Destroy(listTemp[i].gameObject);
        }
        dicView.Clear();
    }
}
