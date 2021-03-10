﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Skill;

public enum ExtraEffect { none, sufferHalfDamage, sufferFiveDamage, ignoreArmor};

[System.Serializable]
public class Effect
{
    public enum EffectType { regenerate, burn, poison, block, magicBlock, statusBuff, statusDebuff, protection, piercing, blind, removeBurn, removePoison, activateNextTurn, skipNextTurn, loseWhenTakeDamage, burnOnContact, attackBuff, spcAttackBuff, defenseBuff, spcDefenseBuff, speedBuff};
    public EffectType effectType;
    public int intensity;
    
    public Effect  (Effect eff) {
        effectType = eff.effectType;
        intensity = eff.intensity;
    }

    public Effect (EffectType type, int intensity) {
        effectType = type;
        this.intensity = intensity;
    }

    public Effect () {

    }
}

[System.Serializable]
public class CastEffect : Effect
{
    public float onCastACC = 100;
    public TargetEnum target;

}

[System.Serializable]
public class DmgSkill
{
    public float baseDmg;
    public float acc = 100;
}


public enum Element { normal, fire, water, thunder, earth, wind, ice, chaos, nature};

[CreateAssetMenu(fileName = "Skill", menuName = "Skill")]

public class Skill : ScriptableObject {

    [SerializeField]
    public List<DmgSkill> dmgSkills;

    public float onHitACC = 100;
    public string skillName;
    public bool isSpecial;

    
    [SerializeField]
    public List<Effect> onHitEffects;
    public List<CastEffect> onCastEffects;

    public Element skillElement;
    public int _manaCost;
    public int manaCost {
        get {

            if (skillElement == Element.fire && 
                PlayerResources.instance.HasArtefact(Artefact.ArtefactEnum.fire_discount) && _manaCost >= 1) {
                return _manaCost - 1;
            }
            return _manaCost;
        }
        set {
            _manaCost = value;
        }
    }

    [TextArea(3, 10)]
    public string info;
    public enum TargetEnum { targetEnemy, allEnemies, self, ally, allAllies, all, none};

    public TargetEnum skillTarget;

    public int skillSpeed;

    public ExtraEffect extraEffect;

    public bool TargetNeedInput {
        get {
            return skillTarget == TargetEnum.targetEnemy || skillTarget == TargetEnum.ally;
        }
    }

    public static bool HitTarget (float skillACC, GameMonster caster, GameMonster target) {
        return true;
        return Random.Range(0, 100) < skillACC + caster.actualACC - target.actualDodge;
    }

    public string SkillType {
        get {
            return isSpecial ? "Special" : "Physic";
        }
    }

    public static float Damage (Skill skill, DmgSkill dmgSkill, GameMonster caster, GameMonster target) {
        float dmg = dmgSkill.baseDmg;
        if (skill.isSpecial) {
            dmg *= ((caster.actualSpcAttack + caster.BonusSpcAttack) / (target.actualSpcDefense + target.BonusSpcDefense));
        }
        else {
            dmg *= ((caster.actualAttack + caster.BonusAttack) / (target.actualDefense + target.BonusDefense));
        }

        if (skill.skillElement == caster.monsterElement) {
            dmg *= 1.25f;
        }
        dmg = Mathf.Max(dmg, 2);
        if (GlobalData.i.SuperEffective(skill, target)) {
            dmg *= 1.5f;
        }
        else if (GlobalData.i.LessEffective (skill, target)) {
            dmg *= 0.66f;
        }

        if (skill.isSpecial) {
            dmg -= target.SpcArmor;
        }
        else if (skill.extraEffect != ExtraEffect.ignoreArmor) {
            dmg -= target.Armor;
        }
        

        
        target.TakeHit(skill.isSpecial, caster);
        dmg = Mathf.Max(dmg, 0);
        return dmg;
    }
}