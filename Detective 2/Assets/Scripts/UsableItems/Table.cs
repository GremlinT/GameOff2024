using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : UsableItem
{
    public override void Use(AlienBehavoiur user)
    {
        base.Use(user);
        Debug.Log("table!");
    }

    public override void StopUse()
    {
        base.StopUse();
    }
}
