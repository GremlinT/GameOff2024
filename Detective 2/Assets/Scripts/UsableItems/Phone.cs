using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : UsableItem
{
    public override void Use(AlienBehavoiur user)
    {
        Debug.Log("ping");
        StartCoroutine(PhoneTalk());
    }

    public override void StopUse()
    {
        Debug.Log("stop");
    }

    private IEnumerator PhoneTalk()
    {
        Debug.Log("1");
        yield return new WaitForSeconds(2);
        Debug.Log("2");
        StopUse();
    }
}
