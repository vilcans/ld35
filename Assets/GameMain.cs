using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;


public class GameMain : MonoBehaviour {

    public static GameMain instance;

    public GameObject[] prefabList;
    public GameObject[] tiles;

    public Vector2 startPosition;

    public AudioSource music;

    private Dictionary<string, GameObject> prefabs;

    private Player player;

    public IEnumerator Start() {
        instance = this;

        music = GameObject.Find("Music").GetComponent<AudioSource>();
        Assert.IsNotNull(music, "No music");

        prefabs = new Dictionary<string, GameObject>();
        AddPrefabs(prefabList);
        AddPrefabs(tiles);
        CreateLevel();
        CreatePlayer();

        yield return new WaitForSeconds(1);
        music.Play();
    }

    public float GetTime() {
        return music.timeSamples / 44100.0f;
    }

    private void AddPrefabs(GameObject[] list) {
        for(int i = 0; i < list.Length; ++i) {
            prefabs[list[i].name] = list[i];
        }
    }

    public void LateUpdate() {
        if(player.alive) {
            Camera cam = Camera.main;
            float xOffset = cam.orthographicSize * cam.aspect * .333f;
            Camera.main.transform.position = new Vector3(
                player.transform.position.x + xOffset,
                Camera.main.transform.position.y,
                Camera.main.transform.position.z
            );
        }
        else {
            if(player.deadTime > 3) {
                SceneManager.LoadScene("Game");
            }
        }
    }

    private void CreateLevel() {
        for(int col = 0; col < MapData.width; ++col) {
            for(int row = 0; row < MapData.height; ++row) {
                byte objectIndex = MapData.tileData[row * MapData.width + col];
                if(objectIndex == 0xff) {
                    continue;
                }
                GameObject newObj = InstantiateByIndex(objectIndex);
                newObj.name += " " + col + "," + row;
                newObj.transform.position = new Vector2(col, MapData.height - row);
            }
        }
    }

    private void CreatePlayer() {
        var obj = (GameObject)Instantiate(prefabs["Player"], startPosition, Quaternion.identity);
        obj.name = "Player";
        player = obj.AddComponent<Player>();
    }

    private GameObject InstantiateByIndex(int tileIndex) {
        GameObject prefab = tiles[tileIndex];
        GameObject newObj = (GameObject)Instantiate(prefab);
        return newObj;
    }
}
