using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OctoChefKnife : NetworkBehaviour{

    public int enemiesToHit;
    private int enemiesStillToHit;

    public float damage;
    public AttackTypes attackType = AttackTypes.Cut;

    private GameObject firstHitEnemy;

    public GameObject octoChef;
    private PlayerController playerController;
    private PlayerHealth octoHealth;

    public bool inNekoMaidComboField = false;
    public bool inFishermanComboField = false;

    private void Start()
    {
        octoChef = FindObjectOfType<OctoChefAttacks>().gameObject;
        octoHealth = octoChef.GetComponent<PlayerHealth>();
        playerController = octoChef.GetComponent<PlayerController>();
        enemiesStillToHit = enemiesToHit;
        firstHitEnemy = null;
        Invoke("Destroy", ConstantsDictionary.OctoChefKnifeDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.enemy))
        {
            if (enemiesStillToHit > 0 && !collision.gameObject.Equals(firstHitEnemy))
            {
                bool inComboField = false;
                enemiesStillToHit--;
                firstHitEnemy = collision.gameObject;
                EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    float damageDealt = DamageFormulas.CalculateBasicAttackDamage(damage, enemyHealth.defense, ConstantsDictionary.randomK, AttackTypeMultiplierFactory.SelectAttackTypeMultiplier(attackType, enemyHealth.type));
                    float threat;
                    if (inFishermanComboField)
                    {
                        threat = ConstantsDictionary.reducedComboFieldThreat;
                        inComboField = true;
                    }
                    else
                    {
                        threat = ConstantsDictionary.OctoChefBasicAttackThreat;
                    }
                    enemyHealth.CmdTakeDamage(damageDealt,ConstantsDictionary.PLAYERS.octo, threat);

                    if (inNekoMaidComboField && octoHealth.currentHealth >0)
                    {
                        float healAmount = damageDealt * ConstantsDictionary.nekoComboRecoveredHpPercentage;
                        octoHealth.Heal(healAmount);
                        inComboField = true;
                    }
                }

                if (inComboField)
                {
                    playerController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForCombo);
                }else
                {
                    playerController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForBasicAttack);

                }

            }

            if (enemiesStillToHit == 0)
            {
                NetworkServer.Destroy(gameObject);
            }
        }
        if (collision.gameObject.CompareTag(Tags.building))
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.building))
        {
            gameObject.SetActive(false);
        }
    }

    //chiamata quando sul server si chiama networkserver destroy
    public override void OnNetworkDestroy()
    {
        //TODO controllare come distruggerlo per bene senza intasare la rete
        gameObject.SetActive(false);
    }

    private void Destroy()
    {
        NetworkServer.Destroy(gameObject);
    } 

    private void OnDisable()
    {
        CancelInvoke();
    }
}