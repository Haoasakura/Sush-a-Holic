using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BasicAttackHitBox : NetworkBehaviour {

    public int number_of_hits = 1;
    private int current_number_of_hits;
    private PlayerAttacks parent;
    private PlayerController parentController;
    private PlayerHealth parentHealth;

    private void Start()
    {
        parent = GetComponentInParent<PlayerAttacks>();
        parentController = GetComponentInParent<PlayerController>();
        parentHealth = GetComponentInParent<PlayerHealth>();
    }

    private void OnEnable()
    {
        current_number_of_hits = number_of_hits;
        Invoke("Deactivate", 0.6f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(current_number_of_hits > 0)
        {
            if (collision.gameObject.CompareTag(Tags.enemy))
            {
                bool inComboField = false;
                current_number_of_hits--;
                float basicAttackDamage = parent.m_basic_attack_damage;
                if (parent.isInOctoComboField)
                {
                    basicAttackDamage += basicAttackDamage * ConstantsDictionary.octoComboIncreasedDamagePercentage;
                    inComboField = true;
                }
                EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    float damageDealt = DamageFormulas.CalculateBasicAttackDamage(basicAttackDamage, enemyHealth.defense, ConstantsDictionary.randomK, AttackTypeMultiplierFactory.SelectAttackTypeMultiplier(parent.m_attack_type, enemyHealth.type));
                    enemyHealth.CmdTakeDamage(damageDealt, parent.playerType, parent.m_basic_attack_threat);
                    if (parent.isInNekoComboField && parentHealth.currentHealth > 0)
                    {
                        float healAmount = damageDealt * ConstantsDictionary.nekoComboRecoveredHpPercentage;
                        CmdHeal(parent.gameObject, healAmount);
                        inComboField = true;
                    }
                }
                if (inComboField)
                {
                    parentController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForCombo);
                }
                else
                {
                    parentController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForBasicAttack);

                }

            }
        }
    }

    [Command]
    public void CmdHeal(GameObject player, float amount)
    {
        player.GetComponent<PlayerHealth>().Heal(amount);
    }

    public void OnDisable()
    {
        CancelInvoke();
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
