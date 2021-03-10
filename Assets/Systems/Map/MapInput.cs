using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapInput : GameInput
{
    public GameObject battleScene;

    public Battlefield battlefield;

    public ChangeTeamUI changeTeam;

    public bool active;

    public Map.MapPlayerTracker tracker;

    Map.MapNode activeNode;
    public GameObject mapScene;
    public RestUI restUI;
    public ArtefactRewardUI artefact;
    public EnemyLeader enemyLeader;

    private void Start() {
        Invoke("LateStart", 0.2f);
    }

    private void LateStart() {
        battleScene.SetActive(false);
        Invoke("EnableReadInput", 0.2f);
    }

    public void ReturnToMapInput() {
        BattleAction.closeTradeMonster -= ReturnToMapInput;
        Invoke("EnableReadInput", 0.2f);
        lastInput = InputEnum.none;
    }

    private void OpenRest() {
        mapScene.SetActive(false);
        restUI.EnableRest();
    }

    public void PlayerSelectNode(Map.MapNode mapNode) {
        if (!canReadInput) {
            return;
        }
        activeNode = mapNode;
        battlefield.startEnemyMonsters = new List<GameMonster>();

        switch (mapNode.Node.nodeType) {

            case Map.NodeType.RestSite:
                OpenRest();
                return;

            case Map.NodeType.Treasure:
                OpenTreasureReward();
                return;

            case Map.NodeType.ChaosEnemy:
                battlefield.startEnemyMonsters.Add(GlobalData.i.GetEncounter(0, Element.chaos));
                break;

            case Map.NodeType.EarthEnemy:
                battlefield.startEnemyMonsters.Add(GlobalData.i.GetEncounter(0, Element.earth));
                break;

            case Map.NodeType.FireEnemy:
                battlefield.startEnemyMonsters.Add(GlobalData.i.GetEncounter(0, Element.fire));
                break;

            case Map.NodeType.IceEnemy:
                battlefield.startEnemyMonsters.Add(GlobalData.i.GetEncounter(0, Element.ice));
                break;

            case Map.NodeType.NatureEnemy:
                battlefield.startEnemyMonsters.Add(GlobalData.i.GetEncounter(0, Element.nature));
                break;
            case Map.NodeType.NeutralEnemy:
                battlefield.startEnemyMonsters.Add(GlobalData.i.GetEncounter(0, Element.normal));
                break;
            case Map.NodeType.ThunderEnemy:
                battlefield.startEnemyMonsters.Add(GlobalData.i.GetEncounter(0, Element.thunder));
                break;
            case Map.NodeType.WaterEnemy:
                battlefield.startEnemyMonsters.Add(GlobalData.i.GetEncounter(0, Element.water));
                break;
            case Map.NodeType.WindEnemy:
                battlefield.startEnemyMonsters.Add(GlobalData.i.GetEncounter(0, Element.wind));
                break;
            case Map.NodeType.TeamBattle:
                enemyLeader.SetTeam(GlobalData.i.GetTeamEncounter(0));
                break;

        }

        if (mapNode.Node.nodeType != Map.NodeType.TeamBattle) {
            enemyLeader.teamBattle = false; 
        }

        mapScene.SetActive(false);
        battleScene.SetActive(true);
        lastInput = InputEnum.none;
        canReadInput = false;
        BattleAction.startBattle?.Invoke();
    }

    private void OpenTreasureReward() {
        mapScene.SetActive(false);
        artefact.EnableArtefactReward();
    }

    public void CompleteNode() {
        mapScene.SetActive(true);
        battleScene.SetActive(false);
        tracker.SendPlayerToNode(activeNode);
        canReadInput = true;
    }

    protected override void Update() {
        base.Update();
        if (lastInput == InputEnum.cancel) {
            BattleAction.closeTradeMonster += ReturnToMapInput;
            lastInput = InputEnum.none;
            changeTeam.OpenChangeTeamUI();
            canReadInput = false;
        }


    }
}
