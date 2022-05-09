using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    float health = 100;
    public void Damage(float damage) {
        health = health - damage;
        Debug.Log(health);
    }
}
