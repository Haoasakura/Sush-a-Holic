using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum PLAYERS { neko, octo, fisherman };

public class PlayerHealth : NetworkBehaviour {

    [HideInInspector]
    public float maxHealth = 100f;
    [SyncVar(hook = "OnChangeHealth")]
    public float currentHealth;

    [SyncVar]
    public float shield;
    [SyncVar]
    public float aura;
    [SyncVar]
    public bool hasThickSkin = false;
    [SyncVar]
    public bool hasSeaLegs = false;
    [SyncVar]
    public float evadedAttacksPercentage = 0;
    [SyncVar]
    public bool immortal = false;

    private PlayerController controller;

    public GameObject healthBar;

    void Start()
    {
        maxHealth = GetComponent<PlayerAttacks>().m_hp;
        controller = GetComponent<PlayerController>();
    }

    private void OnChangeHealth(float currentHealth)
    {
        float percentage = currentHealth / maxHealth;
        healthBar.transform.localScale = new Vector2(percentage, healthBar.transform.localScale.y);
        StartCoroutine("ImmortalityTime", 0.3f);
        
    }

    [Command]
    public void CmdTakeDamage(float amount)
    {
        if (!isServer || immortal)
        {
            return;
        }
        if (hasSeaLegs)
        {
            if(Random.Range(0,1) <= evadedAttacksPercentage)
            {
                //TODO eventualmente aggiungere effetto gui
                return;
            }
        }
        if (hasThickSkin)
        {
            amount -= amount * ConstantsDictionary.ThickSkinDamageReductionPercentage;
        }
        if (shield > 0)
        {
            if (shield >= amount)
            {
                shield -= amount;
                return;
            }else
            {
                //TODO sincronizzare con la sparizione del particellare
                amount = amount - shield;
            }
        }
        if(aura > 0)
        {
            if(aura >= amount)
            {
                aura -= amount;
                return;
            }else
            {
                amount -= aura;
            }
        }
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            controller.RpcDowned();
        }

    }

    public IEnumerator TakeDotDamage(float percentage, float duration, float tick) {
        float timePassed = 0.0f;
        float amount = maxHealth * percentage/100f;
        while (timePassed < duration) {
            CmdTakeDamage(amount);
            yield return new WaitForSeconds(tick);
            timePassed += tick;
        }
    }

    public IEnumerator ApplyWeakness(float percentage, int duration) {
        PlayerAttacks playerAttacks = GetComponent<PlayerAttacks>();
        int timePassed = 0;
        float amount = playerAttacks.m_basic_attack_damage * percentage / 100f;

        playerAttacks.m_basic_attack_damage -= amount;

        while (timePassed < duration) {
            yield return new WaitForSeconds(1f);
            timePassed++;
        }
        playerAttacks.m_basic_attack_damage += amount;
    }

    public IEnumerator ImmortalityTime(float duration) {
        immortal = true;
        float timePassed = 0.0f;
        while (timePassed < duration) {
            yield return new WaitForSeconds(0.1f);
            timePassed += 0.1f;
        }
        immortal = false;
    }


    public void Heal(float amount, bool usePercentage = false, float percentage = 0f)
    {
        if (!isServer)
        {
            return;
        }
        if (usePercentage)
        {
            amount = maxHealth * percentage;
        }
        currentHealth += amount;
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
