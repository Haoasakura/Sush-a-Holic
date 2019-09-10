using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class EnemyAttacks : NetworkBehaviour {

    public bool hasAbility;
    public float basicAttackDamage = 3f;
    protected EnemyController enemyController;

    private void Update() {
        if (!isServer)
            return;
        if (enemyController.target != null && enemyController.target.GetComponent<PlayerHealth>().currentHealth <= 0) {
            //RpcSetAnimBool("TargetDead", true);
            enemyController.target = null;
            enemyController.targetFound = false;
            CmdSetAnimatorBool("TargetDead", true);
        }
        else if(enemyController.target != null && enemyController.target.GetComponent<PlayerHealth>().currentHealth > 0) {
            enemyController.targetFound = true;
            CmdSetAnimatorBool("TargetDead", false);
        }
    }

    [Command]
    public abstract void CmdUseBasicAttack();

    [Command]
    public abstract void CmdUseAbility();

    [Command]
    public void CmdSetDirection() {
        Vector3 direction = (GetComponent<EnemyController>().target.position - transform.position).normalized;

        RpcSetAnimFloat("X", direction.x);
        RpcSetAnimFloat("Y", direction.y);

    }

    [Command]
    public void CmdSetAnimatorBool(string id, bool value) {
        GetComponent<Animator>().SetBool(id,value);
        RpcSetAnimBool(id, value);

    }

    [ClientRpc]
    public void RpcSetAnimBool(string id, bool value) {
        GetComponent<Animator>().SetBool(id, value);
    }

    [ClientRpc]
    public void RpcSetAnimFloat(string id, float value) {
        GetComponent<Animator>().SetFloat(id, value);
    }
}
