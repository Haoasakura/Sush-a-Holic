using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MiniPopcornAttacks : EnemyAttacks {

    void Start() {
        hasAbility = false;
        enemyController = GetComponent<EnemyController>();
    }

    [Command]
    public override void CmdUseBasicAttack() {
        float damageDealt = DamageFormulas.CalculateBasicAttackDamage(basicAttackDamage, enemyController.target.GetComponent<PlayerAttacks>().m_defense, ConstantsDictionary.randomK, 0.05f);
        enemyController.target.GetComponent<PlayerHealth>().CmdTakeDamage(damageDealt);
    }

    [Command]
    public override void CmdUseAbility() { }
}
