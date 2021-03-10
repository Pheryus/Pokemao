using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {

    public Image[] skillImages;
    public Image[] actionImages;

    public Battlefield battlefield;

    public EnemyLeader enemyLeader;

    public TextMeshProUGUI[] opt;

    public GameObject actionInfo, manaCostGO, skillsGO, baseActionsGO;

    public TextMeshProUGUI actionEffect, manaCostText;

    public GameInput input;

    public BattleController battleController;

    enum Direction { leftup, rightup, leftdown, rightdown};

    Direction _selected;

    private Direction actionIndex {
        get {
            return _selected;
        }

        set {
            _selected = value;
            UpdateUIOptions();
        }
    }

    enum State { none, selectingAction, selectingSkill, selectingTarget, selectingNewMonster, capturing, waiting};

    State state;

    State previousState;

    public EnemiesUI enemiesUI;

    Skill selectedSkill;

    private int _targetIndex;

    public PlayerParty playerParty;

    Color selectedColor = new Color(1, 1, 1, 0.5f), confirmedColor = new Color(0.8f, 0, 0, 0.5f);

    int targetIndex {
        get {
            return _targetIndex;
        }

        set {
            _targetIndex = value;
            if (_targetIndex < 0) {
                _targetIndex = 0;
            }
        }
    }

    List<ActiveMonster> actualTargets;

    public ChangeTeamUI changeTeamUI;

    private void Start() {
        BattleAction.newTurn += SelectBattleAction;
        BattleAction.finishWarning += ReturnToBattleUI;
        input.Invoke("EnableReadInput", 0.2f);
    }

    public void ReturnToBattleUI() {
        state = previousState;
        switch (state) {
            case State.selectingSkill:
                skillsGO.SetActive(true);
                UpdateUIOptions();
                UpdateTargetSprite();
                break;
            case State.selectingAction:
                baseActionsGO.SetActive(true);
                break;
        }
    }

    public void SelectBattleAction() {
        state = State.selectingAction;
        actionIndex = 0;
        baseActionsGO.SetActive(true);
        skillsGO.SetActive(false);
        manaCostGO.SetActive(false);
    }

    public void SelectedSkillMenu() {
        baseActionsGO.SetActive(false);
        manaCostGO.SetActive(true);
        skillsGO.SetActive(true);
        state = State.selectingSkill;
        UpdateOptions(SkillArrayToStringArray(battleController.activePlayerMonster.gameMonster.baseSkills));
        UpdateUIOptions(); 
    }

    void SetSelectedSkill() {
        
        selectedSkill = battleController.activePlayerMonster.gameMonster.baseSkills[(int)actionIndex];
        
        skillImages[(int)actionIndex].color = confirmedColor;
        state = State.selectingTarget;

        switch(selectedSkill.skillTarget){
            case Skill.TargetEnum.targetEnemy:
                actualTargets = battleController.battlefield.enemyActiveMonsters;
                break;

            case Skill.TargetEnum.ally:
                actualTargets = battleController.battlefield.playerActiveMonsters;
                break;

            case Skill.TargetEnum.self:
                actualTargets = new List<ActiveMonster>();
                actualTargets.Add(battleController.activePlayerMonster);
                break;
            case Skill.TargetEnum.allEnemies:
                actualTargets = battleController.battlefield.enemyActiveMonsters;
                break;
            case Skill.TargetEnum.allAllies:
                actualTargets = battleController.battlefield.playerActiveMonsters;
                break;
            case Skill.TargetEnum.all:
                actualTargets = new List<ActiveMonster>();
                actualTargets.AddRange(battleController.battlefield.playerActiveMonsters);
                actualTargets.AddRange(battleController.battlefield.enemyActiveMonsters);
                break;
        }

        UpdateTargetSprite();
    }

    void UpdateTargetSprite() {
        if (actualTargets == null) {
            return;
        }
        if (targetIndex >= actualTargets.Count) {
            targetIndex = actualTargets.Count - 1;
        }
        if (targetIndex < 0) {
            Debug.LogError("n tem mais alvo");
        }

        for (int i = 0; i < actualTargets.Count; i++) { 
            if (state == State.selectingTarget && selectedSkill &&
                (selectedSkill.skillTarget != Skill.TargetEnum.targetEnemy && selectedSkill.skillTarget != Skill.TargetEnum.ally)) {
                actualTargets[i].spriteRenderer.color = selectedColor;
            }
            else {
                actualTargets[i].spriteRenderer.color =
                targetIndex == i && (state == State.selectingTarget || state == State.capturing) ? selectedColor : Color.white;
            }
        }
    }

    void Deselect() {
        if (state == State.selectingTarget) {
            state = State.selectingSkill;
            targetIndex = 0;
            UpdateTargetSprite();
            UpdateUIOptions();
        }
        else if (state == State.selectingSkill) {
            SelectBattleAction();
        }
        else if (state == State.capturing) {
            SelectBattleAction();
            actionIndex = (Direction)1;
        }
    }

    void CastSkill() {
        if (selectedSkill.manaCost > battleController.activePlayerMonster.gameMonster.actualMana) {
            BattleAction.notEnoughMana?.Invoke();
            skillsGO.SetActive(false);
            previousState = State.selectingSkill;
            state = State.waiting;
        }
        else {
            battleController.PlayerMonsterStartChannel((int)actionIndex, actualTargets[targetIndex]);
            //StartCoroutine(battleController.PlayerSkill((int)actionIndex, actualTargets[targetIndex]));
            EndTurn();
        }
        
    }

    void EndTurn() {
        BattleAction.playerUseTurn?.Invoke();
        state = State.none;
        UpdateUIOptions();
        UpdateTargetSprite();
        targetIndex = 0;
        actionIndex = 0;
        baseActionsGO.SetActive(false);
        skillsGO.SetActive(false);
        manaCostGO.SetActive(false);
    }

    void OpenChangeMonsterUI() {
        changeTeamUI.SelectMonstertToChangeInBattle(battleController.activePlayerMonster.fieldPosition);
        state = State.waiting;
        BattleAction.closeTradeMonster += CloseChangeMonsterUI;
    }

    void CloseChangeMonsterUI() {
        BattleAction.closeTradeMonster -= CloseChangeMonsterUI;
        state = State.selectingAction;
    }

    void SelectAction() {
        if (actionIndex == 0) {
            SelectedSkillMenu();
        }
        else if (actionIndex == Direction.rightup) {
            OpenChangeMonsterUI();
        }
        else if (actionIndex == Direction.leftdown) {
            if (enemyLeader.teamBattle) {
                return;
            }

            actionImages[(int)actionIndex].color = confirmedColor;
            state = State.capturing;
            actualTargets = battleController.battlefield.enemyActiveMonsters;
            targetIndex = 0;
            UpdateTargetSprite();
        }
        else if (actionIndex == Direction.rightdown) {

        }
    }
    
    void ConfirmAction() {
        if (state == State.selectingSkill) {
            SetSelectedSkill();
        }
        else if (state == State.selectingTarget) {
            CastSkill();
        }
        else if (state == State.selectingAction) {
            SelectAction();
        }
        else if (state == State.capturing) {
            if (PlayerResources.instance.HasPokebolas()) {
                PlayerResources.instance.SpendPokebolas();
                bool b = BattleGlobal.CaptureWasSuccesful(actualTargets[targetIndex].gameMonster);
                if (b) {
                    battleController.CaptureMonster(targetIndex);
                }
                battleController.SpendTurn();
                EndTurn();
            }
            else {

            }
           
        }
    }
    
    public void SubstituteMonsterInBattle(int fieldToChange, int newId) {
        battlefield.RemoveMonsterFromPlayerField(fieldToChange);
        BattleGlobal.Swap(playerParty.playerParty, fieldToChange, newId);

        battlefield.SetNewPlayerMonster(fieldToChange, fieldToChange);
        enemiesUI.SetupMonster(fieldToChange, battlefield.playerActiveMonsters[fieldToChange]);
        enemiesUI.UpdateActiveMonstersUI();
        
        battlefield.playerActiveMonsters[fieldToChange].atb = 0.5f;
        battleController.SpendTurn();
        EndTurn();
    }

    void ReadInput() {
        if (input.lastInput == GameInput.InputEnum.confirm) {
            ConfirmAction();
        }

        if (input.lastInput == GameInput.InputEnum.cancel) {
            Deselect();
        }

        if (input.lastInput == GameInput.InputEnum.left) {

            if (state == State.selectingSkill || state == State.selectingAction) { 
                if ((int)actionIndex == 1 || (int)actionIndex == 3) {
                    actionIndex--;
                }
            }
            else if (state == State.selectingTarget || state == State.capturing) {
                targetIndex--;
                UpdateTargetSprite();
            }
            input.lastInput = GameInput.InputEnum.none;
        }
        else if (input.lastInput == GameInput.InputEnum.right) {
            if (state == State.selectingSkill || state == State.selectingAction) {
                if ((int)actionIndex == 0 || (int)actionIndex == 2) {
                    actionIndex++;
                }
            }

            else if (state == State.selectingTarget || state == State.capturing) {
                targetIndex++;
                UpdateTargetSprite();
            }
        }
        else if (input.lastInput == GameInput.InputEnum.down) {
            if (state == State.selectingSkill || state == State.selectingAction) {
                if ((int)actionIndex < 2) {
                    actionIndex += 2;
                }
            }
        }
        else if (input.lastInput == BattleInput.InputEnum.up) {
            if (state == State.selectingSkill || state == State.selectingAction) {
                if ((int)actionIndex >= 2) {
                    actionIndex -= 2;
                }
            }
        }
        input.lastInput = GameInput.InputEnum.none;
    }


    void UpdateUIOptions() {
        if (state == State.selectingSkill) { 
            for (int i = 0; i < skillImages.Length; i++) {
                skillImages[i].color = Color.white;
            }

            skillImages[(int)actionIndex].color = selectedColor;

            Skill actualSkill = battleController.activePlayerMonster.gameMonster.baseSkills[(int)actionIndex];
            actionEffect.text = actualSkill.info;
            manaCostText.text = ":" + actualSkill.manaCost.ToString();
        }
        else if (state == State.selectingAction) {
            for (int i = 0; i < actionImages.Length; i++) {
                actionImages[i].color = Color.white;
            }
            actionImages[(int)actionIndex].color = selectedColor;
       
        }
    }

    private void Update() {
        if (state != State.none && state != State.waiting) { 
            ReadInput();
        }
    }


    public string[] SkillArrayToStringArray(Skill[] skills) {
        string[] name = new string[skills.Length];
        for (int i = 0; i < skills.Length; i++) {
            name[i] = skills[i].skillName;
        }
        return name;
    }

    void UpdateOptions(string[] text) {
        for (int i = 0; i < opt.Length; i++) {
            opt[i].text = text[i];
        }
    }

}
