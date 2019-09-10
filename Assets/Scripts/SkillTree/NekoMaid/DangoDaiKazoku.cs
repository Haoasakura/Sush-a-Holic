using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangoDaiKazoku : Skill
{
    public DangoDaiKazoku()
    {
        skill_name = "Dango Dai Kazoku";
        description = "Dango Break heals of 60 HP all allies in a limited area around the Neko Maid";
        cost = 15;
        unlocked = false;
    }


    public override void SkillEffect(GameObject player)
    {
        NekoMaidAttacks neko = player.GetComponent<NekoMaidAttacks>();
        neko.dangoBreakHealing = ConstantsDictionary.DangoBreakHealingDangoDaiKazoku;
        neko.dangoBreakSinglePlayer = false;
    }

}
