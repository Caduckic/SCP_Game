using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextDirection : MonoBehaviour
{   
    public Vector3 PickRandom(Vector3 pos, float dist) {
        Vector3 dir = (Random.onUnitSphere * (dist += Random.Range(-2f, 2f))) + pos;
        return dir;
    }
}
