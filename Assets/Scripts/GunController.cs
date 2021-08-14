using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public GunData[] guns;
    public Gun equippedGun;
    public int equippedGunIndex;
    public bool reloading;

    public event System.Action OnEquipGun;

    void Start(){
        if(guns.Length > 0){
            equippedGunIndex = 0;
            EquipGun(guns[equippedGunIndex].gun);
            guns[equippedGunIndex].currentMagAmmo = guns[equippedGunIndex].gun.magCapacity;
        }
        reloading = false;
    }

    public void LookAt(Vector3 lookPoint){
        Vector3 heightCorrectedPoint = new Vector3 (lookPoint.x, transform.position.y, lookPoint.z);
        equippedGun.transform.LookAt(heightCorrectedPoint);
    }

    public void nextGun(){
        if(!reloading){
            selectGun(equippedGunIndex + 1);
        }
    }

    public void previousGun(){
        if(!reloading){
            selectGun(equippedGunIndex - 1);
        }
    }

    public void selectGun(int gunIndex){
        if(!reloading){
            gunIndex = (gunIndex + guns.Length) % guns.Length;
            equippedGunIndex = gunIndex;
            EquipGun(guns[equippedGunIndex].gun);
        }
    }

    public void EquipGun (Gun gunToEquip){
        if(equippedGun != null){
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equippedGun.transform.parent = weaponHold;
        
        if(OnEquipGun != null){
            OnEquipGun();
        }
    }

    public void OnTriggerHold(){
        if(equippedGun != null && !reloading){
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRealease(){
        if(equippedGun != null){
            equippedGun.OnTriggerRealease();
        }
    }

    public void OnReload(){
        equippedGun.Reload();
    }


    [System.Serializable]
    public class GunData{
        public Gun gun;
        public bool infinitMag;
        public int currentMagAmmo = 0;
        public int MagAmount = 0;
    }
}
