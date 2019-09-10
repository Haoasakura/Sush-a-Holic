using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionBreakfast : Skill
{
    public ChampionBreakfast()
    {
        skill_name = "Champions' Breakfast";
        description = "Dango Break healing increases to "+ ConstantsDictionary.ChampionBreakfastDangoBreakHealing + " HP";
        cost = 55;
        unlocked = false;
    }


    public override void SkillEffect(GameObject player)
    {
        NekoMaidAttacks neko = player.GetComponent<NekoMaidAttacks>();
        neko.dangoBreakHealing = ConstantsDictionary.ChampionBreakfastDangoBreakHealing;

    }

}
