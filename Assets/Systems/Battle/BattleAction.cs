using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleAction
{

    public static Action newTurn, takeDmg, takeDmg2, skillEffect, endBattle, gainEffect;
    public static Action startBattle, winBattle, loseBattle, startATB, playerUseTurn, monsterEffectUpdate, allyKO;

    public static Action closeTradeMonster, notEnoughMana, finishWarning, cancelNewSkill, gotReward, gainReward, isSuperEffective, isLessEffective;

    public static Action healParty;

    public static void ResetActions() {
        newTurn = null;
        takeDmg = null;
        takeDmg2 = null;
        skillEffect = null;
        endBattle = null;
        gainEffect = null;
        startBattle = null;
        winBattle = null;
        loseBattle = null;
        startATB = null;
        playerUseTurn = null;
        monsterEffectUpdate = null;
        allyKO = null;
        closeTradeMonster = null;
        notEnoughMana = finishWarning = cancelNewSkill = gotReward = null;
        gainReward = isSuperEffective = isLessEffective = healParty = null;
    }
}
