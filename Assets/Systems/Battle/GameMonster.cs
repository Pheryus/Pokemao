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
            float haste = 1 + Effect.IntensityFrom(actualEffects, Effect.EffectType.channelSpeed) / 100;
            float slow = 1 + Effect.IntensityFrom(actualEffects, Effect.EffectType.channelSlow) / 100;
            return ((TotalFocus / 10) * (haste / slow));
        }
    }

    public float ActionSpeed {
        get {
            float haste = 1 + Effect.IntensityFrom(actualEffects, Effect.EffectType.haste) / 100;
            float slow = 1 + Effect.IntensityFrom(actualEffects, Effect.EffectType.slow) / 100;
            return ((TotalAgility) * (haste / slow));
        }
    }

    public float actualVigor, actualWisdom, actualFocus, actualSpcDefense, actualAgility, actualDodge, actualACC;

    int _actualHp;

    public int monsterLevel;

    public void LevelUpMonster (int levels) {
        
        for (int i = 0; i < levels; i++) {
            int stat = UnityEngine.Random.Range(0, 4 + mainStats.Length);
            MonsterStat statToUpgrade;
            if (stat > 4) {
                statToUpgrade = mainStats[stat - 4];
            }
            else {
                statToUpgrade = (MonsterStat)stat;
            }
            UpgradeStatus(statToUpgrade, 1);
        }
    }

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
            return (int) Effect.IntensityFrom(actualEffects, Effect.EffectType.block);
        }
    }

    public int BonusVigor {
        get {
            return (int) Effect.IntensityFrom(actualEffects, Effect.EffectType.vigorBuff);
        }    
    }

    float EffectIntensity(Effect effect) {
        float value = effect.intensity;
        effect.intensity--;
        if (effect.intensity == 0) {
            actualEffects.Remove(effect);
            BattleAction.monsterEffectUpdate?.Invoke();
        }
        return value;
    }

    public int BonusWisdom {
        get {
            return (int) Effect.IntensityFrom(actualEffects, Effect.EffectType.wisdomBuff);
        }
    }

    public int BonusFocus {
        get {
            return (int) Effect.IntensityFrom(actualEffects, Effect.EffectType.focusBuff);
        }
    }

    public int BonusAgility {
        get {
            return (int) Effect.IntensityFrom(actualEffects, Effect.EffectType.agilityBuff);
        }
    }

    public int Resistance {
        get {
            return (int) Effect.IntensityFrom(actualEffects, Effect.EffectType.resistance);
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

        if (effect.effectType == Effect.EffectType.cancelChannel) {
            return;
        }

        foreach (Effect actualEffect in actualEffects) {
            if (actualEffect.effectType == effect.effectType) {
                actualEffect.intensity += effect.intensity;
                return;
            }
        }
        actualEffects.Add(new Effect(effect, gainThisTurn));

        if (effect.effectType == Effect.EffectType.haste || effect.effectType == Effect.EffectType.slow
            || effect.effectType == Effect.EffectType.channelSpeed || effect.effectType == Effect.EffectType.channelSlow) {
            ResolveConflictingEffects();
        }
        BattleAction.monsterEffectUpdate?.Invoke();
    }

    void ResolveConflictingEffects() {
        Effect haste = actualEffects.Find(t => t.effectType == Effect.EffectType.haste);
        Effect slow = actualEffects.Find(t => t.effectType == Effect.EffectType.slow);
        Effect channelHaste = actualEffects.Find(t => t.effectType == Effect.EffectType.channelSpeed);
        Effect channelSlow = actualEffects.Find(t => t.effectType == Effect.EffectType.channelSlow);
        if (haste != null && slow != null) {
            if (haste.intensity > slow.intensity) {
                haste.intensity /= slow.intensity;
                actualEffects.Remove(slow);
            }
            else if (haste.intensity < slow.intensity){
                slow.intensity /= haste.intensity;
                actualEffects.Remove(haste);
            }
            else {
                actualEffects.Remove(haste);
                actualEffects.Remove(slow);
            }
        }
        if (channelHaste != null && channelSlow != null) {
            if (channelHaste.intensity > channelSlow.intensity) {
                channelHaste.intensity /= channelSlow.intensity;
                actualEffects.Remove(channelSlow);
            }
            else if (channelHaste.intensity < channelSlow.intensity) {
                channelSlow.intensity /= channelHaste.intensity;
                actualEffects.Remove(channelHaste);
            }
            else {
                actualEffects.Remove(channelHaste);
                actualEffects.Remove(channelSlow);
            }
        }
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
                baseVigor += amount;
                break;
            case MonsterStat.wisdom:
                actualWisdom += amount;
                baseWisdom += amount;
                break;
            case MonsterStat.focus:
                actualFocus += amount;
                baseFocus += amount;
                break;
            case MonsterStat.agility:
                actualAgility += amount;
                baseAgility += amount;
                break;
        }
        monsterLevel ++;

    }
}
