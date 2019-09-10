using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Oil : NetworkBehaviour {

    [HideInInspector]
    public float attackDamage;
    public AttackTypes attackType = AttackTypes.Impact;
    public float oilDuration = 3f;
    public int oilSlowDuration=2;

    private void OnEnable() {

        transform.rotation = Quaternion.identity;
        Invoke("Destroy", oilDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(Tags.player)) {
            PlayerAttacks playerAttacks = collision.gameObject.GetComponent<PlayerAttacks>();
            if (playerAttacks != null) {
                float damageDealt = DamageFormulas.CalculateBasicAttackDamage(attackDamage, playerAttacks.m_defense, ConstantsDictionary.randomK, 1f);
                playerAttacks.gameObject.GetComponent<PlayerHealth>().CmdTakeDamage(damageDealt);
                playerAttacks.gameObject.GetComponent<PlayerController>().SpeedChange(-0.5f,oilSlowDuration);
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
