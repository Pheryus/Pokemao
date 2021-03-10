using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectInfo {
    public Effect.EffectType effect;
    public string effectName;
    [TextArea(3, 10)]
    public string info;
    public Sprite image;

}

[CreateAssetMenu(fileName = "Effect", menuName = "EffectList")]
public class EffectsSO : ScriptableObject
{
    [SerializeField]
    public List<EffectInfo> effects;

    public EffectInfo GetEffectInfo (Effect effect) {
        foreach (EffectInfo ei in effects) {
            if (ei.effect == effect.effectType) {
                return ei;
            }
        }
        return null;
    }

}
