using UnityEngine;
using System.Collections.Generic;

public class GameMain : MonoBehaviour {

    public GameObject[] prefabList;

    public Vector2 startPosition;

    private Dictionary<string, GameObject> prefabs;

    public void Start() {

        prefabs = new Dictionary<string, GameObject>();
        for(int i = 0; i < prefabList.Length; ++i) {
            GameObject obj = prefabList[i];
            prefabs[obj.name] = obj;
        }

        CreateLevel();
        CreatePlayer();
    }

    private void CreateLevel() {
        GameObject floorPrefab = prefabs["Square"];
        for(int col = 0; col < 4; ++col) {
            GameObject newObj = (GameObject)Instantiate(
                floorPrefab,
                new Vector2(col, 0),
                Quaternion.identity
            );
        }
    }

    private void CreatePlayer() {
        GameObject player = (GameObject)Instantiate(prefabs["Circle"], startPosition, Quaternion.identity);
        player.name = "Player";
        player.AddComponent<Player>();
    }
}
