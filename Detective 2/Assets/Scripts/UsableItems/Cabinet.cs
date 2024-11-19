using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : UsableItem
{
    public override void Use(AlienBehavoiur user)
    {
        base.Use(user);
        currentUser.LookAt(cameraTargetPoint.position);
    }

    public override void StopUse()
    {
        base.StopUse();
    }
}
