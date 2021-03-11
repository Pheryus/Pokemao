using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlefield : MonoBehaviour
{
    public GameObject[] playerPos, enemyPos;

    public BattleController battleController;

    public List<MonsterSO> startPlayerMonsters;

    public List<GameMonster> startEnemyMonsters;

    public List<ActiveMonster> playerActiveMonsters = new List<ActiveMonster>();
    public List<ActiveMonster> enemyActiveMonsters = new List<ActiveMonster>();


    public EnemiesUI battleUI;

    public GameObject monsterPrefab;

    public PlayerParty playerParty;

    public GameObject battleScene;

    public EnemyLeader enemyLeader;

    private void Start() {
        BattleAction.startBattle += StartBattle;
    }

    public void StartBattle() {
        CreateEnemies();
        SetupPlayerMonsters();
        for (int i = 0; i < playerActiveMonsters.Count; i++) {
            battleUI.SetupMonster(i, playerActiveMonsters[i], playerMonster: true);
        }
        for (int i = 0; i < enemyActiveMonsters.Count; i++) {
            battleUI.SetupMonster(i, enemyActiveMonsters[i], playerMonster: false);
        }
        battleUI.SetupMonstersUI();
    }

    void SetupPlayerMonsters() {
        battleController.battlefield.playerActiveMonsters = new List<ActiveMonster>();
        for (int i = 0; i < 2; i++) {
            SetNewPlayerMonster(i, i);
        }
    }

    public void SetNewPlayerMonster (int i, int fieldPosition) {
        playerParty.playerParty[i].actualGO.transform.position = playerPos[fieldPosition].transform.position;
        playerParty.playerParty[i].actualGO.transform.position = playerPos[fieldPosition].transform.position;

        AddPlayerMonsterToBattlefield(playerParty.playerParty[i], fieldPosition);
        
        playerParty.playerParty[i].actualGO.transform.SetParent(battleScene.transform);
    }

    void CreateEnemies() {
        battleController.battlefield.enemyActiveMonsters = new List<ActiveMonster>();
        CreateEnemyMonster(startEnemyMonsters[0], 0);
        if (startEnemyMonsters.Count > 1) { 
            CreateEnemyMonster(startEnemyMonsters[1], 1);
        }
    }

    public void ChangeEnemyMonsterOnBattlefield(GameMonster monster, int i, int startTeamPosition = -1) {
        CreateEnemyMonster(monster, i, startTeamPosition);
        enemyActiveMonsters[i].atb = 15f;
        battleUI.SetupMonster(i, enemyActiveMonsters[i], playerMonster: false);
    }

    public void CreateEnemyMonster (GameMonster monster, int i, int startTeamPosition = -1) {
        GameObject go = Instantiate(monsterPrefab, enemyPos[i].transform.position, Quaternion.Euler(0, 0, 0));
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = monster.sprite;
        monster.actualGO = go;
        AddEnemyMonsterToBattlefield(monster, sr, i, startTeamPosition: startTeamPosition);
        go.transform.SetParent(battleScene.transform);
    }

    public void AddPlayerMonsterToBattlefield(GameMonster m, int fieldPosition = -1) {
        ActiveMonster newActiveMonster = new ActiveMonster(m);
        newActiveMonster.playerMonster = true;
        newActiveMonster.spriteRenderer = newActiveMonster.gameMonster.actualGO.GetComponent<SpriteRenderer>();
        newActiveMonster.gameMonster.actualGO.SetActive(true);

        if (fieldPosition != -1) {
            newActiveMonster.teamPosition = playerActiveMonsters.Count;
            playerActiveMonsters.Insert(fieldPosition, newActiveMonster);
        }
        else {
            playerActiveMonsters.Add(newActiveMonster);
        }
    }

    public void AddEnemyMonsterToBattlefield(GameMonster m, SpriteRenderer sr, int fieldPosition = -1, int startTeamPosition = -1) {

        ActiveMonster newActiveMonster = new ActiveMonster(m);

        newActiveMonster.spriteRenderer = sr;
        if (fieldPosition != -1) {
            enemyActiveMonsters.Insert(fieldPosition, newActiveMonster);
        }
        else {
            enemyActiveMonsters.Add(newActiveMonster);
        }
        if (startTeamPosition != -1) {
            newActiveMonster.teamPosition = startTeamPosition;
        }
        
    }

    public ActiveMonster GetRandomPlayerMonsterTarget() {
        ActiveMonster m = null;
        while (m == null) {
            m = playerActiveMonsters[UnityEngine.Random.Range(0, playerActiveMonsters.Count)];
        }
        return m;
    }

    public void UpdateATBBar() {
        foreach (ActiveMonster m in playerActiveMonsters) {
            if (m.skill) {
                m.atb += Time.deltaTime * BattleController.ATBBARSPEED * m.skill.skillSpeed;
            }
            else {
                m.atb += m.gameMonster.actualAgility * Time.deltaTime * BattleController.ATBBARSPEED;
            }
        }

        foreach (ActiveMonster m in enemyActiveMonsters) {
            if (m.skill) {
                m.atb += Time.deltaTime * BattleController.ATBBARSPEED * m.skill.skillSpeed;
            }
            else {
                m.atb += m.gameMonster.actualAgility * Time.deltaTime * BattleController.ATBBARSPEED;
            }
        }
    }

    public void DisableMonstersSprites() {
        foreach (ActiveMonster m in playerActiveMonsters) {
            m.gameMonster.actualGO.SetActive(false);
        }
    }

    public void EndCombat() {
        foreach (ActiveMonster m in playerActiveMonsters) {
            m.gameMonster.actualEffects.Clear();
        }
        DisableMonstersSprites();
    }

    public void RemoveMonsterFromPlayerField(int fieldToChange) {
        playerActiveMonsters[fieldToChange].gameMonster.actualGO.SetActive(false);
        playerActiveMonsters.RemoveAt(fieldToChange);
    }
}



