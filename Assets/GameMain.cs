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
    public AudioClip deathSound;

    public Dictionary<string, GameObject> prefabs;

    //private const int cheatColumn = 320;
    private const int cheatColumn = 376;
    //private const int cheatColumn = 0;

    private Player player;

    public float deadTime;

    private float startDelay = .5f;
    private float restartDelay = 1.5f;

    private double? dspStartTime;

    public IEnumerator Start() {
        instance = this;

        music = GameObject.Find("Music").GetComponent<AudioSource>();
        Assert.IsNotNull(music, "No music");

        prefabs = new Dictionary<string, GameObject>();
        AddPrefabs(prefabList);
        AddPrefabs(tiles);
        CreateLevel();
        CreatePlayer();

        yield return new WaitForSeconds(startDelay);

        music.Play();
        dspStartTime = AudioSettings.dspTime;

        if(cheatColumn != 0) {
            float advanceTime = (cheatColumn - 8) * Motion.secondsPerSquare;
            music.timeSamples = (int)(advanceTime * AudioSettings.outputSampleRate);
            dspStartTime -= advanceTime;
        }
    }

    public float GetTime() {
        if(dspStartTime == null) {
            return 0;
        }
        //return music.timeSamples / 44100.0f;
        var t = (float)(AudioSettings.dspTime - dspStartTime);
        return t;
    }

    private void AddPrefabs(GameObject[] list) {
        for(int i = 0; i < list.Length; ++i) {
            prefabs[list[i].name] = list[i];
        }
    }

    public void LateUpdate() {
        if(player.state == Player.State.Playing) {
            Camera cam = Camera.main;
            float xOffset = cam.orthographicSize * cam.aspect * .333f;
            Camera.main.transform.position = new Vector3(
                player.transform.position.x + xOffset,
                Camera.main.transform.position.y,
                Camera.main.transform.position.z
            );
        }
        else {
            deadTime += Time.deltaTime;
            if(deadTime >= restartDelay) {
                SceneManager.LoadScene(player.state == Player.State.Won ? "Ending" : "Game");
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void OnDeath() {
        music.Stop();
        music.PlayOneShot(deathSound);
    }

    private void CreateLevel() {
        for(int col = 0; col < MapData.width; ++col) {
            for(int row = 0; row < MapData.height; ++row) {
                byte objectIndex = MapData.tileData[row * MapData.width + col];
                if(col < cheatColumn) {
                    objectIndex = row == 7 ? (byte)2 : (byte)0xff;
                }
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
        player = obj.GetComponent<Player>();
        Assert.IsNotNull(player);
    }

    private GameObject InstantiateByIndex(int tileIndex) {
        GameObject prefab = tiles[tileIndex];
        GameObject newObj = (GameObject)Instantiate(prefab);
        return newObj;
    }
}
