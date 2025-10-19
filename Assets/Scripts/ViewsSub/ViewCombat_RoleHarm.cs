using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewCombat_RoleHarm : MonoBehaviour
{
    public Text textHarm;
    [System.NonSerialized]
    public Vector3 vecTargetHeight;
    public System.Action<ViewCombat_RoleHarm> actionShow;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, vecTargetHeight, 100 * Time.deltaTime);
        if (Vector3.Distance(transform.position, vecTargetHeight) < 1)
        {
            gameObject.SetActive(false);
            actionShow(this);
        }
    }
}