using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLeader : MonoBehaviour {

    public List<GameMonster> monsters;

    public BattleController battleController;

    public Battlefield battlefield;

    public bool teamBattle;

    public int MonstersAlive {
        get {
            int n = 0;
            foreach (GameMonster gm in monsters) {
                if (!gm.dead) {
                    n++;
                }
            }
            return n;
        }
    }

    public int MonstersDead {
        get {
            return monsters.Count - MonstersAlive;
        }
    }

    public bool HasAvailableMonster () {

        for (int i = 2; i < monsters.Count; i++) {
            for (int j = 0; j < battleController.battlefield.enemyActiveMonsters.Count; j++) {
                if (battlefield.enemyActiveMonsters[j].teamPosition != i && !monsters[i].dead) {
                    return true;
                }
            }
        }
        return false;
    }

    public GameMonster GetMonster (int teamPosition) {
        return monsters[teamPosition];
    }

    public int ChooseNewMonsterTeamPosition() {
        List<int> possibleMonsters = new List<int>();
        for (int i = 0; i < monsters.Count; i++) {
            for (int j = 0; j < battleController.battlefield.enemyActiveMonsters.Count; j++) {
                if (battlefield.enemyActiveMonsters[j].teamPosition != i && !monsters[i].dead) {
                    possibleMonsters.Add(i);
                }
            }
     
        }
        return possibleMonsters[Random.Range(0, possibleMonsters.Count)];
    }

    public void SetTeam (TeamEncounterSO team) {
        monsters = new List<GameMonster>();
        int i = 0;
        foreach (MonsterSO m in team.team) {
            monsters.Add(new GameMonster(m));
            if (i < 2) { 
                battlefield.startEnemyMonsters.Add(monsters[i]);
                i++;
            }
        }
        teamBattle = true;
    }
}
