using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedOption : MonoBehaviour
{
    public MapOption selectedOption;
    public Transform arrow;

    public static SelectedOption instance;

    public MapInput mapInput;

    private void Start() {
        instance = this;
        MoveArrow();
    }

    public void MoveArrow() {
        arrow.position = selectedOption.transform.position + Vector3.left * 2f;
    }

    public void UpdateMapOption() {
        mapInput.battleScene.SetActive(false);
        //mapInput.thisScene.SetActive(true);
        selectedOption = selectedOption.nextMapOption;
        MoveArrow();
        mapInput.Invoke("EnableReadInput", 0.2f);
    }
}
