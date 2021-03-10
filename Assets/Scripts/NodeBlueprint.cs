using UnityEngine;

namespace Map
{
    public enum NodeType
    {
        MinorEnemy,
        TeamBattle,
        RestSite,
        Treasure,
        Store,
        Boss,
        Mystery,
        FireEnemy,
        WaterEnemy,
        EarthEnemy,
        WindEnemy, 
        IceEnemy,
        ThunderEnemy,
        NatureEnemy,
        ChaosEnemy,
        NeutralEnemy
    }
}

namespace Map
{
    [CreateAssetMenu]
    public class NodeBlueprint : ScriptableObject
    {
        public Sprite sprite;
        public NodeType nodeType;
    }
}