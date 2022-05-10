using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlertSystem : MonoBehaviour
{
    [HideInInspector] public List<GameObject> _enemies = new List<GameObject>();
    public static EnemyAlertSystem Instance {get; private set;}
    [SerializeField] private float alertDist = 15f;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    public void AlertNeighbours(GameObject alerter) {
        for (int i = 0; i < _enemies.Count; i++) {
            if (Vector3.Distance(_enemies[i].transform.position, alerter.transform.position) < alertDist 
                && _enemies[i] != alerter && !_enemies[i].GetComponent<MTFBehaviour>().CanSeePlayer()) {
                    _enemies[i].GetComponent<MTFBehaviour>().HelpAlley(alerter.transform);
            }
        }
    }
}
