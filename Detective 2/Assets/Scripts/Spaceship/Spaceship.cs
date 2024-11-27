using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Spaceship : UsableItem
{
    //двери и их открытые и закрытые значения
    [SerializeField]
    private Transform upDoor, downDoor;
    private Quaternion upDoorOpenRotation, downDoorOpenRotation;
    private Quaternion upDoorCloseRotation, downDoorCloseRotation;

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
    [SerializeField]
    private bool userGoInside;
    [SerializeField]
    private bool userInside;
    [SerializeField]
    private bool readyToFly;
    [SerializeField]
    private bool flying;
    [SerializeField]
    private bool landing;
    [SerializeField]
    private bool stopUsing;
    [SerializeField]
    private bool userGoOut;

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
    }

    public override void UseIndividual()
    {
        isUsed = true;
    }

    private void DoorControl(Transform door, Quaternion targetRotation, float angel, out bool doorState, bool open)
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
    }

    public override void StopUse()
    {
        isUsed = false;
        stopUsing = true;
        readyToFly = false;
        /*if (userInside)
        {
            userGoOut = true;
        }
        else
        {
            currentUser = null;
        }*/
        
    }

    private void Update()
    {
        if (isUsed)
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
            /*{
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
            }*/
        }
    }
}
