using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FasterCuts : Skill
{
    public FasterCuts()
    {
        skill_name = "Faster Cuts";
        description = "Increases attack speed of Sashimi Slash";
        cost = 30;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        Animator anim = player.GetComponent<Animator>();
        anim.SetFloat("Speed", ConstantsDictionary.FasterCutsAttackSpeed);
    }
}
