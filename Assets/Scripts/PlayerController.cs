using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent (typeof(Rigidbody))]

public class PlayerController : MonoBehaviour
{
    
    public Rigidbody myRigidBody;
    public Animator animator;

    public Player player;

    public AudioController audioController;
    public AudioClip deathAudioClip;
    public AudioClip hitAudioClip;

    Vector3 velocity;

    void Start()
    {
        player.OnDeath += OnPlayerDeath;
        player.OnHit += OnPlayerHit;
    }

    public void Move(Vector3 inputVelocity){
        velocity = inputVelocity;

        if(velocity.sqrMagnitude > .01f){
            animator.SetBool("IsMoving", true);
        }else{
            animator.SetBool("IsMoving", false);
        }
    }
    public void LookAt(Vector3 lookPoint){
        Vector3 heightCorrectedPoint = new Vector3 (lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }

    void FixedUpdate() {
        myRigidBody.MovePosition(myRigidBody.position + velocity * Time.fixedDeltaTime);
    }

    void OnPlayerDeath(){
        audioController.PlaySound(deathAudioClip, .4f, false);
    }

    void OnPlayerHit(){
        audioController.PlaySound(hitAudioClip, .5f, false);
    }
}
