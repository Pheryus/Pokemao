using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Skill;

public enum ExtraEffect { none, sufferHalfDamage, sufferFiveDamage, ignoreArmor};

[System.Serializable]
public class Effect
{
    public enum EffectType { regenerate = 0, burn = 1, poison = 2, block = 3, resistance = 4, protection = 5, piercing = 6,
        blind = 7, removeBurn = 8, removePoison = 9, activateNextTurn = 10, skipNextTurn = 11, loseWhenTakeDamage = 12,
        burnOnContact = 13, vigorBuff = 14, focusBuff = 15, wisdomBuff = 16, agilityBuff = 18,
        cancelChannel = 24, channelSpeed = 25, haste = 28};

    public EffectType effectType;
    public int intensity;

    [HideInInspector]
    public bool gainThisTurn;
    
    public Effect  (Effect eff, bool gainThisTurn = false) {
        effectType = eff.effectType;
        intensity = eff.intensity;
        this.gainThisTurn = gainThisTurn;
    }

    public Effect (EffectType type, int intensity) {
        effectType = type;
        this.intensity = intensity;
    }

    public Effect () {

    }

    public static int IntensityFrom (List<Effect> effects, EffectType type) {
        Effect ef = effects.Find(eff => eff.effectType == type);
        return ef != null ? ef.intensity : 0;
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

    public enum StatusType { vigor, agility, wisdom, focus };

    public StatusType attackStatus;

    public StatusType defenseStatus;

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


    public static float GetStatusValue(StatusType statusType, GameMonster monster) {
        float status = 0;

        switch (statusType) {
            case StatusType.vigor:
                status = monster.actualVigor + monster.BonusVigor;
                break;
            case StatusType.agility:
                status = monster.actualAgility + monster.BonusAgility;
                break;
            case StatusType.focus:
                status = monster.actualFocus + monster.BonusFocus;
                break;
            case StatusType.wisdom:
                status = monster.actualWisdom + monster.BonusWisdom;
                break;
        }
        return Mathf.Max(1, status);
    }

    public static float Damage (Skill skill, DmgSkill dmgSkill, GameMonster caster, GameMonster target) {
        float dmg = dmgSkill.baseDmg;

        float attackStatus = GetStatusValue(skill.attackStatus, caster);
        float defenseStatus = GetStatusValue(skill.defenseStatus, target);

        dmg *= attackStatus / defenseStatus;

        /*
        if (skill.isSpecial) {
            dmg *= ((caster.actualFocus + caster.BonusFocus) / (target.actualSpcDefense + target.BonusSpcDefense));
        }
        else {
            dmg *= ((caster.actualVigor + caster.BonusVigor) / (target.actualWisdom + target.BonusWisdom));
        }
        */

        if (skill.skillElement == caster.monsterElement) {
            dmg *= 1.25f;
        }
        dmg = Mathf.Max(dmg, 2);
        if (GlobalData.i.SuperEffective(skill.skillElement, target.monsterElement)) {
            dmg *= 1.5f;
        }
        else if (GlobalData.i.LessEffective (skill.skillElement, target.monsterElement)) {
            dmg *= 0.66f;
        }


        if (skill.extraEffect != ExtraEffect.ignoreArmor) {
            if (skill.attackStatus == StatusType.wisdom || skill.attackStatus == StatusType.focus) {
                dmg -= target.Resistance;
            }
            else if (skill.attackStatus == StatusType.vigor || skill.attackStatus == StatusType.agility) {
                dmg -= target.Block;
            }
        }

        target.TakeHit(skill.isSpecial, caster);
        dmg = Mathf.Max(dmg, 0);
        return dmg;
    }
}