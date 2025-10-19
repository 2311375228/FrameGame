using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewHouse_TowerItem : ColumnItemBase
{
    public Text textContent;
    public Text textMoney;

    [SerializeField]
    Button btnSelect;

    // Start is called before the first frame update
    void Start()
    {
        btnSelect.onClick.AddListener(() => { actionBase(numIndexItem, numIndexData); });
    }
}
