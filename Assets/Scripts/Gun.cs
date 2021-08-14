using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public new string name = "Default";
    public enum FireMode {Auto, Burst, Single}
    public FireMode fireMode;

    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetweenShot = 100;
    public float muzzleVelocity = 35;
    public int burstCount;

    public int magCapacity;

    public Transform shell;
    public Transform shellEjection;

    public AudioController audioController;
    public AudioClip shotSound;
    public float shotVolume;
    public AudioClip dryFireSound;
    public float dryFireVolume;
    public AudioClip reloadSound;
    public float reloadVolume;
    
    public float reloadTime = 1;

    Player player;
    GunController gunController;

    MuzzleFlash muzzleFlash;

    float nextShotTime;

    bool triggerRealeasedSinceLastShot;
    int shotsRemainingInBurst;

    void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
        gunController = FindObjectOfType<GunController>();
        if(reloadSound != null){
            reloadTime = reloadSound.length;
        }
    }

    public void Shoot(){
        if(Time.time > nextShotTime && Time.timeScale > 0 && gunController.guns[gunController.equippedGunIndex].gun.name == name && gunController.guns[gunController.equippedGunIndex].currentMagAmmo > 0){

            if(fireMode == FireMode.Burst){
                if(shotsRemainingInBurst == 0){
                    return;
                }
                shotsRemainingInBurst --;
            }else if(fireMode == FireMode.Single){
                if(!triggerRealeasedSinceLastShot){
                    return;
                }
            }

            for(int i = 0; i < projectileSpawn.Length; i ++){
                nextShotTime = Time.time + msBetweenShot / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }
            gunController.guns[gunController.equippedGunIndex].currentMagAmmo --;
            
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();

            audioController.PlaySound(shotSound, shotVolume, false);

        }else if(gunController.guns[gunController.equippedGunIndex].currentMagAmmo <= 0 && (gunController.guns[gunController.equippedGunIndex].infinitMag || gunController.guns[gunController.equippedGunIndex].MagAmount > 0)){
            if(triggerRealeasedSinceLastShot){
                Reload();
            }
        }else{
            if(Time.time > nextShotTime && Time.timeScale > 0 && triggerRealeasedSinceLastShot){
                DryFire();
            }
        }
    }

    public void OnTriggerHold(){
        Shoot();
        triggerRealeasedSinceLastShot = false;
    }

    public void OnTriggerRealease(){
        triggerRealeasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }

    public void DryFire(){
        if(triggerRealeasedSinceLastShot){
            triggerRealeasedSinceLastShot = false;
            audioController.PlaySound(dryFireSound, dryFireVolume, false);
        }
    }

    public void Reload(){
        StartCoroutine(Reloading());
    }

    IEnumerator Reloading(){
        
        if(!gunController.reloading){
            gunController.reloading = true;

            if(gunController.guns[gunController.equippedGunIndex].infinitMag || gunController.guns[gunController.equippedGunIndex].MagAmount > 0){
                gunController.guns[gunController.equippedGunIndex].currentMagAmmo = 0;

                audioController.PlaySound(reloadSound, reloadVolume, false);

                yield return new WaitForSeconds(gunController.guns[gunController.equippedGunIndex].gun.reloadTime);

                gunController.guns[gunController.equippedGunIndex].MagAmount --;
                gunController.guns[gunController.equippedGunIndex].currentMagAmmo = gunController.guns[gunController.equippedGunIndex].gun.magCapacity;
            }

            gunController.reloading = false;
        }
        
    }
}
