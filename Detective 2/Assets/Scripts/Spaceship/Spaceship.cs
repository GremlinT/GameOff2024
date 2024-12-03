using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum SpaceshipStates
{
    idleDoorClosed,
    idleDoorOpen,
    doorsOpened,
    doorsClosed,
    userGoInside,
    userGoOut,
    readyToFly,
    activate,
    active,
    takeOff,
    landing,
    deactivate,
    flyingToPoint,
    flyingToNewLocation
}

public enum SpaceshipButtons
{
    onOff,
    jump,
    map,
    manual,
    autoLanding,
    autoTakeoff
}

public class Spaceship : UsableItem
{
    //����� � �� �������� � �������� ��������
    [SerializeField]
    private Transform upDoor, downDoor;
    private Quaternion upDoorOpenRotation, downDoorOpenRotation;
    private Quaternion upDoorCloseRotation, downDoorCloseRotation;

    //����� ��� ������
    [SerializeField]
    private Transform[] cameraPoints;
    [SerializeField]
    private Transform[] cameraTargetPoints;

    //��������� �������
    private Transform TR;

    //������������
    [SerializeField]
    private GameObject[] mainMonitors;
    [SerializeField]
    private GameObject[] mapProjecties;
    [SerializeField]
    private GameObject[] jumpMonitors;
    [SerializeField]
    private SpasechipPanelButton[] panelButtons;

    //���������
    [SerializeField]
    private bool isUsed;
    [SerializeField]
    private bool doorsIsOpen;
    [SerializeField]
    private bool userInside;
    [SerializeField]
    private bool stopUsing;
    private bool needRotation;

    //������� ������
    private SpaceshipStates currentState;

    //�������
    private float timer;
    [SerializeField]
    private float shutdownTimer;

    //������������ ����� ��� �������� ���������
    [SerializeField]
    private Transform[] pathPoints;

    //������ ��� ������
    private Vector3 activateVector;

    //���� ��� ������� ��� �������
    private bool isInside;

    //����� �������� 
    [SerializeField]
    private List<Transform> spaceshipPathTransforms = new List<Transform>();
    [SerializeField]
    private List<Vector3> spaceshipPathPoints = new List<Vector3>();
    //����� ������� ����� �� ��������
    private int currentpathPoint;
    //�������� �� ��������
    private float pathSpeed;

    //�������� ��������
    [SerializeField]
    private float rotationSpeed;

    //������� �������
    [SerializeField]
    private SpaceStation station;

    //����� ������ ��� ������
    private int jumpSceneNomber;

    private void Start()
    {
        TR = transform;
        upDoorCloseRotation = upDoor.localRotation;
        downDoorCloseRotation = downDoor.localRotation;

        upDoor.Rotate(upDoor.InverseTransformDirection(upDoor.up), 89); ;
        upDoorOpenRotation = upDoor.localRotation;
        upDoor.localRotation = upDoorCloseRotation;

        downDoor.Rotate(downDoor.InverseTransformDirection(downDoor.up), -58);
        downDoorOpenRotation = downDoor.localRotation;
        downDoor.localRotation = downDoorCloseRotation;

        currentState = SpaceshipStates.idleDoorClosed;

        activateVector = Vector3.up * 4f;

        isInside = true;

        currentpathPoint = 0;
    }

    //���������� ����� ��� ������ - ����� ���������� ��� ������� �� �������������� ����� ���� �������
    public void SetTargetForJump(int nomber)
    {
        jumpSceneNomber = nomber;
        panelButtons[2].ButtonIsReadyOrNot(true);
    }

    //��� ������������� ������ ������ ���� isUsed
    public override void UseIndividual()
    {
        isUsed = true;
        currentCamera = FindObjectOfType<CameraScript>();
    }

    //���������� �����, ������� ����� � �������/�������, ���� ������� ��������� - ���������� ���, � ��������� ������ - ����
    private bool DoorRotation(Transform door, Quaternion targetRotation, float angel)
    {
        if (door.localRotation != targetRotation)
        {
            door.Rotate(door.InverseTransformDirection(door.up), angel * Time.deltaTime);
        }
        if (Quaternion.Angle(targetRotation, door.localRotation) < 5)
        {
            door.localRotation = targetRotation;
            return true;
        }
        return false;
    }

    //���������� �����, ����������� ��������� ������. �� ����� ��������� ������� �������� ����� ������ � ��������� ���� ����� ��������� ��������
    //!!!����������� ������� �� ������� � ��������� ������, � ������� ��� ����� ��������!!!!
    private void DoorControl(bool open, out bool _doorIsOpen) //true - want to open, false - want to close
    {
        _doorIsOpen = false;
        switch (open)
        {
            case true:
                if (DoorRotation(upDoor, upDoorOpenRotation, 89) && DoorRotation(downDoor, downDoorOpenRotation, -58))
                {
                    _doorIsOpen = true;
                }
                else _doorIsOpen = false;
                break;
            case false:
                if (DoorRotation(upDoor, upDoorCloseRotation, -89) && DoorRotation(downDoor, downDoorCloseRotation, 58))
                {
                    _doorIsOpen = false;
                }
                else _doorIsOpen = true;
                break;
        }
    }

    //������������ ����������� ������� - ���� ����� ���� ��� ���� � ������� - ���������� �����, �� ��������� ����� �� �������
    public override bool CanStopManualy()
    {
        if (canStopManualy)
        {
            if (userInside)
            {
                stopUsing = true;
                return false;
            }
            StopUse();
            return true;
        }
        return false;
    }
    //������������ ����������� ������, ��������� ��� ���� �� � �������, �������� ������ � �������� ������� � ������ "����� �����������"
    public override void StopUse()
    {
        isUsed = false;
        currentUser = null;
        currentCamera.SetCameraTarget(true);
        currentCamera = null;
        currentState = SpaceshipStates.doorsClosed;
    }

    //������ ������ ������
    public void PanelButtonClick(SpasechipPanelButton clickedButton)
    {
        SpaceshipButtons clickedButtonType = clickedButton.GetButtonType(); //������� ��� ������ ������� ������
        switch (clickedButtonType)
        {
            case SpaceshipButtons.onOff: //���������/��������� - ������ ������
                if (!clickedButton.IsActive())
                {
                    clickedButton.ActivateButton(true);
                    panelButtons[1].TurnButtonOnOff(true);
                    panelButtons[2].TurnButtonOnOff(true);
                    panelButtons[4].TurnButtonOnOff(true);
                    panelButtons[5].TurnButtonOnOff(true);
                    foreach (GameObject monitor in mainMonitors)
                    {
                        monitor.SetActive(true);
                    }
                    JumpMonitorsActivate(true);
                    canStopManualy = false;
                    currentUser.transform.parent = TR;
                    currentCamera.transform.parent = TR;
                    pathSpeed = 0.5f;
                    spaceshipPathPoints.Add(TR.position + activateVector);
                    spaceshipPathTransforms.Add(TR);
                    currentState = SpaceshipStates.active;
                }
                else
                {
                    RaycastHit hitPoint;
                    if (Physics.Raycast(TR.position, -TR.up, out hitPoint, 10f))
                    {
                        timer = shutdownTimer;
                        pathSpeed = 0.5f;
                        spaceshipPathPoints.Add(TR.position + (hitPoint.point - TR.position));
                        spaceshipPathTransforms.Add(TR);
                        clickedButton.ActivateButton(false);
                        clickedButton.ButtonIsReadyOrNot(false);
                        currentState = SpaceshipStates.active;
                    }
                    else
                    {
                        Debug.Log("No place for land");
                    }
                }
                break;
            case SpaceshipButtons.jump: //������ - ������ ������
                if (clickedButton.IsReady())
                {
                    Debug.Log("Jump to " + jumpSceneNomber);
                }
                else
                {
                    Debug.Log("Jump button is not ready");
                }
                break;
            case SpaceshipButtons.map: //����� - ������ ������
                Debug.Log("map button");
                break;
            case SpaceshipButtons.manual: //������ ���������� - ������� ������
                Debug.Log("manual button");
                break;
            case SpaceshipButtons.autoLanding: //����-������� - ������ ��������� ������
                Debug.Log("autoLanding button");
                break;
            case SpaceshipButtons.autoTakeoff: //����-����� - ������ �������� ������
                if (clickedButton.IsReady())
                {
                    pathSpeed = 2f;
                    spaceshipPathTransforms = station.SetPath(true).ToList();
                    foreach (Transform point in spaceshipPathTransforms)
                    {
                        spaceshipPathPoints.Add(point.position);
                    }
                    currentState = SpaceshipStates.active;
                }
                break;
        }
    }

    //�������� �������
    private Vector3 moveToPoint; //�����, ���� ������� ���� �����������
    private float moveSpeed; //�������� � ������� ������� ����� ���������
    //����� ��������, ��������� �������� ��� ���������� ����
    private void MoveToPoint(Vector3 targetPoint, float currentSpeed)
    {
        moveToPoint = targetPoint;
        moveSpeed = currentSpeed;
    }

    //����� �������� �� ����� ����������
    private void RotateToPoint()
    {
        if (needRotation)
        {
            if (Quaternion.Angle(TR.rotation, spaceshipPathTransforms[currentpathPoint].rotation) > 1f)
            {
                TR.rotation = Quaternion.Lerp(TR.rotation, spaceshipPathTransforms[currentpathPoint].rotation, rotationSpeed * Time.deltaTime);
                currentCamera.SetCameraTarget(cameraTargetPoints[1], (cameraPoints[1].position - cameraTargetPoints[1].position), false);
            }
            else
            {
                TR.rotation = spaceshipPathTransforms[currentpathPoint].rotation;
                needRotation = false;

            }
            /*Vector3 targetVector = -(moveToPoint - TR.position).normalized;
            if (Quaternion.Angle(TR.rotation, Quaternion.LookRotation(targetVector)) > 1f)
            {
                TR.rotation = Quaternion.Lerp(TR.rotation, Quaternion.LookRotation(targetVector), rotationSpeed * Time.deltaTime);
                currentCamera.SetCameraTarget(cameraTargetPoints[1], (cameraPoints[1].position - cameraTargetPoints[1].position), false);
            }
            else
            {
                TR.rotation = Quaternion.LookRotation(targetVector);
                needRotation = false;
            }*/
        }
    }

    private void RotateAtPointDirection(Transform targetRotationTransform)
    {
        if (Quaternion.Angle(TR.rotation, targetRotationTransform.rotation) > 1f)
        {
            TR.rotation = Quaternion.Lerp(TR.rotation, targetRotationTransform.rotation, rotationSpeed * Time.deltaTime);
            currentCamera.SetCameraTarget(cameraTargetPoints[1], (cameraPoints[1].position - cameraTargetPoints[1].position), false);
        }
        else
        {
            TR.rotation = targetRotationTransform.rotation;
        }
    }    
    
    //��������� ���� ������� � ����������� ���������� ����� ��������
    private void JumpMonitorsActivate(bool isActivate)
    {
        jumpMonitors[0].SetActive(isActivate);
        if (World.hasInviteToClient) jumpMonitors[2].SetActive(true);
        if (World.knowAboutBar) jumpMonitors[3].SetActive(true);
        if (World.knowStationPosition) jumpMonitors[4].SetActive(true);
    }

    //������ ���������
    /* �������� � ���������:
     * - �����, ����� ������� - ����� ����������� � ����� � ������
     * - �����, ����� ������� - ���� ���� � �������, ���� ���� �� �������
     * - ����� ����������� - �����, ����� �������
     * - ����� ����������� - �����, ����� �������
     * - ���� ���� � ������� - ����� �����������
     * - ���� ������� �� ������� - ������������� �������������, ������� � ����� ������ � �������
     * - ����� � ������ - ����� ����������� (���� �������). 
     * - ��������� �������?
     * - ������� (�������) - ����� � ��������� �����, ����� � ������, �������� �� ��������
     * - �������� �� �������� - ����� � ��������� �����
     * - ����� �� �������!-!
     * - ���� �� �������!-!
     * - �����������?
     * - ����� � ��������� ����� - �������
     * - ����� � ����� �������?
     */
    private void StateMashine()
    {
        switch (currentState)
        {
            case SpaceshipStates.idleDoorClosed: //�����, ����� �������
                if (isUsed)
                {
                    currentState = SpaceshipStates.doorsOpened;
                }
                if (userInside)
                {
                    canStopManualy = true;
                    currentState = SpaceshipStates.readyToFly;
                }
                break;
            case SpaceshipStates.idleDoorOpen: //�����, ����� �������
                if (isUsed)
                {
                    canStopManualy = false;
                    currentCamera.SetCameraTarget(cameraTargetPoints[0], (cameraPoints[0].position - cameraTargetPoints[0].position), false);
                    currentUser.LookAt(pathPoints[pathPoints.Length - 1].position);
                    Vector3[] pathPointPositions = new Vector3[pathPoints.Length];
                    for (int i = 0; i < pathPointPositions.Length; i++)
                    {
                        pathPointPositions[i] = pathPoints[i].position;
                    }
                    currentUser.MoveToByUsableItem(pathPointPositions);
                    
                    currentState = SpaceshipStates.userGoInside;
                }
                if (userInside)
                {
                    currentUser.LookAt(pathPoints[0].position);
                    Vector3[] pathPointPositions = new Vector3[pathPoints.Length + 1]; //������ ����� 4
                    int currentPathPoint = 0;
                    for (int i = pathPointPositions.Length - 2; i >= 0; i--) //� ����� 4 - 2 = 2 (��� ����� 0, 1, 2)
                    {
                        pathPointPositions[i] = pathPoints[currentPathPoint].position;
                        currentPathPoint++;
                    }
                    pathPointPositions[pathPointPositions.Length - 1] = usePoint.position;
                    currentUser.MoveToByUsableItem(pathPointPositions);
                    currentCamera.SetCameraTarget(cameraTargetPoints[0], (cameraPoints[0].position - cameraTargetPoints[0].position), false);
                    
                    currentState = SpaceshipStates.userGoOut;
                }
                break;
            case SpaceshipStates.doorsOpened: //����� �����������
                DoorControl(true, out doorsIsOpen);
                if (doorsIsOpen) 
                    currentState = SpaceshipStates.idleDoorOpen;
                break;
            case SpaceshipStates.doorsClosed: //����� �����������
                DoorControl(false, out doorsIsOpen);
                if (!doorsIsOpen) 
                    currentState = SpaceshipStates.idleDoorClosed;
                break;
            case SpaceshipStates.userGoInside: //���� ���� � �������
                if (Vector3.Distance(currentUser.transform.position, pathPoints[pathPoints.Length - 1].position) < 0.1f)
                {
                    userInside = true;
                    isUsed = false;
                    currentUser.LookAt(mainMonitors[0].transform.position);
                    currentCamera.SetCameraTarget(cameraTargetPoints[1], (cameraPoints[1].position - cameraTargetPoints[1].position), false);

                    currentState = SpaceshipStates.doorsClosed;
                }
                break;
            case SpaceshipStates.userGoOut: //���� ������� �� �������
                if (Vector3.Distance(currentUser.transform.position, usePoint.position) < 0.1f)
                {
                    userInside = false;
                    canStopManualy = true;
                    stopUsing = false;
                    currentUser.StopUseItem();
                }
                break;
            case SpaceshipStates.readyToFly: //����� � ������
                if (stopUsing)
                {
                    canStopManualy = false;
                    currentCamera.SetCameraTarget(cameraTargetPoints[0], (cameraPoints[1].position - cameraTargetPoints[0].position), false);
                    currentState = SpaceshipStates.doorsOpened;
                }
                break;
            case SpaceshipStates.activate: //��������� �������?

                break;
            case SpaceshipStates.active: //������� (�������)
                if (spaceshipPathPoints.Count > 0)
                {
                    panelButtons[4].ButtonIsReadyOrNot(false);
                    MoveToPoint(spaceshipPathPoints[currentpathPoint], pathSpeed);
                    needRotation = true;
                    rotationSpeed = Quaternion.Angle(spaceshipPathTransforms[currentpathPoint].rotation, TR.rotation) / Vector3.Distance(TR.position, spaceshipPathPoints[currentpathPoint]);
                    currentState = SpaceshipStates.flyingToPoint;
                }
                else
                {
                    if (!panelButtons[0].IsActive())
                    {
                        timer -= Time.deltaTime;
                        if (timer <= 0)
                        {
                            panelButtons[0].ButtonIsReadyOrNot(true);
                            panelButtons[1].TurnButtonOnOff(false);
                            panelButtons[2].TurnButtonOnOff(false);
                            panelButtons[4].TurnButtonOnOff(false);
                            panelButtons[5].TurnButtonOnOff(false);
                            foreach (GameObject monitor in mainMonitors)
                            {
                                monitor.SetActive(false);
                            }
                            JumpMonitorsActivate(false);
                            canStopManualy = true;
                            currentUser.transform.parent = null;
                            currentCamera.transform.parent = null;
                            currentState = SpaceshipStates.readyToFly;
                        }
                    }
                    else
                    {
                        panelButtons[4].ButtonIsReadyOrNot(true);
                    }
                }
                break;
            case SpaceshipStates.takeOff: //����� �� �������
                break;
            case SpaceshipStates.landing: //���� �� �������
                break;
            case SpaceshipStates.deactivate: //�����������?
                break;
            case SpaceshipStates.flyingToPoint: //����� � ��������� �����
                if (Vector3.Distance(TR.position, moveToPoint) < 0.1f)
                {
                    TR.position = moveToPoint;
                    currentpathPoint++;
                    if (currentpathPoint >= spaceshipPathPoints.Count)
                    {
                        spaceshipPathPoints.Clear();
                        spaceshipPathTransforms.Clear();
                        currentpathPoint = 0;
                        pathSpeed = 0;
                    }
                    currentState = SpaceshipStates.active;
                }
                else
                {
                    RotateToPoint();
                    TR.position = TR.position + (moveToPoint - TR.position) * moveSpeed * Time.deltaTime;
                }
                break;
            case SpaceshipStates.flyingToNewLocation: //����� � ����� �������?
                break;
            default:
                break;
        }
    }

    //������ ���� ��������� ���� ����� ������
    private void FixedUpdate()
    {
        StateMashine();
        //RotateToPoint();
    }
}
