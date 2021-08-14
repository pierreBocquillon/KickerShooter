using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

[RequireComponent (typeof(NavMeshAgent))]

public class Enemy : LivingEntity
{
    public Loot[] lootTable;

    public enum State {Idle, Chasing, Attacking};
    State currentState;

    public ParticleSystem deathEffect;

    public Renderer myRenderer;

    public AudioClip deathAudioClip;
    AudioController audioController;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;

    Color originalColor;

    float attackDistanceThreshold = .5f;
    float timeBetweenAttack = 1f;
    float damage = 1f;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    protected override void Start()
    {
        base.Start();
        audioController = GameObject.FindGameObjectWithTag("AudioController").transform.GetComponent<AudioController>();
        pathfinder = GetComponent<NavMeshAgent>();
        skinMaterial = myRenderer.material;
        originalColor = skinMaterial.color;

        if(GameObject.FindGameObjectWithTag("Player") != null){
            currentState = State.Chasing;
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }
        
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if(damage >= health){
            Death(hitPoint, hitDirection);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    void OnTargetDeath(){
        hasTarget = false;
        currentState = State.Idle;
    }

    void Death(Vector3 hitPoint, Vector3 hitDirection){
        Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetimeMultiplier);
        if(lootTable.Length > 0){
            float rngNumber = Random.value;
            float maxDropRate = 0f;
            for(int i = 0; i < lootTable.Length; i++){
                maxDropRate += lootTable[i].spawnRate;
                if (rngNumber < maxDropRate){
                    Instantiate(lootTable[i].item.gameObject, transform.position, Quaternion.identity);
                    audioController.PlaySound(deathAudioClip, .6f, false);
                    return;
                }
            }
        }
        audioController.PlaySound(deathAudioClip, .6f, false);
        return;
    }

    void Update()
    {
        if(hasTarget){
            if(Time.time > nextAttackTime){
                float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                if(sqrDistanceToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2)){
                    nextAttackTime = Time.time + timeBetweenAttack;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    IEnumerator Attack(){

        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 OriginalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * myCollisionRadius;
        
        float attackSpeed = 3;
        float percent = 0;

        skinMaterial.color = Color.yellow;

        bool hasAppliedDamage = false;

        while(percent <= 1){

            if(percent >= .5f && !hasAppliedDamage){
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(OriginalPosition, attackPosition, interpolation);

            yield return null;
        }
        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath(){
        float refreshRate = .25f;

        while(hasTarget){
            if(currentState == State.Chasing){
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
                if(!dead){
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    [System.Serializable]

    public class Loot {
        public Pickup item;
        public float spawnRate;
    }

}
