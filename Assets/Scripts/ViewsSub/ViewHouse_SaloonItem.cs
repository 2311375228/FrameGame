using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewHouse_SaloonItem : ColumnItemBase
{
    public Text textContent;
    public Text textWork;

    [SerializeField]
    Button btnEmploy;
    [SerializeField]
    Button btnEmployeeSee;

    public System.Action<int,int> actionEmploy;

    // Start is called before the first frame update
    void Start()
    {
        btnEmploy.onClick.AddListener(() => { actionEmploy(numIndexItem, numIndexData); });
        btnEmployeeSee.onClick.AddListener(() => { actionBase(numIndexItem, numIndexData); });
    }

}
