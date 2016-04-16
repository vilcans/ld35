using UnityEngine;
using System.Collections.Generic;

public class GameMain : MonoBehaviour {

    public GameObject[] prefabList;

    public Vector2 startPosition;

    private Dictionary<string, GameObject> prefabs;

    private GameObject player;

    public void Start() {

        prefabs = new Dictionary<string, GameObject>();
        for(int i = 0; i < prefabList.Length; ++i) {
            GameObject obj = prefabList[i];
            prefabs[obj.name] = obj;
        }

        CreateLevel();
        CreatePlayer();
    }

    public void Update() {
        Camera.main.transform.position = new Vector3(
            player.transform.position.x,
            Camera.main.transform.position.y,
            Camera.main.transform.position.z
        );
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
        player = (GameObject)Instantiate(prefabs["Circle"], startPosition, Quaternion.identity);
        player.name = "Player";
        player.AddComponent<Player>();
    }
}
