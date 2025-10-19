using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewHintBar : ViewBase
{
    public GameObject goCopy;
    List<RectTransform> listItem = new List<RectTransform>();

    protected override void Start()
    {
        base.Start();

        goCopy.SetActive(false);
    }

    int intIndex;
    float floTime;
    protected override void Update()
    {
        floTime += Time.deltaTime;
        if (floTime > 5)
        {
            ManagerView.Instance.Hide(EnumView.ViewHintBar);
            floTime = 0;
            intIndex = 0;

            for (int i = 0; i < listItem.Count; i++)
            {
                listItem[i].gameObject.SetActive(false);
            }
        }
    }

    public override void SetData(Message message)
    {
        MessageHintBar bar = message as MessageHintBar;
        if (bar != null)
        {
            if (listItem.Count <= intIndex)
            {
                GameObject goTemp = Instantiate(goCopy, goCopy.transform.parent, false);
                listItem.Add(goTemp.GetComponent<RectTransform>());
            }
            listItem[intIndex].GetComponent<View_PropertiesItem>().textValueMain.text = bar.strHintBar;
            listItem[intIndex].gameObject.SetActive(true);

            floTime = 0;
            intIndex++;
        }
    }

    public class MessageHintBar : Message
    {
        public string strHintBar;
    }
}
