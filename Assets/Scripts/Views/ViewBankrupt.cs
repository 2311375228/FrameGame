using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBankrupt : ViewBase
{
    public Text textTitle;
    public Text textBankrupt;
    public Text textTimeBankrupt;//破产倒计时

    public Button btnConfirm;

    public GameObject goBankrupt;//破产
    public GameObject goTimeBankrupt;//破产倒计时

    float floTimeBankrupt;

    protected override void Start()
    {
        base.Start();

        btnConfirm.onClick.AddListener(() =>
        {
            ManagerView.Instance.Hide(EnumView.ViewBankrupt);
        });

        textTitle.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Bankruptcy);
        textBankrupt.text = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.WhenYHANBYWGBATGWBO, null);
        textTimeBankrupt.text = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.ResolveHIIBTCE, null);
        btnConfirm.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Bankruptcy);
    }
    public override void Show()
    {
        if (!gameObject.activeSelf)
        {
            floTimeBankrupt = 0;
        }
        base.Show();
        if (ManagerValue.booGamePlaying)
        {
            goTimeBankrupt.SetActive(true);
            goBankrupt.SetActive(false);
        }
        else
        {
            goTimeBankrupt.SetActive(false);
            goBankrupt.SetActive(true);
        }
    }

    protected override void Update()
    {
        if (ManagerValue.booGamePlaying && UserValue.Instance.GetCoin < 0)
        {
            if (floTimeBankrupt < 200)
            {
                floTimeBankrupt += Time.deltaTime;
                textTimeBankrupt.text = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.ResolveHIIBTCE, null) + "\n" + (200 - (int)floTimeBankrupt);//"请在倒计时结束前处理负债累累的问题\n" + (200 - (int)floTimeBankrupt);
            }
            else
            {
                ManagerValue.BackLogin();
                ManagerView.Instance.Show(EnumView.ViewBankrupt);

            }
        }
        else
        {
            ManagerView.Instance.Hide(EnumView.ViewBankrupt);
        }
    }
}
