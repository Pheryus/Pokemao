using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusUI : MonoBehaviour {

    public TextMeshProUGUI quantity;
    public Image statusImage;
    public GameObject panel;
    public EffectInfo monsterEffectInfo;
    public Effect effect;

    public void UpdateEffect (Effect effect) {
        quantity.text = effect.intensity.ToString();
        this.effect = effect;
    }

    public void UpdateMonsterEffectInfo(Effect effect) {
        monsterEffectInfo = GlobalData.i.effectsSO.GetEffectInfo(effect);
        statusImage.sprite = monsterEffectInfo.image;
        UpdateEffect(effect);
    }
}
