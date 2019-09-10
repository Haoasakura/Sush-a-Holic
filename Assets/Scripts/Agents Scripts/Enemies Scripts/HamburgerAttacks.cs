using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HamburgerAttacks : EnemyAttacks {

    public GameObject cucumber;
    public float cucumberVelocity = 5f;
    private float fireRate = 0.1f;
    private float nextFire = 0.0f;
    public bool miniBoss = false;

    void Start () {
        hasAbility = false;
        enemyController = GetComponent<EnemyController>();
    }


	[Command]
    public override void CmdUseBasicAttack() {
        //The attack is called by the animator on the CmdShoot function
    }

    [Command]
    public override void CmdUseAbility() { }

    [Command]
    public void CmdShoot() {
        if (!isServer)
            return;

        if (Time.time > nextFire) {
            nextFire = Time.time + fireRate;

            GameObject cucumber = Instantiate(this.cucumber);
            cucumber.transform.position = transform.position + ((enemyController.target.position - transform.position).normalized * 0.5f);
            cucumber.GetComponent<Cucumber>().attackDamage = basicAttackDamage;
            cucumber.GetComponent<Cucumber>().miniBoss = miniBoss;

            Vector2 direction = enemyController.target.position - transform.position;

            cucumber.GetComponent<Rigidbody2D>().velocity = cucumberVelocity * direction.normalized;
            NetworkServer.Spawn(cucumber);
        }

    }
}
