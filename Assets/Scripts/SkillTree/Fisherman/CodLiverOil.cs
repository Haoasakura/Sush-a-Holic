using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodLiverOil : Skill
{
    public CodLiverOil()
    {
        skill_name = "Cod Liver Oil";
        description = "Increases damages absorbed by Omega-3 Protection";
        cost = 30;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        FishermanAttacks fisherman = player.GetComponent<FishermanAttacks>();
        fisherman.omega3ProtectionAbsorbedDamage += ConstantsDictionary.CodLiverOilAbsorbedDamagesIncrease;
    }
}