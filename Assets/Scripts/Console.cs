using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : MonoBehaviour
{
    public BossTurretHeadController turret;

    public GameObject blueScreen;
    public GameObject blackScreen;

    public bool activated;
    public bool playerInRange;

    public event System.Action OnActivation;

    void Start() {
        activated = false;
        playerInRange = false;
        blueScreen.SetActive(true);
        blackScreen.SetActive(false);
    }

    void Update(){
        blueScreen.SetActive(!activated);
        blackScreen.SetActive(activated);

        if(!activated && playerInRange && Input.GetKeyDown(KeyCode.E)){
            Activate();
        }
    }

    public void Activate(){
        activated = true;
        turret.Deactivate();

        if(OnActivation != null){
            OnActivation();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player"){
            playerInRange = false;
        }
    }
}
