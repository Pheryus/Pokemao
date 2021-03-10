using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveMonster
{
    public GameMonster gameMonster;
    public float atb;

    public bool playerMonster;


    public Skill skill;
    public ActiveMonster skillTarget;

    public int teamPosition;

    public int fieldPosition;

    public ActiveMonster(GameMonster m) {
        gameMonster = m;
    }

    public SpriteRenderer spriteRenderer;

    public void ResetSkill() {
        skill = null;
        skillTarget = null;
    }
}