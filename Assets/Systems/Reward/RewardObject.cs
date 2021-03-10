using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardObject : MonoBehaviour
{
    public ArtefactRewardUI artefactRewardUI;

    public int index;

    private void OnMouseEnter() {
        artefactRewardUI.SetSelectedIndex(index);
    }
    private void OnMouseExit() {
        artefactRewardUI.SetSelectedIndex(-1
            );
    }
}
