using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ViewBase : MonoBehaviour
{
    protected ViewHint.MessageHint viewHint = new ViewHint.MessageHint();
    protected ViewMGToEmployeeAdd mgViewEmployeeAdd = new ViewMGToEmployeeAdd();
    Animator anim;
    protected virtual void Awake()
    {

    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {

    }
    protected virtual void LateUpdate()
    {

    }
    protected virtual void FixedUpdate()
    {

    }

    public virtual void Hide()
    {
        //if (anim == null)
        //{
        //    anim = GetComponent<Animator>();
        //}
        //anim.SetBool("Show", false);

        gameObject.SetActive(false);
    }
    public virtual void Show()
    {
        //if (anim == null)
        //{
        //    anim = GetComponent<Animator>();
        //}
        //anim.SetBool("Show", true);

        transform.SetSiblingIndex(transform.parent.childCount - 1);
        gameObject.SetActive(true);
    }
    public virtual void SetData(Message message)
    {

    }

    protected void EmployeeAdd(EmployeeData employeeData)
    {
        if (employeeData.booEmployee)
        {
            int intEmployeeNum = UserValue.Instance.GetIdleEmployeeNum();
            if (intEmployeeNum == 0)
            {
                ManagerView.Instance.Show(EnumView.ViewHint);
                viewHint.strHint = ManagerLanguage.Instance.GetWord(EnumLanguageWords.ThereANAE);//"没有空闲员工";
                ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
                return;
            }
            else if (intEmployeeNum == -1)
            {
                ManagerView.Instance.Show(EnumView.ViewHint);
                string strBuildName1 = ManagerBuild.Instance.GetBuildName(4004);
                string strBuildName2 = ManagerBuild.Instance.GetBuildName(4003);
                string strBuildName3 = ManagerBuild.Instance.GetBuildName(4002);
                viewHint.strHint = ManagerLanguage.Instance.GetWord(EnumLanguageWords.PleaseConstruct) + strBuildName1;//"请建造:" + strBuildName1;
                //viewHint.strHint = "请建造一下任何一种类型建筑：\n1 " + strBuildName1 + "\n2 " + strBuildName2 + "\n3 " + strBuildName3;
                ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
                return;
            }

            ManagerView.Instance.Show(EnumView.ViewEmployeeAdd);

            mgViewEmployeeAdd.enumView = employeeData.enumView;
            mgViewEmployeeAdd.enumEmployeeProperties = employeeData.enumEmployeeProperties;
            mgViewEmployeeAdd.dicPropertiesInfo = employeeData.dicPropertiesInfo;
            mgViewEmployeeAdd.strBuildName = employeeData.strBuildName;
            ManagerView.Instance.SetData(EnumView.ViewEmployeeAdd, mgViewEmployeeAdd);
        }
    }

    protected void GetScrollColumn(int intCount, GameObject goCopy, RectTransform scrollRectContent, List<RectTransform> listItem)
    {
        GridLayoutGroup gridGroup = scrollRectContent.GetComponent<GridLayoutGroup>();
        float floItemHeight = gridGroup.cellSize.y + gridGroup.spacing.y;
        for (int i = 0; i < intCount; i++)
        {
            if (listItem.Count <= i)
            {
                RectTransform rect = Instantiate(goCopy, goCopy.transform.parent, false).GetComponent<RectTransform>();
                listItem.Add(rect);
            }
        }

        Vector2 vecTemp = scrollRectContent.sizeDelta;
        vecTemp.y = (intCount + 1) * floItemHeight + 20;
        scrollRectContent.sizeDelta = vecTemp;
    }
    protected void GetColumnItem(int intCount, GameObject goCopy, List<RectTransform> listItem)
    {
        for (int i = 0; i < intCount; i++)
        {
            if (listItem.Count <= i)
            {
                RectTransform rect = Instantiate(goCopy, goCopy.transform.parent, false).GetComponent<RectTransform>();
                listItem.Add(rect);
            }
        }
    }

    protected class EmployeeData
    {
        public bool booEmployee;//是否有员工
        public int intIndex;
        public EnumView enumView;
        public string strBuildName;
        public EnumEmployeeProperties[] enumEmployeeProperties;
        public Dictionary<EnumEmployeeProperties, string> dicPropertiesInfo;
        public EventBuildToViewBase mgView;
    }

    public abstract class Message
    { }
}
