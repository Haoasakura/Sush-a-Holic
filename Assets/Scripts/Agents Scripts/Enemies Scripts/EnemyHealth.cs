using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EnemyHealth : NetworkBehaviour {

    public enum EnemyDropType {Base, Medium, Strong, MiniBoss}

    public GameObject[] sushiCoin;

    public float defense;
    public EnemyTypesEnum type;
    public EnemyDropType enemyDropType;
    public float maxHealth = 100;
    public IEnumerator coroutine;
    
    [SyncVar]
    public Vector3 threatLevels;

    private EnemyController enemyController;

    [SyncVar(hook = "OnChangeHealth")]
    public float currentHealth;

    public Slider healthBarSlider;
    public Image fillImage;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;
    public int healthBarShowSeconds = 1;

    public float dropProbability=1f;


    public void OnEnable()
    {
        healthBarSlider.maxValue = maxHealth;
        currentHealth = maxHealth;

        int startTarget =  Random.Range(0, 3);
        if(startTarget==0)
            threatLevels = new Vector3(1.0f, 0.0f, 0.0f);
        else if(startTarget==1)
            threatLevels = new Vector3(0.0f, 1.0f, 0.0f);
        else
            threatLevels = new Vector3(0.0f, 0.0f, 1.0f);

        enemyController = GetComponent<EnemyController>();
        if (enemyDropType == EnemyDropType.MiniBoss)
            dropProbability = 1f;
        
    }

    [Command]
    public void CmdTakeDamage(float amount,ConstantsDictionary.PLAYERS player,float threat)
    {
        if (!isServer)
            return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            RpcSetDead();
            
        }

        CmdSetAggro(player, threat);
        //GetComponent<Animator>().SetBool("NewThreat",true);
        enemyController.RpcSetAnimBool("NewThreat",true);


    }

    [ClientRpc]
    public void RpcSetDead() {
        GetComponent<Animator>().SetBool("Dead", true);
    }

    [Command]
    public void CmdTakeDotDamage(float percentage, float duration, ConstantsDictionary.PLAYERS player, float threat) {
        if (!isServer)
            return;

        coroutine = TakeDotDamage(percentage, duration, player, threat);
        StopCoroutine(coroutine);
        StartCoroutine(coroutine);
    }

    //percentage is the percentage of HP to remove per tick, duration is the total duration of the damage 
    //and tick is the delay between the application of the damage in seconds
    public IEnumerator TakeDotDamage(float percentage, float duration,ConstantsDictionary.PLAYERS player,float threat)
    {
        float timePassed = 0.0f;
        float amount = maxHealth * percentage/100f;
        threat /= 3;
        while(timePassed<duration)
        {
            CmdTakeDamage(amount,player,threat);
            yield return new WaitForSeconds(1f);
            timePassed += 1f;
        }
    }

    [Command]
    public void CmdSetAggro(ConstantsDictionary.PLAYERS player, float threat) {
        if (!isServer)
            return;

        threatLevels[(int)player] += threat;
        RpcChangeThreat(threatLevels);
    }

    private void OnChangeHealth (float currentHealth){
        
        healthBarSlider.value = currentHealth;

        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, currentHealth / maxHealth);
        healthBarSlider.gameObject.SetActive(true);
        StartCoroutine("ShowHealthBar");
    }


    [ClientRpc]
    private void RpcChangeThreat (Vector3 threatLevels) {
        this.threatLevels = threatLevels;
    }

    private IEnumerator ShowHealthBar() {
        yield return new WaitForSeconds(healthBarShowSeconds);
        healthBarSlider.gameObject.SetActive(false);
    }

    [Command]
    public void CmdDie() {
        //StartCoroutine("Die");
        if (sushiCoin.Length > 0) {
            if (sushiCoin.Length == 1) {
                if (Random.value <= dropProbability) {
                    GameObject sushiCoin = Instantiate(this.sushiCoin[0]);
                    sushiCoin.transform.position = transform.position;
                    if(isServer)
                        NetworkServer.Spawn(sushiCoin);
                }
            }
            else {
                if (Random.value <= dropProbability) {
                    GameObject sushiCoin = Instantiate(this.sushiCoin[Random.Range(0,this.sushiCoin.Length)]);
                    sushiCoin.transform.position = transform.position;
                    if (isServer)
                        NetworkServer.Spawn(sushiCoin);
                }
            }
        }
        NetworkServer.Destroy(gameObject);
    }


    /*public IEnumerator Die(Animator animator, int layerIndex) {
        if (sushiCoin.Length > 0) {
            if (sushiCoin.Length == 1) {
                if (Random.value <= dropProbability) {
                    GameObject sushiCoin = Instantiate(this.sushiCoin[0]);
                    sushiCoin.transform.position = transform.position;
                    NetworkServer.Spawn(sushiCoin);
                }
            }
        }
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(layerIndex).length);
        NetworkServer.Destroy(animator.gameObject);
    }*/
}
