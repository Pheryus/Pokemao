using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour {

    public enum InputEnum { left, right, confirm, cancel, up, down, none };

    public InputEnum lastInput;

    protected bool canReadInput;

    public void EnableReadInput() {
        canReadInput = true;
    }

    protected virtual void Update() {
        if (!canReadInput) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            lastInput = InputEnum.left;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
            lastInput = InputEnum.right;
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            lastInput = InputEnum.up;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            lastInput = InputEnum.down;
        }
        else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)) {
            lastInput = InputEnum.confirm;
        }
        else if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Backspace)) {
            lastInput = InputEnum.cancel;
        }
    }

}
