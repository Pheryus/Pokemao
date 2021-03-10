using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class RewardModalUI {
    public TextMeshProUGUI rewardName, rewardDescription;
    public Image selectRewardImages;
    public RewardTypeSO rewardTypeSO;

    public Skill newSkill;
    public int amount;
    public GameMonster monsterTarget;
    public RewardControl.RewardTypeNum rewardTypeNum;
    public MonsterStat monsterStat;

    public void UpdateRewardType(string name) {
        if (rewardTypeNum == RewardControl.RewardTypeNum.restoreHP) {
            rewardName.text = rewardTypeSO.rewardName.Replace("@", name);
            rewardDescription.text = rewardTypeSO.info.Replace("@", name);
            rewardName.text = rewardName.text.Replace("$", name);
            rewardDescription.text = rewardDescription.text.Replace("$", monsterTarget.monsterName);
        }
        else {
            rewardName.text = rewardTypeSO.rewardName.Replace("@", name);
            rewardDescription.text = rewardTypeSO.info.Replace("@", name);
        }
    }
}

public class SelectRewardUI : GameInput {

    [SerializeField]
    public RewardModalUI[] rewardModals;

    public TextMeshProUGUI skipRewardName, skipRewardDescription;
    public Image skipRewardImage;

    enum State { none, selectingReward, choosingMonsterToUpgrade, choosingSkillToUpgrade};

    enum SelectedReward { firstReward, secondReward, skipReward};

    SelectedReward selectedReward;

    State actualState;

    public ChangeTeamUI changeTeamUI;

    private void Start() {
        UpdateUI();
        actualState = State.selectingReward;
        Invoke("EnableReadInput", 0.2f);
    }

    public void ResetUI() {
        selectedReward = SelectedReward.firstReward;
        UpdateUI();
    }

    void ConfirmAction() {

        if (selectedReward == SelectedReward.skipReward) {
            return;
        }

        RewardModalUI selectedRewardModal = rewardModals[(int)selectedReward];

        switch (selectedRewardModal.rewardTypeNum) {
            case RewardControl.RewardTypeNum.newAbility:
                changeTeamUI.SelectMonsterToGainSkill(selectedRewardModal);
                actualState = State.choosingSkillToUpgrade;
                gameObject.SetActive(false);
                break;
            case RewardControl.RewardTypeNum.restoreMana:
                //ayerResources.instance.mana += selectedRewardModal.amount;
                BattleAction.gotReward?.Invoke();
                break;

            case RewardControl.RewardTypeNum.restoreHP:
                selectedRewardModal.monsterTarget.actualHp += selectedRewardModal.amount;
                BattleAction.gotReward?.Invoke();
                break;

            case RewardControl.RewardTypeNum.statsIncrease:
                actualState = State.choosingMonsterToUpgrade;
                changeTeamUI.SelectMonsterToGainStat(selectedRewardModal);
                gameObject.SetActive(false);
                break;

            case RewardControl.RewardTypeNum.increaseMaxMana:
                //PlayerResources.instance.maxMana += selectedRewardModal.amount;
                //PlayerResources.instance.mana += selectedRewardModal.amount;
                BattleAction.gotReward?.Invoke();
                break;
        }
    }

    public void EnableSelectRewardUI() {
        if (actualState == State.choosingSkillToUpgrade || actualState == State.choosingMonsterToUpgrade) {
            gameObject.SetActive(true);
            actualState = State.selectingReward;
            selectedReward = SelectedReward.firstReward;
            UpdateUI();
        }
    }

    public void NewReward() {
        actualState = State.selectingReward;
        selectedReward = SelectedReward.firstReward;
    }

    void CancelAction() {

    }

    void CursorAction() {
        switch (actualState) {
            case State.selectingReward:
                if (lastInput == InputEnum.down) {
                    selectedReward = SelectedReward.skipReward;
                }
                else if (lastInput == InputEnum.up && selectedReward == SelectedReward.skipReward) {
                    selectedReward = SelectedReward.firstReward;
                }
                else if (lastInput == InputEnum.left && selectedReward == SelectedReward.secondReward) {
                    selectedReward = SelectedReward.firstReward;
                }
                else if (lastInput == InputEnum.right && selectedReward == SelectedReward.firstReward) {
                    selectedReward = SelectedReward.secondReward;
                }
                break;
        }

        UpdateUI();
    }

    void UpdateUI() {
        for (int i = 0; i < rewardModals.Length; i++) {
            rewardModals[i].selectRewardImages.color = i == (int)selectedReward ? GlobalData.DisabledColor : Color.white;
        }
        skipRewardImage.color = selectedReward == SelectedReward.skipReward ? GlobalData.DisabledColor : Color.white;
    }

    protected override void Update() {
        if (!canReadInput) {
            return;
        }
        base.Update();

        switch (lastInput) {
            case InputEnum.confirm:
                ConfirmAction();
                break;
            case InputEnum.cancel:
                CancelAction();
                break;
            default:
                if (lastInput != InputEnum.none) { 
                    CursorAction();
                }
                break;
        }


        lastInput = InputEnum.none;
    }
}
