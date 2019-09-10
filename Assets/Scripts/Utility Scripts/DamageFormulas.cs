using UnityEngine;

public class DamageFormulas : ScriptableObject{

    public static float CalculateBasicAttackDamage(float basicAttack, float defense, int randomK, float attackTypeMultiplier)
    {
        return Mathf.Max((basicAttack - (Random.Range(defense-randomK, defense+randomK)) * attackTypeMultiplier),0f);
    }

}