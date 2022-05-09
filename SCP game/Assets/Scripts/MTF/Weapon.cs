using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // these 2 Inaccuracy fields control 1 the general inaccuracy of the gun and at what distance
    // the gun will perform as it would if the player was exactly that distance away
    [SerializeField] private float gunInaccuracy = 2f, distanceInaccuracy = 10f;
    public float attackDistance = 30f;
    public void Aim(Transform target) {
        // first we get a direction that is exact to the player
        Vector3 dir = target.position - transform.position;
        // then we shoot a ray with the direction set to dir
        Ray r = new Ray();
        r.direction = dir;
        // it gets the vector3 point at the end of the ray using distanceInaccuracy as its length
        Vector3 newDir = r.GetPoint(distanceInaccuracy);
        // the offset is made by just randomly setting the xyz wity gunInaccuracy and then is combined with newDir
        Vector3 offset = new Vector3(Random.Range(-gunInaccuracy, gunInaccuracy), Random.Range(-gunInaccuracy, gunInaccuracy), Random.Range(-gunInaccuracy, gunInaccuracy));
        transform.rotation = Quaternion.LookRotation(newDir + offset);
        // end result is if the enemy is closer to the player than distanceInaccuracy the accuracy will 
        // effectively increase and if the enemy is further away the accuracy will decrease
    }
    public void Fire(){
        Debug.DrawRay(transform.position, transform.forward * 10000f, Color.green, Mathf.Infinity);
    }
}
