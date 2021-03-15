using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BattleSnapshot
{
    public Skill skill;
    public ActiveMonster caster, target;

    public List<ActiveMonster> playerMonsters, enemyMonsters;

    public BattleSnapshot(Skill skill, ActiveMonster caster, ActiveMonster target, List<ActiveMonster> playerMonsters, List<ActiveMonster> enemyMonsters) {
        this.skill = skill;
        this.caster = caster;
        this.target = target;
        this.playerMonsters = playerMonsters;
        this.enemyMonsters = enemyMonsters;
    }
}

public class SkillResolution : MonoBehaviour {

    public BattleMsg battleMsg;

    public void CancelSkill(ActiveMonster target) {
        if (target.skill != null) {
            target.ResetSkill();
            target.atb = BattleController.ATBMIN * 0.9f;
        }
    }

    public IEnumerator CastSkill (BattleSnapshot skillCast) {

        List<ActiveMonster> targets = new List<ActiveMonster>();

        switch (skillCast.skill.skillTarget) {
            case Skill.TargetEnum.targetEnemy:
                targets.Add(skillCast.target);
                break;

            case Skill.TargetEnum.allEnemies:
                targets = skillCast.enemyMonsters;
                break;
            case Skill.TargetEnum.self:
                targets.Add(skillCast.caster);
                break;
            case Skill.TargetEnum.ally:
                targets.Add(skillCast.target);
                break;
            case Skill.TargetEnum.allAllies:
                targets.AddRange(skillCast.playerMonsters);
                break;
        }

        int[] hitsOnTargets = new int[targets.Count];
        EnemiesUI en = EnemiesUI.instance;

        if (skillCast.skill.extraEffect == ExtraEffect.sufferFiveDamage) {
            skillCast.caster.gameMonster.actualHp -= 5;
            yield return en.StartCoroutine(en.UpdateMonsterBar(hpBar: true, skillCast.caster, skillCast.caster.gameMonster.pctHp));
        }

        foreach (DmgSkill dmgSkill in skillCast.skill.dmgSkills) {
            int i = 0;
            foreach (ActiveMonster target in targets) {

                if (Skill.HitTarget(dmgSkill.acc, skillCast.caster.gameMonster, target.gameMonster)) {
                    float dmg = Skill.Damage(skillCast.skill, dmgSkill, skillCast.caster.gameMonster, target.gameMonster);
                    dmg = DamageAfterArtefactsEffects(skillCast, dmg);
                    target.gameMonster.actualHp -= (int)dmg;
                    
                    yield return en.StartCoroutine(en.UpdateMonsterBar(hpBar: true, target, target.gameMonster.pctHp));

                    if (GlobalData.i.SuperEffective(skillCast.skill.skillElement, target.gameMonster.monsterElement)) {
                        BattleAction.isSuperEffective?.Invoke();
                        while (BattleMsg.instance.state != BattleMsg.State.none) {
                            yield return null;
                        }
                    }
                    if (GlobalData.i.LessEffective(skillCast.skill.skillElement, target.gameMonster.monsterElement)) {
                        BattleAction.isLessEffective?.Invoke();
                        while (BattleMsg.instance.state != BattleMsg.State.none) {
                            yield return null;
                        }
                    }
                    
                    if (skillCast.skill.extraEffect == ExtraEffect.sufferHalfDamage) {
                        skillCast.caster.gameMonster.actualHp -= (int)dmg / 2;
                        yield return en.StartCoroutine(en.UpdateMonsterBar(hpBar: true, skillCast.caster, skillCast.caster.gameMonster.pctHp));
                    }
                    hitsOnTargets[i]++;
                    yield return new WaitForSeconds(0.2f);

                }
                else {
                    Debug.Log("erro");
                    //MissHit();
                }
                i++;
            }
        }

        int targetIndex = 0;

        foreach (ActiveMonster target in targets) {
            if (hitsOnTargets[targetIndex] > 0) {
                if (Skill.HitTarget(skillCast.skill.onHitACC, skillCast.caster.gameMonster, target.gameMonster)) {
                    foreach (Effect skillEffect in skillCast.skill.onHitEffects) {

                        target.gameMonster.AddEffect(skillEffect);
                        if (skillEffect.effectType == Effect.EffectType.cancelChannel) {
                            CancelSkill(target);
                        }

                        if (!target.gameMonster.dead) {
                            battleMsg.GainEffectMessage(target.gameMonster.monsterName, GlobalData.i.GetEffectString(skillEffect.effectType));
                            while (battleMsg.state != BattleMsg.State.none) {
                                yield return null;
                            }
                        }
                    }
                }
            }
            targetIndex++;
        }

        
        foreach (CastEffect effect in skillCast.skill.onCastEffects) {
            targets.Clear();
            
            switch (effect.target) {
                case Skill.TargetEnum.all:
                    targets.AddRange(skillCast.playerMonsters);
                    targets.AddRange(skillCast.enemyMonsters);
                    break;

                case Skill.TargetEnum.allEnemies:
                    targets.AddRange(skillCast.enemyMonsters);
                    break;

                case Skill.TargetEnum.allAllies:
                    targets.AddRange(skillCast.playerMonsters);
                    break;

                case Skill.TargetEnum.ally:
                    targets.Add(skillCast.target);
                    break;
                case Skill.TargetEnum.targetEnemy:
                    targets.Add(skillCast.target);
                    break;

                case Skill.TargetEnum.self:
                    targets.Add(skillCast.caster);
                    break;
            }

            foreach (ActiveMonster target in targets) {
                target.gameMonster.AddEffect(effect, gainThisTurn: skillCast.caster.GetHashCode() == target.GetHashCode());
                battleMsg.GainEffectMessage(target.gameMonster.monsterName, GlobalData.i.GetEffectString(effect.effectType));
                while (battleMsg.state != BattleMsg.State.none) {
                    yield return null;
                }
            }
        }
        BattleAction.monsterEffectUpdate?.Invoke();
    }

    public float DamageAfterArtefactsEffects(BattleSnapshot battleSnapshot, float dmg) {
        if (battleSnapshot.caster.playerMonster) {
            if (battleSnapshot.skill.skillSpeed <= 10 && PlayerResources.instance.HasArtefact(Artefact.ArtefactEnum.slow_power)) {
                return (dmg * 1.25f);
            }
        }
        return dmg;
    }

}
