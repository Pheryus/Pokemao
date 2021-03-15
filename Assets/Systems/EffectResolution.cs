using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectResolution {
   
    public static void EndOfTurnEffect (ActiveMonster monster, Effect effect) {
        EnemiesUI en = EnemiesUI.instance;
        switch (effect.effectType) {
            case Effect.EffectType.burn:
                monster.gameMonster.actualHp -= (int)effect.intensity;
                en.StartCoroutine(en.UpdateMonsterBar(hpBar: true, monster, monster.gameMonster.pctHp));
                effect.intensity--;
                break;

            case Effect.EffectType.regenerate:
                monster.gameMonster.actualHp += (int)effect.intensity;
                en.StartCoroutine(en.UpdateMonsterBar(hpBar: true, monster, monster.gameMonster.pctHp));
                effect.intensity--;
                break;

            case Effect.EffectType.poison:
                monster.gameMonster.actualHp -= (int)effect.intensity;
                en.StartCoroutine(en.UpdateMonsterBar(hpBar: true, monster, monster.gameMonster.pctHp));
                effect.intensity--;
                break;
        }

        if (effect.effectType == Effect.EffectType.vigorBuff || effect.effectType == Effect.EffectType.agilityBuff
            || effect.effectType == Effect.EffectType.focusBuff || effect.effectType == Effect.EffectType.wisdomBuff) {
            ReduceIntensity(effect, 1);
        }
        if (effect.effectType == Effect.EffectType.channelSpeed || effect.effectType == Effect.EffectType.haste) {
            ReduceIntensity(effect, 5);
        }
    }

    public static void ReduceIntensity(Effect effect, int amount) {
        if (effect.gainThisTurn) {
            effect.gainThisTurn = false;
            return;
        }
        if (effect.intensity < 0) {
            effect.intensity+= amount;
            if (effect.intensity > 0) {
                effect.intensity = 0;
            }
        }
        else if (effect.intensity > 0) {
            effect.intensity -= amount;

            if (effect.intensity < 0) {
                effect.intensity = 0;
            }
        }
    }


}
