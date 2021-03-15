using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParty : MonoBehaviour {
    public List<MonsterSO> startPlayerMonsters;

    public List<GameMonster> playerParty;
    public GameObject monsterPrefab;

    public static PlayerParty instance;

    public int MaxPartySize {
        get {
            return 5;
        }
    }

    public GameMonster RandomPlayerMonster {
        get {
            return playerParty[Random.Range(0, playerParty.Count)];
        }
    }

    public GameMonster LowerMPMonster {
        get {
            float lowMP = 1000;
            int id = -1;
            for (int i = 0; i < playerParty.Count; i++) {
                if (lowMP > playerParty[i].pctMana) {
                    lowMP = playerParty[i].pctMana;
                    id = i;
                }
            }
            return playerParty[id];
        }
    }

    public GameMonster LowerHPMonster {
        get {
            float lowHP = 1000;
            int id = -1;
            for (int i = 0; i < playerParty.Count; i++) {
                if (lowHP > playerParty[i].pctHp) {
                    lowHP = playerParty[i].pctHp;
                    id = i;
                }
            }
            return playerParty[id];
        }
    }

    public int MonstersAlive {
        get {
            int n = 0;
            foreach (GameMonster gm in playerParty) {
                if (!gm.dead) {
                    n++;
                }
            }
            return n;
        }
    }

    public int MonstersDead {
        get {
            return playerParty.Count - MonstersAlive;
        }
    } 
    
    public bool HasAvailableMonster() {
        for (int i = 2; i < playerParty.Count; i++) {
            if (!playerParty[i].dead) {
                return true;
            }
        }
        return false;
    }
    private void Start() {
        BattleAction.winBattle += WinBattleEffect;
        instance = this;
        int i = 0;
        foreach (MonsterSO m in startPlayerMonsters) {
            UpdatePlayerMonster(new GameMonster(m), i);
            i++;
        }
    }

    public void WinBattleEffect() {
        foreach (GameMonster gm in playerParty) {
            gm.EndBattleEffect();
        }
    }

    public void GainMonster (GameMonster newMonster) {
        UpdatePlayerMonster(newMonster, playerParty.Count);
    }

    public void UpdatePlayerMonster(GameMonster monster, int i) {
        GameObject go = Instantiate(monsterPrefab);
        go.SetActive(false);
        go.GetComponent<SpriteRenderer>().sprite = monster.sprite;
        monster.actualGO = go;
        playerParty.Add(monster);
    }


}
