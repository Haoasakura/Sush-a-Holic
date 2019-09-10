using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OctoChefAttacks : PlayerAttacks{

    //Player controller
    private PlayerController playerController;
    private Animator anim;
    private Animation animations;
    private Rigidbody2D rb;
    //public PlayerHealth health;

    //spawn dei coltelli
    public Transform leftSpawn;
    public Transform rightSpawn;
    public Transform upSpawn;
    public Transform downSpawn;

    public GameObject knifePrefab;

    private float poisonAreaDuration;
    public GameObject gunsight;
	private Vector3 gunsightScale;

    private bool started = false;

    private bool finishedSkillOne = false;
    private bool finishedSkillTwo = false;
    private bool finishedUlti = false;

    //attack variables
    public float knifeVelocity;
    public int enemiesHitWithKnife;
    private float fireRate = 0.5f;
    private float nextFire = 0.0f;

    //skill variables
    public float purpleRainPoisonArea;
    public float purpleRainPoisonDamage;
    public float purpleRainPoisonDuration;
    public float hackNSushiIncreaseInDamage;
    public bool hackNSushiBeforeMasterOfKnives = true;
    public float sushiDiscoDanceDuration;
    private float sushiDiscoDanceRadius;
    public float sushiDiscoDancePercentageDamageIncrease;

    private void Awake()
    {
        playerType = ConstantsDictionary.PLAYERS.octo;
        m_speed = ConstantsDictionary.OctoChefSpeed;
        m_defense = ConstantsDictionary.OctoChefDefense;
        m_hp = ConstantsDictionary.OctoChefHP;
        m_basic_attack_damage = ConstantsDictionary.OctoChefBasicAttackDamage;
        m_basic_attack_threat = ConstantsDictionary.OctoChefBasicAttackThreat;
        m_attack_type = ConstantsDictionary.OctoChefBasicAttackType;
        skillOneCooldown = ConstantsDictionary.OctoChefSkillOneCooldown;
        skillTwoCooldown = ConstantsDictionary.OctoChefSkillTwoCooldown;
        knifeVelocity = ConstantsDictionary.OctoChefKnifeVelocity;
        enemiesHitWithKnife = ConstantsDictionary.OctoChefEnemiesHitWithKnife;
        purpleRainPoisonArea = ConstantsDictionary.PurpleRainPoisonArea;
        hackNSushiIncreaseInDamage = ConstantsDictionary.HackNSushiIncreaseInDamage;
        purpleRainPoisonDuration = ConstantsDictionary.PurpleRainPoisonDuration;
        purpleRainPoisonDamage = ConstantsDictionary.PurpleRainPoisonDamage;
        sushiDiscoDanceDuration = ConstantsDictionary.SushiDiscoDanceDuration;
        poisonAreaDuration = ConstantsDictionary.PoisonAreaDuration;
        sushiDiscoDanceRadius = ConstantsDictionary.SushiDiscoDanceRadius;
        sushiDiscoDancePercentageDamageIncrease = ConstantsDictionary.SushiDiscoDancePercentageDamageIncrease;
    }

    private void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();
        anim = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        animations = gameObject.GetComponent<Animation>();
        gunsight = Instantiate(gunsight);
		gunsightScale = gunsight.transform.localScale;
        health = GetComponent<PlayerHealth>();
        CmdSpawnComboField();
		skillTree = SkillTreeSetup();
		stm = gameObject.AddComponent <SkillTreeManager>();
		stm.AssignToSkillTreeManager (skillTree, playerController);
		playerController.AssignSTM (stm);
    }
    [Command]
    public void CmdSpawnComboField() {
        NetworkServer.Spawn(Instantiate(comboField));
    }

    public void Attack()
    {
        Vector3 mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mouse_position - transform.position;
        Vector2 direction = (new Vector2(dir.x, dir.y)).normalized;
        CmdBasicAttack(direction);
    }

    [Command]
    private void CmdBasicAttack(Vector2 direction)
    {
        if(Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject knife = GameObject.Instantiate(knifePrefab);//, transform.position + direction.normalized * 1f, Quaternion.FromToRotation(Vector3.right, new Vector3(direction.x, direction.y, 0)));
            Quaternion resetRot = Quaternion.identity;
            resetRot.eulerAngles = new Vector3(0, 0, 0);
            knife.transform.rotation = resetRot;
            Quaternion rot = Quaternion.identity;
            rot.eulerAngles = direction;
            knife.transform.rotation = Quaternion.FromToRotation(knife.transform.right, new Vector3(direction.x, direction.y, 0));
            knife.transform.position = transform.position + (new Vector3(direction.x, direction.y, 0)) * 1f; //un minimo di distanza dal centro dell'octo
            OctoChefKnife octoKnife = knife.GetComponent<OctoChefKnife>();
            octoKnife.inNekoMaidComboField = isInNekoComboField;
            octoKnife.inFishermanComboField = isInFishermanComboField;
            octoKnife.damage = m_basic_attack_damage;
            octoKnife.octoChef = gameObject;
            octoKnife.enemiesToHit = enemiesHitWithKnife;
            octoKnife.inNekoMaidComboField = isInNekoComboField;
            knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * direction;
            NetworkServer.Spawn(knife);
        }

    }


    public override void BasicAttackDown()
    {
        CmdBasicAttackDown();
    }

    [Command]
    private void CmdBasicAttackDown()
    {
        GameObject knife = GameObject.Instantiate(knifePrefab);
        knife.transform.position = downSpawn.position;
        knife.transform.rotation = Quaternion.AngleAxis(90, new Vector3(0, 0, 1));
        OctoChefKnife octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = isInNekoComboField;
        octoKnife.inFishermanComboField = isInFishermanComboField;
        octoKnife.damage = m_basic_attack_damage;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife;
        octoKnife.inNekoMaidComboField = isInNekoComboField;
        knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.down;
        NetworkServer.Spawn(knife);
    }


    public override void BasicAttackLeft()
    {
        CmdBasicAttackLeft();
    }

    [Command]
    private void CmdBasicAttackLeft()
    {
        GameObject knife = GameObject.Instantiate(knifePrefab);
        knife.transform.position = leftSpawn.position;
        OctoChefKnife octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = isInNekoComboField;
        octoKnife.inFishermanComboField = isInFishermanComboField;
        octoKnife.damage = m_basic_attack_damage;
        octoKnife.enemiesToHit = enemiesHitWithKnife;
        octoKnife.octoChef = gameObject;
        octoKnife.inNekoMaidComboField = isInNekoComboField;
        knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.left;
        NetworkServer.Spawn(knife);
    }

    public override void BasicAttackRight()
    {
        CmdBasicAttackRight();
    }

    [Command]
    private void CmdBasicAttackRight()
    {
        GameObject knife = GameObject.Instantiate(knifePrefab);
        knife.transform.position = rightSpawn.position;
        knife.transform.localScale = new Vector3(-1, 1, 1);
        OctoChefKnife octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = isInNekoComboField;
        octoKnife.inFishermanComboField = isInFishermanComboField;
        octoKnife.damage = m_basic_attack_damage;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife;
        octoKnife.inNekoMaidComboField = isInNekoComboField;
        knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.right;
        NetworkServer.Spawn(knife);
    }

    public override void BasicAttackUp()
    {
        CmdBasicAttackUp();
    }

    [Command]
    private void CmdBasicAttackUp()
    {
        GameObject knife = GameObject.Instantiate(knifePrefab);
        knife.transform.position = upSpawn.position;
        knife.transform.rotation = Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        OctoChefKnife octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = isInNekoComboField;
        octoKnife.inFishermanComboField = isInFishermanComboField;
        octoKnife.damage = m_basic_attack_damage;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife;
        octoKnife.inNekoMaidComboField = isInNekoComboField;
        knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.up;
        NetworkServer.Spawn(knife);
    }

    //HACK 'N' SUSHI
    public override void SkillOne()
    {
        if (finishedSkillOne)
        {
            return;
        }
        //TODO controllare se è meglio far chiamare la funzione dei coltelli dall'animazione per essere più sincronizzata
        if (hackNSushiBeforeMasterOfKnives)
        {
            if (playerController.lastMove.x == 0 && playerController.lastMove.y == 1)
            {
                CmdFire3KnivesBack();
                anim.SetTrigger("SkillOne");
                finishedSkillOne = true;
                StartCoroutine("ResetFinishedOne");
                playerController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForSkill);
                return;
            }
            if (playerController.lastMove.x == 0 && playerController.lastMove.y == -1)
            {
                CmdFire3KnivesFront();
                anim.SetTrigger("SkillOne");
                finishedSkillOne = true;
                StartCoroutine("ResetFinishedOne");
                playerController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForSkill);
                return;
            }
            if (playerController.lastMove.x == -1 && playerController.lastMove.y == 0)
            {
                CmdFire3KnivesLeft();
                anim.SetTrigger("SkillOne");
                finishedSkillOne = true;
                StartCoroutine("ResetFinishedOne");
                playerController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForSkill);
                return;
            }
            if (playerController.lastMove.x == 1 && playerController.lastMove.y == 0)
            {
                CmdFire3KnivesRight();
                anim.SetTrigger("SkillOne");
                finishedSkillOne = true;
                StartCoroutine("ResetFinishedOne");
                playerController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForSkill);
                return;
            }

        }
        else
        {
            CmdFire4Knives();
            anim.SetTrigger("SkillOne");
        }
        finishedSkillOne = true;
        playerController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForSkill);
        StartCoroutine("ResetFinishedOne");
    }

    private IEnumerator ResetFinishedOne()
    {
        yield return new WaitForSeconds(2f);
        finishedSkillOne = false;
    }

    [Command]
    private void CmdFire3KnivesBack()
    {
        List<GameObject> knives = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            knives.Add(GameObject.Instantiate(knifePrefab));
        }

        //first knife
        GameObject knife = knives[0];
        knife.transform.position = upSpawn.position;
        knife.transform.rotation = Quaternion.AngleAxis(90, new Vector3(0, 0, 1));
        OctoChefKnife octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.enemiesToHit = enemiesHitWithKnife;
        octoKnife.octoChef = gameObject;
        knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.up;
        NetworkServer.Spawn(knife);

        //second knife
        knife = knives[1];
        knife.transform.position = rightSpawn.position;
        octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife;
        knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.right;
        NetworkServer.Spawn(knife);

        //third knife
        knife = knives[2];
        knife.transform.position = leftSpawn.position;
        knife.transform.localScale = new Vector3(-1, 1, 1);
        octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.enemiesToHit = enemiesHitWithKnife;
        octoKnife.octoChef = gameObject;
        knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.left;
        NetworkServer.Spawn(knife);
    }

    [Command]
    private void CmdFire3KnivesFront()
    {
        List<GameObject> knives = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            knives.Add(GameObject.Instantiate(knifePrefab));
        }

        //first knife
        GameObject knife = knives[0];
        knife.transform.position = downSpawn.position;
        knife.transform.rotation = Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        OctoChefKnife octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife; knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.down;
        NetworkServer.Spawn(knife);

        //second knife
        knife = knives[1];
        knife.transform.position = rightSpawn.position;
        octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife; knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.right;
        NetworkServer.Spawn(knife);

        //third knife
        knife = knives[2];
        knife.transform.position = leftSpawn.position;
        knife.transform.localScale = new Vector3(-1, 1, 1);
        octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife; knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.left;
        NetworkServer.Spawn(knife);
    }

    [Command]
    private void CmdFire3KnivesLeft()
    {
        List<GameObject> knives = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            knives.Add(GameObject.Instantiate(knifePrefab));
        }

        //first knife
        GameObject knife = knives[0];
        knife.transform.position = leftSpawn.position;
        knife.transform.localScale = new Vector3(-1, 1, 1);
        OctoChefKnife octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife; knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.left;
        NetworkServer.Spawn(knife);

        //second knife
        knife = knives[1];
        knife.transform.position = upSpawn.position;
        knife.transform.rotation = Quaternion.AngleAxis(90, new Vector3(0, 0, 1));
        octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife; knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.up;
        NetworkServer.Spawn(knife);

        //third knife
        knife = knives[2];
        knife.transform.position = downSpawn.position;
        knife.transform.rotation = Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife; knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.down;
        NetworkServer.Spawn(knife);
    }

    [Command]
    private void CmdFire3KnivesRight()
    {
        List<GameObject> knives = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            knives.Add(GameObject.Instantiate(knifePrefab));
        }

        //first knife
        GameObject knife = knives[0];
        knife.transform.position = rightSpawn.position;
        OctoChefKnife octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife; knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.right;
        NetworkServer.Spawn(knife);

        //second knife
        knife = knives[1];
        knife.transform.position = upSpawn.position;
        knife.transform.rotation = Quaternion.AngleAxis(90, new Vector3(0, 0, 1));
        octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife; knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.up;
        NetworkServer.Spawn(knife);

        //third knife
        knife = knives[2];
        knife.transform.position = downSpawn.position;
        knife.transform.rotation = Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife; knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.down;
        NetworkServer.Spawn(knife);
    }

    [Command]
    private void CmdFire4Knives()
    {
        List<GameObject> knives = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            knives.Add(GameObject.Instantiate(knifePrefab));
        }

        //first knife
        GameObject knife = knives[0];
        knife.transform.position = upSpawn.position;
        knife.transform.rotation = Quaternion.AngleAxis(90, new Vector3(0, 0, 1));
        OctoChefKnife octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife; knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.right;
        NetworkServer.Spawn(knife);

        //second knife
        knife = knives[1];
        knife.transform.position = rightSpawn.position;
        octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife; knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.right;
        NetworkServer.Spawn(knife);

        //third knife
        knife = knives[2];
        knife.transform.position = leftSpawn.position;
        knife.transform.localScale = new Vector3(-1, 1, 1);
        octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife; knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.left;
        NetworkServer.Spawn(knife);

        //fourth knife
        knife = knives[3];
        knife.transform.position = downSpawn.position;
        knife.transform.rotation = Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        octoKnife = knife.GetComponent<OctoChefKnife>();
        octoKnife.inNekoMaidComboField = false;
        octoKnife.inFishermanComboField = false;
        octoKnife.damage = (m_basic_attack_damage * hackNSushiIncreaseInDamage) / 100f;
        octoKnife.octoChef = gameObject;
        octoKnife.enemiesToHit = enemiesHitWithKnife; knife.GetComponent<Rigidbody2D>().velocity = knifeVelocity * Vector2.down;
        NetworkServer.Spawn(knife);
    }

    //PURPLE RAIN
    public override void SkillTwo()
    {
        if (finishedSkillTwo)
        {
            gunsight.SetActive(false);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            playerController.AbortSkillTwo();
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            gunsight.SetActive(false);
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePosition, purpleRainPoisonArea);
            foreach (Collider2D collider in colliders) {
                if (collider.CompareTag(Tags.enemy)) {
                    CmdSkillTwo(collider.gameObject);
                }
            }
            CmdComboField(mousePosition);
            anim.SetTrigger("SkillTwo");
            StartCoroutine("ResetFinishedSkillTwo");
            finishedSkillTwo = true;
            playerController.IncreaseUltimateCharge(ConstantsDictionary.ultiIncreaseForSkill);
            return;
        }
        if (!gunsight.activeInHierarchy && !finishedSkillTwo)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gunsight.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
			gunsight.transform.localScale = gunsightScale*purpleRainPoisonArea;
            gunsight.SetActive(true);
        }else
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gunsight.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        }

    }

    [Command]
    public void CmdSkillTwo(GameObject collider) {
        collider.gameObject.GetComponent<EnemyHealth>().CmdTakeDotDamage(purpleRainPoisonDamage, purpleRainPoisonDuration, ConstantsDictionary.PLAYERS.octo, ConstantsDictionary.OctoChefBasicAttackThreat);
    }

    [Command]
    private void CmdComboField(Vector3 position)
    {
        GameObject combo = Instantiate(comboField);
        combo.transform.position = new Vector3(position.x, position.y, 0);
        NetworkServer.Spawn(combo);
    }

    private IEnumerator ResetFinishedSkillTwo()
    {
        yield return new WaitForSeconds(4f);
        finishedSkillTwo = false;
    }

    //SUSHI DISCO DANCE
    public override void UltimateSkill()
    {

        if (!finishedUlti) { 
            anim.SetTrigger("Ultimate");
            started = true;
            StartCoroutine("SushiDiscoDanceDamage");
            StartCoroutine("FinishSushiDiscoDance");
            finishedUlti = true;
        }
    }

    public void FixedUpdate()
    {
        if (started)
        {
            //TODO meglio translate con deltatime?
            float m_horizontal = Input.GetAxisRaw("Horizontal");
            float m_vertical = Input.GetAxisRaw("Vertical");
            rb.velocity = new Vector2(m_horizontal * m_speed, m_vertical * m_speed);
            return;
        }
    }

    public override void StopBasicAttackDown()
    {
        playerController.StopAttacking();
    }

    public override void StopBasicAttackLeft()
    {
        playerController.StopAttacking();
    }

    public override void StopBasicAttackRight()
    {
        playerController.StopAttacking();
    }

    public override void StopBasicAttackUp()
    {
        playerController.StopAttacking();
    }

    private IEnumerator SushiDiscoDanceDamage()
    {
        while (started)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, sushiDiscoDanceRadius);
            foreach(Collider2D collider in hitColliders)
            {
                if (collider.gameObject.CompareTag(Tags.enemy))
                {
                    EnemyHealth enemyHealth = collider.gameObject.GetComponent<EnemyHealth>();
                    enemyHealth.CmdTakeDamage(
                        DamageFormulas.CalculateBasicAttackDamage(
                            (m_basic_attack_damage + m_basic_attack_damage * sushiDiscoDancePercentageDamageIncrease),
                            enemyHealth.defense, ConstantsDictionary.randomK, AttackTypeMultiplierFactory.SelectAttackTypeMultiplier(m_attack_type, enemyHealth.type)), playerType, m_basic_attack_threat);
                }

            }
            yield return new WaitForSeconds(ConstantsDictionary.SushiDiscoDanceDamageTick);
        }
    }

    private IEnumerator FinishSushiDiscoDance()
    {
        yield return new WaitForSeconds(sushiDiscoDanceDuration);
        started = false;
        finishedUlti = false;
        StopCoroutine("SushiDiscoDanceDamage");
        playerController.StopUltimate();
        playerController.ResetUltimateCharge();
        rb.velocity = Vector2.zero;
    }

    private List<Skill> SkillTreeSetup()
    {
        SlipperyTentacles slipperyTentacles = new SlipperyTentacles();

        List<Skill> firstTier = new List<Skill>();
        firstTier.Add(slipperyTentacles);

        MyBeakIsBigger myBeakIsBigger = new MyBeakIsBigger();
        RealShokunin realShokunin = new RealShokunin();

        myBeakIsBigger.previousSkills = firstTier;
        realShokunin.previousSkills = firstTier;

        List<Skill> secondTier = new List<Skill>();
        secondTier.Add(myBeakIsBigger);
        secondTier.Add(realShokunin);

        FasterCuts fasterCuts = new FasterCuts();
        fasterCuts.previousSkills = secondTier;

        List<Skill> thirdTier = new List<Skill>();
        thirdTier.Add(fasterCuts);

        InkThis inkThis = new InkThis();
        MasterOfKnives masterOfKnives = new MasterOfKnives();

        inkThis.previousSkills = thirdTier;
        masterOfKnives.previousSkills = thirdTier;

        List<Skill> fourthTier = new List<Skill>();
        fourthTier.Add(inkThis);

        LingeringToxin lingeringToxin = new LingeringToxin();
        lingeringToxin.previousSkills = fourthTier;

        List<Skill> fifthTier = new List<Skill>();
        fifthTier.Add(inkThis);
        fifthTier.Add(masterOfKnives);

        Showtime showtime = new Showtime();
        showtime.previousSkills = fifthTier;

        List<Skill> sixthTier = new List<Skill>();
        sixthTier.Add(masterOfKnives);

        SharpenedEdges sharpenedEdges = new SharpenedEdges();
        sharpenedEdges.previousSkills = sixthTier;

        List<Skill> skillTree = new List<Skill>();
        skillTree.Add(slipperyTentacles);
        skillTree.Add(myBeakIsBigger);
        skillTree.Add(realShokunin);
        skillTree.Add(fasterCuts);
        skillTree.Add(inkThis);
        skillTree.Add(masterOfKnives);
        skillTree.Add(lingeringToxin);
        skillTree.Add(showtime);
        skillTree.Add(sharpenedEdges);

        return skillTree;
    }

    public override void Downed()
    {
        //Not needed here
        return;
    }

    public override void ResetVariables(float healthPercentage)
    {
        finishedSkillOne = false;
        finishedSkillTwo = false;
        finishedUlti = false;
        started = false;
        CmdHeal(gameObject, (health.maxHealth * healthPercentage));
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
