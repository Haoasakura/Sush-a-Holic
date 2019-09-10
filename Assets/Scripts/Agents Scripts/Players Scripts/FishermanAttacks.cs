using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FishermanAttacks : PlayerAttacks
{
    private Animator anim;
    //public PlayerHealth health;
	private PlayerController playerController;

    public GameObject leftAttackBox;
    public GameObject rightAttackBox;
    public GameObject upAttackBox;
    public GameObject downAttackBox;
    public GameObject gunsight;
    public GameObject hook;

	private List<PlayerController> allPlayers;
	private GameObject[] players;

    private GameObject enemySelected;
    private EnemyController lastEnemySelected;
    private bool lastFound = false;

    private bool finishedSkillOne = false;
    private bool finishedSkillTwo = false;
    private bool finishedUlti = false;

    //skill variables
    public float omega3ProtectionAbsorbedDamage;
    public float omega3ProtectionDuration;
    public float iGotYaHookRange;
    public float iGotYaDmg;
	public float iGotYaDmgArea;
    public bool iGotYaStun = false;
    public float iGotYaStunDuration;
    public float iGotYaThreat;
    public float sushiDivineProtectionDuration;
	public float sushiDivineKnockback;
	public float sushiDivineProtectionAura;
    private float sushiDivineProtectionKnockBackDuration;

    private void Awake()
    {
        playerType = ConstantsDictionary.PLAYERS.fisherman;
        m_speed = ConstantsDictionary.FishermanSpeed;
        m_defense = ConstantsDictionary.FishermanDefense;
        m_hp = ConstantsDictionary.FishermanHP;
        m_basic_attack_damage = ConstantsDictionary.FishermanBasicAttackDamage;
        m_basic_attack_threat = ConstantsDictionary.FishermanBasicAttackThreat;
        m_attack_type = ConstantsDictionary.FishermanBasicAttackType;
        skillOneCooldown = ConstantsDictionary.FishermanSkillOneCooldown;
        skillTwoCooldown = ConstantsDictionary.FishermanSkillTwoCooldown;
        omega3ProtectionAbsorbedDamage = ConstantsDictionary.Omega3ProtectionAbsorbedDamage;
        omega3ProtectionDuration = ConstantsDictionary.Omega3ProtectionDuration;
        iGotYaHookRange = ConstantsDictionary.IGotYaHookRange;
		iGotYaDmgArea = ConstantsDictionary.IGotYaDmgArea;
        sushiDivineProtectionDuration = ConstantsDictionary.SushiDivineProtectionDuration;
		sushiDivineKnockback = ConstantsDictionary.SushiDivineKnockbackArea;
		sushiDivineProtectionAura = ConstantsDictionary.SushiDivineProtectionAura;
		players = GameObject.FindGameObjectsWithTag(Tags.player);
        sushiDivineProtectionKnockBackDuration = ConstantsDictionary.SushiDivineKnockbackDuration;
        iGotYaDmg = ConstantsDictionary.IGotYaDamage;
        iGotYaStunDuration = ConstantsDictionary.IGotYaStunDuration;
        iGotYaThreat = ConstantsDictionary.IGotYaThreat;
        gunsight = GameObject.Instantiate(gunsight);
    }

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
		playerController = gameObject.GetComponent<PlayerController>();
        health = gameObject.GetComponent<PlayerHealth>();
        allPlayers = new List<PlayerController>();
        foreach (GameObject player in players)
		{
			allPlayers.Add(player.GetComponent<PlayerController>());
		}
        skillTree = SkillTreeSetup();
		stm = gameObject.AddComponent <SkillTreeManager>();
		stm.AssignToSkillTreeManager (skillTree, playerController);
		playerController.AssignSTM (stm);
    }

    //OMEGA-3-PROTECTION
    public override void SkillOne()
    {
        if (finishedSkillOne)
        {
            return;
        }
        anim.SetTrigger("SkillOne");
        health.shield = omega3ProtectionAbsorbedDamage;
        StartCoroutine("DestroyShield");
        playerController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForSkill);
        finishedSkillOne = true;
        StartCoroutine("ResetFinishedSkillOne");
    }

    private IEnumerator ResetFinishedSkillOne()
    {
        yield return new WaitForSeconds(5f);
        finishedSkillOne = false;
    }

    private IEnumerator DestroyShield()
    {
        yield return new WaitForSeconds(omega3ProtectionDuration);
        health.shield = 0;
    }

    //I GOT YA
    public override void SkillTwo()
    {
        if (finishedSkillTwo)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gunsight.SetActive(false);
            playerController.AbortSkillTwo();
            return;
        }
        if (Input.GetMouseButtonDown(0) && enemySelected != null)
        {
            //deactivate aim e gunsight
            gunsight.SetActive(false);
            EnemyController enContr = enemySelected.GetComponent<EnemyController>();
            enContr.DeactivateAimBox();

            //throw hook
            GameObject hookToThrow = GameObject.Instantiate(hook);
            hookToThrow.transform.position = gameObject.transform.position;
            Hook hookScript = hookToThrow.GetComponent<Hook>();
            hookScript.playerToReach = enemySelected.transform;
            NetworkServer.Spawn(hookToThrow);
            anim.SetTrigger("SkillTwo");

            //apply stun is skill is unlocked
            if (iGotYaStun)
            {
                enContr.ApplyStun(iGotYaStunDuration);          
            }

            //deal damage to the enemies in the area and gets aggro on all of them
            Collider2D[] colliders = Physics2D.OverlapCircleAll(enemySelected.transform.position, iGotYaDmgArea);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag(Tags.enemy))
                    CmdSkillTwo(collider.gameObject);
                    


            }
            finishedSkillTwo = true;
            StartCoroutine("ResetFinishedSkillTwo");
            playerController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForSkill);
            CmdComboField(enemySelected.transform.position);
            return;
        }
        //far vedere l'area intorno al fisherman in cui si possono selezionare i nemici
        gunsight.transform.position = transform.position;
        gunsight.transform.localScale = new Vector3(iGotYaDmgArea, iGotYaDmgArea, 1);
        gunsight.SetActive(true);

        //inizializzazione variabili di ricerca
        bool found = false;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool check = false;

        //prendo tutti i collider dove c'è il mouse
        Collider2D[] col = Physics2D.OverlapPointAll(mousePosition);
        EnemyController controllerSelected = null;
        if (col.Length > 0)
        {
            foreach (Collider2D c in col)
            {
                if (c.gameObject.CompareTag(Tags.enemy) &&
                    (Vector2.Distance(gameObject.transform.position, c.gameObject.transform.position) < iGotYaDmgArea))
                {
                    check = true;
                    enemySelected = c.gameObject;
                    controllerSelected = enemySelected.GetComponent<EnemyController>();
                    controllerSelected.ActivateAimBox();
                    break;
                }
            }
        }
        if (check)
        {
            found = true;
            if (controllerSelected != lastEnemySelected)
            {
                if(lastEnemySelected != null)
                {
                    lastEnemySelected.DeactivateAimBox();
                }

                lastEnemySelected = controllerSelected;
            }
        }
        if (lastFound == true && found == false)
        {
            if (lastEnemySelected != null)
            {
                lastEnemySelected.DeactivateAimBox();
            }
            enemySelected = null;
        }
        lastFound = found;
    }

    [Command]
    public void CmdSkillTwo(GameObject collider) {
        collider.GetComponent<EnemyHealth>().CmdTakeDamage(iGotYaDmg, ConstantsDictionary.PLAYERS.fisherman, iGotYaThreat);
    }

    private IEnumerator ResetFinishedSkillTwo()
    {
        yield return new WaitForSeconds(2f);
        finishedSkillTwo = false;
    }

    [Command]
    private void CmdComboField(Vector3 position)
    {
        GameObject combo = Instantiate(comboField);
        combo.transform.position = new Vector3(position.x, position.y, 0);
        NetworkServer.Spawn(combo);
    }

    //chiamate in update
    public override void UltimateSkill()
    {
        if (finishedUlti)
        {
            return;
        }
		anim.SetTrigger("Ultimate");
		playerController.ResetUltimateCharge();
		foreach (PlayerController player in allPlayers)
		{
            CmdApplyAura(player.gameObject, sushiDivineProtectionAura);
		}
        StartCoroutine("AuraDuration");
		Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, sushiDivineKnockback);
        foreach (Collider2D coll in hitEnemies)
        {
            if (coll.gameObject.CompareTag(Tags.enemy))
            {
                Debug.Log("true");
                coll.GetComponent<EnemyController>().CmdApplyFear(sushiDivineProtectionKnockBackDuration);
            }
        }
		/*foreach(Collider2D enemyColl in hitEnemies)
		{
            Vector3 direction = (enemyColl.gameObject.transform.position - gameObject.transform.position).normalized;
            Vector2 force = new Vector2(sushiDivineProtectionKnockbackForce * direction.x, sushiDivineProtectionKnockbackForce * direction.y);
            enemyColl.gameObject.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
		}*/
        finishedUlti = true;
        StartCoroutine("ResetFinishedUlti");
    }

    [Command]
    public void CmdApplyAura(GameObject player, float amount)
    {
        player.GetComponent<PlayerHealth>().aura = amount;
    }

    private IEnumerator ResetFinishedUlti()
    {
        yield return new WaitForSeconds(2f);
        finishedUlti = false;
    }

    private void FinishUlti()
    {
        playerController.StopUltimate();
        playerController.ResetUltimateCharge();
    }

    private IEnumerator AuraDuration()
	{
		yield return new WaitForSeconds (sushiDivineProtectionDuration);
		foreach (PlayerController player in allPlayers)
		{
            //player.gameObject.GetComponent<PlayerHealth> ().aura = 0f;
            CmdApplyAura(player.gameObject, 0);
		}
	}

    public override void BasicAttackLeft()
    {
        leftAttackBox.SetActive(true);
    }

    public override void BasicAttackRight()
    {
        rightAttackBox.SetActive(true);
    }

    public override void BasicAttackUp()
    {
        upAttackBox.SetActive(true);
    }

    public override void BasicAttackDown()
    {
        downAttackBox.SetActive(true);
    }

    public override void StopBasicAttackDown()
    {
        downAttackBox.SetActive(false);
        playerController.StopAttacking();
    }

    public override void StopBasicAttackLeft()
    {
        leftAttackBox.SetActive(false);
        playerController.StopAttacking();

    }

    public override void StopBasicAttackRight()
    {
        rightAttackBox.SetActive(false);
        playerController.StopAttacking();

    }

    public override void StopBasicAttackUp()
    {
        upAttackBox.SetActive(false);
        playerController.StopAttacking();

    }

    private List<Skill> SkillTreeSetup()
    {
		ThickSkin thickskin = new ThickSkin();

		ItsGoodForYou itsGoodForYou = new ItsGoodForYou();
		ReelItIn reelItIn = new ReelItIn();

		List<Skill> firstTier = new List<Skill>();
		firstTier.Add(thickskin);
		itsGoodForYou.previousSkills = firstTier;
		reelItIn.previousSkills = firstTier;

		TisButAScratch tisButAScratch = new TisButAScratch();

		List<Skill> secondTier = new List<Skill>();
		secondTier.Add(itsGoodForYou);
		secondTier.Add(reelItIn);
		tisButAScratch.previousSkills = secondTier;

		CodLiverOil codLiverOil = new CodLiverOil();
		HeavyHook heavyHook = new HeavyHook();

		List<Skill> thirdTier = new List<Skill>();
		thirdTier.Add(tisButAScratch);
		codLiverOil.previousSkills = thirdTier;
		heavyHook.previousSkills = thirdTier;

		GotMySeaLegs gotMySeaLegs = new GotMySeaLegs();
		BlessingOfTheArk blessingOfTheArk = new BlessingOfTheArk();
        QualityBait qualityBait = new QualityBait();
        

		List<Skill> fourthTier = new List<Skill>();
		fourthTier.Add(codLiverOil);
		heavyHook.previousSkills = fourthTier;
		gotMySeaLegs.previousSkills = fourthTier;

		List<Skill> fifthTier = new List<Skill>();
		fifthTier.Add(heavyHook);
		fifthTier.Add(gotMySeaLegs);
		qualityBait.previousSkills = fifthTier;

		List<Skill> sixthTier = new List<Skill>();
		sixthTier.Add(qualityBait);
		blessingOfTheArk.previousSkills = sixthTier;

		List<Skill> skillTree = new List<Skill>();
		skillTree.Add(thickskin);
		skillTree.Add(itsGoodForYou);
		skillTree.Add(reelItIn);
		skillTree.Add(tisButAScratch);
		skillTree.Add(codLiverOil);
		skillTree.Add(heavyHook);
		skillTree.Add(gotMySeaLegs);
		skillTree.Add(qualityBait);
		skillTree.Add(blessingOfTheArk);
		return skillTree;
    }

    public override void Downed()
    {
        leftAttackBox.SetActive(false);
        rightAttackBox.SetActive(false);
        upAttackBox.SetActive(false);
        downAttackBox.SetActive(false);
        StopCoroutine("ResetFinishedUlti");
        StopCoroutine("ResetFinishedSkillTwo");
        StopCoroutine("ResetFinishedSkillOne");

    }

    public override void ResetVariables(float healthPercentage)
    {

        CmdHeal(gameObject, (health.maxHealth * healthPercentage));
        finishedUlti = false;
        finishedSkillOne = false;
        finishedSkillTwo = false;
    }

    [Command]
    public void CmdHeal(GameObject player, float amount)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        health.Heal(amount);
        health.shield = 0;
        health.aura = 0;
    }
}
