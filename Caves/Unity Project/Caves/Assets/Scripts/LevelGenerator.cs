using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

	private enum LevelObject {
        NULL,
        WALL
    }

    public GameObject wall;

    public int levelWidth = 100;
    public int levelHeight = 100;
    public float noiseScalar = 0.1f;

    private LevelObject[,] levelArray;

    void Start() {
        Initialize();
        Design();
        Instantiate();
    }

    private void Initialize() {
        levelArray = new LevelObject[levelWidth, levelHeight];
    }

    private void Design() {

        float noiseOffset_x = Random.Range(-100.0f, 100.0f);
        float noiseOffset_y = Random.Range(-100.0f, 100.0f);

        for (int x = 0; x < levelArray.GetLength(0); x++) {
            for (int y = 0; y < levelArray.GetLength(1); y++) {

                float noiseSample_x = (float) x * noiseScalar + noiseOffset_x;
                float noiseSample_y = (float) y * noiseScalar + noiseOffset_y;
                float noiseSample = Mathf.PerlinNoise(noiseSample_x, noiseSample_y);

                if (noiseSample < 0.5f) levelArray[x, y] = LevelObject.WALL;
            }
        }
    }

    private void Instantiate() {
        for (int x = 0; x < levelArray.GetLength(0); x++) {
            for (int y = 0; y < levelArray.GetLength(1); y++) {
                switch (levelArray[x, y]) {
                    case LevelObject.WALL:
                        Instantiate(wall, new Vector3(x, y, 0), Quaternion.identity);
                        break;
                }
            }
        }
    }
}
