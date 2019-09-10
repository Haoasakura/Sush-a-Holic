using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerController : NetworkBehaviour {

    //identity variables
    private bool isFisherman = false;
    private bool isOcto = false;
    private bool isNeko = false;

    //private component variables
    public PlayerAttacks characterAttacks;
    private Rigidbody2D rb;
    private float m_speed;
    public float m_normal_speed;
    private Animator anim;
    public GameObject aimBox;
    private CircleCollider2D downedCollider;

    //state variables
    public bool attacking = false;
    public bool downed = false;
    public bool dead = false;
    public bool slowed = false;
    private float fireRate = 0.6f;
    private float nextFire = 0f;

    //movements
    private float m_horizontal;
    private float m_vertical;

    private bool playerMoving;
    public Vector2 lastMove;

    //sushi coins
    private int sushi_coins;

    //skills
    public bool skillOneCanBeUsed = true;
    public bool skillTwoCanBeUsed = true;
    private bool usingSkillOne = false;
    private bool usingSkillTwo = false;
    private bool usingUltimate = false;
    private float skillOneCooldown;
    private float skillTwoCooldown;
    private float ultimate_charge;

    //speed boost
    public float speed_boost_duration;

    //downed vars
    public DownedManager downedManager;
    private float downedTime;

    //Arrows to indicate downed players
    public GameObject nekoArrow;
    public GameObject octoArrow;
    public GameObject fishermanArrow;

    private GameObject nekoMaid;
    private GameObject octoChef;
    private GameObject fisherman;

    public Slider nekoDownedTimerSlider;
    public Slider octoDownedTimerSlider;
    public Slider fishermanDownedTimerSlider;

    //UI
    private GameObject skill1Cover;
    private Text skill1Cooldown;
    private GameObject skill2Cover;
    private Text skill2Cooldown;

    public Slider ultimateChargeSlider;

	//SkillTree
	private SkillTreeManager stm;
    private GameObject SushiCoins;


    private void Start() {
        StartCoroutine(AllLoaded());

        if (!isLocalPlayer) {
            gameObject.GetComponentInChildren<Camera>().enabled = false;
        }

        characterAttacks = GetComponent<PlayerAttacks>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.SetFloat("Speed", 1.0f);
        downedCollider = GetComponent<CircleCollider2D>();
        m_speed = characterAttacks.m_speed;
        m_normal_speed = m_speed;

        sushi_coins = 0;
        ultimate_charge = 0;

        skillOneCooldown = characterAttacks.skillOneCooldown;
        skillTwoCooldown = characterAttacks.skillTwoCooldown;

        downedTime = ConstantsDictionary.MaxDownedTime;

        Slider[] DownedTimerSliders= GetComponentsInChildren<Slider>();
        foreach (Slider slider in DownedTimerSliders) {
            if (slider.gameObject.name.Contains("Neko"))
                nekoDownedTimerSlider = slider;
            else if (slider.gameObject.name.Contains("Octo"))
                octoDownedTimerSlider = slider;
            else if (slider.gameObject.name.Contains("Fisherman"))
                fishermanDownedTimerSlider = slider;

        }
        nekoDownedTimerSlider.maxValue = downedTime;
        octoDownedTimerSlider.maxValue = downedTime;
        fishermanDownedTimerSlider.maxValue = downedTime;
        nekoArrow.SetActive(false);
        octoArrow.SetActive(false);
        fishermanArrow.SetActive(false);

        lastMove = new Vector2(0, -1);
        playerMoving = false;
        anim.SetBool("Moving", playerMoving);

        ultimateChargeSlider = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<Slider>();

        ultimateChargeSlider.value = 0;
        ultimateChargeSlider.GetComponent<Text>().text = "0%";

		stm = gameObject.GetComponent <SkillTreeManager>();
    }

    void Update() {
        if (!isLocalPlayer) {
            return;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            Debug.Log("not attacking");
            StopAttacking();
        }

        //codice per input management
        if (dead || downed) {
            return;
        }
        //cerchi con frecce che indicano player downed
        if (downedManager != null && downedManager.nekoAlive == 0 && !isNeko) {
            if (nekoMaid == null) {
                nekoMaid = FindObjectOfType<NekoMaidAttacks>().gameObject;
            }
            if(!nekoArrow.activeInHierarchy)
                nekoDownedTimerSlider.maxValue = downedManager.nekoDownedTimer;
            nekoArrow.SetActive(true);
            Vector3 nekoPos = nekoMaid.transform.position;
            Vector3 myPos = transform.position;
            Vector3 direction = (nekoPos - myPos).normalized;

            Quaternion resetRot = Quaternion.identity;
            resetRot.eulerAngles = new Vector3(0, 0, 0);
            nekoArrow.transform.rotation = resetRot;
            Quaternion rot = Quaternion.identity;
            rot.eulerAngles = direction;
            nekoArrow.transform.rotation = Quaternion.FromToRotation(nekoArrow.transform.up, new Vector3(direction.x, direction.y, 0));
            nekoDownedTimerSlider.value = downedManager.nekoDownedTimer;
        }else
        {
            nekoArrow.SetActive(false);
        }
        if (downedManager != null && downedManager.octoAlive == 0 && !isOcto) {
            if (octoChef == null) {
                octoChef = FindObjectOfType<OctoChefAttacks>().gameObject;
            }
            if (!octoArrow.activeInHierarchy)
                octoDownedTimerSlider.maxValue = downedManager.octoDownedTimer;
            octoArrow.SetActive(true);
            Vector3 octoPos = octoChef.transform.position;
            Vector3 myPos = transform.position;
            Vector3 direction = (octoPos - myPos).normalized;

            Quaternion resetRot = Quaternion.identity;
            resetRot.eulerAngles = new Vector3(0, 0, 0);
            octoArrow.transform.rotation = resetRot;
            Quaternion rot = Quaternion.identity;
            rot.eulerAngles = direction;
            octoArrow.transform.rotation = Quaternion.FromToRotation(octoArrow.transform.up, new Vector3(direction.x, direction.y, 0));
            octoDownedTimerSlider.value = downedManager.octoDownedTimer;
        }else
        {
            octoArrow.SetActive(false);
        }
        if (downedManager != null && downedManager.fishermanAlive == 0 && !isFisherman) {
            if (fisherman == null) {
                fisherman = FindObjectOfType<FishermanAttacks>().gameObject;
            }
            if (!fishermanArrow.activeInHierarchy)
                fishermanDownedTimerSlider.maxValue = downedManager.fishermanDownedTimer;
            fishermanArrow.SetActive(true);
            Vector3 fishPos = fisherman.transform.position;
            Vector3 myPos = transform.position;
            Vector3 direction = (fishPos - myPos).normalized;

            Quaternion resetRot = Quaternion.identity;
            resetRot.eulerAngles = new Vector3(0, 0, 0);
            fishermanArrow.transform.rotation = resetRot;
            Quaternion rot = Quaternion.identity;
            rot.eulerAngles = direction;
            fishermanArrow.transform.rotation = Quaternion.FromToRotation(fishermanArrow.transform.up, new Vector3(direction.x, direction.y, 0));
            fishermanDownedTimerSlider.value = downedManager.fishermanDownedTimer;
        }else
        {
            fishermanArrow.SetActive(false);
        }

        //movimenti
        playerMoving = false;

        m_horizontal = Input.GetAxisRaw("Horizontal");
        m_vertical = Input.GetAxisRaw("Vertical");
        if (m_horizontal != 0 || m_vertical != 0)
        {
            playerMoving = true;
            lastMove = new Vector2(m_horizontal, m_vertical);
        }

        anim.SetFloat("MoveX", m_horizontal);
        anim.SetFloat("MoveY", m_vertical);
        anim.SetBool("Moving", playerMoving);
        anim.SetFloat("LastMoveX", lastMove.x);
        anim.SetFloat("LastMoveY", lastMove.y);

        //chiamate alle skill dei personaggi singoli
        if (usingUltimate) {
            characterAttacks.UltimateSkill();
            return;
        }
        if (usingSkillOne) {
            characterAttacks.SkillOne();
            return;
        }
        if (usingSkillTwo) {
            characterAttacks.SkillTwo();
            return;
        }
        //per debug
        if (Input.GetKeyDown(KeyCode.L))
        {
            StopAttacking();
        }

        //blocco input mentre si attacca
        /*if (attacking) {
            return;
        }*/



        //revive downed player
        if (Input.GetKeyDown(KeyCode.F)) {

            Collider2D[] nearPlayers = Physics2D.OverlapCircleAll(transform.position, 2);
            foreach (Collider2D coll in nearPlayers)
            {
                GameObject go = coll.gameObject;
                if (go.CompareTag(Tags.player) && go != gameObject)
                {
                    PlayerController pl = go.GetComponent<PlayerController>();
                    if (pl.downed && !pl.dead)
                    {
                        CmdReviveDownedPlayer(go, ConstantsDictionary.MedicateHealthPercentage, "");
                    }
                }
            }
        }

        //skill one
        if (Input.GetKeyDown(KeyCode.Q) && !attacking && skillOneCanBeUsed) {
            attacking = true;
            playerMoving = false;
            anim.SetBool("Skill", true);
            anim.SetBool("Moving", playerMoving);
            rb.velocity = Vector2.zero;
            usingSkillOne = true;
            characterAttacks.SkillOne();
            return;
        }

        //skill two
        if (Input.GetKeyDown(KeyCode.E) && !attacking && skillTwoCanBeUsed) {
            attacking = true;
            playerMoving = false;
            anim.SetBool("Skill", true);
            anim.SetBool("Moving", playerMoving);
            rb.velocity = Vector2.zero;
            usingSkillTwo = true;
            characterAttacks.SkillTwo();
            return;
        }

        //ultimate
        if (Input.GetMouseButtonDown(1) && !attacking && ultimate_charge >= 100) {
            attacking = true;
            playerMoving = false;
            anim.SetBool("Skill", true);
            anim.SetBool("Moving", playerMoving);
            rb.velocity = Vector2.zero;
            usingUltimate = true;
            characterAttacks.UltimateSkill();
            return;
        }

        //basic attack
        if (Input.GetMouseButtonDown(0) && !attacking && Time.time > nextFire) {
            nextFire = Time.time + fireRate;

            attacking = true;
            playerMoving = false;
            anim.SetBool("Attacking", true);
            anim.SetBool("Moving", playerMoving);
            rb.velocity = Vector2.zero;
        }

		//Skill Tree
		if (Input.GetKeyDown (KeyCode.Tab))
		{
			ShowSkillTree ();
		}
    }

    private void FixedUpdate() {
        if (!isLocalPlayer) {
            return;
        }

        if (dead || downed) {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = new Vector2(m_horizontal * m_speed, m_vertical * m_speed);
    }

    public void StopAttacking() {
        anim.SetBool("Attacking", false);
        attacking = false;
    }

    public bool HasEnoughSushiCoins(int amount) {
        if (sushi_coins >= amount) {
            return true;
        }

        return false;
    }

    public void AddSushiCoins(int amount) {
        sushi_coins += amount;
        UpdateSushiCoinsText();

    }

    //fare sempre il controllo che ci siano abbastanza sushi coins da spendere 
    public void SpendSushiCoins(int cost) {
        sushi_coins -= cost;
		UpdateSushiCoinsText ();
    }

    public IEnumerator StartSkillOneCooldown() {
        if (isLocalPlayer) {
            skill1Cover.SetActive(true);
            for (int i = 0; i < skillOneCooldown; i++) {
                skill1Cooldown.text = (skillOneCooldown - i) + " ";
                yield return new WaitForSeconds(1f);

            }
            skill1Cover.SetActive(false);
            skillOneCanBeUsed = true;
        }
    }

    public IEnumerator StartSkillTwoCooldown() {
		if (isLocalPlayer) {
            skill2Cover.SetActive(true);
            for (int i = 0; i < skillTwoCooldown; i++) {
                skill2Cooldown.text = (skillTwoCooldown - i) + " ";
                yield return new WaitForSeconds(1f);
            }
            skill2Cover.SetActive(false);
            skillTwoCanBeUsed = true;
        }
    }

    public void AbortSkillOne() {
        attacking = false;
        anim.SetBool("Skill", false);
        skillOneCanBeUsed = true;
        usingSkillOne = false;
    }

    public void AbortSkillTwo() {
        attacking = false;
        anim.SetBool("Skill", false);
        skillTwoCanBeUsed = true;
        usingSkillTwo = false;
    }

    public void StopSkillOne() {
        attacking = false;
        anim.SetBool("Skill", false);
        skillOneCanBeUsed = false;
        usingSkillOne = false;
        StartCoroutine("StartSkillOneCooldown");
    }

    public void StopSkillTwo() {
        attacking = false;
        anim.SetBool("Skill", false);
        skillTwoCanBeUsed = false;
        usingSkillTwo = false;
        StartCoroutine("StartSkillTwoCooldown");
    }

    public void StopUltimate() {
        attacking = false;
        anim.SetBool("Skill", false);
        usingUltimate = false;
    }

    public void IncreaseUltimateCharge(float amount) {
        if (isLocalPlayer){
            ultimate_charge += amount;
            if (ultimate_charge >= 100)
            {
                ultimate_charge = 100f;
            }
            ultimateChargeSlider.value = ultimate_charge;
            if (ultimate_charge == 100f)
                ultimateChargeSlider.GetComponent<Text>().text = "RMB";
            else
                ultimateChargeSlider.GetComponent<Text>().text = ultimate_charge + "%";
        }
    }

    public void ResetUltimateCharge() {
        if (isLocalPlayer) {
            ultimate_charge = 0;
            ultimateChargeSlider.value = 0;
            ultimateChargeSlider.GetComponent<Text>().text = "0%";
        }

    }

    public void SpeedChange(float percentage, float duration) {
        if (!isLocalPlayer) {
            slowed = true;
            speed_boost_duration = duration;
            m_speed += m_speed * percentage;
            StartCoroutine("ResetSpeedBoost");
        }
    }

    public IEnumerator ResetSpeedBoost() {
        yield return new WaitForSeconds(speed_boost_duration);
        m_speed = m_normal_speed;
        slowed = false;
    }

    public void ActivateAimBox() {
        aimBox.SetActive(true);
    }

    public void DeactivateAimBox() {
        aimBox.SetActive(false);
    }

    [ClientRpc]
    public void RpcDowned() {
        downed = true;
        anim.SetBool("Downed", true);
        downedCollider.enabled = true;
        if (isLocalPlayer)
        {
            if (usingSkillOne)
            {
                AbortSkillOne();
            }
            if (usingSkillTwo)
            {
                AbortSkillTwo();
            }
            octoArrow.SetActive(false);
            nekoArrow.SetActive(false);
            fishermanArrow.SetActive(false);
            StopCoroutine("ResetSpeedBoost");
            skill1Cover.SetActive(false);
            skill2Cover.SetActive(false);
            StopCoroutine("StartSkillOneCooldown");
            StopCoroutine("StartSkillTwoCooldown");
            characterAttacks.Downed();
            CmdChangeDownedVector(isNeko, isOcto, isFisherman, 0, downedTime, 0);
        }
    }

    [ClientRpc]
    public void RpcReviveFromDowned(float healthPercentage, string penalties) {
        downedCollider.enabled = false;
        anim.SetBool("Downed", false);
        downed = false;
        if (isLocalPlayer)
        {
            characterAttacks.ResetVariables(healthPercentage);
            ResetVariables();
            if (penalties.Equals("last"))
            {
                //non applico nessuna penalty
                return;
            }
            if (penalties.Equals("all"))
            {
                RemoveAllDownedTimePenalties();
                return;
            }
            AddDownedTimePenalty();
        }
    }

    [Command]
    public void CmdReviveDownedPlayer(GameObject player, float healingPercentage, string penalties)
    {
        PlayerAttacks plContr = player.GetComponent<PlayerAttacks>();
        if(plContr is NekoMaidAttacks)
        {
            downedManager.StopCoroutine("NekoDownedTimer");
            downedManager.nekoAlive = 1;
            downedManager.neko.RpcReviveFromDowned(healingPercentage, penalties);
            return;
        }
        if (plContr is OctoChefAttacks)
        {
            downedManager.StopCoroutine("OctoDownedTimer");
            downedManager.octoAlive = 1;
            downedManager.octo.RpcReviveFromDowned(healingPercentage,penalties);
            return;
        }
        if (plContr is FishermanAttacks)
        {
            downedManager.StopCoroutine("FishDownedTimer");
            downedManager.fishermanAlive = 1;
            downedManager.fisherman.RpcReviveFromDowned(healingPercentage,penalties);
            return;
        }
        
    }

    [ClientRpc]
    public void RpcReviveFromDead()
    {
        downed = false;
        dead = false;
        anim.SetTrigger("Revive");
        if (isLocalPlayer)
        {
            characterAttacks.ResetVariables(100);
            ResetVariables();
            downedTime = ConstantsDictionary.MaxDownedTime;
            GetComponentInChildren<Camera>().enabled = true;
        }
    }




    [Command]
    public void CmdChangeDownedVector(bool isNeko, bool isOcto, bool isFish, int value, float downedTimer, float healthPercentage)
    {
        if (!isServer)
        {
            Debug.Log("not server");
            return;
        }
        if (isNeko)
        {
            downedManager.nekoAlive = value;
            if (value == 0)
            {
                downedManager.nekoDownedTimer = downedTimer;
                downedManager.neko = this;
                downedManager.StartCoroutine("NekoDownedTimer");
            }
            return;
        }
        if (isOcto)
        {
            downedManager.octoAlive = value;
            //downedManager.RpcChangeDowned(isNeko, isOcto, isFish, value);
            if (value == 0)
            {
                downedManager.octoDownedTimer = downedTimer;
                downedManager.octo = this;
                downedManager.StartCoroutine("OctoDownedTimer");
            }
            return;
        }
        if (isFish)
        {
            downedManager.fishermanAlive = value;
            //downedManager.RpcChangeDowned(isNeko, isOcto, isFish, value);
            if (value == 0)
            {
                downedManager.fishermanDownedTimer = downedTimer;
                downedManager.fisherman = this;
                downedManager.StartCoroutine("FishDownedTimer");
            }
            return;
        }
    }

    [ClientRpc]
    public void RpcDie() {
        dead = true;
        downedCollider.enabled = false;
        anim.SetTrigger("Dead");
        if (isLocalPlayer)
        {
            //sposto la camera del local player morto su quella di un altro player vivo
            PlayerController[] players = FindObjectsOfType<PlayerController>();
            foreach(PlayerController player in players)
            {
                if(this != player && !player.downed)
                {
                    GetComponentInChildren<Camera>().enabled = false;
                    player.gameObject.GetComponentInChildren<Camera>().enabled = true;
                }

            }
        }
    }

    public void AddDownedTimePenalty() {
        downedTime -= ConstantsDictionary.SinglePenalty;
        if (downedTime < ConstantsDictionary.MinimumDownedTime) {
            downedTime = ConstantsDictionary.MinimumDownedTime;
        }
    }

    public void RemoveOneDownedTimePenalty() {
        downedTime += ConstantsDictionary.SinglePenalty;
        if (downedTime > ConstantsDictionary.MaxDownedTime) {
            downedTime = ConstantsDictionary.MaxDownedTime;
        }
    }

    public void RemoveAllDownedTimePenalties() {
        downedTime = ConstantsDictionary.MaxDownedTime;
    }

    private void ResetVariables() {
        ultimate_charge = 0;
        m_speed = m_normal_speed;
        lastMove = new Vector2(0, -1);
        attacking = false;
        playerMoving = false;
        skillOneCanBeUsed = true;
        skillTwoCanBeUsed = true;
        usingSkillOne = false;
        usingSkillTwo = false;
        usingUltimate = false;
        anim.SetBool("Moving", playerMoving);
        anim.SetFloat("MoveX", 0);
        anim.SetFloat("MoveY", 0);
        anim.SetFloat("LastMoveX", lastMove.x);
        anim.SetFloat("LastMoveY", lastMove.y);
        anim.SetBool("Skill", false);
        anim.SetBool("Attacking", false);
    }

    private void SetupLocalPlayer() {
        GetComponent<PlayerHealth>().healthBar = GameObject.Find("UI/LifeBars/Player1Bar/GreenBar");
        skill1Cover = GameObject.Find("UI/SkillSet/Skill1Cover");
        skill1Cooldown = skill1Cover.transform.Find("Text").gameObject.GetComponent<Text>();
        skill2Cover = GameObject.Find("UI/SkillSet/Skill2Cover");
        skill2Cooldown = skill2Cover.transform.Find("Text").gameObject.GetComponent<Text>();
        skill1Cover.SetActive(false);
        skill2Cover.SetActive(false);

        GameObject skill1thumb = GameObject.Find("UI/SkillSet/Skill1");
        GameObject skill2thumb = GameObject.Find("UI/SkillSet/Skill2");

        GameObject skill1Image = null;
        GameObject skill2Image = null;

        if (characterAttacks is NekoMaidAttacks)
        {
            isNeko = true;
            GameObject.Find("UI/SkillSet/Skill1Name").GetComponent<Text>().text = ConstantsDictionary.NekoSkillOneName;
            GameObject.Find("UI/SkillSet/Skill2Name").GetComponent<Text>().text = ConstantsDictionary.NekoSkillTwoName;
            skill1Image = Instantiate(Resources.Load("UI Prefabs/NekoSkill1", typeof(GameObject))) as GameObject;
            skill2Image = Instantiate(Resources.Load("UI Prefabs/NekoSkill2", typeof(GameObject))) as GameObject;
        }


        if (characterAttacks is OctoChefAttacks)
        {
            isOcto = true;
            GameObject.Find("UI/SkillSet/Skill1Name").GetComponent<Text>().text = ConstantsDictionary.OctoSkillOneName;
            GameObject.Find("UI/SkillSet/Skill2Name").GetComponent<Text>().text = ConstantsDictionary.OctoSkillTwoName;
            skill1Image = Instantiate(Resources.Load("UI Prefabs/OctoSkill1", typeof(GameObject))) as GameObject;
            skill2Image = Instantiate(Resources.Load("UI Prefabs/OctoSkill2", typeof(GameObject))) as GameObject;
        }


        if (characterAttacks is FishermanAttacks)
        {
            isFisherman = true;
            GameObject.Find("UI/SkillSet/Skill1Name").GetComponent<Text>().text = ConstantsDictionary.FishSkillOneName;
            GameObject.Find("UI/SkillSet/Skill2Name").GetComponent<Text>().text = ConstantsDictionary.FishSkillTwoName;
            skill1Image = Instantiate(Resources.Load("UI Prefabs/FishSkill1", typeof(GameObject))) as GameObject;
            skill2Image = Instantiate(Resources.Load("UI Prefabs/FishSkill2", typeof(GameObject))) as GameObject;
        }

		skill1Image.transform.position = skill1Cover.transform.position;
		skill1Image.transform.SetParent(skill1thumb.transform);
		skill2Image.transform.position = skill2Cover.transform.position;
		skill2Image.transform.SetParent(skill2thumb.transform);

        Button btn = GameObject.Find("UI/SkillSet/SkillTree").GetComponent<Button>();
        btn.onClick.AddListener(ShowSkillTree);
        SushiCoins = GameObject.Find("SushiCoinsNumber");

        GameObject localPlayerThumb = GameObject.Find("UI/LifeBars/Player1Bar/LocalPlayerThumbnail");
        GameObject player2Thumb = GameObject.Find("UI/LifeBars/Player2Bar/Player2Thumbnail");
        GameObject player3Thumb = GameObject.Find("UI/LifeBars/Player3Bar/Player3Thumbnail");
        GameObject player2healthbar = GameObject.Find("UI/LifeBars/Player2Bar/GreenBar");
        GameObject player3healthbar = GameObject.Find("UI/LifeBars/Player3Bar/GreenBar");


        GameObject localPlayer = null;

        if (isNeko) {
            localPlayer = Instantiate(Resources.Load("UI Prefabs/NekoLocalThumbnail", typeof(GameObject))) as GameObject;
        }
        else if (isOcto) {
            localPlayer = Instantiate(Resources.Load("UI Prefabs/OctoLocalThumbnail", typeof(GameObject))) as GameObject;
        }
        else if (isFisherman) {
            localPlayer = Instantiate(Resources.Load("UI Prefabs/FishermanLocalThumbnail", typeof(GameObject))) as GameObject;
        }
        localPlayer.transform.position = localPlayerThumb.transform.position;
        localPlayer.transform.parent = localPlayerThumb.transform;
        characterAttacks.GetComponent<PlayerHealth>().currentHealth = characterAttacks.GetComponent<PlayerHealth>().maxHealth;
        CmdGetGameManager(isNeko, isOcto, isFisherman);

    }

    [Command]
    public void CmdGetGameManager(bool isNeko, bool isOcto, bool isFish)
    {
        RpcGetGameManager(isNeko, isOcto, isFisherman);

    }

    [ClientRpc]
    public void RpcGetGameManager(bool isNeko, bool isOcto, bool isFish) {
        downedManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<DownedManager>();
        if (isLocalPlayer) {
            downedManager.AddPlayer(isNeko, isOcto, isFisherman);
        }
    }

    private void SetupOtherPlayers() {
        
        CmdGetGameManager(isNeko, isOcto, isFisherman);

        GameObject player2Bar = GameObject.Find("UI/LifeBars/Player2Bar");
        GameObject player3Bar = GameObject.Find("UI/LifeBars/Player3Bar");
        GameObject player2Thumb = GameObject.Find("UI/LifeBars/Player2Bar/Player2Thumbnail");
        GameObject player3Thumb = GameObject.Find("UI/LifeBars/Player3Bar/Player3Thumbnail");
        GameObject player2healthbar = GameObject.Find("UI/LifeBars/Player2Bar/GreenBar");
        GameObject player3healthbar = GameObject.Find("UI/LifeBars/Player3Bar/GreenBar");

        if (characterAttacks is NekoMaidAttacks) {
            if (!player2Bar.activeInHierarchy) {
                player2Bar.SetActive(true);
                GameObject nekoThumb = Instantiate(Resources.Load("UI Prefabs/NekoThumbnail", typeof(GameObject))) as GameObject;
                nekoThumb.transform.position = player2Thumb.transform.position;
                nekoThumb.transform.parent = player2Thumb.transform;
                FindObjectOfType<NekoMaidAttacks>().gameObject.GetComponent<PlayerHealth>().healthBar = player2healthbar;
            }
            else {
                player3Bar.SetActive(true);
                GameObject nekoThumb = Instantiate(Resources.Load("UI Prefabs/NekoThumbnail", typeof(GameObject))) as GameObject;
                nekoThumb.transform.position = player3Thumb.transform.position;
                nekoThumb.transform.parent = player3Thumb.transform;
                FindObjectOfType<NekoMaidAttacks>().gameObject.GetComponent<PlayerHealth>().healthBar = player3healthbar;
            }
        }

        if (characterAttacks is OctoChefAttacks) {
            if (!player2Bar.activeInHierarchy) {
                player2Bar.SetActive(true);
                GameObject octoThumb = Instantiate(Resources.Load("UI Prefabs/OctoThumbnail", typeof(GameObject))) as GameObject;
                octoThumb.transform.position = player2Thumb.transform.position;
                octoThumb.transform.parent = player2Thumb.transform;
                FindObjectOfType<OctoChefAttacks>().gameObject.GetComponent<PlayerHealth>().healthBar = player2healthbar;
            }
            else {
                player3Bar.SetActive(true);
                GameObject octoThumb = Instantiate(Resources.Load("UI Prefabs/OctoThumbnail", typeof(GameObject))) as GameObject;
                octoThumb.transform.position = player3Thumb.transform.position;
                octoThumb.transform.parent = player3Thumb.transform;
                FindObjectOfType<OctoChefAttacks>().gameObject.GetComponent<PlayerHealth>().healthBar = player3healthbar;
            }
        }

        if (characterAttacks is FishermanAttacks) {
            if (!player2Bar.activeInHierarchy) {
                player2Bar.SetActive(true);
                GameObject fishermanThumb = Instantiate(Resources.Load("UI Prefabs/FishermanThumbnail", typeof(GameObject))) as GameObject;
                fishermanThumb.transform.position = player2Thumb.transform.position;
                fishermanThumb.transform.parent = player2Thumb.transform;
                FindObjectOfType<FishermanAttacks>().gameObject.GetComponent<PlayerHealth>().healthBar = player2healthbar;
            }
            else {
                player3Bar.SetActive(true);
                GameObject fishermanThumb = Instantiate(Resources.Load("UI Prefabs/FishermanThumbnail", typeof(GameObject))) as GameObject;
                fishermanThumb.transform.position = player3Thumb.transform.position;
                fishermanThumb.transform.parent = player3Thumb.transform;
                FindObjectOfType<FishermanAttacks>().gameObject.GetComponent<PlayerHealth>().healthBar = player3healthbar;
            }
        }
    }

    IEnumerator AllLoaded() {
        while (!ClientScene.ready) {
            yield return null;
        }
        
        if (isLocalPlayer)
            SetupLocalPlayer();
        else 
            SetupOtherPlayers();
    }

	public void AssignSTM(SkillTreeManager skillTreeManager)
	{
		stm = skillTreeManager;
	}

	private void ShowSkillTree()
    {
		GameObject SkillTreeBackground = GameObject.Find ("UI/SkillSet/SkillTreeBackground");
		Debug.Log (SkillTreeBackground);
		if (!SkillTreeBackground.activeSelf) {
			SkillTreeBackground.SetActive (true);
			stm.setStubs ();
			stm.UpdateCost ();
			UpdateSushiCoinsText ();
		} else {
			SkillTreeBackground.SetActive (false);
		}
    }

	private void UpdateSushiCoinsText()
	{
		Text SushiCoinsText = SushiCoins.GetComponent<Text> ();
		SushiCoinsText.text = sushi_coins.ToString ();
		stm.CheckForAvailableSkill ();
	}
}
