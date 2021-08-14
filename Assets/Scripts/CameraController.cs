using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Player target;

    Vector3 cameraOffset;
    bool targetIsAlive = true;

    void Start() {
        if(target != null){
            cameraOffset = new Vector3(transform.position.x - target.transform.position.x, transform.position.y - target.transform.position.y, transform.position.z - target.transform.position.z);
            target.OnDeath += OnTargetDeath;
        }
    }

    void Update() {
        if(targetIsAlive){
            transform.position = target.transform.position + cameraOffset;
        }
    }

    public void OnTargetDeath(){
        targetIsAlive = false;
    }

}
