using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTeamUI : GameInput {

    int _selectedMonster;

    public int selectedMonster {
        get {
            return _selectedMonster;
        }
        set {
            _selectedMonster = value;
            UpdateSelectedMonsterUI();

        }
    }

    public GameObject uiGO, selectMonsterUI;

    int confirmedMonsterForChange = -1;

    public MonsterTeam[] team;

    public SpriteRenderer closeButton;

    public PlayerParty playerParty;

    public enum State { none, selectingMonster, changingMonster, selectingMonsterToGainSkill, selectingMonsterToGainStat, selectingOption, selectingMonsterToSubstitute, confirmingMonsterToGainStat, selectingMonsterToSubstituteInBattle};

    public State state, previousState;

    public bool ConfirmingMonsterToGainStat {
        get {
            return state == State.confirmingMonsterToGainStat;
        }
    }

    Color selectedColor = new Color(1, 1, 1, 0.5f);
    Color confirmedForTradeColor = new Color(0.5f, 1, 1, 0.5f);

    RewardModalUI reward;

    public MonsterInfoUI monsterinfoUI;

    public SelectingMonsterInfoUI selectingMonsterInfoUI;

    public SelectRewardUI selectRewardUI;

    public BattleController battleController;

    public Battlefield battlefield;

    public BattleUI battleUI;

    int previousMonsterFieldPosition;

    private void Start() {
        BattleAction.cancelNewSkill += OpenChangeTeamUI;
        BattleAction.gotReward += CloseUI;
    }

    public void SelectMonsterToSubstitute(GameMonster defeatedMonster, int pos) {
        previousMonsterFieldPosition = pos;

        int i = 0;
        foreach (GameMonster gm in playerParty.playerParty) {
            if (gm.actualGO.GetInstanceID() == defeatedMonster.actualGO.GetInstanceID()) {
                selectedMonster = i;
                SelectMonsterToChange();
                break;
            }
            i++;
        }

        OpenChangeTeamUI();
        state = State.selectingMonsterToSubstitute;
    }


    public void OpenChangeTeamUI() {
        canReadInput = false;
        uiGO.SetActive(true);
        selectMonsterUI.SetActive(true);
        UpdateMonstersSprites();
        UpdateSelectedMonsterUI();
        Invoke("EnableReadInput", 0.2f);

        if (previousState == State.selectingMonsterToGainSkill) {
            state = previousState;
            previousState = State.none;
        }
        else {
            state = State.selectingMonster;
        }
        
    }

    public void ReturnToChangeTeamUI() {
        canReadInput = false;
        if (state == State.confirmingMonsterToGainStat) {
            state = State.selectingMonsterToGainStat;
        }
        else {
            state = State.selectingMonster;
        }
        Invoke("EnableReadInput", 0.2f);
    }

    public void SelectMonstertToChangeInBattle (int selectedMonster) {
        OpenChangeTeamUI();
        confirmedMonsterForChange = selectedMonster;
        UpdateSelectedMonsterUI();
        state = State.selectingMonsterToSubstituteInBattle;
    }

    public void SelectMonsterToChange() {
        confirmedMonsterForChange = selectedMonster;
        UpdateSelectedMonsterUI();
        state = State.changingMonster;
    }

    public void SelectMonsterToGainSkill (RewardModalUI rewardModalUI) {
        OpenChangeTeamUI();
        state = State.selectingMonsterToGainSkill;
        reward = rewardModalUI;
        lastInput = InputEnum.none;
        canReadInput = false;
    }

    void UpdateMonstersSprites() {
        int i = 0;
        foreach (GameMonster gm in playerParty.playerParty) {
            team[i].UpdateInfo(gm);
            i++;
        }
    }

    public void UpdateSelectedMonsterUI() {
        int i = 0;
        foreach (MonsterTeam mt in team) {
            if (i == confirmedMonsterForChange){
                mt.UpdateWindowColor(confirmedForTradeColor);
            }
            else if (i == selectedMonster) {
                mt.UpdateWindowColor(selectedColor);
            }
            else {
                mt.UpdateWindowColor(Color.white);
            }
            i++;
        }
        if (selectedMonster == 5) {
            closeButton.color = selectedColor;
        }
        else {
            closeButton.color = Color.white;
        }
    }

    public void CloseChangeTeamUI() {
        CloseUI();
        BattleAction.closeTradeMonster?.Invoke();
    }

    void CloseUI () {
        selectMonsterUI.SetActive(false);
        canReadInput = false;
        lastInput = InputEnum.none;
    }

    void OpenSelectSkillWindow() {
        previousState = State.selectingMonsterToGainSkill;
        CloseUI();
        monsterinfoUI.OpenSelectSkillUI(reward.newSkill, playerParty.playerParty[selectedMonster]);
    }

    public void ConfirmMonsterToGainStat() {
        CloseUI();
        state = State.none;
        playerParty.playerParty[selectedMonster].UpgradeStatus(reward.monsterStat, reward.amount);
        BattleAction.gotReward?.Invoke();
    }

    public void ConfirmMonster() {

        if (selectedMonster >= playerParty.playerParty.Count) {
            return;
        }

        if (state == State.selectingMonsterToGainSkill) {
            OpenSelectSkillWindow();
        }

        else if (state == State.selectingMonster){
            
            selectingMonsterInfoUI.Open(playerParty.playerParty[selectedMonster]);
            state = State.selectingOption;
            return;
        }
        else if (state == State.selectingMonsterToGainStat) {
            state = State.confirmingMonsterToGainStat;

            selectingMonsterInfoUI.Open(playerParty.playerParty[selectedMonster]);
        }
        else if (state == State.selectingMonsterToSubstitute) {
            GameMonster monsterToSubstitute = playerParty.playerParty[selectedMonster];
            if (selectedMonster >= 2 && !monsterToSubstitute.dead) {
                BattleGlobal.Swap(playerParty.playerParty, selectedMonster, confirmedMonsterForChange);
                battlefield.SetNewPlayerMonster(confirmedMonsterForChange, previousMonsterFieldPosition);
                confirmedMonsterForChange = -1;
                battleController.battleScene.SetActive(true);
                CloseUI();
            }
        }

        
    }

    public void SelectMonsterToGainStat(RewardModalUI rewardModalUI) {
        reward = rewardModalUI;
        OpenChangeTeamUI();
        state = State.selectingMonsterToGainStat;
        lastInput = InputEnum.none;
        canReadInput = false;
    }

    public void RemoveSelectedTarget() {
        confirmedMonsterForChange = -1;
        UpdateSelectedMonsterUI();
        state = State.selectingMonster;
    }

    public void TradeMonsters() {
        if (confirmedMonsterForChange > playerParty.playerParty.Count) {
            return;
        }
        BattleGlobal.Swap(playerParty.playerParty, selectedMonster, confirmedMonsterForChange);
        confirmedMonsterForChange = -1;
        state = State.selectingMonster;
        UpdateMonstersSprites();
        UpdateSelectedMonsterUI();
    }

    void ReturnToSelectRewardUI() {
        state = State.none;
        CloseChangeTeamUI();
        selectRewardUI.EnableSelectRewardUI();
    }

    void SubstituteMonsterInBattle() {
        if (confirmedMonsterForChange > playerParty.playerParty.Count || selectedMonster <= 1 || playerParty.playerParty[selectedMonster].dead) {
            return;
        }
        battleUI.SubstituteMonsterInBattle(confirmedMonsterForChange, selectedMonster);
        confirmedMonsterForChange = 0;
        selectedMonster = 0;
        CloseUI();
        
    }

    protected override void Update() {

        if (state == State.none || !canReadInput || state == State.selectingOption || state == State.confirmingMonsterToGainStat) {
            return;
        }

        base.Update();

        switch (lastInput) {
            case InputEnum.left:
                if (selectedMonster >= 2 && selectedMonster < 5) {
                    selectedMonster = 0;
                }
                break;
            case InputEnum.right:
                if (selectedMonster == 0 || selectedMonster == 1) {
                    selectedMonster += 2;
                }
                break;
            case InputEnum.up:
                if (selectedMonster >= 1) {
                    selectedMonster--;
                }
                break;
            case InputEnum.down:
                if (state == State.changingMonster) {
                    if (selectedMonster < 4) {
                        selectedMonster++;
                    }
                }
                else if (selectedMonster < 5) {
                    selectedMonster++;
                }
                break;
            case InputEnum.confirm:
                if (state == State.selectingMonster) {
                    if (selectedMonster == 5) {
                        CloseChangeTeamUI();
                    }
                    else {
                        ConfirmMonster();
                    }
                }
                else if (state == State.selectingMonsterToSubstitute) {
                    ConfirmMonster();
                }
                else if (state == State.changingMonster) {
                    if (selectedMonster != confirmedMonsterForChange) {
                        TradeMonsters();
                    }
                }
                else if (state == State.selectingMonsterToGainSkill || state == State.selectingMonsterToGainStat) {
                    if (selectedMonster == 5) {
                        ReturnToSelectRewardUI();
                    }
                    else {
                        ConfirmMonster();
                    }
                }
                else if (state == State.selectingMonsterToSubstituteInBattle) {
                    SubstituteMonsterInBattle();

                }
                break;

            case InputEnum.cancel:
                if (state == State.selectingMonster) {
                    CloseChangeTeamUI();
                }
                else if (state == State.changingMonster) {
                    RemoveSelectedTarget();
                }
                else if (state == State.selectingMonsterToGainSkill) {
                    ReturnToSelectRewardUI();
                }
                else if (state == State.selectingMonsterToSubstituteInBattle) {
                    CloseChangeTeamUI();
                }
                break;
        }
        if (state == State.changingMonster) { 
            if (selectedMonster >= playerParty.playerParty.Count) {
                selectedMonster = playerParty.playerParty.Count - 1;
            }
        }



        lastInput = InputEnum.none;
        
            
    }
    



}
