using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : UsableItem
{
    [SerializeField]
    protected Transform TR;

    [SerializeField]
    private Transform wearingPoint;

    public override void UseIndividual()
    {
        canStopManualy = true;
        if (wearingPoint != null)
        {
            TR.SetParent(wearingPoint);
            TR.localPosition = Vector3.zero;
            TR.rotation = wearingPoint.rotation;
        }
        else
        {
            TR.SetParent(currentUser.transform);
            TR.localPosition = Vector3.zero;
            this.enabled = false;
        }
        currentUser.TakeItem(this);
    }
    public override void StopUse()
    {
        currentUser = null;
    }
}
