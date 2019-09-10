using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SamuraiChipAttacks : EnemyAttacks {

    public LayerMask playerLayerMask;

    void Start() {
        hasAbility = false;
        enemyController = GetComponent<EnemyController>();
    }

    [Command]
    public override void CmdUseBasicAttack() {
        Vector3 damageArea = transform.position + ((enemyController.target.position - transform.position).normalized);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(damageArea, 1f, playerLayerMask);

        foreach (Collider2D collider in colliders) {
            if (collider.CompareTag(Tags.player) && collider.GetComponent<PlayerHealth>()!=null) {
                float damageDealt = DamageFormulas.CalculateBasicAttackDamage(basicAttackDamage, collider.GetComponent<PlayerAttacks>().m_defense, ConstantsDictionary.randomK, 0.7f);
                collider.GetComponent<PlayerHealth>().CmdTakeDamage(damageDealt);
            }
        }

        
    }

    [Command]
    public override void CmdUseAbility() { }
}
