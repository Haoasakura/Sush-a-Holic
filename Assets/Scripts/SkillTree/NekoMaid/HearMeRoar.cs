using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearMeRoar : Skill
{
    public HearMeRoar()
    {
        skill_name = "Hear Me Roar";
        description = "Increases damage dealt by Nyaa~ Scream's fear by "+ConstantsDictionary.HearMeRoarIncreaseInScreamDamage;
        cost = 30;
        unlocked = false;
    }


    public override void SkillEffect(GameObject player)
    {
        NekoMaidAttacks neko = player.GetComponent<NekoMaidAttacks>();
        neko.nyaaScreamDamageDealt += ConstantsDictionary.HearMeRoarIncreaseInScreamDamage;

    }

}