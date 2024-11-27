using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : UsableItem
{
    [SerializeField]
    private GameObject monitor, keyboard;
    
    private bool isOn;

    private void Start()
    {
        isOn = false;
    }
    public override void UseIndividual()
    {
        isOn = !isOn;
        monitor.SetActive(isOn);
        keyboard.SetActive(isOn);
        StopUse();
    }

    public override void StopUse()
    {
        currentUser = null;
        parentItem.StopUseChild();
    }
}
