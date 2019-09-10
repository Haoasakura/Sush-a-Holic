using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ArcherChipAttacks : EnemyAttacks {

    public GameObject chip;
    public Sprite[] chipSprites;

    public float chipVelocity = 5f;
    public float abilityDamage=15f;
    public int abilityCooldown = 10;

    private float fireRate = 0.4f;
    private float nextFire = 0.0f;
    private bool isAbility;

    void Start() {
        hasAbility = true;
        enemyController = GetComponent<EnemyController>();
    }

    [Command]
    public override void CmdUseBasicAttack() {
        //The attack is called by the animator on the CmdShoot function
    }

    [Command]
    public override void CmdUseAbility() {
        isAbility = true;
        hasAbility = false;

        //the stop is only a precaution
        StopCoroutine("AbilityCooldown");
        StartCoroutine("AbilityCooldown");
    }

    [Command]
    public void CmdShoot() {
        if (!isServer)
            return;

        if (Time.time > nextFire) {
            nextFire = Time.time + fireRate;

            GameObject chip = Instantiate(this.chip);
            Chip chipScript = chip.GetComponent<Chip>();

            
            if (isAbility) {
                
                chipScript.chipType = (ConstantsDictionary.CHIP_TYPE)Random.Range(1, 3);
                chipScript.abilityDamage = abilityDamage;
                isAbility = false;
            }
            else {
                chipScript.chipType = ConstantsDictionary.CHIP_TYPE.Normal;
                chipScript.attackDamage = basicAttackDamage;
            }

            chip.SetActive(false);
            chipScript.GetComponent<SpriteRenderer>().sprite = chipSprites[(int)chipScript.chipType];
            chip.SetActive(true);

            chip.transform.position = transform.position + ((enemyController.target.position - transform.position).normalized * 0.5f);

            Vector2 direction = enemyController.target.position - transform.position;
            chip.GetComponent<Rigidbody2D>().velocity = chipVelocity * direction.normalized;
            
            NetworkServer.Spawn(chip);
            CmdChipRotation(direction, chip);

        }
    }

    private IEnumerator AbilityCooldown() {
        int timePassed = 0;
        while (timePassed < abilityCooldown) {
            yield return new WaitForSeconds(1f);
            timePassed++;
        }
        hasAbility = true;
    }

    [Command]
    void CmdChipRotation(Vector3 direction, GameObject chip) {
        RpcChipRotation(direction, chip);
    }
    [ClientRpc]
    void RpcChipRotation(Vector3 direction, GameObject chip) {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
            if (direction.x > 0)
                chip.transform.localScale = new Vector3(-1f, 1f, 1f);
            else
                chip.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else {
            chip.transform.Rotate(new Vector3(0f, 0f, 90f));
            if (direction.y > 0) {
                chip.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else {
                chip.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
}
