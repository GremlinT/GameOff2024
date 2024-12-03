using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceStation : MonoBehaviour
{
    [SerializeField]
    private Transform[] leavingPath;
    [SerializeField]
    private Transform[] enteringPath;

    public Transform[] SetPath(bool isLeaving)
    {
        if (isLeaving) 
        {
            return leavingPath;
        }
        else
        {
            return enteringPath;
        }
    }
}
