using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        TR = transform;
        currentState = AlienStates.idle;
        currentMoveToPoint = Vector3.zero;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (currentState != AlienStates.use) ClearItem();
        currentMoveToPoint = targetPosition;
    }

    public void UseItem(UsableItem item)
    {
        currentItem = item;
    }

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
                if (Vector3.Distance(TR.position, agent.destination) < 0.1f)
                {
                    animator.SetBool("isWalking", false);
                    currentMoveToPoint = Vector3.zero;
                    currentState = AlienStates.idle;
                }
                break;
            case AlienStates.use:
                if (currentMoveToPoint != Vector3.zero)
                {
                    if (currentItem.canStopManualy)
                    {
                        ClearItem();
                    }
                    else
                    {
                        currentMoveToPoint = Vector3.zero;
                    }
                }
                if (currentItem == null) currentState = AlienStates.idle;
                break;
        }
    }

    private void ClearItem()
    {
        currentItem = null;
    }
}
