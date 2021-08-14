using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTurretHeadController : MonoBehaviour
{   
    public float rotationSpeed;
    public float timeBetweenShots;

    public AudioController audioController;
    public AudioClip shotSound;
    
    public Projectile projectile;
    public Transform projectileSpawn;
    public float muzzleVelocity;
    public Transform deactivatedTarget;
    public GameObject headLight;

    Material headLightMat;

    Transform target;
    float nextShotTimer;

    public bool isActive = true;

    void Start(){
        nextShotTimer = 0f;
        headLightMat = headLight.GetComponent<Renderer>().material;
    }

    void Update() {
        if(isActive && target != null){
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            if(Time.time >= nextShotTimer && Mathf.Abs(Quaternion.Dot(transform.rotation, targetRotation)) > .999f ){
                Shoot();
            }
        }

        if(!isActive){
            Quaternion targetRotation = Quaternion.LookRotation(deactivatedTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            headLightMat.SetColor("_EmissionColor", Color.black);
            headLightMat.color = Color.black;
        }
        
        //Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward) * 100);
    }

    public void Deactivate(){
        isActive = false;
    }

    public void Shoot(){
        Projectile newProjectile = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation) as Projectile;
        newProjectile.SetSpeed(muzzleVelocity);
        audioController.PlaySound(shotSound, .8f, false);
        nextShotTimer = Time.time + timeBetweenShots;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            target = other.transform;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player"){
            target = null;
        }
    }
}
