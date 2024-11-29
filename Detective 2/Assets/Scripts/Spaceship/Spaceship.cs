using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    //двери и их открытые и закрытые значения
    [SerializeField]
    private Transform upDoor, downDoor;
    private Quaternion upDoorOpenRotation, downDoorOpenRotation;
    private Quaternion upDoorCloseRotation, downDoorCloseRotation;

    //точки для камеры
    [SerializeField]
    private Transform[] cameraPoints;
    [SerializeField]
    private Transform[] cameraTargetPoints;

    //трансформ корабля
    private Transform TR;

    //оборудование
    [SerializeField]
    private GameObject[] mainMonitors;
    [SerializeField]
    private GameObject[] mapProjecties;
    [SerializeField]
    private GameObject[] jumpMonitors;
    [SerializeField]
    private SpasechipPanelButton[] panelButtons;

    /*[SerializeField]
    GameObject mainMonitor, leftMonitor, rightMonitor;
    [SerializeField]
    GameObject statusMonitorMain, statusMonitorMoveFirst, statusMonitorMoveSecond, statusMonitorRotate;
    [SerializeField]
    Transform statusMonitorMoveTRFirst, statusMonitorMoveTRSecond, statusMonitorRotateTR;*/

    //состояния
    [SerializeField]
    private bool isUsed;
    [SerializeField]
    private bool doorsIsOpen;
    [SerializeField]
    private bool userInside;
    [SerializeField]
    private bool stopUsing;

    private SpaceshipStates currentState;

    //таймеры
    [SerializeField]
    float timeBeforeTakeOff;

    //констрольные точки
    [SerializeField]
    private Transform[] pathPoints;

    //контрольные точки космолета
    private Vector3 activateVector;

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
    }

    public void SetTargetForJump(int nomber)
    {
        jumpSceneNomber = nomber;
    }

    public override void UseIndividual()
    {
        isUsed = true;
        currentCamera = FindObjectOfType<CameraScript>();
    }

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
    public override void StopUse()
    {
        isUsed = false;
        currentUser = null;
        currentCamera.SetCameraTarget(true);
        currentCamera = null;
        currentState = SpaceshipStates.doorsClosed;
    }

    //методы кнопок панели
    public void PanelButtonClick(SpasechipPanelButton clickedButton)
    {
        SpaceshipButtons clickedButtonType = clickedButton.GetButtonType();
        switch (clickedButtonType)
        {
            case SpaceshipButtons.onOff:
                if (!clickedButton.IsActive())
                {
                    clickedButton.ActivateButton(true);
                    panelButtons[1].ButtonIsReadyOrNot(true);
                    panelButtons[2].TurnButtonOnOff(true);
                    panelButtons[4].ButtonIsReadyOrNot(true);
                    panelButtons[5].TurnButtonOnOff(true);
                    foreach (GameObject monitor in mainMonitors)
                    {
                        monitor.SetActive(true);
                    }
                    jumpMonitors[0].SetActive(true);
                    MoveToPoint(TR.position + activateVector, 0.5f);//is temporary
                    canStopManualy = false;
                    currentUser.transform.parent = TR;
                    currentCamera.transform.parent = TR;
                    currentState = SpaceshipStates.flyingToPoint;
                }
                else
                {
                    RaycastHit hitPoint;
                    if (Physics.Raycast(TR.position, -TR.up, out hitPoint, 10f))
                    {
                        clickedButton.ActivateButton(false);
                        MoveToPoint(TR.position + (hitPoint.point - TR.position), 0.5f);
                        currentState = SpaceshipStates.flyingToPoint;
                    }
                    else
                    {
                        Debug.Log("No place for land");
                    }
                }
                break;
            case SpaceshipButtons.jump:
                Debug.Log("Jump button");
                break;
            case SpaceshipButtons.map:
                Debug.Log("map button");
                break;
            case SpaceshipButtons.manual:
                Debug.Log("manual button");
                break;
            case SpaceshipButtons.autoLanding:
                Debug.Log("autoLanding button");
                break;
            case SpaceshipButtons.autoTakeoff:
                Debug.Log("autoTakeoff button");
                break;
        }
    }

    private Vector3 moveToPoint;
    private float moveSpeed;
    private void MoveToPoint(Vector3 targetPoint, float currentSpeed)
    {
        moveToPoint = targetPoint;
        moveSpeed = currentSpeed;
    }

    private void StateMashine()
    {
        switch (currentState)
        {
            case SpaceshipStates.idleDoorClosed:
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
            case SpaceshipStates.idleDoorOpen:
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
                    Vector3[] pathPointPositions = new Vector3[pathPoints.Length + 1]; //длинна равна 4
                    int currentPathPoint = 0;
                    for (int i = pathPointPositions.Length - 2; i >= 0; i--) //и равно 4 - 2 = 2 (мне нужны 0, 1, 2)
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
            case SpaceshipStates.doorsOpened:
                DoorControl(true, out doorsIsOpen);
                if (doorsIsOpen) 
                    currentState = SpaceshipStates.idleDoorOpen;
                break;
            case SpaceshipStates.doorsClosed:
                DoorControl(false, out doorsIsOpen);
                if (!doorsIsOpen) 
                    currentState = SpaceshipStates.idleDoorClosed;
                break;
            case SpaceshipStates.userGoInside:
                if (Vector3.Distance(currentUser.transform.position, pathPoints[pathPoints.Length - 1].position) < 0.1f)
                {
                    userInside = true;
                    isUsed = false;
                    currentUser.LookAt(mainMonitors[0].transform.position);
                    currentCamera.SetCameraTarget(cameraTargetPoints[1], (cameraPoints[1].position - cameraTargetPoints[1].position), false);

                    currentState = SpaceshipStates.doorsClosed;
                }
                break;
            case SpaceshipStates.userGoOut:
                if (Vector3.Distance(currentUser.transform.position, usePoint.position) < 0.1f)
                {
                    userInside = false;
                    canStopManualy = true;
                    stopUsing = false;
                    currentUser.StopUseItem();
                }
                break;
            case SpaceshipStates.readyToFly:
                if (stopUsing)
                {
                    canStopManualy = false;
                    currentCamera.SetCameraTarget(cameraTargetPoints[0], (cameraPoints[1].position - cameraTargetPoints[0].position), false);
                    currentState = SpaceshipStates.doorsOpened;
                }
                break;
            case SpaceshipStates.activate:

                break;
            case SpaceshipStates.active:
                if (!panelButtons[0].IsActive())
                {
                    panelButtons[1].TurnButtonOnOff(false);
                    panelButtons[2].TurnButtonOnOff(false);
                    panelButtons[4].TurnButtonOnOff(false);
                    panelButtons[5].TurnButtonOnOff(false);
                    foreach (GameObject monitor in mainMonitors)
                    {
                        monitor.SetActive(false);
                    }
                    jumpMonitors[0].SetActive(false);
                    canStopManualy = true;
                    currentUser.transform.parent = null;
                    currentCamera.transform.parent = null;
                    currentState = SpaceshipStates.readyToFly;
                }
                break;
            case SpaceshipStates.takeOff:
                break;
            case SpaceshipStates.landing:
                break;
            case SpaceshipStates.deactivate:
                break;
            case SpaceshipStates.flyingToPoint:
                if (Vector3.Distance(TR.position, moveToPoint) < 0.3f)
                {
                    TR.position = moveToPoint;
                    currentState = SpaceshipStates.active;
                }
                else
                {
                    TR.position = TR.position + (moveToPoint - TR.position) * moveSpeed * Time.deltaTime;
                }
                break;
            case SpaceshipStates.flyingToNewLocation:
                break;
            default:
                break;
        }
    }

    private void JumpMonitorsActivate(bool isActivate)
    {
        jumpMonitors[0].SetActive(isActivate);
        if (World.hasInviteToClient) jumpMonitors[2].SetActive(true);
        if (World.knowAboutBar) jumpMonitors[3].SetActive(true);
        if (World.knowStationPosition) jumpMonitors[4].SetActive(true);
    }
    private void FixedUpdate()
    {
        StateMashine();

        /*if (isUsed)
        {
            if (!doorsIsOpen && !readyToFly)
            {
                DoorControl(upDoor, upDoorOpenRotation, 89, out doorsIsOpen, true);
                DoorControl(downDoor, downDoorOpenRotation, -58, out doorsIsOpen, true);
            }
            if (doorsIsOpen && !userGoInside)
            {
                canStopManualy = false;
                userGoInside = true;
                currentUser.LookAt(pathPoints[pathPoints.Length - 1].position);
                Vector3[] pathPointPositions = new Vector3[pathPoints.Length];
                for (int i = 0; i < pathPointPositions.Length; i++)
                {
                    pathPointPositions[i] = pathPoints[i].position;
                }
                currentUser.MoveToByUsableItem(pathPointPositions);
            }
            if (userGoInside && Vector3.Distance(currentUser.transform.position, pathPoints[pathPoints.Length - 1].position) < 0.1f)
            {
                userInside = true;
            }
            
            if (userInside && doorsIsOpen)
            {
                DoorControl(downDoor, downDoorCloseRotation, 58, out doorsIsOpen, false);
                DoorControl(upDoor, upDoorCloseRotation, -89, out doorsIsOpen, false);
                currentUser.LookAt(mainMonitor.transform.position);
                if (!doorsIsOpen)
                {
                    readyToFly = true;
                    userGoInside = false;
                    canStopManualy = true;
                }
            }
        }
        if (stopUsing)
        {
            if (doorsIsOpen && !userInside)
            {
                DoorControl(downDoor, downDoorCloseRotation, 58, out doorsIsOpen, false);
                DoorControl(upDoor, upDoorCloseRotation, -89, out doorsIsOpen, false);
            }
            {
                if (!userInside)
                {
                    if (!doorsIsOpen)
                    {
                        stopUsing = false;
                    }
                }
                else
                {
                    userGoOut = true;
                    canStopManualy = false;
                    currentUser.LookAt(pathPoints[0].position);
                    Vector3[] pathPointPositions = new Vector3[pathPoints.Length + 1];
                    for (int i = pathPointPositions.Length - 2; i >= 0; i--)
                    {
                        pathPointPositions[i] = pathPoints[i].position;
                    }
                    pathPointPositions[pathPointPositions.Length - 1] = usePoint.position;
                    currentUser.MoveToByUsableItem(pathPointPositions);
                }
            }
            if (userGoOut && Vector3.Distance(currentUser.transform.position, pathPoints[0].position) < 0.1f)
            {
                userInside = false;
            }
            else
            {
                DoorControl(upDoor, upDoorOpenRotation, 89, out doorsIsOpen, true);
                DoorControl(downDoor, downDoorOpenRotation, -58, out doorsIsOpen, true);
            }
        }
    }*/
    }
}
