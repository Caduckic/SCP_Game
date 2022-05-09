using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopIfSeen : MonoBehaviour
{
    public Camera playerCam;
    public Transform playerTransform;
    [HideInInspector] public bool seen = false;
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3 currentRay, direction;
    private RaycastHit hit;
    void Start() {
        mesh = GetComponent<MeshFilter>().mesh;
    }
    void Update() {
        // vertices collected here so that the script can check if just a bit of the object is seen
        vertices = mesh.vertices;
        direction = transform.position - playerTransform.position;
        CheckIfSeen();
        Debug.DrawRay(playerTransform.position, direction, Color.green);
        for (int i = 0; i < vertices.Length; i++) {
            Debug.DrawRay(playerTransform.position, direction + vertices[i], Color.green);
        }
    }
    void CheckIfSeen() {
        // goes through all verts to see if any are on the screen
        for (int i = 0; i < vertices.Length; i++) {
            Vector3 position = playerCam.WorldToScreenPoint(transform.position + vertices[i]);
            // if any verts are behind screen, skip checking all of them
            // most likely this will be the object not being on screen at all
            if (position.z < 0) {
                seen = false;
                break;
            }
            // if it finds one then stop and check if its behind a wall
            if (position.z > 0 && position.x > 0 && position.x < Screen.width 
                && position.y > 0 && position.y < Screen.height) {
                    seen = CheckIfCovered();
                    break;
                }
            else seen = false;
        }
    }
    bool CheckIfCovered() {
        // layermask here so the ray doesn't return a collision with the player itself
        int layerMask =~ LayerMask.GetMask("Player");
        // if its already seen we shouldn't have to shoot all the rays
        // just the ray it was seen with, for performace
        if (seen) {
            var hitObject = Physics.Raycast(playerTransform.position, currentRay,
                out hit, Mathf.Infinity, layerMask);
            if (hitObject) {
                if (hit.collider.gameObject == gameObject) return true;
            }
        }
        // shoots a ray at all verts, if any of them hit SCP the function saves the ray and returns true
        for (int i = 0; i < vertices.Length; i++) {
            var hitObject = Physics.Raycast(playerTransform.position, direction + vertices[i],
                out hit, Mathf.Infinity, layerMask);
                if (hitObject) {
                    if (hit.collider.gameObject == gameObject) {
                        currentRay = direction + vertices[i];
                        return true;
                    }
                }
        }
        return false;
    }
}