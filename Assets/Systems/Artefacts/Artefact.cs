using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Artefact", menuName = "Create Artefact")]
public class Artefact : ScriptableObject {
    
    public enum ArtefactEnum { initiative, fire_discount, slow_power};
    public enum Rarity { common, rare, legendary}


    public Sprite sprite;
    public Rarity rarity;
    public string artefactName;

    public ArtefactEnum artefact;

    [TextArea(3, 5)]
    public string artefactEffect;
}
