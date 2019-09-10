using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BasicAttackHitBoxNeko : NetworkBehaviour
{

    public int number_of_hits = 1;
    private int current_number_of_hits;
    private NekoMaidAttacks parent;
    private PlayerController parentController;
    private PlayerHealth nekoHealth;
    private List<GameObject> otherPlayers;

    private void Start()
    {
        parent = GetComponentInParent<NekoMaidAttacks>();
        parentController = GetComponentInParent<PlayerController>();
        nekoHealth = GetComponent<PlayerHealth>();

        //salvo la lista degli altri player per curarli con sharing is caring
        otherPlayers = new List<GameObject>();
        GameObject[] players = GameObject.FindGameObjectsWithTag(Tags.player);
        foreach(GameObject player in players)
        {
            if(player != gameObject)
            {
                otherPlayers.Add(player);
            }
        }
    }

    private void OnEnable()
    {
        current_number_of_hits = number_of_hits;
        Invoke("Deactivate", 0.4f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (current_number_of_hits > 0)
        {
            if (collision.gameObject.CompareTag(Tags.enemy))
            {
                bool inComboField = false;
                current_number_of_hits--;
                EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    float basicAttackDamage = parent.m_basic_attack_damage;
                    if (parent.isInOctoComboField)
                    {
                        basicAttackDamage += basicAttackDamage * ConstantsDictionary.octoComboIncreasedDamagePercentage;
                        inComboField = true;
                    }
                    float damage = DamageFormulas.CalculateBasicAttackDamage(basicAttackDamage, enemyHealth.defense, ConstantsDictionary.randomK, AttackTypeMultiplierFactory.SelectAttackTypeMultiplier(parent.m_attack_type, enemyHealth.type));
                    float threat;
                    if (parent.isInFishermanComboField)
                    {
                        threat = ConstantsDictionary.reducedComboFieldThreat;
                        inComboField = true;
                    } else
                    {
                        threat = parent.m_basic_attack_threat;
                    }
                    enemyHealth.CmdTakeDamage(damage, ConstantsDictionary.PLAYERS.neko, threat);

                    if(parent == null)
                    {
                        parent = GetComponentInParent<NekoMaidAttacks>();
                    }
                    if(nekoHealth == null)
                    {
                        nekoHealth = GetComponent<PlayerHealth>();
                    }
                    //se ho attivato la skill passiva kawaii dello skill tree della neko
                    if (parent.kawaiiPercentageOfDamageInHp > 0 && nekoHealth.currentHealth > 0)
                    {
                        nekoHealth.Heal(damage * parent.kawaiiPercentageOfDamageInHp);
                    }
                    //se ho attivato sharing is caring tutti gli altri player ricevono
                    //una percentuale di danni della neko in hp
                    if (parent.sharingIsCaringActivated)
                    {
                        foreach (GameObject player in otherPlayers)
                        {
                            PlayerHealth health = player.GetComponent<PlayerHealth>();
                            if (health.currentHealth > 0)
                            {
                                CmdHeal(nekoHealth.gameObject, damage * parent.sharingIsCaringPercentageOfDamageInHp);
                                //health.Heal(damage * parent.sharingIsCaringPercentageOfDamageInHp);
                            }
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

