using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : Pickup
{
    public Gun gun;

    Player player;
    GunController gunController;

    protected override void Start(){
        base.Start();
        player = FindObjectOfType<Player>();
        gunController = player.GetComponent<GunController>();
    }

    public override void PickItem(){
        if(pickingAudioClip != null){
            audioController.PlaySound(pickingAudioClip, .8f, false);
        }
        foreach(GunController.GunData gunData in gunController.guns){
            if(gunData.gun.name == gun.name){
                if(gunData.currentMagAmmo <= 0){
                    gunData.currentMagAmmo = gunData.gun.magCapacity;
                }else{
                    gunData.MagAmount ++;
                }
            }
        }
        GameObject.Destroy(gameObject);
    }
}
