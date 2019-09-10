using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WonaldAttacks : EnemyAttacks {

    private enum AnimationToPlay {Ability1, Ability2, Attack1, Attack2}

    public int ability1Cooldown = 10;
    public int ability2Cooldown = 20;
    public int enemyToSpawn = 3;

    public GameObject oil;
    public float oilVelocity = 5f;

    private int currentAnimation = 0;
    private bool ability1Avaible;
    private bool ability2Avaible;
    public bool usingAbility1;
    public bool usingAbility2;
    private float fireRate = 0.1f;
    private float nextFire = 0.0f;

    private EnemySpawner enemySpawner;

    void Start() {
        hasAbility = true;
        ability1Avaible = true;
        ability2Avaible = true;
        usingAbility1 = false;
        usingAbility2 = false;
        enemyController = GetComponent<EnemyController>();
        CmdGetTagObject();
    }

    [Command]
    public void CmdGetTagObject() {
        enemySpawner = GameObject.FindGameObjectWithTag(Tags.gameManager).GetComponent<EnemySpawner>();
    }

    [Command]
    public override void CmdUseBasicAttack() {
        if (!usingAbility1 && !usingAbility2) {
            if ((enemyController.target.transform.position - transform.position).magnitude < 2f) {
                Debug.Log("Attack 1");
                currentAnimation = (int)AnimationToPlay.Attack1;
                RpcSetAttackAnimation();
                float damageDealt = DamageFormulas.CalculateBasicAttackDamage(basicAttackDamage, enemyController.target.GetComponent<PlayerAttacks>().m_defense, ConstantsDictionary.randomK, 1f);
                enemyController.target.GetComponent<PlayerHealth>().CmdTakeDamage(damageDealt);
            }
            else {
                Debug.Log("Attack 2");
                currentAnimation = (int)AnimationToPlay.Attack2;
                RpcSetAttackAnimation();
            }
        }
    }

    [Command]
    public override void CmdUseAbility() {
        if(ability2Avaible && !usingAbility1) {
            Debug.Log("Ability 2");
            ability2Avaible = false;
            usingAbility2 = true;
            currentAnimation = (int)AnimationToPlay.Ability2;
            RpcSetAttackAnimation();
			enemySpawner.WonaldsCall(transform.position,enemyToSpawn);

        }
        else if (ability1Avaible && !usingAbility2) {
            Debug.Log("Ability 1");
            ability1Avaible = false;
            usingAbility1 = true;
            currentAnimation = (int)AnimationToPlay.Ability1;
            RpcSetAttackAnimation();
            transform.position = Vector3.MoveTowards(transform.position, GetComponent<EnemyController>().target.position, GetComponent<EnemyController>().speed * Time.deltaTime);
            enemyController.BoostSpeed(20, 3);
            StartAbility1Cooldown();
        }
        if (!ability1Avaible && !ability2Avaible)
            hasAbility = false;
    }

    [Command]
    public void CmdShoot() {
        if (!isServer)
            return;

        if (Time.time > nextFire) {
            Debug.Log("Shoot");
            nextFire = Time.time + fireRate;

            GameObject oil = Instantiate(this.oil);
            oil.transform.position = transform.position + ((enemyController.target.position - transform.position).normalized * 0.5f);
            oil.GetComponent<Oil>().attackDamage = basicAttackDamage;

            Vector2 direction = enemyController.target.position - transform.position;

            oil.GetComponent<Rigidbody2D>().velocity = oilVelocity * direction.normalized;

            NetworkServer.Spawn(oil);
        }
    }

    [Command]
    public void CmdSetAttackAnimation() {
        RpcSetAttackAnimation();

    }

    [ClientRpc]
    public void RpcSetAttackAnimation() {
        if(currentAnimation==(int)AnimationToPlay.Ability1)
            GetComponent<Animator>().SetFloat("CurrentAttack", 0f);
        else if (currentAnimation == (int)AnimationToPlay.Ability2)
            GetComponent<Animator>().SetFloat("CurrentAttack", 0.33f);
        else if (currentAnimation == (int)AnimationToPlay.Attack1)
            GetComponent<Animator>().SetFloat("CurrentAttack", 0.66f);
        else if (currentAnimation == (int)AnimationToPlay.Attack2)
            GetComponent<Animator>().SetFloat("CurrentAttack", 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(Tags.player) && usingAbility1) {
            Debug.Log("Trumpled");
            float damageDealt = DamageFormulas.CalculateBasicAttackDamage(basicAttackDamage, enemyController.target.GetComponent<PlayerAttacks>().m_defense, ConstantsDictionary.randomK, 1f);
            collision.gameObject.GetComponent<PlayerHealth>().CmdTakeDamage(damageDealt);
        }
    }

    public void StartAbility1Cooldown() {
        if(!ability1Avaible)
            StartCoroutine("Ability1Cooldown");
        
    }
    public void StartAbility2Cooldown() {
        if (!ability2Avaible)
            StartCoroutine("Ability2Cooldown");

    }
    private IEnumerator Ability1Cooldown() {
        int timePassed = 0;
        StartCoroutine("Trumple");
        while (timePassed < ability1Cooldown) {
            yield return new WaitForSeconds(1f);
            timePassed++;
            if (enemyController.target!=null && (enemyController.target.position - transform.position).magnitude <= 0.5f)
                usingAbility1 = false;
        }
        StopCoroutine("Trumple");
        usingAbility1 = false;
        ability1Avaible = true;
        hasAbility = true;
    }
    private IEnumerator Trumple() {
        while(usingAbility1) {
            if (GetComponent<EnemyController>().target!=null    )
            transform.position = Vector3.MoveTowards(transform.position, GetComponent<EnemyController>().target.position, GetComponent<EnemyController>().speed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator Ability2Cooldown() {
        int timePassed = 0;
        usingAbility2 = false;
        while (timePassed < ability2Cooldown) {
            yield return new WaitForSeconds(1f);
            timePassed++;
        }
        ability2Avaible = true;
        hasAbility = true;
    }
}
