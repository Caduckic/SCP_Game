using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MTFAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public StarterAssets.StarterAssetsInputs n;
    [SerializeField] private Weapon weapon;
    [SerializeField] private NextDirection nextDirection;
    [SerializeField] private float ViewDistance = 50f, minDistance = 2f, randomMoveDist = 5f, searchDist = 20f;
    private Vector3 direction;
    private bool dirSetting, playerFound, playerLost, searching, lostTimerComplete = true;
    public Transform player;
    private float viewingAngle;
    private RaycastHit hit, lastLocation;
    private int layerMask;
    
    void Start()
    {
        layerMask =~ LayerMask.GetMask("MTF");
        direction = nextDirection.PickRandom(transform.position, randomMoveDist);
        agent.SetDestination(direction);
    }
    void Update()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        viewingAngle = Vector3.Angle(transform.forward, directionToPlayer);
        if (Mathf.Abs(viewingAngle) <= 60f && !playerFound) {
            
            var hitObject = Physics.Raycast(transform.position, directionToPlayer, out hit, ViewDistance, layerMask);
            Debug.DrawRay(transform.position,directionToPlayer, Color.green, Mathf.Infinity);
            if (hitObject) {
                if (hit.collider.gameObject.layer == player.gameObject.layer) {
                    playerLost = false;
                    searching = false;
                    playerFound = true;
                }
            }
        }
        if(playerFound) {
            //if (Mathf.Abs(viewingAngle) <= 90f) {
                //agent.updateRotation = true;
               // playerLost = true;
                //playerFound = false;
            //}
            var hitObject = Physics.Raycast(transform.position, directionToPlayer, out hit, ViewDistance, layerMask);
            if (lostTimerComplete) {
                lostTimerComplete = false;
                StartCoroutine(PlayerLostTimer());
            }
            var distance = Vector3.Distance(transform.position, hit.point);
            Ray r = new Ray();
            r.origin = transform.position;
            r.direction = hit.point - transform.position;
            Debug.DrawRay(transform.position, directionToPlayer, Color.blue, Mathf.Infinity);
            Debug.DrawRay(r.GetPoint(distance - minDistance), Vector3.down, Color.black, Mathf.Infinity);
            bool hitGround = false;
            if (agent.isStopped) {
                hitGround = Physics.Raycast(r.GetPoint(distance - minDistance), Vector3.down, out lastLocation, Mathf.Infinity, layerMask);
            } 
            if (hitGround) {
                if (Vector3.Distance(transform.position, lastLocation.point) > minDistance/3f) {
                    agent.isStopped = false;
                    agent.updateRotation = false;
                }
            }
            if (agent.remainingDistance < agent.stoppingDistance) {
                agent.isStopped = true;
            }
        }
        if (!agent.updateRotation && playerFound) {
            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            Debug.Log("look");
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir.normalized), (5) * Time.deltaTime);
        }
        if (n.dir && !dirSetting) {
            dirSetting = true;
            direction = nextDirection.PickRandom(transform.position, randomMoveDist);
            agent.SetDestination(direction);
        }
        if (!n.dir) dirSetting = false;
        if (agent.remainingDistance < agent.stoppingDistance && !playerFound && !playerLost && !searching) {
            direction = nextDirection.PickRandom(transform.position, randomMoveDist);
            agent.SetDestination(direction);
        }
        if (playerLost && !searching) {
            agent.SetDestination(lastLocation.point);
            if (agent.remainingDistance < agent.stoppingDistance) {
                searching = true;
            }
        }
        if (searching) {
            if (agent.remainingDistance < agent.stoppingDistance) {
                direction = nextDirection.PickRandom(player.position, searchDist);
                agent.SetDestination(direction);
                Debug.Log("done");
            }
        }
        IEnumerator PlayerLostTimer() {
            yield return new WaitForSeconds(5f);
            lostTimerComplete = true;

            if (!Physics.Raycast(transform.position, directionToPlayer, out hit, ViewDistance, layerMask)) {
                agent.updateRotation = true;
                playerLost = true;
                playerFound = false;
                Debug.Log("lostbyDist");
            }
            else {
                if (hit.collider.gameObject.layer != player.gameObject.layer) {
                    agent.updateRotation = true;
                    playerLost = true;
                    playerFound = false;
                }
            }
        }
    }
}
