using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class PlayerAttacks : NetworkBehaviour {
    public ConstantsDictionary.PLAYERS playerType;

    public float m_speed;
    public float m_defense;
    public float m_hp;
    public float m_basic_attack_damage;
    public float m_basic_attack_threat;
    public AttackTypes m_attack_type;
    public float skillOneCooldown;
    public float skillTwoCooldown;

    [SyncVar]
    public bool isInNekoComboField = false;
    [SyncVar]
    public bool isInOctoComboField = false;
    [SyncVar]
    public bool isInFishermanComboField = false;

    public GameObject comboField;
    public PlayerHealth health;
    public List<Skill> skillTree;
	public SkillTreeManager stm;

    public abstract void BasicAttackLeft();
    public abstract void BasicAttackRight();
    public abstract void BasicAttackUp();
    public abstract void BasicAttackDown();

    public abstract void StopBasicAttackDown();
    public abstract void StopBasicAttackLeft();
    public abstract void StopBasicAttackRight();
    public abstract void StopBasicAttackUp();

    public abstract void SkillOne();
    public abstract void SkillTwo();
    public abstract void UltimateSkill();

    public abstract void Downed();
    public abstract void ResetVariables(float healthPercentage);

}
