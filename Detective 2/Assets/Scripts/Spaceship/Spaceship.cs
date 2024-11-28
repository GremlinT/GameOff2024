using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum SpasehipStates
{
    idleDoorClosed,
    idleDoorOpen,
    doorsOpened,
    doorsClosed,
    userGoInside,
    userGoOut,
    readyToFly,
    takeOff,
    landing,
    flyingToPoint,
    flyingToNewLocation
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
    GameObject mainMonitor, leftMonitor, rightMonitor;
    [SerializeField]
    GameObject statusMonitorMain, statusMonitorMoveFirst, statusMonitorMoveSecond, statusMonitorRotate;
    [SerializeField]
    Transform statusMonitorMoveTRFirst, statusMonitorMoveTRSecond, statusMonitorRotateTR;

    //состояния
    [SerializeField]
    private bool isUsed;
    [SerializeField]
    private bool doorsIsOpen;
    /*[SerializeField]
    private bool userGoInside;*/
    [SerializeField]
    private bool userInside;
    /*[SerializeField]
    private bool readyToFly;
    [SerializeField]
    private bool flying;
    [SerializeField]
    private bool landing;*/
    [SerializeField]
    private bool stopUsing;
    [SerializeField]
    /*private bool userGoOut;*/

    private SpasehipStates currentState;

    //таймеры
    [SerializeField]
    float timeBeforeTakeOff;

    //констрольные точки
    [SerializeField]
    private Transform[] pathPoints;

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

        currentState = SpasehipStates.idleDoorClosed;
    }

    public override void UseIndividual()
    {
        isUsed = true;
        currentCamera = FindObjectOfType<CameraScript>();
    }

    /*private void DoorControl(Transform door, Quaternion targetRotation, float angel, out bool doorState, bool open)
    {
        doorState = false;
        if (door.localRotation != targetRotation)
        {
            door.Rotate(door.InverseTransformDirection(door.up), angel * Time.deltaTime);
            if (open) doorState = false;
            else doorState = true;
            if (Quaternion.Angle(targetRotation, door.localRotation) < 5)
            {
                door.localRotation = targetRotation;
                if (open) doorState = true;
                else doorState = false;
            }
        }
    }*/
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
        currentState = SpasehipStates.doorsClosed;
    }

    private void StateMashine()
    {
        switch (currentState)
        {
            case SpasehipStates.idleDoorClosed:
                if (isUsed)
                {
                    currentState = SpasehipStates.doorsOpened;
                }
                if (userInside)
                {
                    canStopManualy = true;
                    currentState = SpasehipStates.readyToFly;
                }
                break;
            case SpasehipStates.idleDoorOpen:
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
                    
                    currentState = SpasehipStates.userGoInside;
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
                    
                    currentState = SpasehipStates.userGoOut;
                }
                break;
            case SpasehipStates.doorsOpened:
                DoorControl(true, out doorsIsOpen);
                if (doorsIsOpen) 
                    currentState = SpasehipStates.idleDoorOpen;
                break;
            case SpasehipStates.doorsClosed:
                DoorControl(false, out doorsIsOpen);
                if (!doorsIsOpen) 
                    currentState = SpasehipStates.idleDoorClosed;
                break;
            case SpasehipStates.userGoInside:
                if (Vector3.Distance(currentUser.transform.position, pathPoints[pathPoints.Length - 1].position) < 0.1f)
                {
                    userInside = true;
                    isUsed = false;
                    currentUser.LookAt(mainMonitor.transform.position);
                    currentCamera.SetCameraTarget(cameraTargetPoints[1], (cameraPoints[1].position - cameraTargetPoints[1].position), false);

                    currentState = SpasehipStates.doorsClosed;
                }
                break;
            case SpasehipStates.userGoOut:
                if (Vector3.Distance(currentUser.transform.position, usePoint.position) < 0.1f)
                {
                    userInside = false;
                    canStopManualy = true;
                    stopUsing = false;
                    currentUser.StopUseItem();
                }
                break;
            case SpasehipStates.readyToFly:
                if (stopUsing)
                {
                    canStopManualy = false;
                    currentCamera.SetCameraTarget(cameraTargetPoints[0], (cameraPoints[1].position - cameraTargetPoints[0].position), false);
                    currentState = SpasehipStates.doorsOpened;
                }
                break;
            case SpasehipStates.takeOff:
                break;
            case SpasehipStates.landing:
                break;
            case SpasehipStates.flyingToPoint:
                break;
            case SpasehipStates.flyingToNewLocation:
                break;
            default:
                break;
        }
    }

    private void Update()
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
