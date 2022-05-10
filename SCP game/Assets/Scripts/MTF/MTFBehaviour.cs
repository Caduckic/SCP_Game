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
    private bool walkPointSet, playerFoundRecently, playerLost, trackingPlayer, alertedByAlley;
    private float FOV = 60f, searchTimer = 0f, trackingTimer = 0f, strafeTimer = 0f, strafeRate;
    private Vector3 strafeDir;
    private Transform alertingAlley;
    private void Awake() {
        EnemyAlertSystem.Instance._enemies.Add(gameObject);
    }
    void Start()
    {
        strafeRate = Random.Range(1.5f, 2f);
        strafeDir = transform.right;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        if (!CanSeePlayer() && !playerFoundRecently && !alertedByAlley) {
            FOV = 60f;
            Patrol();
            Debug.Log(gameObject.name + " is patrolling");
        }
        if (!CanSeePlayer() && playerFoundRecently && !playerLost) {
            TrackingTimer();
            ChasePlayer();
        }
        if (playerLost && playerFoundRecently && !alertedByAlley) {
            SearchTimer();
            SearchForPlayer();
        }
        if (CanSeePlayer()) {
            alertedByAlley = false;
            FOV = 90f;
            playerLost = false;
            trackingPlayer = false;
            ChasePlayer();
            Attack(player);
        }
        if (alertedByAlley) {
            GoToAlley();
        }
    }
    public bool CanSeePlayer() {
        Vector3 directionToPlayer = player.position - transform.position;
        float viewingAngle = Vector3.Angle(transform.forward, directionToPlayer);
        RaycastHit hit;
        var hitObject = Physics.Raycast(transform.position, directionToPlayer, out hit, viewDistance);
        if (Mathf.Abs(viewingAngle) <= FOV && hitObject && hit.collider.gameObject.layer == player.gameObject.layer) return true;
        else return false;
    }
    private void Patrol() {
        searchTimer = 0f;
        trackingTimer = 0f;
        if (agent.remainingDistance < agent.stoppingDistance) {
            Vector3 next = nextDirection.PickRandom(transform.position, patrolWalkDist);
            agent.SetDestination(next);
        }
    }
    private void ChasePlayer() {
        Debug.Log(gameObject.name + " is chasing player");
        EnemyAlertSystem.Instance.AlertNeighbours(gameObject);
        StrafeSwitcher();
        if (!trackingPlayer) {
            trackingTimer = 0f;
        }
        searchTimer = 0f;
        playerFoundRecently = true;
        if (Vector3.Distance(transform.position, player.position) > closeUpDist){
            agent.isStopped = false;
            agent.updateRotation = true;
            agent.SetDestination(player.position);
        }
        if (Vector3.Distance(transform.position, player.position) < closeUpDist) {
            agent.updateRotation = false;
            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir.normalized), Time.deltaTime/0.6f);
            if (Vector3.Distance(transform.position, player.position) < closeUpDist) {
                agent.isStopped = true;
                agent.Move(strafeDir * Time.deltaTime);
            }
        }
    }
    private void Attack(Transform target) {
        if (Vector3.Distance(target.position, transform.position) < weapon.attackDistance) {
            weapon.Aim(target);
            Debug.Log(gameObject.name + " is attacking player");
            weapon.Fire();
        }
    }
    private void TrackingTimer() {
        trackingPlayer = true;
        trackingTimer += Time.deltaTime;
        if (trackingTimer >= 10f) {
            playerLost = true;
        }
    }
    private void SearchForPlayer() {
        Debug.Log(gameObject.name + " is searching for the player");
        if (agent.remainingDistance < agent.stoppingDistance) {
            Vector3 next = nextDirection.PickRandom(player.position, playerSearchDist);
            agent.SetDestination(next);
        }
    }
    private void SearchTimer() {
        trackingTimer = 0f;
        searchTimer += Time.deltaTime;
        if (searchTimer >= 60f) {
            Debug.Log(gameObject.name + "gave up looking");
            playerFoundRecently = false;
            playerLost = false;
        }
    }
    private void StrafeSwitcher() {
        strafeTimer += Time.deltaTime;
        if (strafeTimer > strafeRate) {
            strafeRate = Random.Range(1.5f, 2f);
            strafeTimer = 0f;
            strafeDir = Random.Range(0f,1f) > 0.5f ? transform.right + -transform.forward : -transform.right + -transform.forward;
        }
    }
    private void GoToAlley() {
        Debug.Log(gameObject.name + " is coming to help" + alertingAlley.gameObject.name);
        if (alertingAlley != null){
            if (Vector3.Distance(alertingAlley.position, transform.position) > 3f) {
                agent.SetDestination(alertingAlley.position);
            }
            else if (Vector3.Distance(alertingAlley.position, transform.position) < 3f) {
                playerFoundRecently = true;
                playerLost = true;
                alertedByAlley = false;
            }
        }
    }
    public void HelpAlley(Transform alley) {
        alertedByAlley = true;
        alertingAlley = alley;
    }
}
