using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "Monster SO", menuName = "Monster")]
public class MonsterSO : ScriptableObject {

    public int baseHp, baseMana;
    public float baseAttack, baseDefense, baseSpcAttack, baseSpcDefense, baseSpeed;
    public Skill[] baseSkills;

    public string monsterName;

    public Sprite sprite;

    [Range(1, 3)]
    public float catchRate = 1;

    public Element monsterElement;

    public float baseDodge, baseBonusACC;

    public MonsterPassive passive;

}
