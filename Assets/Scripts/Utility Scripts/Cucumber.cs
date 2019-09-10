using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Cucumber : NetworkBehaviour {

    [HideInInspector]
    public float attackDamage;
    [HideInInspector]
    public bool miniBoss = false;
    public AttackTypes attackType = AttackTypes.Impact;
    public float cucumberDuration = 3f;
    public int slowDuration = 3;
    

    private void OnEnable() {
        
        transform.rotation = Quaternion.identity;
        Invoke("Destroy",cucumberDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(Tags.player)) {
            PlayerAttacks playerAttacks = collision.gameObject.GetComponent<PlayerAttacks>();
            if (playerAttacks != null) {
                float damageDealt = DamageFormulas.CalculateBasicAttackDamage(attackDamage, playerAttacks.m_defense, ConstantsDictionary.randomK,1f);
                playerAttacks.gameObject.GetComponent<PlayerHealth>().CmdTakeDamage(damageDealt);
                if (miniBoss)
                    playerAttacks.gameObject.GetComponent<PlayerController>().SpeedChange(-0.5f,slowDuration);
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