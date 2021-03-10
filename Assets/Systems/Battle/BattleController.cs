using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleController : MonoBehaviour {

    public Battlefield battlefield;

    public const float ATBMIN = 30, ATBBARSPEED = 1;

    bool runningATP;

    public ActiveMonster activePlayerMonster;

    public enum State { running, loseBattle, winBattle};

    public State battleState;

    public PlayerParty playerParty;

    public SkillResolution skillResolution;

    public RewardControl rewardControl;

    List<GameMonster> defeatedEnemies = new List<GameMonster>();

    public GameObject battleScene;

    public EnemiesUI enemiesUI;

    public ChangeTeamUI changeTeamUI;

    public EnemyLeader enemyLeader;

    public void Start() {
        BattleAction.startATB += StartATB;
        BattleAction.endBattle += EndBattle;
    }

    public void StartATB() {
        battleState = State.running;
        foreach (ActiveMonster m in battlefield.playerActiveMonsters) {
            m.atb = UnityEngine.Random.Range(0, 15);
        }
        foreach (ActiveMonster m in battlefield.enemyActiveMonsters) {
            m.atb = UnityEngine.Random.Range(0, 15);
        }

        if (PlayerResources.instance.HasArtefact(Artefact.ArtefactEnum.initiative)) {
            battlefield.playerActiveMonsters[0].atb = ATBMIN / 2;
        }

        runningATP = true;
    }

    public void EndBattle() {
        runningATP = false;
        battleScene.SetActive(false);        
        battlefield.EndCombat();
        rewardControl.GiveReward(defeatedEnemies);
        defeatedEnemies.Clear();

        //SelectedOption.instance.UpdateMapOption();
    }

    public void SpendTurn() {
        runningATP = true;
        activePlayerMonster.atb = 0;
    }

    public void CaptureMonster(int monsterIndex) {
        playerParty.GainMonster(new GameMonster(battlefield.enemyActiveMonsters[monsterIndex].gameMonster));
        battlefield.enemyActiveMonsters[monsterIndex].gameMonster.dead = true;
        StartCoroutine(CheckDeads());
    }

    IEnumerator UseSkill(Skill skill, ActiveMonster caster, ActiveMonster target) {

        yield return null;

        BattleMsg.instance.StartAttackMessage(caster.gameMonster, skill);

        while (BattleMsg.instance.state != BattleMsg.State.none) {
            yield return null;
        }

        BattleSnapshot skillCast = new BattleSnapshot(skill, caster, target, battlefield.playerActiveMonsters, battlefield.enemyActiveMonsters);

        yield return StartCoroutine(skillResolution.CastSkill(skillCast));

        yield return StartCoroutine(CheckDeads());
        
        yield return null;
    }

    IEnumerator LockMessage() {
        while (!BattleMsg.instance.SkipMessage) {
            yield return null;
        }
    }

    public void PlayerMonsterStartChannel(int skillID, ActiveMonster target) {
        GameMonster playerMonsterSO = activePlayerMonster.gameMonster;
        activePlayerMonster.skill = playerMonsterSO.baseSkills[skillID];
        activePlayerMonster.skillTarget = target;
        activePlayerMonster.atb = 0;
        
        if (activePlayerMonster.skill.skillSpeed == 0) {
            StartCoroutine(PlayerSkill());
        }
        else {
            runningATP = true;
        }
    }

    IEnumerator AllyMonsterDie(int i) {
        GameMonster defeatedMonster = battlefield.playerActiveMonsters[i].gameMonster;
        defeatedMonster.actualGO.SetActive(false);
        defeatedMonster.actualGO.transform.SetParent(null);
        battlefield.playerActiveMonsters.RemoveAt(i);

        BattleMsg.instance.AllyDefeateadMessage(defeatedMonster);

        yield return StartCoroutine(LockMessage());

        if (playerParty.HasAvailableMonster()) {

            BattleMsg.instance.ChooseNewAlly();

            yield return StartCoroutine(LockMessage());
            battleScene.SetActive(false);
            changeTeamUI.SelectMonsterToSubstitute(defeatedMonster, i);

            while (battlefield.playerActiveMonsters.Count == 1) {

                yield return null;
            }
            enemiesUI.SetupMonster(i, battlefield.playerActiveMonsters[i]);
            enemiesUI.UpdateActiveMonstersUI();
            battlefield.playerActiveMonsters[i].atb = 0.5f;
        }

    }

    IEnumerator WildMonsterDied(ActiveMonster deadMonster, int i) {
        if (deadMonster.gameMonster.Fainted) { 
            defeatedEnemies.Add(deadMonster.gameMonster);
        }
        Destroy(deadMonster.gameMonster.actualGO);
        battlefield.enemyActiveMonsters.RemoveAt(i);
        yield return null;
    }

    IEnumerator TeamMonsterDied(ActiveMonster deadMonster, int i) {
        Debug.Log("team monster died");
        Destroy(deadMonster.gameMonster.actualGO);
        battlefield.enemyActiveMonsters.RemoveAt(i);

        BattleMsg.instance.OpponentDefeatedMessage(deadMonster.gameMonster);
        yield return StartCoroutine(LockMessage());

        if (enemyLeader.HasAvailableMonster()) {
            BattleMsg.instance.OpponentSelectingMonster(deadMonster.gameMonster);
            yield return StartCoroutine(LockMessage());
            int monsterTeamPosition = enemyLeader.ChooseNewMonsterTeamPosition();
            Debug.Log("monster team Position: " + monsterTeamPosition);
            GameMonster gm = enemyLeader.GetMonster(monsterTeamPosition);
            battlefield.ChangeEnemyMonsterOnBattlefield(gm, i, startTeamPosition: monsterTeamPosition);

            EnemiesUI.instance.CheckUI();
            yield return new WaitForSeconds(0.5f);
            BattleMsg.instance.OpponentInvokesMonster(deadMonster.gameMonster);
            yield return StartCoroutine(LockMessage());
        }
        else {
            Debug.Log("não tem mais monstro");
        }

    }

    IEnumerator CheckDeads() {
        for (int i = battlefield.enemyActiveMonsters.Count - 1; i >= 0; i--) {
            ActiveMonster deadMonster = battlefield.enemyActiveMonsters[i];
            if (deadMonster.gameMonster.actualHp <= 0 ||deadMonster.gameMonster.dead) {
                if (!enemyLeader.teamBattle) {
                    yield return StartCoroutine(WildMonsterDied(deadMonster, i));
                }
                else {
                    yield return StartCoroutine(TeamMonsterDied(deadMonster, i));
                }
            }
            else {
                deadMonster.gameMonster.dead = false;
            }
        }

        for (int i = battlefield.playerActiveMonsters.Count - 1; i >= 0; i--) {
            if (battlefield.playerActiveMonsters[i].gameMonster.dead) {
                yield return StartCoroutine(AllyMonsterDie(i));
            }
        }

        if (battlefield.playerActiveMonsters.Count == 0) {
            BattleAction.loseBattle?.Invoke();
            battleState = State.loseBattle;
            runningATP = false;
        }
        else if (battlefield.enemyActiveMonsters.Count == 0) {
            WinBattle();
        }
        else {
            EnemiesUI.instance.CheckUI();
        }
        

        yield return null;
    }

    void WinBattle() {
        EnemiesUI.instance.CheckUI();
        runningATP = false;
        BattleAction.winBattle?.Invoke();
        battleState = State.winBattle;
    }

    public IEnumerator PlayerSkill () {
        runningATP = false;
        GameMonster playerMonsterSO = activePlayerMonster.gameMonster;
        ActiveMonster target = activePlayerMonster.skillTarget;

        playerMonsterSO.actualMana -= activePlayerMonster.skill.manaCost;
        yield return enemiesUI.StartCoroutine(enemiesUI.UpdateMonsterBar(hpBar: false, activePlayerMonster, playerMonsterSO.pctMana));
        
        yield return StartCoroutine(UseSkill(activePlayerMonster.skill, caster: activePlayerMonster, target: target));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(EndOfTurnEffects(activePlayerMonster));
        activePlayerMonster.ResetSkill();
        activePlayerMonster.atb = 0;
        runningATP = true;
    }

    private IEnumerator EndOfTurnEffects (ActiveMonster monster) {
        
        foreach (Effect effect in monster.gameMonster.actualEffects) {
            EffectResolution.EndOfTurnEffect(monster, effect);
        }

        for (int i = monster.gameMonster.actualEffects.Count - 1; i >= 0; i--) {
            if (monster.gameMonster.actualEffects[i].intensity <= 0) {
                monster.gameMonster.actualEffects.RemoveAt(i);
            }
        }
        BattleAction.monsterEffectUpdate?.Invoke();
        CheckDeads();
        yield return null;
    }

    public IEnumerator EnemyTurn(ActiveMonster monster) {
        runningATP = false;
        Skill enemySkill = BattleGlobal.EnemySelectSkill(monster.gameMonster);
        ActiveMonster target = battlefield.GetRandomPlayerMonsterTarget();

        monster.skill = enemySkill;
        monster.skillTarget = target;
        monster.atb = 0;
        if (monster.skill.skillSpeed == 0) {
            yield return StartCoroutine(EnemyCastSkill(monster));
        }
        yield return StartCoroutine(EndOfTurnEffects(monster));
        //BattleMsg.instance.ChooseNewAlly();

        runningATP = true;
    }

    IEnumerator EnemyCastSkill(ActiveMonster am) {
        runningATP = false;
        am.atb = 0;
        yield return StartCoroutine(UseSkill(am.skill, caster: am, am.skillTarget));
        am.ResetSkill();
        yield return new WaitForSeconds(1f);
        runningATP = true;
    }

    void CheckNewTurn() {
        foreach (ActiveMonster m in battlefield.playerActiveMonsters) {
            if (m.skill == null) { 
                if (m.atb >= ATBMIN) {
                    runningATP = false;
                    activePlayerMonster = m;
                    BattleAction.newTurn?.Invoke();
                    return;
                }
            }
            else if (m.atb >= ATBMIN) {
                StartCoroutine(PlayerSkill());
            }
        }
        foreach (ActiveMonster m in battlefield.enemyActiveMonsters) {
            if (m.skill == null) { 
                if (m.atb >= ATBMIN) {
                    StartCoroutine(EnemyTurn(m));
                    return;
                }
            }
            else if (m.atb >= ATBMIN) {
                StartCoroutine(EnemyCastSkill(m));
                return;
            }
        }
    }
    private void Update() {
        if (runningATP && battleState == State.running) {
            battlefield.UpdateATBBar();
            CheckNewTurn();
        }
    }

}
