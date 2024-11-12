using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableItem : MonoBehaviour
{
    [SerializeField]
    private Transform usePoint;
    [SerializeField]
    private Transform cameraPoint;
    [SerializeField]
    private float treshold;
    [field: SerializeField]
    public bool canStopManualy { get; private set; }

    public virtual void Use()
    {
        Debug.Log(this.name);
    }
    
    public bool IsCanUsed(Vector3 userPosition)
    {
        if (Vector3.Distance(userPosition, usePoint.position) < treshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector3 UsePoint()
    {
        return usePoint.position;
    }
}
