using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharingIsCaring : Skill
{
    public SharingIsCaring()
    {
        skill_name = "Sharing Is Caring";
        description = "All other players receive "+ConstantsDictionary.SharingIsCaringPercentage+"% of the damages dealt to enemies with Cute Claw in HP";
        cost = 20;
        unlocked = false;
    }


    public override void SkillEffect(GameObject player)
    {
        NekoMaidAttacks neko = player.GetComponent<NekoMaidAttacks>();
        neko.sharingIsCaringPercentageOfDamageInHp = ConstantsDictionary.SharingIsCaringPercentage;
        neko.sharingIsCaringActivated = true;
    }

}