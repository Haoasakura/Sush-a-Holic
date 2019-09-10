using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeppanyakiDango : Skill
{
    public TeppanyakiDango()
    {
        skill_name = "Teppanyaki Dango";
        description = "Dango Break also gives a temporary speed boost to all players";
        cost = 30;
        unlocked = false;
    }


    public override void SkillEffect(GameObject player)
    {
        NekoMaidAttacks neko = player.GetComponent<NekoMaidAttacks>();
        neko.dangoBreakSpeedBoost = true;

    }

}