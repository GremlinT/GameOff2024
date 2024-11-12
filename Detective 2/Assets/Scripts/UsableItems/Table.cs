using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : UsableItem
{
    public override void Use()
    {
        Debug.Log("Table " + this.name);
    }
}
