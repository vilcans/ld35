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
        Camera cam = Camera.main;
        float xOffset = cam.orthographicSize * cam.aspect * .333f;
        Camera.main.transform.position = new Vector3(
            player.transform.position.x + xOffset,
            Camera.main.transform.position.y,
            Camera.main.transform.position.z
        );
    }

    private void CreateLevel() {
        for(int col = 0; col < MapData.width; ++col) {
            for(int row = 0; row < MapData.height; ++row) {
                byte objectIndex = MapData.tileData[row * MapData.width + col];
                if(objectIndex == 0xff) {
                    continue;
                }
                GameObject newObj = InstantiateByIndex(objectIndex);
                newObj.transform.position = new Vector2(col, MapData.height - row);
            }
        }
    }

    private void CreatePlayer() {
        player = (GameObject)Instantiate(prefabs["Square"], startPosition, Quaternion.identity);
        player.name = "Player";
        player.AddComponent<Player>();
    }

    private GameObject InstantiateByIndex(int objectIndex) {
        GameObject prefab = prefabList[objectIndex];
        GameObject newObj = (GameObject)Instantiate(prefab);
        if(prefab.name == "CirclePickup") {
            /*ShapePickup pickup =*/ newObj.AddComponent<ShapePickup>();
        }
        return newObj;
    }
}
