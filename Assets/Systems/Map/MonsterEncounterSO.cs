using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "MonsterEncounter", menuName = "Monster/Create Monster Encounter")]
public class MonsterEncounterSO : ScriptableObject {

    public Element element;
    
    [System.Serializable]
    public struct MonsterEncounter
    {
        public MonsterSO monsterSO;
        public int difficulty;
    }

    [SerializeField]
    public List<MonsterEncounter> monsters;
}

[CreateAssetMenu(fileName = "MonsterEncounter", menuName = "Monster/Create Team Encounter")]
public class TeamEncounterSO : ScriptableObject
{
    public int difficulty;
    [SerializeField]
    public List<MonsterSO> team;
}