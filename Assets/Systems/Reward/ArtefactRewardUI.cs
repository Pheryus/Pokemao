using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class ArtefactReward
{
    public TextMeshProUGUI name, effect;
    public Image sprite, icon;
    public Artefact artefact;

    public void UpdateInfo() {
        name.text = artefact.artefactName;
        effect.text = artefact.artefactEffect;
        icon.sprite = artefact.sprite;
    }
}

public class ArtefactRewardUI : GameInput {

    [SerializeField]
    public ArtefactReward[] artefactRewards;

    public GameObject artefactRewardGO;

    int _selectedIndex;

    public SpriteRenderer treasureImage;

    public MapInput mapInput;

    public ArtefactUI artefactUI;

    private int GetSelectedIndex() {
        return _selectedIndex;
    }


    public void SetSelectedIndex(int value) {
        Transform target = artefactRewards[_selectedIndex].icon.transform;
        target.DOKill();
        target.DOScale(1, 0.3f);

        _selectedIndex = value;
        if (_selectedIndex < 0) {
            _selectedIndex = 0;
        }
        else if (_selectedIndex > artefactRewards.Length - 1) {
            _selectedIndex = artefactRewards.Length - 1;
        }
        UpdateArtefactUI();
    }

    public void EnableArtefactReward() {
        _selectedIndex = 0;
        canReadInput = false;
        AddNewRewards();
        gameObject.SetActive(true);
        artefactRewardGO.SetActive(true);
        treasureImage.transform.DOShakePosition(100, 0.05f, 2);
        Invoke("EnableReadInput", 0.2f);
        UpdateArtefactUI();
        UpdateArtefactsInfo();
    }

    void AddNewRewards() {
        foreach (ArtefactReward artefactReward in artefactRewards) {
            artefactReward.artefact = GlobalData.i.GetRandomArtefact();
        }
    }

    void UpdateArtefactsInfo() {
        foreach (ArtefactReward artefactReward in artefactRewards) {
            artefactReward.UpdateInfo();
        }
    }

    void UpdateArtefactUI() {
        for (int i = 0; i < artefactRewards.Length; i++) {
            if (i == GetSelectedIndex()) {
                artefactRewards[i].icon.transform.DOKill();
                artefactRewards[i].icon.transform.DOScale(1.4f, 0.3f);
            }
        }
    }

    void ConfirmReward() {
        gameObject.SetActive(false);
        artefactRewardGO.SetActive(false);
        artefactUI.AddArtefact(artefactRewards[_selectedIndex].artefact);
        mapInput.CompleteNode();
    }


    protected override void Update() {
        if (!canReadInput) {
            return;
        }
        base.Update();

        switch (lastInput) {
            case InputEnum.left:
                SetSelectedIndex(GetSelectedIndex() - 1);
                break;
            case InputEnum.right:
                SetSelectedIndex(GetSelectedIndex() + 1);
                break;
            case InputEnum.confirm:
                ConfirmReward();
                break;
        }
        lastInput = InputEnum.none;
    }
}
