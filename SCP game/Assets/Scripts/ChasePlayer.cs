using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class ChasePlayer : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    public StopIfSeen seenState;
    public Blink eyes;
    public CapsuleCollider scp;
    void Update()
    {
        // Very Simple here, this is seperated out due to me not being sure how you want movement to work
        if (seenState.seen && !eyes.eyesShut) {
            agent.isStopped = true;
            scp.isTrigger = false;
        }
        else {
            agent.isStopped = false;
            scp.isTrigger = true;
        }
        agent.SetDestination(player.position);
    }
}
