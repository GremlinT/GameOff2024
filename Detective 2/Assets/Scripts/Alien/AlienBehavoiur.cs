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
    private Vector3 currentMoveToPoint;
    [SerializeField]
    private float standartTreshold;
    private float currentTreshold;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        TR = transform;
        currentState = AlienStates.idle;
        currentMoveToPoint = Vector3.zero;
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
            currentTreshold = currentItem.SetItemTreshold();
        }
        else
        {
            if (currentItem != item.SetCurrentItem())
            {
                currentMoveToPoint = item.UsePoint();
                currentTreshold = item.SetItemTreshold();
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
                if (currentItem != null)
                {
                    if (!currentItem.IsCanUsed(TR.position))
                    {
                        currentMoveToPoint = currentItem.UsePoint();
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
                if (currentItem != null && currentMoveToPoint != currentItem.UsePoint())
                {
                    currentMoveToPoint = currentItem.UsePoint();
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
                if (currentMoveToPoint != Vector3.zero)
                {
                    if (currentItem.CanStopManualy())
                    {
                        ClearItem();
                    }
                    else
                    {
                        currentMoveToPoint = Vector3.zero;
                    }
                }
                if (currentItem == null)
                {
                    agent.enabled = true;
                    currentState = AlienStates.idle;
                }
                break;
        }
    }

    private void ClearItem()
    {
        currentItem = null;
    }

    //методы для управления действиями персонажа вне стейтмашины
    
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

    //описание предмета
}
