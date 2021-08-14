using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{   
    public float lifeTime = 10;
    public float rotationSpeed = .5f;
    public float floatingSpeed = 3;
    public float floatingAmplitude = .5f;
    public float initialYPosition = 1;

    public AudioClip pickingAudioClip;
    public AudioController audioController;


    float deSpawnTime;

    protected virtual void Start() {
        deSpawnTime = Time.time + lifeTime;
        audioController = GameObject.FindGameObjectWithTag("AudioController").transform.GetComponent<AudioController>();
    }

    void Update() {
        if(deSpawnTime < Time.time){
            DeSpawn();
        }else{
            transform.Rotate(new Vector3(0, rotationSpeed, 0));
            transform.position = new Vector3(transform.position.x, initialYPosition + Mathf.Sin(Time.time * floatingSpeed) * floatingAmplitude, transform.position.z);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player"){
            PickItem();
        }
    }

    public void DeSpawn(){
        GameObject.Destroy(gameObject);
    }

    public virtual void PickItem(){
        if(pickingAudioClip != null){
            audioController.PlaySound(pickingAudioClip, .8f, false);
        }
        GameObject.Destroy(gameObject);
        
    }
}
