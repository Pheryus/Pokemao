using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TypeEffectiveness { normal, superEffective, lessEffective };

[System.Serializable]
public struct Effectiveness {
    public TypeEffectiveness type;
    public Element element;
}

[System.Serializable]
public struct ElementEffectiveness
{
    public List<Effectiveness> effectivenesses;
    public Element element;
}

[CreateAssetMenu(fileName = "Element Table", menuName = "ElementTable")]
public class ElementTable : ScriptableObject
{

    public List<ElementEffectiveness> elementsEffectiveness;
//    { normal, fire, water, thunder, earth, wind, ice, chaos, nature;

    public bool CheckTypeEffectiveness (Element element, Element target, TypeEffectiveness typeEffectiveness) {
        foreach (ElementEffectiveness elementEffectiveness in elementsEffectiveness) {
            if (elementEffectiveness.element == element) {
                foreach (Effectiveness effectiveness in elementEffectiveness.effectivenesses) {
                    if (effectiveness.element == target && effectiveness.type == typeEffectiveness) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

}
