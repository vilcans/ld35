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

    public void LateUpdate() {
        Camera.main.transform.position = new Vector3(
            player.transform.position.x,
            Camera.main.transform.position.y,
            Camera.main.transform.position.z
        );
    }

    private void CreateLevel() {
        //GameObject floorPrefab = prefabs["Square"];
        for(int col = 0; col < MapData.width; ++col) {
            for(int row = 0; row < MapData.height; ++row) {
                byte objectIndex = MapData.tileData[row * MapData.width + col];
                if(objectIndex == 0xff) {
                    continue;
                }
                GameObject newObj = (GameObject)Instantiate(
                    prefabList[objectIndex],
                    new Vector2(col, MapData.height - row),
                    Quaternion.identity
                );
            }
        }
    }

    private void CreatePlayer() {
        player = (GameObject)Instantiate(prefabs["Circle"], startPosition, Quaternion.identity);
        player.name = "Player";
        player.AddComponent<Player>();
    }
}
