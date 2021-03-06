using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterStat { vigor, wisdom, focus, agility };

public class GlobalData : MonoBehaviour
{
    public EffectsSO effectsSO;

    public static GlobalData i;

    public static Color DisabledColor = new Color(0.8f, 0, 0, 0.5f);

    public Skill[] skills;

    public Sprite[] elementsSprite;

    [SerializeField]
    public List<MonsterEncounterSO> encounters;

    [SerializeField]
    public List<TeamEncounterSO> teamEncounters;

    public MonsterSO errorMonsterSO;

    public Skill errorSkill;

    public ElementTable elementTable;

    public List<Artefact> artefacts;

    static Color32[] hpColors = new Color32[6]{
        new Color32(105, 179, 76, 255),
        new Color32(172, 179, 52, 255),
        new Color32(250, 183, 51, 255),
        new Color32(255, 142, 21, 255),
        new Color32(255, 78, 17, 255),
        new Color32(255, 13, 13, 255)
    };

    public void Awake() {
        i = this;
    }

    public Sprite GetElementSprite (Element element) {
        return elementsSprite[(int)element];
    }

    public string GetElementString (Element element) {
        switch (element) {
            case Element.normal:
                return "Normal";
            case Element.fire:
                return "Fire";
            case Element.water:
                return "Water";
            case Element.earth:
                return "Earth";
            case Element.wind:
                return "Wind";
        }
        return "";
    }

    public static Color HpColor (float pct) {
        return hpColors[(int)Mathf.Lerp(hpColors.Length - 1, 0, pct)];
    }
    public string GetStatString (MonsterStat stat) {
        switch (stat) {
            case MonsterStat.vigor:
                return "Vigor";
            case MonsterStat.wisdom:
                return "Wisdom";
            case MonsterStat.focus:
                return "Focus";
            case MonsterStat.agility:
                return "Agility";
        }
        return "";
    }

    public bool SuperEffective (Element element, Element target) {

        return elementTable.CheckTypeEffectiveness(element, target, TypeEffectiveness.superEffective);
    }

    public bool LessEffective (Element element, Element target) {
        return elementTable.CheckTypeEffectiveness(element, target, TypeEffectiveness.lessEffective);
    }

    public Artefact GetRandomArtefact() {
        return artefacts[Random.Range(0, artefacts.Count)];
    }

    public string GetEffectString(Effect.EffectType effect) {
        switch (effect) {
            case Effect.EffectType.burn:
                return " is burning";
            case Effect.EffectType.blind:
                return " is blinded";
            case Effect.EffectType.block:
                return " now blocks physical attacks";
            case Effect.EffectType.resistance:
                return " now blocks special attacks";
            case Effect.EffectType.burnOnContact:
                return "'s skin is burning a lot!";
            case Effect.EffectType.channelSpeed:
                return " is now channeling faster!";
        }
        return "";
    }

    public Skill GetRandomSkill (Element element) {
        int tries = 0;
        while (tries < 1000) {
            Skill skill = skills[Random.Range(0, skills.Length)];
            if (skill.skillElement == element) {
                return skill;
            }
            tries++;
        }
        return errorSkill;
    }

    public MonsterStat GetRandomStat() {
        return (MonsterStat)Random.Range(0, 4);
    }

    public Skill GetRandomSkill() {
        return skills[Random.Range(0, skills.Length)];
    
    }


    public TeamEncounterSO GetTeamEncounter (int difficulty) {
        List<TeamEncounterSO> possibleEncounters = new List<TeamEncounterSO>();
        foreach (TeamEncounterSO t in teamEncounters) {
            if (t.difficulty == difficulty) {
                possibleEncounters.Add(t);
            }
        }
        return possibleEncounters[Random.Range(0, possibleEncounters.Count - 1)];
    }

    public GameMonster GetEncounter (int difficulty, Element monsterElement) {
        List<MonsterSO> possibleMonsters = new List<MonsterSO>();
        foreach (MonsterEncounterSO m in encounters) {
            if (m.element != monsterElement) {
                continue;
            }
            foreach (MonsterEncounterSO.MonsterEncounter me in m.monsters) {
                if (me.difficulty == difficulty && me.monsterSO != null) {
                    possibleMonsters.Add(me.monsterSO);
                }
            }
        }
        if (possibleMonsters.Count == 0) {
            return new GameMonster(errorMonsterSO);
        }
        GameMonster newEnemyMonster = new GameMonster(possibleMonsters[Random.Range(0, possibleMonsters.Count - 1)]);
        newEnemyMonster.LevelUpMonster(difficulty * UnityEngine.Random.Range(1, 2));
        return newEnemyMonster;
    }

}
