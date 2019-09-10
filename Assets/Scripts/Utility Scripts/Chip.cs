using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Chip : NetworkBehaviour {

    public AttackTypes attackType = AttackTypes.Impact;
    public ConstantsDictionary.CHIP_TYPE chipType;

    [HideInInspector]
    public float attackDamage;
    [HideInInspector]
    public float abilityDamage;
    public float ketchupDamage = 10f;
    public int mayonnaiseDuration=4;
    public float chipDuration = 1.5f;

    private void OnEnable() {
        Invoke("Destroy", chipDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(Tags.player)) {
            PlayerAttacks playerAttacks = collision.gameObject.GetComponent<PlayerAttacks>();
            if (playerAttacks != null) {
                if (chipType == ConstantsDictionary.CHIP_TYPE.Normal) {
                    float damageDealt = DamageFormulas.CalculateBasicAttackDamage(attackDamage, playerAttacks.m_defense, ConstantsDictionary.randomK, 1f);
                    playerAttacks.gameObject.GetComponent<PlayerHealth>().CmdTakeDamage(damageDealt);
                }
                else if (chipType == ConstantsDictionary.CHIP_TYPE.Ketchup)
                    playerAttacks.gameObject.GetComponent<PlayerHealth>().CmdTakeDamage(abilityDamage + ketchupDamage);
                else if (chipType == ConstantsDictionary.CHIP_TYPE.Mayonnaise) {
                    playerAttacks.gameObject.GetComponent<PlayerHealth>().CmdTakeDamage(abilityDamage);
                    playerAttacks.gameObject.GetComponent<PlayerController>().SpeedChange(-0.5f,mayonnaiseDuration);
                }
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
