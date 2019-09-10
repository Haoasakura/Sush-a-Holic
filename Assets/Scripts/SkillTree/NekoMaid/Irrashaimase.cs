using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Irrashaimase : Skill
{
    public Irrashaimase()
    {
        skill_name = "Irrashaimase~ *w*";
        description = "Maid's Welcome revives all downed players, heals all players of "+(ConstantsDictionary.MaidWelcomeHealingPercentage+ConstantsDictionary.IrrashaimaseMaidWelcomeHealingPercentageIncrease)+"% HP and removes all downed timer's penalties";
        cost = 65;
        unlocked = false;
    }


    public override void SkillEffect(GameObject player)
    {
        NekoMaidAttacks neko = player.GetComponent<NekoMaidAttacks>();
        neko.maidWelcomeHealingPercentage += ConstantsDictionary.IrrashaimaseMaidWelcomeHealingPercentageIncrease;
        neko.maidWelcomeWithIrrashaimase = true;

    }

}
