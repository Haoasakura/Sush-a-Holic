using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PopcornAttacks : EnemyAttacks {

    public LayerMask playerMask;
    public GameObject popcorn;
    public GameObject miniPopcorn;

    public float explosionRadius = 5f;
    public int popcornToSpawn = 2;
    public int miniPopcornToSpawn = 3;
    public bool miniBoss = false;

    [HideInInspector]
    public bool onlyOnce;

    void Start() {
        hasAbility = false;
        onlyOnce = true;
        enemyController = GetComponent<EnemyController>();
    }

    [Command]
    public override void CmdUseBasicAttack() {
        enemyController.canMove = false;
    }

    [Command]
    public override void CmdUseAbility() { }

    public void Explosion() {

        if (!onlyOnce)
            return;

        onlyOnce = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerMask);

        for (int i = 0; i < colliders.Length; i++) {
            PlayerHealth playerHealth = colliders[i].GetComponent<PlayerHealth>();

            if (!playerHealth)
                continue;

            float damageDealt = DamageFormulas.CalculateBasicAttackDamage(basicAttackDamage, playerHealth.GetComponent<PlayerAttacks>().m_defense, ConstantsDictionary.randomK, 0.5f);
            playerHealth.CmdTakeDamage(damageDealt);
        }

        StopAllCoroutines();
        Destroy(gameObject);

        //Spawn Of Childs
        if (miniBoss) {
            for (int i = 0; i < popcornToSpawn; i++) {
                var popCorn = Instantiate(popcorn, transform.position, popcorn.transform.rotation);
				if (NetworkServer.active) {
					NetworkServer.Spawn (popCorn);
				}
            }
        }
        else {
            for (int i = 0; i < miniPopcornToSpawn; i++) {
                var miniPopCorn = Instantiate(miniPopcorn, transform.position + new Vector3(Random.value*0.5f, Random.value*0.5f, 0f), miniPopcorn.transform.rotation);
				if (NetworkServer.active) {
					NetworkServer.Spawn (miniPopCorn);
				}
            }
        }
    }

}
