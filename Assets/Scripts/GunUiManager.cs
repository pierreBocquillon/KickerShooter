using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunUiManager : MonoBehaviour
{
    public GunModel[] modelList;
    public GunController playerGunController;


    void Start()
    {
        playerGunController.OnEquipGun += RefreshGuns;
    }
    
    public void RefreshGuns(){
        foreach(GunModel gun in modelList){
            gun.model.SetActive(playerGunController.equippedGun.name == gun.gunName);
        }
    }

    [System.Serializable]
    
    public class GunModel{
        public GameObject model;
        public string gunName;
    }
}
