using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtefactUI : MonoBehaviour{

    public GameObject artefactListGO;

    public GameObject artefactPrefab;

    public void AddArtefact(Artefact artefact) {
        PlayerResources.instance.artefacts.Add(artefact);
        GameObject go = Instantiate(artefactPrefab, artefactListGO.transform);
        go.GetComponent<Image>().sprite = artefact.sprite;
    }
}
