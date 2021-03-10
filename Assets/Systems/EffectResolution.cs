using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectResolution {
   
    public static void EndOfTurnEffect (ActiveMonster monster, Effect effect) {
        EnemiesUI en = EnemiesUI.instance;
        switch (effect.effectType) {
            case Effect.EffectType.burn:
                monster.gameMonster.actualHp -= effect.intensity;
                en.StartCoroutine(en.UpdateMonsterBar(hpBar: true, monster, monster.gameMonster.pctHp));
                effect.intensity--;
                break;

            case Effect.EffectType.regenerate:
                monster.gameMonster.actualHp += effect.intensity;
                en.StartCoroutine(en.UpdateMonsterBar(hpBar: true, monster, monster.gameMonster.pctHp));
                effect.intensity--;
                break;

            case Effect.EffectType.poison:
                monster.gameMonster.actualHp -= effect.intensity;
                en.StartCoroutine(en.UpdateMonsterBar(hpBar: true, monster, monster.gameMonster.pctHp));
                effect.intensity--;
                break;
        }
    }


}
