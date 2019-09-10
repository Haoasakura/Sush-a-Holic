using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kawaii : Skill
{
    public Kawaii()
    {
        skill_name = "Kawaii *w*";
        description = "NekoMaid receives " + ConstantsDictionary.KawaiiHPPercentage + "% of the damages dealt to enemies with Cute Claw in HP";
        cost = 10;
        unlocked = false;
    }



    public override void SkillEffect(GameObject player)
    {
        NekoMaidAttacks neko = player.GetComponent<NekoMaidAttacks>();
        neko.kawaiiPercentageOfDamageInHp = ConstantsDictionary.KawaiiHPPercentage;
        Debug.Log("kawaii attivata");
    }

    public new bool CanBeUnlocked()
    {
        return true;
    }
}
