# Tutorial-ProceduralCaves
###Procedurally Generating 2D Caves in Unity using libnoise
---
#####What we're going to be making:
	<screenshot>
 
#####What topics we're going to be covering:
* Procedural generation basics using Unity's built-in Perlin noise
* Integration of the libnoise library
* Switching from Unity's Perlin noise to libnoise's RidgedMulti noise
* Flood Fill Algorithm
 
#####What tools we're going to be using:
* Unity 5.3.4
* libnoise
* Any basic image editor

---
#####Getting Started

Our objective here is to create a single, hopefully somewhat interesting cave, for use in either a top-down or platformer game. 

When doing any sort of level generation, I usually start with the following high-level design:
* (Initialize) Create an array of some predetermined size
* (Design) Populate the array with values that will later be transformed into objects
* (Instantiate) Walk the array

To start, we'll need to setup our Unity project, create a "LevelGenerator" script, attach it to an object in our scene, and create a single 1x1 tile prefab. The tile prefab can just have a SpriteRenderer with a square solid-color sprite, just ensure it's 1x1 size in Unity space.
	<screenshot 2>

---
#####Building the LevelGenerator Script

In the LevelGenerator script, we're going to:
* Create a "LevelObject" enum
* Add levelWidth and levelHeight fields
* Add the levelArray field (2d array of LevelObject)
* Add methods for Initialize, Design, and Instantiate
* Call the three methods from Unity's "Start()" method
* Initialize the levelArray field in our "Initialize()" method

It should look something like this:

```C#
using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

	private enum LevelObject {
        NULL,
        WALL
    }

    public int levelWidth = 100;
    public int levelHeight = 100;

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

    }

    private void Instantiate() {

    }
}
```
Let's go ahead and build the Instantiate() method since it's relatively straightforward. For each LevelObject enum value, we will need a corresponding GameObject prefab to instantiate. Generally speaking, I like to use an ObjectFactory to handle all of my instantiating, but for the sake of simplicity, we're just going to instantiate from within our LevelGenerator script since we only have the one object to deal with. Add the "public GameObject wall;" field.

Now we're going to walk the 2D levelArray, and if it's LevelObject enum value is not null, instantiate the corresponding GameObject via a switch statement at the arrays [x,y] location.

After those two additions we should have something like this:

```c#
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
```

Now onto the "Design()" method. This method is in charge of defining what objects make up our level array. We're just going to start off with some basic Perlin noise. This method is available by default through the method "Mathf.PerlinNoise(float x, float y)" which takes an [x,y] coordinate, and outputs a float in the range 0.0-1.0. The general thought here is to walk through our 2D array, get the noise sample for each [x,y] coordinate, and if it is less than some value (0.5 to keep things simple), that means we have a wall. One other thing we have to do is scale the noise based on what we want. Based on a 100x100 level size, I found 0.1 to be a good noise scalar. When we go lower, we are "zooming in" and have larger blobs of noise, when we "zoom out" to 0.8, the levelArray starts to look like white noise.

This is what our code should look like now. It should be runnable at this point, just remember to add the tile prefab as the "Wall" object in the LevelGenerator. I've also zoomed my camera out to a size of 60, and moved it to position [50,50,-10].

```c#
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
        for (int x = 0; x < levelArray.GetLength(0); x++) {
            for (int y = 0; y < levelArray.GetLength(1); y++) {

                float noiseSample_x = (float) x * noiseScalar;
                float noiseSample_y = (float) y * noiseScalar;
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
```
<screenshot 3>

You might notice that repeated runs of the program yield the same level layout, this is because Unity's PerlinNoise is not randomized. We can fix this by adding a random offset to our noise sample:
```C#
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
```
<screenshot 4>



#####More Interesting Noise: Integrating the libnoise Library