using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DoughnutAttacks : EnemyAttacks {

    public LayerMask abilityLayerMask;

    public AnimatorOverrideController[] doughnutAnimators;
    public Sprite[] doughnutSprites;
    [SyncVar]
    private ConstantsDictionary.DOUGHNUT_TYPE doughnutType;
    public GameObject abilityParticles;

    public int slowDuration = 2;
    public int abilityCooldown = 10;
    public float abilityRadius = 3f;
    public float poisonPercentage = 3f;
    public float poisonDuration = 3f;
    public float poisonTick = 1f;
    public float weaknessPercentage = 15f;
    public int weaknessDuration = 3;
    public float boostPercentage = 20f;
    public int boostDuration = 5;

    void Start() {
        hasAbility = true;
        enemyController = GetComponent<EnemyController>();
        doughnutType = (ConstantsDictionary.DOUGHNUT_TYPE)Random.Range(0, 3);
        ChangeSprite();
        GetComponent<TrailRenderer>().sortingLayerName = "Default";
        GetComponent<TrailRenderer>().sortingOrder = 1;
        StartCoroutine("PassiveAbility");
    }

    

    [Command]
    public override void CmdUseBasicAttack() {
        float damageDealt = DamageFormulas.CalculateBasicAttackDamage(basicAttackDamage, enemyController.target.GetComponent<PlayerAttacks>().m_defense, ConstantsDictionary.randomK, 0.7f);
        enemyController.target.GetComponent<PlayerHealth>().CmdTakeDamage(damageDealt);
    }

    [Command]
    public override void CmdUseAbility() {
        Vector3 abilityPosition = transform.position + ((enemyController.target.position - transform.position).normalized * 1.5f);
        //RpcChangeActiveParticles(true, abilityPosition);
        ChangeActiveParticles(true, abilityPosition);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(abilityPosition, abilityRadius);
        if (colliders.Length > 0) {
            foreach (Collider2D collider in colliders) {
                if (collider.gameObject.CompareTag(Tags.player)) {
                    if (doughnutType.Equals(ConstantsDictionary.DOUGHNUT_TYPE.Chocolate)) {
                        ChocolateEffect(collider);
                    }
                    else if (doughnutType.Equals(ConstantsDictionary.DOUGHNUT_TYPE.Strawberry)) {
                        StrawberryEffect(collider);
                    }
                }
                else if (collider.gameObject.CompareTag(Tags.enemy)) {
                    if (doughnutType.Equals(ConstantsDictionary.DOUGHNUT_TYPE.Cream))
                        CreamEffect(collider);
                }
            }
        }

        hasAbility = false;
        //RpcChangeActiveParticles(false, transform.position);
        ChangeActiveParticles(false, transform.position);
        //the stop is only a precaution
        StopCoroutine("AbilityCooldown");
        StartCoroutine("AbilityCooldown");
    }

    public IEnumerator PassiveAbility() {
        while (true) {
            Vector3[] positions = new Vector3[GetComponent<TrailRenderer>().positionCount];
            GetComponent<TrailRenderer>().GetPositions(positions);
            foreach (Vector3 position in positions) {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.4f, abilityLayerMask);
                foreach (Collider2D collider in colliders) {
                    if (collider.CompareTag(Tags.player)) {
                        collider.GetComponent<PlayerController>().SpeedChange(-0.5f, slowDuration);
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
        
    }


    void ChocolateEffect (Collider2D collider) {
        StartCoroutine(collider.GetComponent<PlayerHealth>().ApplyWeakness(weaknessPercentage, weaknessDuration));
    }

    void CreamEffect(Collider2D collider) {
        StartCoroutine(collider.GetComponent<EnemyController>().BoostSpeed(boostPercentage, boostDuration));
    }

    void StrawberryEffect(Collider2D collider) {
        StartCoroutine(collider.GetComponent<PlayerHealth>().TakeDotDamage(poisonPercentage, poisonDuration, poisonTick));
    }

    private IEnumerator AbilityCooldown () {
        int timePassed = 0;
        while (timePassed< abilityCooldown) {
            yield return new WaitForSeconds(1f);
            timePassed++;
        }
        hasAbility = true;
    }

    private void ChangeSprite() {
        gameObject.SetActive(false);
        GetComponent<SpriteRenderer>().sprite = doughnutSprites[(int)doughnutType];
        GetComponent<Animator>().runtimeAnimatorController = doughnutAnimators[(int)doughnutType];
        gameObject.SetActive(true);
    }

    [ClientRpc]
    void RpcChangeActiveParticles(bool into,Vector3 abilityPosition) {
        abilityParticles.transform.position = abilityPosition;
        abilityParticles.SetActive(into);
        if (into)
            abilityParticles.transform.parent = null;
        else
            abilityParticles.transform.parent = gameObject.transform;
        
    }

    void ChangeActiveParticles(bool into, Vector3 abilityPosition) {
        abilityParticles.transform.position = abilityPosition;
        abilityParticles.SetActive(into);
        if (into)
            abilityParticles.transform.parent = null;
        else
            abilityParticles.transform.parent = gameObject.transform;
        
    }
}
