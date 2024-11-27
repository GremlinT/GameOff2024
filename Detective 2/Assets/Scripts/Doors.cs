using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    private Transform doorTR;
    [SerializeField]
    private Transform openDoorPoint, closeDoorPoint;
    [SerializeField]
    private float doorSpeed;
    [SerializeField]
    private float openDoorDistance, doorTreshhold;
    [SerializeField]
    private bool isLocked;

    private bool isOpen, isClosed;
    [SerializeField]
    private AlienBehavoiur[] allAliens;

    private Vector3 openDoorPosition, closeDoorPosition;
    private Quaternion openDoorRotation, closeDoorRotation;

    private void Start()
    {
        doorTR = transform;
        allAliens = FindObjectsByType<AlienBehavoiur>(FindObjectsSortMode.None);
        openDoorPosition = openDoorPoint.position;
        closeDoorPosition = closeDoorPoint.position;
        openDoorRotation = openDoorPoint.rotation;
        closeDoorRotation = closeDoorPoint.rotation;
    }

    private void OpenCloseDoor()
    {
        if (!isLocked)
        {
            foreach (var alien in allAliens)
            {
                if (Vector3.Distance(closeDoorPosition, alien.transform.position) < openDoorDistance)
                {
                    if (!isOpen)
                    {
                        doorTR.position = Vector3.Lerp(doorTR.position, openDoorPosition, doorSpeed * Time.deltaTime);
                        doorTR.rotation = Quaternion.Lerp(doorTR.rotation, openDoorRotation, doorSpeed * Time.deltaTime);
                        isClosed = false;
                        if (Vector3.Distance(doorTR.position, openDoorPosition) < doorTreshhold 
                            && Quaternion.Angle(doorTR.rotation, openDoorRotation) < doorTreshhold)
                        {
                            doorTR.position = openDoorPosition;
                            doorTR.rotation = openDoorRotation;
                            isOpen = true;
                        }
                    }
                }
                else
                {
                    if (!isClosed)
                    {
                        doorTR.position = Vector3.Lerp(doorTR.position, closeDoorPosition, doorSpeed * Time.deltaTime);
                        doorTR.rotation = Quaternion.Lerp(doorTR.rotation, closeDoorRotation, doorSpeed * Time.deltaTime);
                        isOpen = false;
                        if (Vector3.Distance(doorTR.position, closeDoorPosition) < doorTreshhold
                            && Quaternion.Angle(doorTR.rotation, closeDoorRotation) < doorTreshhold)
                        {
                            doorTR.position = closeDoorPosition;
                            doorTR.rotation = closeDoorRotation;
                            isClosed = true;
                        }
                    }
                }
            }
        }
    }

    public void UnlockedDoor()
    {
        isLocked = false;
    }

    private void FixedUpdate()
    {
        OpenCloseDoor();
    }

}
