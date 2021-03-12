using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameMonster : MonsterSO
{
    public GameObject actualGO;

    public float pctHp {
        get {
            if (baseHp == 0) {
                return 0;
            }
            return actualHp / (float)baseHp;
        }
    }

    public bool Fainted {
        get {
            return actualHp <= 0;
        }
    }
    public float pctMana {
        get {
            if (baseMana == 0) {
                return 0;
            }
            return actualMana / (float)baseMana;
        }
    }

    public float CastSpeed {
        get {
            float castMultiplier = 100 + Effect.IntensityFrom(actualEffects, Effect.EffectType.channelSpeed);
            return (TotalFocus / 10) * (castMultiplier / 100);
        }
    }

    public float ActionSpeed {
        get {
            float actionSpeedMultiplier = 100 + Effect.IntensityFrom(actualEffects, Effect.EffectType.haste);
            return TotalAgility * actionSpeedMultiplier / 100;
        }
    }

    public float actualVigor, actualWisdom, actualFocus, actualSpcDefense, actualAgility, actualDodge, actualACC;

    int _actualHp;

    public int actualHp {
        get {
            return _actualHp;
        }
        set {
            _actualHp = value;
            if (_actualHp <= 0) {
                dead = true;
                _actualHp = 0;
            }
            if (_actualHp > baseHp) {
                _actualHp = baseHp;
            }
        }
    }

    public float TotalAgility {
        get {
            return actualAgility + BonusAgility;
        }
    }

    public float TotalFocus {
        get {
            return actualFocus + BonusFocus;
        }
    }

    public int actualMana {
        get {
            return _actualMana;
        }
        set {
            _actualMana = value;
            if (_actualMana <= 0) {
                _actualMana = 0;
            }
            if (_actualMana > baseMana) {
                _actualMana = baseMana;
            }
        }
        
    }

    int _actualMana;

    public int Block {
        get {
            return Effect.IntensityFrom(actualEffects, Effect.EffectType.block);
        }
    }

    public int BonusVigor {
        get {
            return Effect.IntensityFrom(actualEffects, Effect.EffectType.vigorBuff);
        }    
    }

    int EffectIntensity(Effect effect) {
        int value = effect.intensity;
        effect.intensity--;
        if (effect.intensity == 0) {
            actualEffects.Remove(effect);
            BattleAction.monsterEffectUpdate?.Invoke();
        }
        return value;
    }

    public int BonusWisdom {
        get {
            return Effect.IntensityFrom(actualEffects, Effect.EffectType.wisdomBuff);
        }
    }

    public int BonusFocus {
        get {
            return Effect.IntensityFrom(actualEffects, Effect.EffectType.focusBuff);
        }
    }

    public int BonusAgility {
        get {
            return Effect.IntensityFrom(actualEffects, Effect.EffectType.agilityBuff);
        }
    }

    public int Resistance {
        get {
            return Effect.IntensityFrom(actualEffects, Effect.EffectType.resistance);
        }
    }

    public void EndBattleEffect() {
        if (dead) {
            actualHp = 1;
            dead = false;
        }
    }

    public void TakeHit(bool isSpc, GameMonster attacker) {
        foreach (Effect effect in actualEffects) {
            if (isSpc && effect.effectType == Effect.EffectType.resistance || (!isSpc && effect.effectType == Effect.EffectType.block)) {
                effect.intensity--;
            }
            if (effect.effectType == Effect.EffectType.burnOnContact && !isSpc) {
                attacker.AddEffect(new Effect(Effect.EffectType.burn, effect.intensity));
            }
        }
    }

    public bool dead;

    public List<Effect> actualEffects;

    public void AddEffect (Effect effect, bool gainThisTurn = false) {
        Debug.Log(monsterName + " ganhou " + effect.effectType);
        foreach (Effect actualEffect in actualEffects) {
            if (actualEffect.effectType == effect.effectType) {
                actualEffect.intensity += effect.intensity;
                Debug.Log("intensidade: " + effect.intensity);
                BattleAction.monsterEffectUpdate?.Invoke();
                return;
            }
        }

        actualEffects.Add(new Effect(effect, gainThisTurn));

    }

    public void RestoreHp(int amount) {
        actualHp += amount;
    }

    public void RestoreMp(int amount) {
        actualMana += amount;
    }

    public GameMonster(int baseHp, float baseVigor, float baseWisdom, float baseFocus, float baseSpcDefense, float baseAgility, Skill[] skills, string monsterName, Sprite sprite, float baseACC, float baseDodge, Element ele, int baseMana, MonsterPassive passive) {
        
        actualVigor = this.baseVigor = baseVigor;
        actualWisdom = this.baseWisdom = baseWisdom;
        actualFocus = this.baseFocus = baseFocus;
        actualSpcDefense = this.baseSpcDefense = baseSpcDefense;
        actualDodge = this.baseDodge = baseDodge;
        actualACC = this.baseBonusACC = baseACC;

        actualAgility = this.baseAgility = baseAgility;
        this.baseSkills = new Skill[skills.Length];
        
        for (int i = 0; i < skills.Length; i++) {
            baseSkills[i] = skills[i];
        }
        
        this.monsterName = monsterName;
        this.sprite = sprite;
        actualEffects = new List<Effect>();
        monsterElement = ele;
        actualHp = this.baseHp = (int) (baseHp + baseVigor * 2 + baseAgility);
        actualMana = this.baseMana = (int)(baseMana + baseWisdom + baseFocus / 2);
        this.passive = passive;
    }

    public GameMonster (GameMonster monster) {
        baseHp = monster.baseHp;
        actualHp = monster.actualHp;
        actualVigor = baseVigor = monster.actualVigor;
        actualWisdom = baseWisdom = monster.actualWisdom;
        actualFocus = baseFocus = monster.actualFocus;
        actualSpcDefense = baseSpcDefense =  monster.actualSpcDefense;
        actualAgility = baseAgility = monster.baseAgility;
        baseSkills = monster.baseSkills;
        monsterName = monster.monsterName;
        sprite = monster.sprite;
        actualACC = monster.actualACC;
        actualDodge = monster.actualDodge;
        monsterElement = monster.monsterElement;
        actualMana = baseMana = monster.baseMana;
    }

    public GameMonster(MonsterSO monsterSO) : this(monsterSO.baseHp, monsterSO.baseVigor,
        monsterSO.baseWisdom, monsterSO.baseFocus, monsterSO.baseSpcDefense, monsterSO.baseAgility, monsterSO.baseSkills, monsterSO.monsterName, monsterSO.sprite, monsterSO.baseDodge, monsterSO.baseBonusACC, monsterSO.monsterElement, monsterSO.baseMana, monsterSO.passive) { }

    public void UpgradeStatus(MonsterStat statToGain, int amount) {
        switch (statToGain) {
            case MonsterStat.vigor:
                actualVigor += amount;
                break;
            case MonsterStat.wisdom:
                actualWisdom += amount;
                break;
            case MonsterStat.focus:
                actualFocus += amount;
                break;
            case MonsterStat.spcDef:
                actualSpcDefense += amount;
                break;
            case MonsterStat.speed:
                actualAgility += amount;
                break;
            case MonsterStat.maxHP:
                actualHp += amount;
                baseHp += amount;
                break;
            case MonsterStat.maxMana:
                actualMana += amount;
                baseMana += amount;
                break;
        }
    }
}
