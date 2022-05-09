using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    public PlayerHealth playerHealth;
     void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "SCP") {
            Debug.Log("contact");
            playerHealth.Damage(100f);
        }
    }
}
