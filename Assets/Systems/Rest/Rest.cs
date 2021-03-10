using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rest : MonoBehaviour
{

    public PlayerParty pt;

    private void Start() {
        BattleAction.healParty += RestParty;    
    }

    void RestParty() {
        foreach (GameMonster gm in pt.playerParty) {
            gm.RestoreHp(20);
            gm.RestoreMp(5);
        }
    }


}
