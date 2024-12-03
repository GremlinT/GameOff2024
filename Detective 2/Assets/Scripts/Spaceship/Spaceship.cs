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

    //состояния
    [SerializeField]
    private bool isUsed;
    [SerializeField]
    private bool doorsIsOpen;
    [SerializeField]
    private bool userInside;
    [SerializeField]
    private bool stopUsing;
    private bool needRotation;

    //текущий статус
    private SpaceshipStates currentState;

    //таймеры
    private float timer;
    [SerializeField]
    private float shutdownTimer;

    //констрольные точки для движения персонажа
    [SerializeField]
    private Transform[] pathPoints;

    //вектор для взлета
    private Vector3 activateVector;

    //флаг что корабль вне станции
    private bool isInside;

    //точки маршрута 
    [SerializeField]
    private List<Transform> spaceshipPathTransforms = new List<Transform>();
    [SerializeField]
    private List<Vector3> spaceshipPathPoints = new List<Vector3>();
    //номер текущей точки на маршруте
    private int currentpathPoint;
    //скорость по маршруту
    private float pathSpeed;

    //скорость вращения
    [SerializeField]
    private float rotationSpeed;

    //текушая станция
    [SerializeField]
    private SpaceStation station;

    //номер сценны для прыжка
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

    //определяем сцену для прыжка - метод вызывается при нажатии на соответсвующий пункт меню прыжков
    public void SetTargetForJump(int nomber)
    {
        jumpSceneNomber = nomber;
        panelButtons[2].ButtonIsReadyOrNot(true);
    }

    //при использовании просто ставим флаг isUsed
    public override void UseIndividual()
    {
        isUsed = true;
        currentCamera = FindObjectOfType<CameraScript>();
    }

    //внутренний метод, вращает двери в открыто/закрыто, если врщение закончено - возвращает тру, в противном случае - фолс
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

    //внутренний метод, управляющий вращением дверей. на входе указываем искомое состояие обеих дверей и указываем куда пишем результат открытия
    //!!!Рассмотреть вариант не выводит в отдельный булеан, а сделать сам метод булеаном!!!!
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

    //переписываем стандартный кэнСтоп - если стоит флаг что юзер в корабле - возвращаем фалсе, но запускаем выход из корабля
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
    //переписанный стандартный стопЮс, указываем что юзер не в корабле, обнуляем камеру и перевоим корабль в статус "двери закрываются"
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
        SpaceshipButtons clickedButtonType = clickedButton.GetButtonType(); //получае тип кнопки которую нажали
        switch (clickedButtonType)
        {
            case SpaceshipButtons.onOff: //включение/выклчение - нижняя кнопка
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
            case SpaceshipButtons.jump: //прыжок - третья кнопка
                if (clickedButton.IsReady())
                {
                    Debug.Log("Jump to " + jumpSceneNomber);
                }
                else
                {
                    Debug.Log("Jump button is not ready");
                }
                break;
            case SpaceshipButtons.map: //карта - вторая кнопка
                Debug.Log("map button");
                break;
            case SpaceshipButtons.manual: //ручное управление - верхняя кнопка
                Debug.Log("manual button");
                break;
            case SpaceshipButtons.autoLanding: //авто-посадка - нижняя маленькая кнопка
                Debug.Log("autoLanding button");
                break;
            case SpaceshipButtons.autoTakeoff: //авто-взлет - верхяя малеьная кнопка
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

    //движение корабля
    private Vector3 moveToPoint; //точка, куда корабль надо передвинуть
    private float moveSpeed; //скорость с которой корабль будет двигаться
    //метод движения, указывает значения для переменных выше
    private void MoveToPoint(Vector3 targetPoint, float currentSpeed)
    {
        moveToPoint = targetPoint;
        moveSpeed = currentSpeed;
    }

    //метод вращения на точку следования
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
    
    //включение меню прыжков и отображение изхвестных точек маршрута
    private void JumpMonitorsActivate(bool isActivate)
    {
        jumpMonitors[0].SetActive(isActivate);
        if (World.hasInviteToClient) jumpMonitors[2].SetActive(true);
        if (World.knowAboutBar) jumpMonitors[3].SetActive(true);
        if (World.knowStationPosition) jumpMonitors[4].SetActive(true);
    }

    //машина состояний
    /* Переходы и состояния:
     * - стоит, двери закрыты - двери открываются и готов к полету
     * - стоит, двери открыты - юзер идет в корабль, юзер идет из корабля
     * - двери открываются - стоит, двери открыты
     * - двери закрываются - стоит, двери закрыты
     * - юзер идет в корабль - двери закрываются
     * - юзер выходит из корабля - заканчикается использование, переход в новый статус в стопЮзе
     * - готов к полету - двери открываются (если выходим). 
     * - активация корабля?
     * - активен (взлетел) - полет в указанную точку, готов к полету, движение по маршруту
     * - движение по маршруту - полет в указанную точку
     * - вылет из станции!-!
     * - влет на станцию!-!
     * - деактивация?
     * - полет в указанную точку - активен
     * - полет в новую локацию?
     */
    private void StateMashine()
    {
        switch (currentState)
        {
            case SpaceshipStates.idleDoorClosed: //стоит, двери закрыты
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
            case SpaceshipStates.idleDoorOpen: //стоит, двери открыты
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
            case SpaceshipStates.doorsOpened: //двери открываются
                DoorControl(true, out doorsIsOpen);
                if (doorsIsOpen) 
                    currentState = SpaceshipStates.idleDoorOpen;
                break;
            case SpaceshipStates.doorsClosed: //двери закрываются
                DoorControl(false, out doorsIsOpen);
                if (!doorsIsOpen) 
                    currentState = SpaceshipStates.idleDoorClosed;
                break;
            case SpaceshipStates.userGoInside: //юзер идет в корабль
                if (Vector3.Distance(currentUser.transform.position, pathPoints[pathPoints.Length - 1].position) < 0.1f)
                {
                    userInside = true;
                    isUsed = false;
                    currentUser.LookAt(mainMonitors[0].transform.position);
                    currentCamera.SetCameraTarget(cameraTargetPoints[1], (cameraPoints[1].position - cameraTargetPoints[1].position), false);

                    currentState = SpaceshipStates.doorsClosed;
                }
                break;
            case SpaceshipStates.userGoOut: //юзер выходит из корабля
                if (Vector3.Distance(currentUser.transform.position, usePoint.position) < 0.1f)
                {
                    userInside = false;
                    canStopManualy = true;
                    stopUsing = false;
                    currentUser.StopUseItem();
                }
                break;
            case SpaceshipStates.readyToFly: //готов к полету
                if (stopUsing)
                {
                    canStopManualy = false;
                    currentCamera.SetCameraTarget(cameraTargetPoints[0], (cameraPoints[1].position - cameraTargetPoints[0].position), false);
                    currentState = SpaceshipStates.doorsOpened;
                }
                break;
            case SpaceshipStates.activate: //активация корабля?

                break;
            case SpaceshipStates.active: //активен (взлетел)
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
            case SpaceshipStates.takeOff: //вылет из станции
                break;
            case SpaceshipStates.landing: //влет на станцию
                break;
            case SpaceshipStates.deactivate: //деактивация?
                break;
            case SpaceshipStates.flyingToPoint: //полет в указанную точку
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
            case SpaceshipStates.flyingToNewLocation: //полет в новую локацию?
                break;
            default:
                break;
        }
    }

    //каждый кадр запускаем нашу стэйт машину
    private void FixedUpdate()
    {
        StateMashine();
        //RotateToPoint();
    }
}
