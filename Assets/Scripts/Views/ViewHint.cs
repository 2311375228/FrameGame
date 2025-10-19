using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewHint : ViewBase
{
    public Button btnClose;
    public Button btnConfirm;
    public Text textHint;

    MessageHint mgHint;
    protected override void Start()
    {
        base.Start();

        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            mgHint.actionConfirm = null;
            ManagerView.Instance.Hide(EnumView.ViewHint);
        });
        btnConfirm.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            if (mgHint.actionConfirm != null)
            {
                mgHint.actionConfirm();
            }
            mgHint.actionConfirm = null;
            ManagerView.Instance.Hide(EnumView.ViewHint);
        });

    }

    public override void Show()
    {
        base.Show();

        btnConfirm.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Confirm);
    }

    public override void SetData(Message message)
    {
        mgHint = message as MessageHint;
        if (mgHint != null)
        {
            textHint.text = mgHint.strHint;
        }
    }

    public class MessageHint : ViewBase.Message
    {
        public string strHint;
        public System.Action actionConfirm;
    }
}
