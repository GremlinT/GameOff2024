using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableItem : MonoBehaviour
{
    [SerializeField]
    protected Transform usePoint;
    [SerializeField]
    protected Transform cameraPoint, cameraTargetPoint;
    [SerializeField]
    private float treshold;
    [field: SerializeField]
    protected bool canStopManualy;
    [SerializeField]
    string basicInformation, opinionDiscription, dispalyedName;
    [SerializeField]
    private UsableItem parentItem;
    public UsableItem childToUse;

    protected AlienBehavoiur currentUser;
    private CameraScript currentCamera;
    private BaseUIBehavoiur UIBehavoiur;

    public void SetUIBehavoiur(BaseUIBehavoiur _UIBehavoiur)
    {
        UIBehavoiur = _UIBehavoiur;
    }

    public UsableItem SetCurrentItem()
    {
        if (parentItem == null)
        {
            return this;
        }
        else
        {
            parentItem.childToUse = this;
            return parentItem; 
        }
    }
    public virtual void Use(AlienBehavoiur user)
    {
        currentUser = user;
        UIBehavoiur.HideItemName();
        currentCamera = FindObjectOfType<CameraScript>();
        currentCamera.SetCameraTarget(cameraTargetPoint, (cameraPoint.position - cameraTargetPoint.position), false);
        currentUser.LookAt(cameraTargetPoint.position);
        if (childToUse != null)
        {
            childToUse.Use(user);
            childToUse = null;
        }
    }

    public virtual void StopUse()
    {
        currentCamera.SetCameraTarget();
        currentUser = null;
        currentCamera = null;
    }
    
    public bool CanStopManualy()
    {
        if (canStopManualy)
        {
            StopUse();
            return true;
        }
        else return false;
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

    public string MakeOpinion()
    {
        return opinionDiscription;
    }

    private void OnMouseOver()
    {
        if (currentUser == null)
        UIBehavoiur.ShowItemName(dispalyedName);
    }
    private void OnMouseExit()
    {
        UIBehavoiur.HideItemName();
    }
}
