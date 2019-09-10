using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NekoBanshee : Skill
{
    public NekoBanshee()
    {
        skill_name = "Neko Banshee";
        description = "Increases the area in which enemies are affected by Nyaa~ Scream's fear";
        cost = 15;
        unlocked = false;
    }


    public override void SkillEffect(GameObject player)
    {
        NekoMaidAttacks neko = player.GetComponent<NekoMaidAttacks>();
        neko.nyaaScreamAreaRadius += ConstantsDictionary.NekoBansheeIncreaseInNyaaScreamRadius;
    }

}
