using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class MonsterUI
{
    public Image hpBar, atpBar, manaBar, channelBar;
    public ActiveMonster activeMonster;
    public GameObject monsterUI;
    public Transform statusPanel;

    public TextMeshProUGUI hpText, manaText;

    public List<EffectGO> effectsGO = new List<EffectGO>();

    public struct EffectGO
    {
        public GameObject go;
        public StatusUI statusUI;

        public EffectGO (GameObject go, StatusUI status) {
            this.go = go;
            this.statusUI = status;
        }
    }

    public void AddEffectGO (GameObject go) {
        effectsGO.Add(new EffectGO(go, go.GetComponent<StatusUI>()));
    }

    public void SetActive(bool b) {
        monsterUI.SetActive(b);
    }

    public void ResetUI() {
        hpBar.fillAmount = activeMonster == null ? 1 : activeMonster.gameMonster.pctHp;
        hpBar.color = GlobalData.HpColor(hpBar.fillAmount);
        
        channelBar.fillAmount = 0;
    }

}

public class EnemiesUI : MonoBehaviour {

    public Battlefield battlefield;

    public BattleController battleController;

    public PlayerParty playerParty;

    public EnemyLeader enemyLeader;

    [SerializeField]
    public List<MonsterUI> enemyMonstersUI;

    [SerializeField]
    public List<MonsterUI> playerMonstersUI;

    public static EnemiesUI instance;

    public GameObject effectPrefab;

    public Image[] yourMonstersIcon;

    public Image[] enemyMonstersIcon;

    private void Start() {
        instance = this;
        BattleAction.monsterEffectUpdate += UpdateGameMonstersEffectUI;
    }

    void UpdateMonsterIcons(List<GameMonster> monsters, Image[] icons) {
        for(int i = 0; i < playerParty.MaxPartySize; i++) {
            if (monsters.Count > i ) {
                icons[i].color = monsters[i].dead ? Color.red : Color.white;
            }
            else {
                icons[i].color = new Color(1, 1, 1, 0.5f);
            }
        }
    }

    public void SetupMonstersUI() {
        playerMonstersUI[0].monsterUI.transform.parent.gameObject.SetActive(true);
        enemyMonstersUI[0].monsterUI.transform.parent.gameObject.SetActive(true);
        UpdateActiveMonstersUI();
        CheckUI();
    }


    public void UpdateActiveMonstersUI() {
        UpdateMonsterIcons(playerParty.playerParty, yourMonstersIcon);

        if (enemyLeader.teamBattle) {
            UpdateMonsterIcons(enemyLeader.monsters, enemyMonstersIcon);
        }
    }

    public void SetupMonster (int i, ActiveMonster activeMonster, bool playerMonster = true) {
        MonsterUI m = playerMonster ? playerMonstersUI[i] : enemyMonstersUI[i];
        m.activeMonster = activeMonster;
        m.ResetUI();
        activeMonster.fieldPosition = i;
        m.hpText.text = m.activeMonster.gameMonster.actualHp + "/" + m.activeMonster.gameMonster.baseHp;
        m.manaText.text = m.activeMonster.gameMonster.actualMana + "/" + m.activeMonster.gameMonster.baseMana;
        

        for (int j = m.statusPanel.childCount - 1; j >= 0; j--) {
            Destroy(m.statusPanel.GetChild(j).gameObject);
        }
        m.SetActive(true);
    }

    public void UpdateGameMonstersEffectUI() {
        foreach (MonsterUI m in enemyMonstersUI) {
            UpdateGameMonsterEffectUI(m);
        }

        foreach (MonsterUI m in playerMonstersUI) {
            UpdateGameMonsterEffectUI(m);
        }
    }


    void UpdateGameMonsterEffectUI (MonsterUI m) {
        int effectIndex = 0;

        if (m.activeMonster == null) {
            return;
        }

        foreach (Effect eff in m.activeMonster.gameMonster.actualEffects) {
            Debug.Log("eff intensity: " + eff.intensity);
            if (m.effectsGO.Count > effectIndex) {
                StatusUI statusUI = m.effectsGO[effectIndex].statusUI;
                statusUI.UpdateMonsterEffectInfo(eff);
            }
            else {
                GameObject effectGO = Instantiate(effectPrefab, m.statusPanel);
                m.AddEffectGO(effectGO);
                StatusUI statusUI = m.effectsGO[effectIndex].statusUI;
                statusUI.UpdateMonsterEffectInfo(eff);
            }
            effectIndex++;
        }
        for (int i = m.effectsGO.Count - 1; i >= 0; i--) {
            if (i >= m.activeMonster.gameMonster.actualEffects.Count) {
                Destroy(m.effectsGO[i].go);
            }
        }
    }



    public IEnumerator UpdateMonsterBar(bool hpBar,  ActiveMonster monster, float newPct) {
        if (monster.playerMonster) {
            foreach (MonsterUI ui in playerMonstersUI) {
                if (ui.activeMonster.gameMonster.actualGO.GetInstanceID() == monster.gameMonster.actualGO.GetInstanceID()) {
                    if (hpBar) {
                        yield return StartCoroutine(AnimateBar(ui.hpBar, newPct, ui));
                    }
                    else {
                        yield return StartCoroutine(AnimateBar(ui.manaBar, newPct, ui, hpBar: false));
                    }
                    break;
                }
                
            }
        }
        else {
            foreach (MonsterUI ui in enemyMonstersUI) {
                if (ui.activeMonster.gameMonster.actualGO.GetInstanceID() == monster.gameMonster.actualGO.GetInstanceID()) {
                    if (hpBar) {
                        yield return StartCoroutine(AnimateBar(ui.hpBar, newPct, ui));
                    }
                    else {
                        yield return StartCoroutine(AnimateBar(ui.manaBar, newPct, ui, hpBar: false));
                    }
                    
                    break;  
                }
            }
        }
        yield return null;
    }


    public void CheckUI() {
        for (int i = 0; i < playerMonstersUI.Count; i++) {
            playerMonstersUI[i].SetActive(!playerMonstersUI[i].activeMonster.gameMonster.dead);
        }

        for (int i = 0; i < enemyMonstersUI.Count; i++) {
            if (enemyMonstersUI[i].activeMonster == null) {
                enemyMonstersUI[i].SetActive(false);
            }
            else {
                enemyMonstersUI[i].SetActive(!enemyMonstersUI[i].activeMonster.gameMonster.dead);
            }
        }
        UpdateActiveMonstersUI();
    }

    IEnumerator AnimateBar (Image target, float targetPct, MonsterUI m, bool hpBar = true) {
        float pctSpeed = 0.7f;
        targetPct = Mathf.Max(0, targetPct);

        while (Mathf.Abs(target.fillAmount - targetPct) > 0.01f) {
            target.fillAmount -= pctSpeed * Time.deltaTime;
            if (hpBar) { 
                int hp = (int) (m.activeMonster.gameMonster.baseHp * target.fillAmount);
                m.hpText.text = hp.ToString() + "/" + m.activeMonster.gameMonster.baseHp;
                target.color = GlobalData.HpColor(target.fillAmount);
            }
            else {
                int mp = (int)(m.activeMonster.gameMonster.baseMana * target.fillAmount);
                m.manaText.text = mp.ToString() + "/" + m.activeMonster.gameMonster.baseMana;
            }
            yield return null;
        }
        target.fillAmount = targetPct;
        m.hpText.text = m.activeMonster.gameMonster.actualHp.ToString() + "/" + m.activeMonster.gameMonster.baseHp;
        m.manaText.text = m.activeMonster.gameMonster.actualMana.ToString() + "/" + m.activeMonster.gameMonster.baseMana;
    }

    private void Update() {
        foreach (MonsterUI ui in playerMonstersUI) {
            if (ui.activeMonster != null) { 
                if (ui.activeMonster.skill) {
                    ui.channelBar.fillAmount = ui.activeMonster.atb / BattleController.ATBMIN;
                 
                }
                else {
                    ui.atpBar.fillAmount = ui.activeMonster.atb / BattleController.ATBMIN;
                    ui.channelBar.fillAmount = 0;
                }
                
            }
        }
        foreach (MonsterUI ui in enemyMonstersUI) {
            if (ui.activeMonster != null) {
                if (ui.activeMonster.skill) {
                    ui.channelBar.fillAmount = ui.activeMonster.atb / BattleController.ATBMIN;
                }
                else {
                    ui.atpBar.fillAmount = ui.activeMonster.atb / BattleController.ATBMIN;
                    ui.channelBar.fillAmount = 0;
                }
            }
        }
    }

}
