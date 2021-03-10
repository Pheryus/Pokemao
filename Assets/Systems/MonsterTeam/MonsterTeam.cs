using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MonsterTeam : MonoBehaviour
{
    public TextMeshProUGUI hpValue, mpValue, monsterName;

    public Image hpBar, mpBar, window, monsterSprite;

    public GameObject dead, monster;

    public void UpdateWindowColor (Color c) {
        window.color = c;
    }

    public void UpdateInfo (GameMonster gameMonster) {
        monster.SetActive(true);
        monsterSprite.sprite = gameMonster.sprite;
        dead.SetActive(gameMonster.dead);
        hpBar.fillAmount = gameMonster.pctHp;
        mpBar.fillAmount = gameMonster.pctMana;
        hpBar.color = GlobalData.HpColor(gameMonster.pctHp);
        monsterName.text = gameMonster.monsterName;
        hpValue.text = gameMonster.actualHp + "/" + gameMonster.baseHp;
        mpValue.text = gameMonster.actualMana + "/" + gameMonster.baseMana;
    }

}
