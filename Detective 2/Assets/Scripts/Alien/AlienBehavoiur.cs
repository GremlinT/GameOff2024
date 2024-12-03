using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum AlienStates
{
    idle,
    walking,
    use
}

public class AlienBehavoiur : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform TR;
    [SerializeField]
    private AlienStates currentState;
    [SerializeField]
    private UsableItem currentItem;
    [SerializeField]
    private UsableItem newPlanedItem;
    [SerializeField]
    private Vector3 currentMoveToPoint;
    [SerializeField]
    private float standartTreshold;
    private float currentTreshold;
    private InventorySystem invSys;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        TR = transform;
        currentState = AlienStates.idle;
        currentMoveToPoint = Vector3.zero;
        currentTreshold = standartTreshold;
        invSys = GetComponent<InventorySystem>();
    }

    //методы обработчики пользовательского ввода
    public void MoveTo(Vector3 targetPosition)
    {
        if (currentState != AlienStates.use) ClearItem();
        currentMoveToPoint = targetPosition;
    }

    public void UseItem(UsableItem item)
    {
        if (currentItem == null)
        {
            currentItem = item.SetCurrentItem();
        }
        else
        {
            if (currentItem != item.SetCurrentItem())
            {
                newPlanedItem = item.SetCurrentItem();
            }
            else
            {
                if (currentState == AlienStates.use)
                {
                    if (item != currentItem)
                    {
                        item.Use(this);
                    }
                    else
                    {
                        //итем это кликнутый итем но не родитель
                    }
                }
            }
        }
    }
    //----

    private void Update()
    {
        StateMashine();
    }

    private void StateMashine()
    {
        switch(currentState)
        {
            case AlienStates.idle:
                if (currentMoveToPoint != Vector3.zero)
                {
                    agent.SetDestination(currentMoveToPoint);
                    animator.SetBool("isWalking", true);
                    currentState = AlienStates.walking;
                }
                if (newPlanedItem != null)
                {
                    currentItem = newPlanedItem;
                    newPlanedItem = null;
                }
                if (currentItem != null)
                {
                    if (!currentItem.IsCanUsed(TR.position))
                    {
                        currentMoveToPoint = currentItem.UsePoint();
                        currentTreshold = currentItem.SetItemTreshold();
                    }
                    else
                    {
                        currentItem.Use(this);
                        agent.enabled = false;
                        currentTreshold = standartTreshold;
                        currentState = AlienStates.use;
                    }
                }
                break;
            case AlienStates.walking:
                if (newPlanedItem != null)
                {
                    currentItem = newPlanedItem;
                    newPlanedItem = null;
                    currentMoveToPoint = currentItem.UsePoint();
                    currentTreshold = currentItem.SetItemTreshold();
                }
                if (currentItem != null && currentMoveToPoint != currentItem.UsePoint())
                {
                    currentMoveToPoint = currentItem.UsePoint();
                    currentTreshold = currentItem.SetItemTreshold();
                }
                if (currentMoveToPoint != agent.destination)
                {
                    agent.SetDestination(currentMoveToPoint);
                }
                if (Vector3.Distance(TR.position, agent.destination) < currentTreshold)
                {
                    animator.SetBool("isWalking", false);
                    currentMoveToPoint = Vector3.zero;
                    currentState = AlienStates.idle;
                }
                break;
            case AlienStates.use:
                Rotating();
                MovingTo();
                if (currentMoveToPoint != Vector3.zero || newPlanedItem != null)
                {
                    if (!StopUseItem()) currentMoveToPoint = Vector3.zero;
                }
                if (currentItem == null)
                {
                    agent.enabled = true;
                    currentState = AlienStates.idle;
                }
                break;
        }
    }

    public bool StopUseItem()
    {
        if (currentItem.CanStopManualy())
        {
            ClearItem();
            return true;
        }
        else
        {
            return false;
        }
    }
    
    private void ClearItem()
    {
        currentItem = null;
    }

    //методы для управления действиями персонажа вне стейтмашины
    
    public void TakeItem(PickableItem item)
    {
        invSys.AddItemToInventory(item);
        if (item == currentItem) StopUseItem();
    }

    //вращение в нужную сторону
    [SerializeField]
    private float rotateSpeed;
    private Vector3 lookAtPoint;
    private bool isRotating;

    public void LookAt(Vector3 targetPositon) // - внешний метод
    {
        targetPositon.y = TR.position.y;
        lookAtPoint = targetPositon;
        isRotating = true;
    }
    private void Rotating() // - внутренний метод, вызывается в рамках статуса Use
    {
        if (isRotating)
        {
            if (Vector3.Angle(TR.forward, lookAtPoint - TR.position) > .5f)
                TR.rotation = Quaternion.Lerp(TR.rotation, Quaternion.LookRotation(lookAtPoint - TR.position), Time.deltaTime * rotateSpeed);
            else
            {
                lookAtPoint = Vector3.zero;
                isRotating = false;
            } 
        }
    }

    private List<Vector3> moveToByUsibleItemPosition = new List<Vector3>();
    private bool isMoving;
    private int currentPathPoint;
    public void MoveToByUsableItem(Vector3 targetPosition)
    {
        moveToByUsibleItemPosition[0] = targetPosition;
        isMoving = true;
        animator.SetBool("isWalking", true);
        currentPathPoint = 0;
    }
    public void MoveToByUsableItem(Vector3[] targetPositions)
    {
        moveToByUsibleItemPosition = targetPositions.ToList<Vector3>();
        isMoving = true;
        animator.SetBool("isWalking", true);
        currentPathPoint = 0;
    }
    private void MovingTo()
    {
        if (isMoving)
        {
            if (Vector3.Distance(TR.position, moveToByUsibleItemPosition[currentPathPoint]) > 0.1f)
            {
                //TR.position = Vector3.Lerp(TR.position, moveToByUsibleItemPosition[currentPathPoint], Time.deltaTime * agent.speed);
                TR.position = TR.position + (moveToByUsibleItemPosition[currentPathPoint] - TR.position) * Time.deltaTime * agent.speed;
                Debug.Log(agent.speed);
            }
            else
            {
                TR.position = moveToByUsibleItemPosition[currentPathPoint];
                currentPathPoint++;
                if (currentPathPoint < moveToByUsibleItemPosition.Count)
                {
                    return;
                }
                else
                {
                    moveToByUsibleItemPosition.Clear();
                    isMoving = false;
                    animator.SetBool("isWalking", false);
                    currentPathPoint = 0;
                }
            }
        }
    }

    //описание предмета
}
