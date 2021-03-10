using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RestUI : GameInput {

    public GameObject restGO;

    int _selected;

    public Image[] options;
    public SpriteRenderer[] icons;
    public MapInput mapInput;

    public int selected {
        get {
            return _selected;
        }
        set {
            _selected = value;
            UpdateUI();
        }
    }

    public void EnableRest() {
        restGO.SetActive(true);
        canReadInput = false;
        Invoke("EnableReadInput", 0.3f);
        UpdateUI();
    }

    void UpdateUI() {
        for (int i = 0; i < options.Length; i++) {
            if (i == selected) {
                options[i].color = GlobalData.DisabledColor;
                icons[i].transform.DOKill();
                icons[i].transform.DOScale(3f, 0.3f);
            }
            else {
                options[i].color = Color.white;
                icons[i].transform.DOKill();
                icons[i].transform.DOScale(2, 0.3f);
            }
        }

    }

    
    void ConfirmAction() {
        if (selected == 0) {
            BattleAction.healParty?.Invoke();
        }
        restGO.SetActive(false);
        mapInput.CompleteNode();


    }

    protected override void Update() {
        if (!canReadInput) {
            return;
        }

        base.Update();

        switch (lastInput) {
            case InputEnum.left:
                selected = 0;
                break;

            case InputEnum.right:
                selected = 1;
                break;

            case InputEnum.confirm:
                ConfirmAction();
                break;
        }
    }
}
