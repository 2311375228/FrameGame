using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_BuyBuild : ViewBuild_Base
{
    public RectTransform rectBtnRoot;

    List<GameObject> listGoBuild = new List<GameObject>();
    ViewBuild_BuildSelect select;
    Message messageBuildType = new Message();
    public override void Show()
    {
        base.Show();

        if (listGoBuild.Count == 0)
        {
            for (int i = 0; i < rectBtnRoot.childCount; i++)
            {
                listGoBuild.Add(rectBtnRoot.GetChild(i).gameObject);
                listGoBuild[i].GetComponent<Button>().onClick.AddListener(OnClickBuildItem(i));
            }
        }

        for (int i = 0; i < listGoBuild.Count; i++)
        {
            listGoBuild[i].SetActive(true);
            switch (i)
            {
                case 0:
                    listGoBuild[i].transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Farm);
                    break;
                case 1:
                    listGoBuild[i].transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Pasture);
                    break;
                case 2:
                    listGoBuild[i].transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Workshop);
                    break;
                case 3:
                    listGoBuild[i].transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Other);
                    break;
                default:
                    listGoBuild[i].SetActive(false);
                    break;
            }
        }

        if (select == null)
        {
            select = getCentre(EnumViewCentre.ViewBuild_BuildSelect) as ViewBuild_BuildSelect;
            select.Show();
        }
        select.Hide();
    }

    public override void Hide()
    {
        base.Hide();

        select.Hide();
    }

    public override void BuildMessage(EventBuildToViewBase message)
    {
        select.BuildMessage(message);
    }

    UnityEngine.Events.UnityAction OnClickBuildItem(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            messageBuildType.intBuildType = intIndex + 1;
            select.Show();
            select.SetData(messageBuildType);
        };
    }

    public class Message : ViewBase.Message
    {
        public int intBuildType;
    }
}
