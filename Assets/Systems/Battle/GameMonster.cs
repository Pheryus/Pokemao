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

    public float actualAttack, actualDefense, actualSpcAttack, actualSpcDefense, actualSpeed, actualDodge, actualACC;

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

    public int Armor {
        get {
            foreach (Effect effect in actualEffects) {
                if (effect.effectType == Effect.EffectType.block) {
                    return effect.intensity;
                }
            }
            return 0;
        }
    }

    public int BonusAttack {
        get {
            Effect ef = actualEffects.Find(eff => eff.effectType == Effect.EffectType.attackBuff);
            int value = 0;

            if (ef != null) {
                value = ef.intensity;
                ef.intensity--;
                if (ef.intensity == 0) {
                    actualEffects.Remove(ef);
                    BattleAction.monsterEffectUpdate?.Invoke();
                }
            }
            return value;
        }    
    }

    public int BonusDefense {
        get {
            Effect ef = actualEffects.Find(eff => eff.effectType == Effect.EffectType.defenseBuff);
            return ef != null ? ef.intensity : 0;
        }
    }

    public int BonusSpcAttack {
        get {
            Effect ef = actualEffects.Find(eff => eff.effectType == Effect.EffectType.spcAttackBuff);
            return ef != null ? ef.intensity : 0;
        }
    }

    public int BonusSpcDefense {
        get {
            Effect ef = actualEffects.Find(eff => eff.effectType == Effect.EffectType.spcDefenseBuff);
            return ef != null ? ef.intensity : 0;
        }
    }

    public int BonusSpeed {
        get {
            Effect ef = actualEffects.Find(eff => eff.effectType == Effect.EffectType.speedBuff);
            return ef != null ? ef.intensity : 0;
        }
    }

    public int SpcArmor {
        get {
            foreach (Effect effect in actualEffects) {
                if (effect.effectType == Effect.EffectType.magicBlock) {
                    return effect.intensity;
                }
            }
            return 0;
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
            if (isSpc && effect.effectType == Effect.EffectType.magicBlock || (!isSpc && effect.effectType == Effect.EffectType.block)) {
                effect.intensity--;
            }
            if (effect.effectType == Effect.EffectType.burnOnContact && !isSpc) {
                attacker.AddEffect(new Effect(Effect.EffectType.burn, effect.intensity));
            }
        }
    }

    public bool dead;

    public List<Effect> actualEffects;

    public void AddEffect (Effect effect) {
        Debug.Log(monsterName + " ganhou " + effect.effectType);
        foreach (Effect actualEffect in actualEffects) {
            if (actualEffect.effectType == effect.effectType) {
                actualEffect.intensity += effect.intensity;
                Debug.Log("intensidade: " + effect.intensity);
                BattleAction.monsterEffectUpdate?.Invoke();
                return;
            }
        }
        actualEffects.Add(new Effect(effect));
    }

    public void RestoreHp(int amount) {
        actualHp += amount;
    }

    public void RestoreMp(int amount) {
        actualMana += amount;
    }

    public GameMonster(int baseHp, float baseAttack, float baseDefense, float baseSpcAttack, float baseSpcDefense, float baseSpeed, Skill[] skills, string monsterName, Sprite sprite, float baseACC, float baseDodge, Element ele, int baseMana, MonsterPassive passive) {
        actualHp = this.baseHp = baseHp * 5 + 20;
        actualAttack = this.baseAttack = baseAttack;
        actualDefense = this.baseDefense = baseDefense;
        actualSpcAttack = this.baseSpcAttack = baseSpcAttack;
        actualSpcDefense = this.baseSpcDefense = baseSpcDefense;
        actualDodge = this.baseDodge = baseDodge;
        actualACC = this.baseBonusACC = baseACC;

        actualSpeed = this.baseSpeed = baseSpeed;
        this.baseSkills = new Skill[skills.Length];
        
        for (int i = 0; i < skills.Length; i++) {
            baseSkills[i] = skills[i];
        }
        
        this.monsterName = monsterName;
        this.sprite = sprite;
        actualEffects = new List<Effect>();
        monsterElement = ele;
        actualMana = this.baseMana = baseMana * 2 + 5;
        this.passive = passive;
    }

    public GameMonster (GameMonster monster) {
        baseHp = monster.baseHp;
        actualHp = monster.actualHp;
        actualAttack = baseAttack = monster.actualAttack;
        actualDefense = baseDefense = monster.actualDefense;
        actualSpcAttack = baseSpcAttack = monster.actualSpcAttack;
        actualSpcDefense = baseSpcDefense =  monster.actualSpcDefense;
        actualSpeed = baseSpeed = monster.baseSpeed;
        baseSkills = monster.baseSkills;
        monsterName = monster.monsterName;
        sprite = monster.sprite;
        actualACC = monster.actualACC;
        actualDodge = monster.actualDodge;
        monsterElement = monster.monsterElement;
        actualMana = baseMana = monster.baseMana;
    }

    public GameMonster(MonsterSO monsterSO) : this(monsterSO.baseHp, monsterSO.baseAttack,
        monsterSO.baseDefense, monsterSO.baseSpcAttack, monsterSO.baseSpcDefense, monsterSO.baseSpeed, monsterSO.baseSkills, monsterSO.monsterName, monsterSO.sprite, monsterSO.baseDodge, monsterSO.baseBonusACC, monsterSO.monsterElement, monsterSO.baseMana, monsterSO.passive) { }

    public void UpgradeStatus(MonsterStat statToGain, int amount) {
        switch (statToGain) {
            case MonsterStat.atk:
                actualAttack += amount;
                break;
            case MonsterStat.def:
                actualDefense += amount;
                break;
            case MonsterStat.spcAtk:
                actualSpcAttack += amount;
                break;
            case MonsterStat.spcDef:
                actualSpcDefense += amount;
                break;
            case MonsterStat.speed:
                actualSpcDefense += amount;
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
