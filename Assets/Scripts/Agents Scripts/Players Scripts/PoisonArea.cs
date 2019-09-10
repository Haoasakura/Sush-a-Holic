using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonArea : MonoBehaviour {

    public float damagePercentage;
    public float damageDuration;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag(Tags.enemy))
        {
            collision.gameObject.GetComponent<EnemyHealth>().CmdTakeDotDamage(damagePercentage, damageDuration, ConstantsDictionary.PLAYERS.octo, ConstantsDictionary.OctoChefBasicAttackThreat);
        }
    }
}
