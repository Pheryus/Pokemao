using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleGlobal  {

    public static bool PlayerActsFirst (GameMonster player, GameMonster enemy) {
        float v = player.actualAgility + enemy.actualAgility;
        return UnityEngine.Random.Range(0, v) < player.actualAgility;
    }

    public static Skill EnemySelectSkill (GameMonster enemy) {
        int max = enemy.baseSkills.Length;
        return enemy.baseSkills[UnityEngine.Random.Range(0, max)];
    }

    public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB) {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
        return list;
    }

    public static bool CaptureWasSuccesful (GameMonster target) {
        return true;
        float hpPCT = (target.baseHp * 3 - target.actualHp * 2) / (target.baseHp * 3);

        return UnityEngine.Random.Range(0, 1f) < hpPCT * target.catchRate;
    }
}
