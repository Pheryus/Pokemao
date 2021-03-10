using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleMsg : MonoBehaviour {
    public TextMeshProUGUI msg;

    public BattleController battleController;

    public GameObject hud;

    public enum State { none, newEncounter, hitDescription, hitDescription2, endMessage, winBattle, loseBattle, warning, infoMessage};

    public State state;

    public GameInput input;

    public static BattleMsg instance;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        BattleAction.startBattle += NewEncounterMessage;
        BattleAction.loseBattle += LoseMessage;
        BattleAction.winBattle += WinMessage;
        BattleAction.notEnoughMana += NotEnoughMana;
        BattleAction.isSuperEffective += SuperEffectiveMessage;
        BattleAction.isLessEffective += LessEffectiveMessage;
    }

    public void NotEnoughMana() {
        msg.text = "Not enought mana.";
        state = State.warning;
    }

    public void GainEffectMessage(string name, string effectDescription) {
        hud.SetActive(true);
        msg.text = name + effectDescription;
        state = State.infoMessage;
    }

    public void NewEncounterMessage() {
        hud.SetActive(true);
        //msg.text = "A " + battleController.enemyMonster.monsterName + " appears!";
        state = State.newEncounter;
    }

    public void LessEffectiveMessage() {
        hud.SetActive(true);
        msg.text = "It's ineffective...";
        state = State.infoMessage;
    }

    public void SuperEffectiveMessage() {
        hud.SetActive(true);
        msg.text = "It's super effective!";
        state = State.infoMessage;
    }

    public void AllyDefeateadMessage(GameMonster monster) {
        hud.SetActive(true);
        msg.text = "Your " + monster.monsterName + " fainted.";
        state = State.infoMessage;
    }

    public void OpponentDefeatedMessage (GameMonster monster) {
        hud.SetActive(true);
        state = State.infoMessage;
        msg.text = monster.monsterName + " fainted.";
    }

    public void OpponentSelectingMonster (GameMonster monster) {
        hud.SetActive(true);
        state = State.infoMessage;
        msg.text = "Your opponent is choosing a new monster";
    }

    public void OpponentInvokesMonster (GameMonster newMonster) {
        hud.SetActive(true);
        state = State.infoMessage;
        msg.text = newMonster + " enters the battlefield!";
    }

    void LoseMessage() {
        state = State.loseBattle;
        msg.text = "All your monsters died... Game Over :(";
    }

    public bool SkipMessage {
        get {
            return state == State.none;
        }
    }

    public void ChooseNewAlly() {
        state = State.infoMessage;
        msg.text = "Choose a new monster to battle:";
    }

    void WinMessage() {
        state = State.winBattle;
        msg.text = "Victory! G00d b0y!";
    }

    public void StartAttackMessage(GameMonster monster, Skill skill) {
        msg.text = monster.monsterName + " used " + skill.skillName + "!";
        state = State.hitDescription;
    }

    public void EffectAttackMessage (GameMonster monster, Skill skill) {

    }

    private void Update() {

        if (input.lastInput == BattleInput.InputEnum.confirm && state != State.none) {
            input.lastInput = BattleInput.InputEnum.none;
            if (state == State.newEncounter) {
                BattleAction.startATB?.Invoke();
            }
            if (state == State.winBattle) { 
                BattleAction.endBattle?.Invoke();
            }
            else if (state == State.loseBattle) {
                SceneManager.LoadScene("Map");
                return;
            }
            else if (state == State.warning) {
                BattleAction.finishWarning?.Invoke();
                msg.text = string.Empty;
            }
            else if (state == State.infoMessage) {
                msg.text = string.Empty;
            }


            if (state != State.none) {
                state = State.none;
                msg.text = string.Empty;
            }

        }
        
    }
}
