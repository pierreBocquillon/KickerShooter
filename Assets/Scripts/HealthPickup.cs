using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    Player player;

    protected override void Start(){
        base.Start();
        player = FindObjectOfType<Player>();
    }

    public override void PickItem(){
        if(player.health < player.startingHealth){
            if(pickingAudioClip != null){
                audioController.PlaySound(pickingAudioClip, 1f, false);
            }
            player.health ++;
            GameObject.Destroy(gameObject);
        }
    }
}
