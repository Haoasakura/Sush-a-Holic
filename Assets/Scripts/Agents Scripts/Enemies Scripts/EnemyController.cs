using System.Collections;
using UnityEngine;
using System.Threading;
using UnityEngine.Networking;

public class EnemyController : NetworkBehaviour
{

    const float minPathUpdateTime = 0.3f;
    const float pathUpdateMoveThreshold = 0.5f;

    public LayerMask playerLayerMask;

    [SyncVar]
    public Transform target;
    public GameObject aimBox;
    public Animator anim;
    
    public float speed = 10f;
    public float slowDownDistance = 5f;
    public float attackRange = 1f;
    public float attackSpeed = 0.5f;

    public float threatRange = 5f;

    public int slowDownIndex = 0;
    public bool isFearead;
    public bool canMove;
    [SyncVar]
    public bool targetFound;

    public DecisionTree chasingDT;
    public BehaviourTree attackingBT;

    

    private EnemyAttacks enemyAttacks;
    private EnemyHealth enemyHealth;

    private Vector3[] pathToTarget;

    private int targetIndex;
    private bool following;
    private bool wasInRangeWhenFeared = false;

    private void Awake() {

        enemyHealth = GetComponent<EnemyHealth>();
        enemyAttacks = GetComponent<EnemyAttacks>();
        anim = GetComponent<Animator>();

        isFearead = false;
        canMove = true;
        targetFound = false;
        following = false;

        DecisionTreeDecision playersInThreatRangeDecision = new DecisionTreeDecision(PlayersInThreatRange);

        DecisionTreeAction targetMoreThreatening = new DecisionTreeAction(TargetMoreThreatening);
        DecisionTreeAction targetMoreNear = new DecisionTreeAction(TargetMoreNear);

        playersInThreatRangeDecision.AddLink(true, targetMoreThreatening);
        playersInThreatRangeDecision.AddLink(false, targetMoreNear);
        chasingDT = new DecisionTree(playersInThreatRangeDecision);

        BehaviourTreeCondition abilityAvaible = new BehaviourTreeCondition(AbilityAvaible);

        BehaviourTreeAction useAbility = new BehaviourTreeAction(UseAbility);
        BehaviourTreeAction useBasicAttack = new BehaviourTreeAction(UseBasicAttack);

        BehaviourTreeSequence abilitySequence = new BehaviourTreeSequence(new BehaviourTreeTask[] {abilityAvaible,useAbility});

        BehaviourTreeDecoratorUntilFail abilityDecorator = new BehaviourTreeDecoratorUntilFail(abilitySequence);
        BehaviourTreeDecoratorUntilFail basicAttackDecorator = new BehaviourTreeDecoratorUntilFail(useBasicAttack);

        BehaviourTreeSequence attackSequence = new BehaviourTreeSequence(new BehaviourTreeTask[] {abilityDecorator,basicAttackDecorator});
        BehaviourTreeDecoratorUntilFail attackSequenceDecorator = new BehaviourTreeDecoratorUntilFail(attackSequence);

        attackingBT = new BehaviourTree(attackSequenceDecorator);


    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) {
        if (pathSuccessful) {
            pathToTarget = waypoints;
            targetIndex = 0;

            if(following)
                StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdatePath() {

        if (Time.timeSinceLevelLoad < .3f)
            yield return new WaitForSeconds(.3f);

        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3[] targetsOldPosition = new Vector3[3];
        while (targetFound) {
            yield return new WaitForSeconds(minPathUpdateTime);

            foreach (GameObject player in GameObject.FindGameObjectsWithTag(Tags.player)) {
                if (target!=null && (player.transform.position - targetsOldPosition[(int)player.GetComponent<PlayerAttacks>().playerType]).sqrMagnitude > sqrMoveThreshold) {
                    PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                    targetsOldPosition[(int)player.GetComponent<PlayerAttacks>().playerType] = player.transform.position;
                }
            }
        }
    }

    IEnumerator FollowPath() {
        following = true;
        float speedPercent = 1;
        Vector3 currentWaypoint = pathToTarget[0];

        while (true) {
            if (transform.position == currentWaypoint) {
                targetIndex++;

                if (targetIndex >= pathToTarget.Length-attackRange && InLineOfSight()) {
                    targetIndex = 0;
                    pathToTarget = new Vector3[0];
                    anim.SetBool("TargetInRange", true);
                    RpcSetAnimBool("TargetInRange", true);
                    following = false;
                    yield break;
                }

                if ((PathRequestManager.DistanceFromTarget(pathToTarget, transform.position, targetIndex) < attackRange) && InLineOfSight()) {
                    targetIndex = 0;
                    pathToTarget = new Vector3[0];
                    anim.SetBool("TargetInRange", true);
                    RpcSetAnimBool("TargetInRange", true);
                    following = false;
                    yield break;
                }

                if(targetIndex<pathToTarget.Length)
                    currentWaypoint = pathToTarget[targetIndex];

                if (targetIndex >= slowDownIndex && slowDownDistance > 0) {
                    speedPercent = Mathf.Clamp01(PathRequestManager.DistanceFromTarget(pathToTarget, transform.position, targetIndex) / slowDownDistance);
                    if (speedPercent < 0.01f) {
                        anim.SetBool("TargetInRange", true);
                        RpcSetAnimBool("TargetInRange", true);
                        following = false;
                        yield break;
                    }
                }
            }

            if (canMove) {
                RpcSetDirection();

                if(wasInRangeWhenFeared && target!=null)
                    transform.position = Vector3.MoveTowards(transform.position, transform.position+(target.position-transform.position), -0.25f * speed * speedPercent * Time.deltaTime);
                else
                    transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, (isFearead ? (-0.25f * speed) : speed) * speedPercent * Time.deltaTime);
            }
            yield return null;
        }
    }

    

    [Command]
    public void CmdStartSearch() {
        anim.SetBool("TargetDead", false);
        RpcSetAnimBool("TargetDead", false);
        StopCoroutine("SearchTarget");
        StartCoroutine("SearchTarget");
    }

    public IEnumerator SearchTarget() {
        while (!targetFound) {
            chasingDT.Walk();
            yield return null;
        }
        if (targetFound) {
            PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
            StartCoroutine(UpdatePath());
        }

    }

    public IEnumerator AttackTarget() {
        while (anim.GetBool("TargetInRange")) {
            attackingBT.Step();
            //attack speed/2
            yield return new WaitForSeconds(attackSpeed*0.5f);
        }
    }

    [Command]
    public void CmdApplyFear(float duration) {
        StartCoroutine("ApplyFear", duration);
    }

    //enemy does't move away when overlapping with player
    public IEnumerator ApplyFear(float duration) {
        isFearead = true;
        //anim.SetBool("Feared", true);
        CmdSetAnimatorBool("Feared", true);

        if (anim.GetBool("TargetInRange") == true) {
            wasInRangeWhenFeared = true;

        }
        yield return new WaitForSeconds(duration);
        wasInRangeWhenFeared = false;
        isFearead = false;
        //anim.SetBool("Feared", false);
        //anim.SetBool("TargetInRange", false);
        CmdSetAnimatorBool("Feared", false);
        CmdSetAnimatorBool("TargetInRange", false);
        canMove = true;
        CmdStartSearch();
    }

    [Command]
    public void CmdSetAnimatorBool(string id, bool value) {
        if(!isServer)
            anim.SetBool(id, value);
        RpcSetAnimBool(id, value);
        

    }

    public IEnumerator ApplyStun(float duration) {
        canMove = false;
        if (anim.GetBool("TargetInRange") == true)
            StopCoroutine("AttackTarget");
        yield return new WaitForSeconds(duration);
        if (anim.GetBool("TargetInRange") == true)
            StartCoroutine("AttackTarget");
        else
            CmdStartSearch();
        canMove = true;
    }

    public IEnumerator BoostSpeed(float percentage, int duration) {
        int timePassed = 0;
        float amount = speed * percentage / 100f;

        speed += amount;

        while (timePassed < duration) {
            yield return new WaitForSeconds(1f);
            timePassed++;
        }
        speed -= amount;
    }

    /*public void OnTriggerStay2D(Collider2D collision) {
        if (isFearead) {
            if (collision.gameObject.CompareTag(Tags.building)) {
                canMove = false;
            }
        }
    }*/

    public bool InLineOfSight() {
        if (target != null) {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (target.position - transform.position), attackRange + 0.1f);
            if (hit.collider != null)
                if (hit.collider.gameObject.name == target.gameObject.name)
                    return true;
            Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, 0.3f, playerLayerMask);
            foreach (Collider2D collision in collisions)
                if (collision.gameObject.name == target.gameObject.name)
                    return true;
        }
        return false;
    }

    //DT & BT Decisions
    public object PlayersInThreatRange() {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag(Tags.player)) {
            if ((player.transform.position - transform.position).magnitude < threatRange && !player.GetComponent<PlayerController>().downed)
                return true;
        }

        return false;
    }

    public bool AbilityAvaible() {
        return enemyAttacks.hasAbility;
    }

    //DT & BT Actions
    public object TargetMoreThreatening() {
        Transform currentMaxThreat = null;
        if (target != null)
            currentMaxThreat = target;

        foreach (Collider2D player in Physics2D.OverlapCircleAll(transform.position,threatRange,playerLayerMask) /*GameObject.FindGameObjectsWithTag(Tags.player)*/) {
            if (/*(player.transform.position - transform.position).magnitude < threatRange &&*/ !player.GetComponent<PlayerController>().downed) {
                if (currentMaxThreat == null)
                        currentMaxThreat = player.transform;
                else if (enemyHealth.threatLevels[(int)player.GetComponent<PlayerController>().characterAttacks.playerType] > enemyHealth.threatLevels[(int)currentMaxThreat.GetComponent<PlayerController>().characterAttacks.playerType])
                    currentMaxThreat = player.transform;
            }
        }

        if(currentMaxThreat !=null) {
            target = currentMaxThreat;
            targetFound = true;
        }
        /*anim.SetBool("TargetDead", false);
        RpcSetAnimBool("TargetDead", false);*/
        return null;
    }

    public object TargetMoreNear() {
        float nearestTargetDistance = Mathf.Infinity;
        GameObject currentMinDist = null;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag(Tags.player)) {
            if (!player.GetComponent<PlayerController>().downed) {
                PathRequestManager.RequestPath(new PathRequest(transform.position, player.transform.position, delegate (Vector3[] waypoints, bool pathSuccessful) {
                    if (pathSuccessful) {
                        float tmpDistance = PathRequestManager.DistanceFromTarget(waypoints, transform.position, targetIndex = 0);
                        if (tmpDistance < nearestTargetDistance) {
                            nearestTargetDistance = tmpDistance;
                            currentMinDist = player;
                            target = currentMinDist.transform;
                            targetFound = true;
                        }
                    }
                }));
            }
        }
        
        if (target != null)
            targetFound = true;
        /*anim.SetBool("TargetDead", false);
        RpcSetAnimBool("TargetDead", false);*/
        return null;
    }

    [ClientRpc]
    public void RpcSetDirection() {
        if (target != null) {
            Vector3 direction = (target.position - transform.position).normalized;

            if (!isFearead) {
                GetComponent<Animator>().SetFloat("X", direction.x);
                GetComponent<Animator>().SetFloat("Y", direction.y);
            }
            else {
                GetComponent<Animator>().SetFloat("X", -direction.x);
                GetComponent<Animator>().SetFloat("Y", -direction.y);
            }
        }
    }

    [ClientRpc]
    public void RpcSetAnimBool(string id, bool value) {
        anim.SetBool(id, value);
    }

    [ClientRpc]
    public void RpcSetAnimFloat(string id, float value) {
        anim.SetFloat(id, value);
    }

    public bool UseAbility() {
        enemyAttacks.CmdUseAbility();
        return enemyAttacks.hasAbility;
    }

    public bool UseBasicAttack() {
        enemyAttacks.CmdUseBasicAttack();
        return false;
    }

    public void DeactivateAimBox() {
        aimBox.SetActive(false);
    }

    public void ActivateAimBox() {
        aimBox.SetActive(true);
    }

    /*public void OnDrawGizmos() {
        if (pathToTarget != null) {
            for (int i = targetIndex; i < pathToTarget.Length; i++) {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(pathToTarget[i], Vector3.one * 0.5f);

                if (i == targetIndex)
                    Gizmos.DrawLine(transform.position, pathToTarget[i]);
                else
                    Gizmos.DrawLine(pathToTarget[i - 1], pathToTarget[i]);
            }
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Gizmos.DrawWireSphere(transform.position, attackRange);

            //pathToTarget.DrawWithGizmos();
        }
    }*/
}