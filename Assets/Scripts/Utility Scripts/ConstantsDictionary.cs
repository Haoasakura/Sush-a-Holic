using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantsDictionary : ScriptableObject {


    public enum PLAYERS { neko, octo, fisherman };
    public enum DOUGHNUT_TYPE { Chocolate, Cream, Strawberry };
    public enum CHIP_TYPE {Normal, Ketchup, Mayonnaise };

    //Damage variables
    public static int randomK = 2;
    public static float ultiIncreaseForSkill = 3f;
    public static float ultiIncreaseForBasicAttack = 2f;
    public static float ultiIncreaseForCombo = 4f;
    public static float reducedComboFieldThreat = 0.5f;

    //Combo fields variables
    public static float comboFieldDuration = 5f;
    public static float nekoComboRecoveredHpPercentage = 0.25f;
    public static float octoComboIncreasedDamagePercentage = 0.03f;

    //Downed time variables
    public static float MaxDownedTime = 15f;
    public static float SinglePenalty = 1f;
    public static float MinimumDownedTime = 1f;
    public static float MedicateHealthPercentage = 0.2f;

    //Sushi coins
    public static float DurationTime = 10f;

    //Health packs
    public static float RespawnTime = 25f;

    //Neko Maid variables
    public static float NekoMaidSpeed = 5.5f;
    public static float NekoMaidDefense = 33f;
    public static float NekoMaidHP = 100f;
    public static float NekoMaidBasicAttackDamage = 45f;
    public static float NekoMaidBasicAttackThreat = 1f;
    public static AttackTypes NekoMaidBasicAttackType = AttackTypes.Cut;
    public static float NekoMaidBasicAttackDuration = 0.3f;
    //skills
    public static string NekoSkillOneName = "Dango Break ";
    public static string NekoSkillTwoName = "Nyaa~ Scream";
    public static float NekoMaidSkillOneCooldown = 10f;
    public static float NekoMaidSkillTwoCooldown = 12f;
    public static float DangoBreakHealing = 40f;
    public static float DangoBreakSpeedBoost = 0.5f;
    public static float DangoBreakSpeedBoostDuration = 2f;
    public static float StartingNyaaScreamRadius = 4f;
    public static float StartingNyaaScreamDamageDealt = 10f;
    public static float StartingNyaaScreamDuration = 2f;
    public static float NyaaScreamColliderDuration = 0.75f;
    public static float MaidWelcomeHealingPercentage = 75f;
    //skill tree
    public static float KawaiiHPPercentage = 0.5f;
    public static float NekoBansheeIncreaseInNyaaScreamRadius = 2f;
    public static float DangoBreakHealingDangoDaiKazoku = 60f;
    public static float DangoBreakAreaDangoDaiKazoku = 5f;
    public static float SharingIsCaringPercentage = 1f;
    public static float HearMeRoarIncreaseInScreamDamage = 10f;
    public static float FearOfTheCatNyaaScreamDurationIncrease = 1f;
    public static float IrrashaimaseMaidWelcomeHealingPercentageIncrease = 25f;
    public static float ChampionBreakfastDangoBreakHealing = 70f;

    //Fisherman variables
    public static float FishermanSpeed = 5f;
    public static float FishermanDefense = 50f;
    public static float FishermanHP = 125f;
    public static float FishermanBasicAttackDamage = 50f;
    public static float FishermanBasicAttackThreat = 1f;
    public static string FishSkillOneName = "Omega-3 Protection";
    public static string FishSkillTwoName = "I Got Ya! ";
    public static float FishermanSkillOneCooldown = 10f;
    public static float FishermanSkillTwoCooldown = 10f;
    public static AttackTypes FishermanBasicAttackType = AttackTypes.Impact;
    //skill
    public static float Omega3ProtectionAbsorbedDamage = 60f;
    public static float Omega3ProtectionDuration = 3f;
    public static float IGotYaHookRange = 4f;
	public static float IGotYaDmgArea = 4f;
    public static float IGotYaDamage = 20f;
    public static float IGotYaStunDuration = 2f;
    public static float IGotYaThreat = 1000f;
    public static float SushiDivineProtectionDuration = 5f;
	public static float SushiDivineProtectionAura = 80f;
	public static float SushiDivineKnockbackArea = 3f;
    public static float SushiDivineKnockbackDuration = 2f;
    //skill tree
    public static float ThickSkinDamageReductionPercentage = 0.15f;
    public static float ItsGoodForYouOmega3DurationIncrease = 2f;
    public static float ReelItInHookRange = 5f;
    public static float TisButAScratchHealthIncrease = 25f;
    public static float CodLiverOilAbsorbedDamagesIncrease = 20f;
    public static float GotMySeasLegsEvadedAttacksPercentage = 0.1f;
    public static float BlessingOfTheArkDurationIncrease = 2f;
    public static float QualityBaitIncreaseInArea = 2f;


    //Octo Chef variables
    public static float OctoChefSpeed = 6.5f;
    public static float OctoChefDefense = 30f;
    public static float OctoChefHP = 75f;
    public static float OctoChefBasicAttackDamage = 60f;
    public static float OctoChefBasicAttackThreat = 1f;
    public static float OctoChefKnifeVelocity = 10f;
    public static AttackTypes OctoChefBasicAttackType = AttackTypes.Cut;
    public static string OctoSkillOneName = "Hack ‘n’ Sushi";
    public static string OctoSkillTwoName = "Purple Rain";
    public static float OctoChefSkillOneCooldown = 5f;
    public static float OctoChefSkillTwoCooldown = 12f;
    public static int OctoChefEnemiesHitWithKnife = 1;
    public static float OctoChefKnifeDuration = 2f;
    public static float PoisonAreaDuration = 0.2f;

    //skill
    public static float PurpleRainPoisonArea = 1f;
    public static float PurpleRainPoisonDamage = 4f;
    public static float PurpleRainPoisonDuration = 5f;
    public static float PurpleRainPoisonTick = 1f;
    public static float HackNSushiIncreaseInDamage = 200f;
    public static float SushiDiscoDancePercentageDamageIncrease = 0.15f;
    public static float SushiDiscoDanceDuration = 3f;
    public static float SushiDiscoDanceDamageTick = 0.3f;
    public static float SushiDiscoDanceRadius = 1f;
    
    //skill tree
    public static float SlipperyTentaclesIncreaseInSpeed = 5f;
    public static float MyBeakIsBiggerPoisonArea = 2f;
    public static float RealShokuninIncreaseInDamage = 10f;
    public static float FasterCutsAttackSpeed = 1.05f;
    public static float InkThisPoisonDamage = 0.3f;
    public static float LigeringToxinPoisonDuration = 7f;
    public static float ShowtimeDiscoDancePercentageDamageIncrease = 0.25f;
    public static float ShowTimeDiscoDanceDuration = 5f;
    public static int SharpenedEdgesEnemiesHit = 2;

}
