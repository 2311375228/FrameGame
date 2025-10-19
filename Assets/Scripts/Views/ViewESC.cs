using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewESC : ViewBase
{
    public Button btnSaveGame;
    public Button btnBackLogin;
    public Button btnLeaveGame;

    public Button btnCloseMenu;
    public Button btnCloseSaveGame;

    public GameObject goSaveGame;

    List<Text> listSvaeGameText = new List<Text>();

    SaveGameWrite writeGame = new SaveGameWrite();

    ViewHintBar.MessageHintBar bar = new ViewHintBar.MessageHintBar();
    protected override void Start()
    {
        //保存为当前存档,如果没有需要取名字
        btnSaveGame.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            writeGame.SaveGame();
        });
        //是否保存 然后回到登陆界面
        btnBackLogin.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Unable);
            viewHint.strHint = ManagerLanguage.Instance.GetWord(EnumLanguageWords.DoYWTRTL);//"是否返回登陆";
            viewHint.actionConfirm = () =>
            {
                ManagerValue.BackLogin();
            };
            ManagerView.Instance.Show(EnumView.ViewHint);
            ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
        });
        //是否保存 然后退出游戏
        btnLeaveGame.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Unable);
            viewHint.strHint = ManagerLanguage.Instance.GetWord(EnumLanguageWords.DoYWTETG);//"是否退出游戏";
            viewHint.actionConfirm = () =>
            {
                writeGame.SaveGame();
                Application.Quit();
            };
            ManagerView.Instance.Show(EnumView.ViewHint);
            ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);

        });

        btnCloseMenu.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            ManagerView.Instance.Hide(EnumView.ViewESC);
        });
        btnCloseSaveGame.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            goSaveGame.SetActive(false);
        });

        for (int i = 0; i < goSaveGame.transform.GetChild(0).childCount; i++)
        {
            listSvaeGameText.Add(goSaveGame.transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Text>());
            goSaveGame.transform.GetChild(0).GetChild(i).GetChild(1).GetComponent<Button>().onClick.AddListener(OnClickDelete(i));

        }
        goSaveGame.SetActive(false);
    }

    public override void Show()
    {
        base.Show();

        goSaveGame.SetActive(false);

        if (ManagerValue.booGamePlaying)
        {
            btnSaveGame.gameObject.SetActive(true);
            btnBackLogin.gameObject.SetActive(true);
        }
        else
        {
            btnSaveGame.gameObject.SetActive(false);
            btnBackLogin.gameObject.SetActive(false);
        }

        btnSaveGame.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.SaveGame);
        btnBackLogin.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.ReturnTLogin);
        btnLeaveGame.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.ExitGame);
    }

    UnityEngine.Events.UnityAction OnClickDelete(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Unable);
            goSaveGame.SetActive(false);
        };
    }
}
