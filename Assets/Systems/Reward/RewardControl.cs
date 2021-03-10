using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardControl : MonoBehaviour {

    public SelectRewardUI selectRewardUI;

    public RewardTypeSO[] rewardsSO;

    public List<GameMonster> defeatedMonster = new List<GameMonster>();

    public GameObject battleRewardGO;

    public MapInput mapInput;

    public enum RewardTypeNum { newAbility, newPassive, powerupAbility, restoreHP, restoreMana, statsIncrease, increaseMaxMana };
    
    private void Start() {
        BattleAction.gotReward += CheckReward;
        BattleAction.gainReward += CheckReward;
    }

    public void GiveReward(List<GameMonster> monsters) {
        defeatedMonster = new List<GameMonster>();
        foreach (GameMonster monster in monsters) {
            defeatedMonster.Add(new GameMonster(monster));
        }
        CheckReward();
        selectRewardUI.ResetUI();
    }

    void CheckReward() {

        if (defeatedMonster.Count > 0) { 
            CreateElementReward(RewardTypeNum.newAbility, 0);
            CreateElementReward(RewardTypeNum.statsIncrease, 1);
            defeatedMonster.RemoveAt(0);
            gameObject.SetActive(true);
            battleRewardGO.SetActive(true);
        }
        else {
            gameObject.SetActive(false);
            battleRewardGO.SetActive(false);
            mapInput.CompleteNode();
        }
        
    }

    public void CreateElementReward(RewardTypeNum rewardType, int option) {
        RewardTypeSO rewardOption = rewardsSO[(int)rewardType];
        RewardModalUI rewardModalUI = selectRewardUI.rewardModals[option];

        selectRewardUI.NewReward();
        

        rewardModalUI.rewardTypeNum = rewardType;
        rewardModalUI.rewardTypeSO = rewardOption;
        switch (rewardType) {
            case RewardTypeNum.newAbility:
                rewardModalUI.newSkill = GlobalData.i.GetRandomSkill(defeatedMonster[0].monsterElement);
                rewardModalUI.UpdateRewardType(rewardModalUI.newSkill.skillName);
                break;

            case RewardTypeNum.restoreMana:
                rewardModalUI.amount = 5;
                rewardModalUI.UpdateRewardType(rewardModalUI.amount.ToString());
                break;

            case RewardTypeNum.restoreHP:
                rewardModalUI.amount = 20;
                rewardModalUI.monsterTarget = PlayerParty.instance.LowerHPMonster;

                rewardModalUI.UpdateRewardType(rewardModalUI.amount.ToString());
                break;

            case RewardTypeNum.statsIncrease:
                rewardModalUI.amount = 1;
                rewardModalUI.monsterStat = MonsterStat.maxMana;// GlobalData.i.GetRandomStat();
                if (rewardModalUI.monsterStat == MonsterStat.maxHP) {
                    rewardModalUI.amount = 5;
                }
                else if (rewardModalUI.monsterStat == MonsterStat.maxMana) {
                    rewardModalUI.amount = 2;
                }
                rewardModalUI.UpdateRewardType(GlobalData.i.GetStatString(rewardModalUI.monsterStat));
                break;

        }
    }

}
