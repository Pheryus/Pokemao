using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive", menuName = "New Monster Passive")]
public class MonsterPassive : ScriptableObject {
 
    public enum Passive { none};

    public Passive passive;

    public string passiveName;

    [TextArea(3, 7)]
    public string description;

}
