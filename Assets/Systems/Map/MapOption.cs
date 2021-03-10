using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOption : MonoBehaviour {
    
    public enum OptionType { battle, bossBattle, rest, reward, shop};

    public OptionType optionType;

    public List<MonsterSO> monsters;

    public MapOption nextMapOption;

    public bool BattleType {
        get {
            return optionType == OptionType.battle;
        }
    }

}
