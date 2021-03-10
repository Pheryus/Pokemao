using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


[System.Serializable]
public struct SkillUI
{
    public Image sprite;
    public TextMeshProUGUI skillName;
}

public class MonsterInfoUI : GameInput {

    public ChangeTeamUI changeTeamUI;

    [SerializeField]
    public SkillUI[] skillUIs;

    public TextMeshProUGUI skillEffect, manaCost, skillType, passiveName;
    public Image skillElement;

    public enum SelectedSkillEnum { first, second, third, fourth, newSkill, passiveSkill};

    SelectedSkillEnum _selected;

    SelectedSkillEnum selected {
        get {
            return _selected;
        }

        set {
            _selected = value;
            if (_selected == SelectedSkillEnum.newSkill && newSkill == null) {
                _selected = SelectedSkillEnum.fourth;
            }
            UpdateSkillsUI();
        }
    }

    public GameObject monsterInfoGO;

    public enum State { none, selectingSkillToSubstitute};

    public State actualState;

    Skill newSkill;

    GameMonster gameMonster;

    public TextMeshProUGUI hpStat, manaStat, attackStat, defenseStat, spcAttackStat, spcDefenseStat, speedStat, monsterName, elementName;

    public TextMeshProUGUI castSpeed, dmgText;

    public Image monsterElementImage, skillElementImage, monsterSprite, passiveImage;

    public GameObject newSkillGO, manaGO, skillExtraInfo;

    public SelectingMonsterInfoUI selectingMonsterInfoUI;

    public void OpenSelectSkillUI(Skill newSkill, GameMonster _gameMonster) {
        this.newSkill = newSkill;
        actualState = State.selectingSkillToSubstitute;
        newSkillGO.SetActive(true);
        OpenMonsterInfoUI(_gameMonster);
    }


    public void OpenMonsterInfoUI (GameMonster _gameMonster) {
        monsterInfoGO.SetActive(true);
        Invoke("EnableReadInput", 0.2f);
        gameMonster = _gameMonster;
        UpdateMonsterStats();
        UpdateSkillNames();
        UpdateSkillsUI();
    }

    void UpdateMonsterStats() {
        hpStat.text = "Hp: " + gameMonster.actualHp.ToString() + "/" + gameMonster.baseHp.ToString() ;
        manaStat.text = "Mp: " + gameMonster.actualMana.ToString() + "/" + gameMonster.baseMana.ToString();
        attackStat.text = "Attack: " + gameMonster.actualAttack.ToString();
        defenseStat.text = "Defense: " + gameMonster.actualDefense.ToString();
        spcAttackStat.text = "Spc. Attack: " + gameMonster.actualSpcAttack.ToString();
        spcDefenseStat.text = "Spc. Defense: " + gameMonster.actualSpcDefense.ToString();
        speedStat.text = "Speed: " + gameMonster.actualSpeed.ToString();
        monsterName.text = gameMonster.monsterName;
        monsterSprite.sprite = gameMonster.sprite;
        
        elementName.text = GlobalData.i.GetElementString(gameMonster.monsterElement);
        monsterElementImage.sprite = GlobalData.i.GetElementSprite(gameMonster.monsterElement);
    }

    void UpdateSkillNames() {
        for (int i = 0; i < skillUIs.Length - 1; i++) {
            skillUIs[i].skillName.text = gameMonster.baseSkills[i].skillName;
        }
        if (newSkill != null) { 
            skillUIs[4].skillName.text = newSkill.skillName;
        }
        passiveName.text = gameMonster.passive.passiveName;
    }

    void ConfirmSkill() {
        if (selected != SelectedSkillEnum.newSkill && selected != SelectedSkillEnum.passiveSkill && actualState == State.selectingSkillToSubstitute) {
            gameMonster.baseSkills[(int)selected] = newSkill;
            changeTeamUI.previousState = ChangeTeamUI.State.none;
            BattleAction.gotReward?.Invoke();
            CloseUI();
        }
    }

    void CloseUI() {
        selected = SelectedSkillEnum.first;
        monsterInfoGO.SetActive(false);
        newSkillGO.SetActive(false);
        actualState = State.none;
        canReadInput = false;
    }

    void Cancel() {
        if (actualState == State.selectingSkillToSubstitute) { 
            BattleAction.cancelNewSkill?.Invoke();
        }
        else {
            selectingMonsterInfoUI.ReturnToUI();
        }
        CloseUI();
    }

    void UpdateSkillsUI() {
        for (int i = 0; i < skillUIs.Length; i++) {
            skillUIs[i].sprite.color = i == (int)selected ?  GlobalData.DisabledColor  : Color.white;
        }
        passiveImage.color = SelectedSkillEnum.passiveSkill == selected ? GlobalData.DisabledColor : Color.white;
        UpdateSkillInfo();
    }

    void UpdateSkillInfo() {
        Skill selectedSkill;
        if (selected == SelectedSkillEnum.newSkill) {
            selectedSkill = newSkill;
        }
        else if (selected != SelectedSkillEnum.passiveSkill){
            selectedSkill = gameMonster.baseSkills[(int)selected];
        }
        else {
            skillEffect.text = gameMonster.passive.description;
            SetSkillUI(false);
            return;
        }
        SetSkillUI(true);
        skillEffect.text = selectedSkill.info;
        manaCost.text = ":" + selectedSkill.manaCost.ToString();
        skillType.text = selectedSkill.SkillType;
        castSpeed.text = selectedSkill.skillSpeed.ToString();
        if (selectedSkill.dmgSkills.Count > 0) { 
            dmgText.text = selectedSkill.dmgSkills[0].baseDmg.ToString();
        }
        else {
            dmgText.text = "-";
        }
        skillElement.sprite = GlobalData.i.GetElementSprite(selectedSkill.skillElement);
    }

    void SetSkillUI (bool b) {
        manaGO.SetActive(b);
        skillExtraInfo.SetActive(b);
    }

    protected override void Update() {
        base.Update();

        switch (lastInput) {
            case InputEnum.down:
                if (selected != SelectedSkillEnum.newSkill) {
                    selected++;
                }
                break;

            case InputEnum.up:
                if (selected != SelectedSkillEnum.first) {
                    selected--;
                }
                break;

            case InputEnum.left:
                if (selected == SelectedSkillEnum.fourth || selected == SelectedSkillEnum.newSkill) {
                    selected = SelectedSkillEnum.passiveSkill;
                }
                break;

            case InputEnum.right:
                if (selected == SelectedSkillEnum.passiveSkill) {
                    selected = SelectedSkillEnum.newSkill;
                }
                break;

            case InputEnum.confirm:
                ConfirmSkill();
                break;

            case InputEnum.cancel:
                Cancel();
                break;
        }
        lastInput = InputEnum.none;

    }

}
