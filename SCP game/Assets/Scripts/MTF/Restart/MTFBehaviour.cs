using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MTFBehaviour : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float viewDistance = 50f, patrolWalkDist = 10f, playerSearchDist = 20f, closeUpDist = 2f;
    [SerializeField] private NextDirection nextDirection;
    [SerializeField] private Weapon weapon;
    private bool walkPointSet, playerFoundRecently;
    private float FOV = 60f, timer = 0f;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CanSeePlayer() && !playerFoundRecently) {
            FOV = 60f;
            Patrol();
            Debug.Log("patrolling");
        }
        if (!CanSeePlayer() && playerFoundRecently) {
            Debug.Log("Searching");
            SearchTimer();
            SearchForPlayer();
        }
        if (CanSeePlayer()) {
            Debug.Log("Chasing");
            FOV = 90f;
            ChasePlayer();
            Attack(player);
        }
    }
    private bool CanSeePlayer() {
        Vector3 directionToPlayer = player.position - transform.position;
        float viewingAngle = Vector3.Angle(transform.forward, directionToPlayer);
        RaycastHit hit;
        var hitObject = Physics.Raycast(transform.position, directionToPlayer, out hit, viewDistance);
        if (Mathf.Abs(viewingAngle) <= FOV && hitObject && hit.collider.gameObject.layer == player.gameObject.layer) return true;
        else return false;
    }
    private void Patrol() {
        timer = 0f;
        if (agent.remainingDistance < agent.stoppingDistance) {
            Vector3 next = nextDirection.PickRandom(transform.position, patrolWalkDist);
            agent.SetDestination(next);
        }
    }
    private void ChasePlayer() {
        timer = 0f;
        playerFoundRecently = true;
        if (Vector3.Distance(transform.position, player.position) > closeUpDist){
            agent.SetDestination(player.position);
        }
        if (Vector3.Distance(transform.position, player.position) < closeUpDist) {
            agent.SetDestination(transform.position);
        }
    }
    private void Attack(Transform target) {
        if (Vector3.Distance(target.position, transform.position) < weapon.attackDistance) {
            Debug.Log("Shooting");
            weapon.Aim(target);
            weapon.Fire();
        }
    }
    private void SearchForPlayer() {
        if (agent.remainingDistance < agent.stoppingDistance) {
            Vector3 next = nextDirection.PickRandom(player.position, playerSearchDist);
            agent.SetDestination(next);
        }
    }
    private void SearchTimer() {
        timer += Time.deltaTime;
        if (timer >= 60f) {
            playerFoundRecently = false;
        }
    }
}
