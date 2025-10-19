using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewTask_SubItem : MonoBehaviour
{
    public Text textTaskName;
    public Text textInfo;
    public Button btnFinish;

    public GameObject goSelect;

    [System.NonSerialized]
    public RectTransform rectItemRoot;
    RectTransform rectSelf;

    public int intIndex;

    float floDis;
    void Start()
    {
        rectSelf = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        floDis = Vector2.Distance(rectItemRoot.position, rectSelf.position);
        if (floDis > 0.1f)
        {
            rectSelf.position = Vector2.MoveTowards(rectSelf.position, rectItemRoot.position, 700 * Time.deltaTime);
        }
    }
}
