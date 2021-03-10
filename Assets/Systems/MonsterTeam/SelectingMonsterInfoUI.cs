using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectingMonsterInfoUI : GameInput {


    [System.Serializable]
    public struct SelectedMonsterOption
    {
        public TextMeshProUGUI text;
        public GameObject go;
        public Image sprite;
    }

    [SerializeField]
    public SelectedMonsterOption[] options;

    public GameObject ui;

    public ChangeTeamUI changeTeamUI;

    int _selectedOption;

    public MonsterInfoUI monsterInfoUI;

    GameMonster selectedMonster;

    public int selectedOption {
        get {
            return _selectedOption;
        }
        set {
            _selectedOption = value;
            UpdateSelectedOption();
        }
    }

    void UpdateSelectedOption() {
        for (int i = 0; i < options.Length; i++) {
            if (i == selectedOption) {
                options[i].sprite.color = GlobalData.DisabledColor;
            }
            else {
                options[i].sprite.color = Color.white;
            }
        }
    }

    public void Open(GameMonster monster) {
        selectedMonster = monster;
        ui.SetActive(true);
        SetOptions();
        selectedOption = 0;
        Invoke("EnableReadInput", 0.2f);
    }

    public void ReturnToUI() {
        int opt = selectedOption;
        Open(selectedMonster);
        selectedOption = opt;
        
    }

    void Close() {
        canReadInput = false;
        ui.SetActive(false);
    }

    void SetOptions() {
        if (changeTeamUI.ConfirmingMonsterToGainStat) {
            options[0].text.text = "Boost Stat!";
        }
        else {
            options[0].text.text = "Change";
        }
        
        options[1].text.text = "Info";
        options[2].text.text = "Cancel";
    }

    void ConfirmAction() {
        switch (selectedOption) {
            case 0:
                Close();
                if(changeTeamUI.ConfirmingMonsterToGainStat){
                    changeTeamUI.ConfirmMonsterToGainStat();
                }
                else {
                    changeTeamUI.SelectMonsterToChange();
                }
                break;

            case 1:
                Close();
                monsterInfoUI.OpenMonsterInfoUI(selectedMonster);
                break;
            case 2:
                Close();
                changeTeamUI.ReturnToChangeTeamUI();
                break;
        }
    }

    void Cancel() {
        selectedOption = 2;
    }

    protected override void Update() {
        if (!canReadInput) {
            return;
        }
        base.Update();

        switch (lastInput) {
            case InputEnum.right:
                if (selectedOption < 2) {
                    selectedOption++;
                }
                break;
            case InputEnum.left:
                if (selectedOption > 0) {
                    selectedOption--;
                }
                break;
            case InputEnum.confirm:
                ConfirmAction();
                break;

            case InputEnum.cancel:
                Cancel();
                break;
        }
        lastInput = InputEnum.none;
    }
}
