using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryConditionCheck : MonoBehaviour
{
    public Console[] consoleArray;
    public int mapConditionIndex;
    
    MapManager mapManager;
    

    void Start() {
        mapManager = FindObjectOfType<MapManager>();

        for(int i = 0; i < consoleArray.Length; i ++){
            consoleArray[i].OnActivation += CheckVictoryCondition;
        }
    }

    public void CheckVictoryCondition() {
        if(mapConditionIndex < mapManager.maps.Length && CheckActivation()){
            mapManager.maps[mapConditionIndex].victoryCondition = true;
        }
    }

    public bool CheckActivation(){
        for(int i = 0; i < consoleArray.Length; i ++){
            if(!consoleArray[i].activated){
                return false;
            }
        }
        return true;
    }
}
