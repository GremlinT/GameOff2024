using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV : UsableItem
{
    public override void Use(AlienBehavoiur user)
    {
        cameraPoint.position = user.transform.position + Vector3.up * 1.8f;
        base.Use(user);
        currentUser.LookAt(cameraTargetPoint.position);
    }

    public override void StopUse()
    {
        base.StopUse();
    }
}
