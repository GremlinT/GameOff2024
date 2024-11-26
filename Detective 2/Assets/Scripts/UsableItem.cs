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
    
    protected AlienBehavoiur currentUser;
    private CameraScript currentCamera;
    protected BaseUIBehavoiur baseUIBehavoiur;

    [SerializeField]
    protected UsableItem parentItem;
    [SerializeField]
    private UsableItem usedChildItem;

    //�����, ������� ��������� ����� ����� �� ������� � ������� �� ������ ���� ������� ���� ��� ������, � ����� ��������� � ������ ������� �� �������������
    public UsableItem SetCurrentItem()
    {
        if (parentItem != null)
        {
            parentItem.usedChildItem = this;
            return parentItem;
        }
        else return this;
    }

    public void SetUIBehavoiur(BaseUIBehavoiur _UIBehavoiur)
    {
        baseUIBehavoiur = _UIBehavoiur;
    }

    //� use ���� �������� ��� ������ ���� � ����������� �� ���� ���� ��� ��� ������������� ������������ �������� ������ 
    public virtual void Use(AlienBehavoiur user)
    {
        currentUser = user;
        baseUIBehavoiur.HideItemName();
        UseIndividual();
        if (usedChildItem != null) UseAsParent(usedChildItem);

        //UIBehavoiur.HideItemName();
        //currentCamera = FindObjectOfType<CameraScript>();
        //currentCamera.SetCameraTarget(cameraTargetPoint, (cameraPoint.position - cameraTargetPoint.position), false);
        //currentUser.LookAt(cameraTargetPoint.position);
    }

    //��� ������, ���������� � ����������� �� ����, ���� ��� ��� ������������ �������� ���� ��� ��������
    public virtual void UseAsParent(UsableItem _usedChildItem)
    {
        _usedChildItem.Use(currentUser);
    }
    public virtual void UseIndividual()
    {
        currentCamera = FindObjectOfType<CameraScript>();
        currentCamera.SetCameraTarget(cameraTargetPoint, (cameraPoint.position - cameraTargetPoint.position), false); //������ ��������� ������������� ������ ��� ��������� ���, ���� �������� � ����������
        currentUser.LookAt(cameraTargetPoint.position);
    }

    public virtual void StopUse()
    {
        currentCamera.SetCameraTarget();
        usedChildItem = null;
        currentUser = null;
        currentCamera = null;
    }
    public void StopUseChild()
    {
        usedChildItem = null;
    }
    
    public bool CanStopManualy()
    {
        if (usedChildItem != null)
        {
            if (usedChildItem.CanStopManualy() && canStopManualy)
            {
                StopUse();
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (canStopManualy)
            {
                StopUse();
                return true;
            }
            else
            {
                return false;
            }
        }
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

    public float SetItemTreshold()
    {
        return treshold;
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
            baseUIBehavoiur.ShowItemName(dispalyedName);
    }
    private void OnMouseExit()
    {
        baseUIBehavoiur.HideItemName();
    }
}
