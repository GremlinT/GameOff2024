using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AlienStates
{
    idle,
    walking
}

public class AlienBehavoiur : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform TR;

    private AlienStates currentState;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        TR = transform;
        currentState = AlienStates.idle;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);
        animator.SetBool("isWalking", true);
        currentState = AlienStates.walking;
    }

    public void UseItem(UsableItem item)
    {
        Debug.Log(item.gameObject.name);
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

                break;
            case AlienStates.walking:
                if (Vector3.Distance(TR.position, agent.destination) < 0.1f)
                {
                    animator.SetBool("isWalking", false);
                    currentState = AlienStates.idle;
                }
                break;
        }
    }
}
