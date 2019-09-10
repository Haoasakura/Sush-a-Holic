using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NekoMaidAttacks : PlayerAttacks
{
    //Player controller
    private PlayerController playerController;
    private Animator anim;
    //public PlayerHealth health;
    //Hitboxes
    public GameObject leftAttackBox;
    public GameObject rightAttackBox;
    public GameObject upAttackBox;
    public GameObject downAttackBox;

    public GameObject dangoSpawn;
    private Transform dangoSpawnTransform;
    public GameObject dangoPrefab;

    private GameObject playerSelected;
    private List<PlayerController> allPlayers;
    private bool lastFound = false;
    private bool finishedSkillOne = false;
    private bool finishedSkillTwo = false;
    private bool finishedUlti = false;


    //skill variables
    public float dangoBreakHealing;
    public bool dangoBreakSinglePlayer = true;
    public float dangoBreakArea;
    public bool dangoBreakSpeedBoost = false;
    public float dangoBreakAmountSpeedBoost;
    public float dangoBreakSpeedBoostDuration;
    public float nyaaScreamAreaRadius;
    public float nyaaScreamDamageDealt;
    public float nyaaScreamFearDuration;
    public float nyaaScreamDuration;
    public float maidWelcomeHealingPercentage;
    public bool maidWelcomeWithIrrashaimase = false;

    //passive skill variables
    public float kawaiiPercentageOfDamageInHp;
    public bool sharingIsCaringActivated = false;
    public float sharingIsCaringPercentageOfDamageInHp;

    GameObject[] players;

    private void Awake()
    {
        m_speed = ConstantsDictionary.NekoMaidSpeed;
        m_defense = ConstantsDictionary.NekoMaidDefense;
        m_hp = ConstantsDictionary.NekoMaidHP;
        m_basic_attack_damage = ConstantsDictionary.NekoMaidBasicAttackDamage;
        m_basic_attack_threat = ConstantsDictionary.NekoMaidBasicAttackThreat;
        m_attack_type = ConstantsDictionary.NekoMaidBasicAttackType;
        skillOneCooldown = ConstantsDictionary.NekoMaidSkillOneCooldown;
        skillTwoCooldown = ConstantsDictionary.NekoMaidSkillTwoCooldown;
        dangoBreakHealing = ConstantsDictionary.DangoBreakHealing;
        kawaiiPercentageOfDamageInHp = 0;
        nyaaScreamAreaRadius = ConstantsDictionary.StartingNyaaScreamRadius;
        nyaaScreamDamageDealt = ConstantsDictionary.StartingNyaaScreamDamageDealt;
        nyaaScreamFearDuration = ConstantsDictionary.StartingNyaaScreamDuration;
        nyaaScreamDuration = ConstantsDictionary.NyaaScreamColliderDuration;
        sharingIsCaringPercentageOfDamageInHp = 0;
        dangoBreakArea = ConstantsDictionary.DangoBreakAreaDangoDaiKazoku;
        dangoBreakAmountSpeedBoost = ConstantsDictionary.DangoBreakSpeedBoost;
        dangoBreakSpeedBoostDuration = ConstantsDictionary.DangoBreakSpeedBoostDuration;
        maidWelcomeHealingPercentage = ConstantsDictionary.MaidWelcomeHealingPercentage;
    }

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag(Tags.player);
        playerController = gameObject.GetComponent<PlayerController>();
        anim = gameObject.GetComponent<Animator>();
        allPlayers = new List<PlayerController>();
        health = GetComponent<PlayerHealth>();
        foreach(GameObject player in players)
        {
            allPlayers.Add(player.GetComponent<PlayerController>());
        }
		dangoSpawnTransform = dangoSpawn.transform;
		skillTree = SkillTreeSetup();
		stm = gameObject.AddComponent <SkillTreeManager>();
		stm.AssignToSkillTreeManager (skillTree, playerController);
		playerController.AssignSTM (stm);
    }

    //DANGO BREAK
    public override void SkillOne()
    {
        if (finishedSkillOne)
        {
            return;
        }
        //prima e dopo passive skill dango dai kazoku
        if (dangoBreakSinglePlayer)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(playerSelected != null)
                {
                    playerSelected.GetComponent<PlayerController>().DeactivateAimBox();
                    playerSelected = null;
                }
                playerController.AbortSkillOne();
                return;
            }
            if (Input.GetMouseButtonDown(0) && playerSelected != null)
            {
                PlayerController plContr = playerSelected.GetComponent<PlayerController>();
                plContr.DeactivateAimBox();
                CmdDangoSpawn(playerSelected);
                anim.SetTrigger("SkillOne");
                //NetworkServer.Spawn(dango);
                if (dangoBreakSpeedBoost)
                {
                    plContr.SpeedChange(dangoBreakAmountSpeedBoost, dangoBreakSpeedBoostDuration);
                }
                //playerSelected.GetComponent<PlayerHealth>().Heal(dangoBreakHealing);
                CmdHeal(playerSelected, dangoBreakHealing);
                playerSelected = null;
                playerController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForSkill);
                finishedSkillOne = true;
                StartCoroutine("ResetFinished");
                return;
            }

            bool found = false;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bool check=false;

            Collider2D[] col = Physics2D.OverlapPointAll(mousePosition);
            PlayerController controllerSelected=null;
            if (col.Length > 0) {
                foreach (Collider2D c in col) {
                    if (c.gameObject.CompareTag(Tags.player)) {
                        if(!c.gameObject.GetComponent<PlayerController>().downed)
                        {
                            check = true;
                            playerSelected = c.gameObject;
                            controllerSelected = c.gameObject.GetComponent<PlayerController>();
                            break;
                        }

                    }
                }
            }
            if (check) {
                found = true;
                controllerSelected.ActivateAimBox();
                foreach (PlayerController contr in allPlayers)
                {
                    if (contr != controllerSelected)
                    {
                        contr.DeactivateAimBox();
                    }
                }
            }
            if(lastFound == true && found == false)
            {
                foreach (PlayerController contr in allPlayers)
                {
                    contr.DeactivateAimBox();
                }
                playerSelected = null;
            }
            lastFound = found;            
        }
        //quando dango break prende tutti i player
        else
        {
            anim.SetTrigger("SkillOne");
            foreach(GameObject player in players)
            {
                if(Vector3.Distance(gameObject.transform.position, player.transform.position) <= dangoBreakArea)
                {
                    //lancio il dango solo a chi non è la neko
                    if(gameObject != player)
                    {
                        CmdDangoSpawn(player);
                    }
                    //qui va tutto il blocco in cmdheal
                    PlayerHealth health = player.GetComponent<PlayerHealth>();
                    if (health.currentHealth > 0)
                    {
                        CmdHeal(player, dangoBreakHealing);

                        //con teppanyaki dango questa skill da anche un temporaneo speed boost a tutti i player nell'area
                        if (dangoBreakSpeedBoost)
                        {
                            PlayerController contr = player.GetComponent<PlayerController>();
                            contr.SpeedChange(dangoBreakAmountSpeedBoost, dangoBreakSpeedBoostDuration);
                        }
                    }
                }
            }
            playerController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForSkill);
            finishedSkillOne = true;
            StartCoroutine("ResetFinished");
        }
    }

    [Command]
    public void CmdHeal(GameObject player, float amount)
    {
        player.GetComponent<PlayerHealth>().Heal(amount);
    }

    [Command]
    public void CmdHealPercentage(GameObject player, float percentage)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        health.Heal(health.maxHealth * percentage);
    }

    private IEnumerator ResetFinished()
    {
        yield return new WaitForSeconds(2f);
        finishedSkillOne = false;
        playerSelected = null;
    }

    [Command]
    public void CmdDangoSpawn(GameObject playerSelected)
    {
        GameObject dango = GameObject.Instantiate(dangoPrefab);
        Dango dangoScript = dango.GetComponent<Dango>();
        dango.transform.position = dangoSpawnTransform.position;
        dangoScript.playerToReach = playerSelected.transform;
        NetworkServer.Spawn(dango);
    }

    //NYAA SCREAM
    public override void SkillTwo()
    {
        if (finishedSkillTwo)
        {
            return;
        }
        anim.SetTrigger("SkillTwo");

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, nyaaScreamAreaRadius);
        foreach (Collider2D collider in colliders) {
            if (collider.CompareTag(Tags.enemy)) {
                CmdSkillTwo(collider.gameObject);
            }
        }
        CmdComboField();
        playerController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForSkill);
        finishedSkillTwo = true;
        StartCoroutine("NyaaScream");
    }
    [Command]
    public void CmdSkillTwo(GameObject collider) {
        collider.GetComponent<EnemyHealth>().CmdTakeDamage(nyaaScreamDamageDealt, ConstantsDictionary.PLAYERS.neko, 1);
        collider.GetComponent<EnemyController>().CmdApplyFear(nyaaScreamFearDuration);
    }

    [Command]
    private void CmdComboField()
    {
        GameObject combo = Instantiate(comboField);
        combo.transform.position = transform.position;
        NetworkServer.Spawn(combo);
    }

    private IEnumerator NyaaScream()
    {
        yield return new WaitForSeconds(nyaaScreamDuration);
        playerController.StopSkillTwo();
        finishedSkillTwo = false;
    }

    //MAID'S WELCOME
    public override void UltimateSkill()
    {
        if (finishedUlti)
        {
            return;
        }
        anim.SetTrigger("Ultimate");
        playerController.ResetUltimateCharge();
        //prima e dopo l'attivazione della skill passiva irrashaimase
        //toglie una o tutte le penalties sui downed players
        foreach (PlayerController player in allPlayers)
        {
            if (player.dead)
            {
                continue;
            }
            if (player.downed)
            {

                if (maidWelcomeWithIrrashaimase)
                {
                    playerController.CmdReviveDownedPlayer(player.gameObject, maidWelcomeHealingPercentage, "all");
                }
                else
                {
                    playerController.CmdReviveDownedPlayer(player.gameObject, maidWelcomeHealingPercentage, "last");
                }
                continue;
            }

        }
        finishedUlti = true;
        StartCoroutine("ResetFinishedUlti");
    }

    private IEnumerator ResetFinishedUlti()
    {
        yield return new WaitForSeconds(2f);
        finishedUlti = false;
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
        Kawaii kawaii = new Kawaii();

        NekoBanshee nekoBanshee = new NekoBanshee();
        DangoDaiKazoku dangoDaiKazoku = new DangoDaiKazoku();

        List<Skill> firstTier = new List<Skill>();
        firstTier.Add(kawaii);
        nekoBanshee.previousSkills = firstTier;
        dangoDaiKazoku.previousSkills = firstTier;

        SharingIsCaring sharingIsCaring = new SharingIsCaring();

        List<Skill> secondTier = new List<Skill>();
        secondTier.Add(nekoBanshee);
        secondTier.Add(dangoDaiKazoku);
        sharingIsCaring.previousSkills = secondTier;

        HearMeRoar hearMeRoar = new HearMeRoar();
        TeppanyakiDango teppanyakiDango = new TeppanyakiDango();

        List<Skill> thirdTier = new List<Skill>();
        thirdTier.Add(sharingIsCaring);
        hearMeRoar.previousSkills = thirdTier;
        teppanyakiDango.previousSkills = thirdTier;

        FearOfTheCat fearOfTheCat = new FearOfTheCat();
        Irrashaimase irrashaimase = new Irrashaimase();
        ChampionBreakfast championBreakfast = new ChampionBreakfast();

        List<Skill> fourthTier = new List<Skill>();
        fourthTier.Add(hearMeRoar);
        fearOfTheCat.previousSkills = fourthTier;

        List<Skill> fifthTier = new List<Skill>();
        fifthTier.Add(hearMeRoar);
        fifthTier.Add(teppanyakiDango);
        irrashaimase.previousSkills = fifthTier;

        List<Skill> sixthTier = new List<Skill>();
        sixthTier.Add(teppanyakiDango);
        championBreakfast.previousSkills = sixthTier;

        List<Skill> skillTree = new List<Skill>();
        skillTree.Add(kawaii);
        skillTree.Add(nekoBanshee);
        skillTree.Add(dangoDaiKazoku);
        skillTree.Add(sharingIsCaring);
        skillTree.Add(hearMeRoar);
        skillTree.Add(teppanyakiDango);
        skillTree.Add(fearOfTheCat);
        skillTree.Add(irrashaimase);
        skillTree.Add(championBreakfast);
        return skillTree;
    }

    public override void Downed()
    {
        leftAttackBox.SetActive(false);
        rightAttackBox.SetActive(false);
        upAttackBox.SetActive(false);
        downAttackBox.SetActive(false);
        if(playerSelected != null)
        {
            playerSelected.GetComponent<PlayerController>().DeactivateAimBox();
            playerSelected = null;
        }
        StopCoroutine("ResetFinished");
        StopCoroutine("ResetFinishedUlti");
        //nyaaScreamObject.SetActive(false);
    }

    public override void ResetVariables(float healthPercentage)
    {
        finishedSkillOne = false;
        finishedSkillTwo = false;
        finishedUlti = false;
        CmdHeal(gameObject, (health.maxHealth * healthPercentage));
    }

    [Command]
    public void CmdResetVars(GameObject go)
    {
        PlayerHealth health = go.GetComponent<PlayerHealth>();
        health.shield = 0;
        health.aura = 0;
    }
}
