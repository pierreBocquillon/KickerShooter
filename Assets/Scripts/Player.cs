using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(GunController))]

public class Player : LivingEntity
{

    public float moveSpeed = 5f;
    public float hitDuration;
    public Color flashColor;
    float nextHitTime;
    public Renderer playerRenderer;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        nextHitTime = Time.time + hitDuration;
    }

    void Update()
    {
        if(Time.timeScale > 0){
            //movement input
            Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),0f,Input.GetAxisRaw("Vertical"));
            Vector3 moveVelocity = moveInput.normalized * moveSpeed;
            controller.Move(moveVelocity);

            //look input
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up,Vector3.up);
            if(gunController.equippedGun != null){
                groundPlane = new Plane(Vector3.up,new Vector3(0,gunController.equippedGun.transform.position.y,0));
            }
            float rayDistance;

            if(groundPlane.Raycast(ray,out rayDistance)){
                Vector3 point = ray.GetPoint(rayDistance);
                //Debug.DrawLine(ray.origin,point,Color.red);
                controller.LookAt(point);
                gunController.LookAt(point);
            }

            //weapon input

            if(Input.GetMouseButton(0)){
                gunController.OnTriggerHold();
            }
            
            if(Input.GetMouseButtonUp(0)){
                gunController.OnTriggerRealease();
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f ){
                gunController.nextGun();
            }else if(Input.GetAxis("Mouse ScrollWheel") > 0f ){
                gunController.previousGun();
            }

            if(Input.GetKeyDown(KeyCode.R)){
                gunController.OnReload();
            }

            if(Input.GetKeyDown(KeyCode.Alpha1)){
                gunController.selectGun(0);
            }
            if(Input.GetKeyDown(KeyCode.Alpha2)){
                gunController.selectGun(1);
            }
            if(Input.GetKeyDown(KeyCode.Alpha3)){
                gunController.selectGun(2);
            }
            if(Input.GetKeyDown(KeyCode.Alpha4)){
                gunController.selectGun(3);
            }
            if(Input.GetKeyDown(KeyCode.Alpha5)){
                gunController.selectGun(4);
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        if(Time.time >= nextHitTime){
            nextHitTime = Time.time + hitDuration;
            StartCoroutine(invicibilityFlash());
            base.TakeDamage(damage);
        }
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection){
        TakeDamage(damage);
    }

    IEnumerator invicibilityFlash(){
        Material playerMat = playerRenderer.material;
        Color initialColor = playerMat.color;
        float flashTimer = 0f;

        while(flashTimer < hitDuration){

            playerMat.color = Color.Lerp(initialColor,flashColor, Mathf.PingPong(flashTimer * 5f, 1));

            flashTimer += Time.deltaTime;
            yield return null;
        }

        playerMat.color = initialColor;
    }
}
