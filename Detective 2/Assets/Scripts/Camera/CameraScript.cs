using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Transform TR;
    [SerializeField]
    Transform playerCameraTarget;
    [SerializeField]
    private float minDistance, maxDistance, minRotateAngle, minGroudDistance;

    [SerializeField]
    private float rotateSpeed;
    
    private Transform cameraTarget;
    
    [SerializeField]
    private Vector3 baseCameraRelativeVector;
    private Vector3 currentCameraRelativeVector;
    [SerializeField]
    private Vector3 cameraRelativeVector;

    private bool isFreeLook;


    private void Start()
    {
        TR = transform;
        SetCameraTarget(true);
    }

    public void SetCameraTarget(Transform _cameraTarget, bool _isFreeLook)
    {
        cameraTarget = _cameraTarget;
        isFreeLook = _isFreeLook;
    }

    public void SetCameraTarget(Transform _cameraTarget, Vector3 _cameraVector, bool _isFreeLook)
    {
        cameraTarget = _cameraTarget;
        currentCameraRelativeVector = cameraRelativeVector;
        cameraRelativeVector = _cameraVector;
        isFreeLook = _isFreeLook;
    }

    public void SetCameraTarget(bool hasManyCameraChanges)
    {
        cameraTarget = playerCameraTarget;
        if (hasManyCameraChanges)
        {
            cameraRelativeVector = baseCameraRelativeVector;
        }
        else
        {
            cameraRelativeVector = currentCameraRelativeVector;
        }
        isFreeLook = true;
    }

    private void CameraRotate(float rotate, Vector3 rotateVector)
    {
        TR.RotateAround(cameraTarget.position, rotateVector, rotate * rotateSpeed);
        UpdateCameraRelativeVector();
    }
    private void CameraZoom(float zoom)
    {
        TR.Translate(Vector3.forward * zoom);
        UpdateCameraRelativeVector();
    }

    private void UpdateCameraRelativeVector()
    {
        cameraRelativeVector = TR.position - cameraTarget.position;
    }

    private void CameraFollow()
    {
        TR.position = Vector3.Lerp(TR.position, cameraTarget.position + cameraRelativeVector, Time.deltaTime);
        TR.LookAt(cameraTarget);
    }
    private void Update()
    {
        if (isFreeLook)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                float zoom = Input.GetAxis("Mouse ScrollWheel");
                if (zoom < 0 && Vector3.Distance(TR.position, cameraTarget.position) < maxDistance)
                    CameraZoom(zoom);
                if (zoom > 0 && Vector3.Distance(TR.position, cameraTarget.position) > minDistance)
                    CameraZoom(zoom);
            }
            if (Input.GetMouseButton(1))
            {
                float rotateX = Input.GetAxis("Mouse X");
                float rotateY = Input.GetAxis("Mouse Y");
                if (rotateX != 0)
                    CameraRotate(rotateX, Vector3.up);
                if (rotateY != 0)
                {
                    if (rotateY > 0 && Vector3.Angle(Vector3.up, (TR.position - cameraTarget.position)) > minRotateAngle)
                        CameraRotate(rotateY, TR.right);
                    if (rotateY < 0 && TR.position.y > minGroudDistance)
                        CameraRotate(rotateY, TR.right);
                }
            }
        }
        CameraFollow();
    }
}
