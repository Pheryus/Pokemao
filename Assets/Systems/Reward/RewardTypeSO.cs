using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RewardType", menuName = "New Reward")]
public class RewardTypeSO : ScriptableObject {

    public string rewardName;

    [TextArea(3, 10)]
    public string info;

}
