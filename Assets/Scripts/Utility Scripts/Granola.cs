using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Granola : NetworkBehaviour {

    [HideInInspector]
    public float attackDamage;
    [HideInInspector]
    public bool miniBoss = false;
    public AttackTypes attackType = AttackTypes.Impact;
    public float granolaDuration = 1.5f;
    public float poisonPercentage = 3f;
    public float poisonDuration = 3f;
    public float poisonTick = 1f;

    private void OnEnable() {
        transform.rotation = Quaternion.identity;
        Invoke("Destroy", granolaDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(Tags.player)) {
            PlayerAttacks playerAttacks = collision.gameObject.GetComponent<PlayerAttacks>();
            if (playerAttacks != null) {
                float damageDealt = DamageFormulas.CalculateBasicAttackDamage(attackDamage, playerAttacks.m_defense, ConstantsDictionary.randomK, 1f);
                playerAttacks.gameObject.GetComponent<PlayerHealth>().CmdTakeDamage(damageDealt);
                if(miniBoss)
                    playerAttacks.gameObject.GetComponent<PlayerHealth>().TakeDotDamage(poisonPercentage, poisonDuration, poisonTick);
            }
            NetworkServer.Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag(Tags.building)) {
            NetworkServer.Destroy(gameObject);
        }
    }

    private void Destroy() {
        NetworkServer.Destroy(gameObject);
    }

    private void OnDisable() {
        CancelInvoke();
    }
}
