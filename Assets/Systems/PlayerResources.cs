using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerResources : MonoBehaviour {

    public GameObject resourcesCanvas;

    public TextMeshProUGUI manaText, pokebolaTexts;
    
    int totalPokebolas = 2;

    public static PlayerResources instance;

    public List<Artefact> artefacts = new List<Artefact>();

    private void Awake() {
        BattleAction.ResetActions();
    }

    private void Start() {
        instance = this;
    }

    private void Update() {
        pokebolaTexts.text = "x" + totalPokebolas;
    }

    public bool HasPokebolas() {
        return totalPokebolas > 0;
    }

    public void SpendPokebolas() {
        totalPokebolas--;
    }

    public void GainPokebola (int amount) {
        totalPokebolas += amount;
    }

    public bool HasArtefact (Artefact.ArtefactEnum artefact) {
        foreach (Artefact a in artefacts) {
            if (a.artefact == artefact) {
                return true;
            }
        }
        return false;
    }
}
