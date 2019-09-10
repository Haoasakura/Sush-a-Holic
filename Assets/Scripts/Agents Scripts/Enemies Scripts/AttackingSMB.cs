using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingSMB : StateMachineBehaviour {

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
        animator.gameObject.GetComponent<EnemyController>().StopCoroutine("AttackTarget");
        animator.gameObject.GetComponent<EnemyController>().StartCoroutine("AttackTarget");
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

        if (animator.gameObject.GetComponent<PopcornAttacks>() == null) {
            if (animator.gameObject.GetComponent<EnemyController>().target!=null && (animator.gameObject.GetComponent<EnemyController>().target.position - animator.transform.position).magnitude - 0.75f > animator.gameObject.GetComponent<EnemyController>().attackRange || !animator.gameObject.GetComponent<EnemyController>().InLineOfSight()) {
                animator.SetBool("TargetInRange", false);
                
            }
            //if(animator.gameObject.GetComponent<EnemyController>().target.GetComponent<PlayerController>().downed)
        }
        animator.gameObject.GetComponent<EnemyAttacks>().CmdSetDirection();

    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.gameObject.GetComponent<EnemyController>().targetFound = false;
        animator.gameObject.GetComponent<EnemyAttacks>().CmdSetAnimatorBool("TargetInRange", false);
        animator.SetBool("TargetInRange", false);
        animator.gameObject.GetComponent<EnemyAttacks>().CmdSetAnimatorBool("TargetDead", false);
        animator.SetBool("TargetDead", false);
        animator.gameObject.GetComponent<EnemyController>().StopCoroutine("AttackTarget");
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
