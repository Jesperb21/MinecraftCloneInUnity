using UnityEngine;
using System.Collections;
using SimplexNoise;
using System.Collections.Generic;
using System.Threading;

//not really a Perlin Noise generator anymore
public class WorldGenerator : MonoBehaviour
{
    GameObject world;
    public GameObject grass;
    public GameObject stone;
    public GameObject dirt;
    public GameObject emeraldOre;

    public int chunkSize = 4;
    public int radiusToGenerateAroundPlayer = 4;

    private bool generatingChunk = false;
    private bool generatingWorld = false;

    private GameObject player;

    // Use this for initialization
    void Start()
    {
        player = transform.Find("Player").gameObject;
        if (!player)
        {
            Debug.Log("requires a plyer");
        }
    }
    void Update()
    {
        if (player && !generatingChunk && !generatingWorld)
        {
            StartCoroutine(GenerateChunksOverTime());
        }
    }
    IEnumerator GenerateChunksOverTime()
    {
        generatingWorld = true;
        Vector3 playerPos = player.transform.position;
        int playerChunkXPos = (int)(playerPos.x / chunkSize);
        int playerChunkZPos = (int)(playerPos.z / chunkSize);


        for (int x = playerChunkXPos-radiusToGenerateAroundPlayer; x <= playerChunkXPos+radiusToGenerateAroundPlayer; x++)
        {
            for (int z = playerChunkZPos-radiusToGenerateAroundPlayer; z <= playerChunkZPos+radiusToGenerateAroundPlayer; z++)
            {
                while (generatingChunk)
                {
                    //yield return new WaitForSeconds(5f);//wait 5 sec between chunks
                    yield return new WaitForEndOfFrame(); //only wait till next frame between chunks
                }
                if (!transform.Find("chunk_x" + x + "_z" + z))//dont generate the same chunk twice
                {
                    GameObject chunk = new GameObject("chunk_x" + x + "_z" + z);
                    chunk.tag = "Chunk";
                    chunk.transform.parent = transform;
                    chunk.transform.position = new Vector3(x * chunkSize, 0f, z * chunkSize);

                    int actualChunkX = x * chunkSize;
                    int actualChunkZ = z * chunkSize;

                    StartCoroutine(CreateChunksOverTime(actualChunkX, actualChunkZ, chunk));
                }
            }
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            if (obj.tag == "Chunk")
            {
                int x = (int)obj.transform.position.x / chunkSize;
                int z = (int)obj.transform.position.z / chunkSize;
                bool breakIt = true;

                if ((x >= playerChunkXPos-radiusToGenerateAroundPlayer && x <= playerChunkXPos+radiusToGenerateAroundPlayer))
                {
                    if ((z >= playerChunkZPos-radiusToGenerateAroundPlayer && z <= playerChunkZPos+radiusToGenerateAroundPlayer))
                    {
                        breakIt = false;
                    }
                }

                if (breakIt)
                    Destroy(obj);
                
            }
        }
        yield return new WaitForSeconds(1f); //when the world has been generated around the player wait for a second before attempting to create it again
        generatingWorld = false;
    }

    

    /// <summary>
    /// generates chunks over time, attmepting to stay at 60fps while generating chunks
    /// </summary>
    /// <param name="actualChunkX">actual x coords of the chunk</param>
    /// <param name="actualChunkZ">actual z coords of the chunk</param>
    /// <param name="chunkObject">the gameObject to add all the blocks to</param>
    /// <param name="generateColumnsPerFrame">generate columns per frame, if this is set to true it will generate columns, 
    /// if false it will generate rows of columns per frame; generating columns per frame results in the fastest 
    /// appearance of grass blocks, but generating rows per frame (set to false) results in the fastest overall generation
    /// of the terrain, might use more cpu power while generating, but will be faster.... ultimately generating columns 
    /// per frame looks coolest, so i'll enable that per default ;)</param>
    /// <returns>nothing of note, IEnumerator's are used for coroutines</returns>
    IEnumerator CreateChunksOverTime(int actualChunkX, int actualChunkZ, GameObject chunkObject, bool generateColumnsPerFrame = false)
    {
        generatingChunk = true;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                #region comments
                //these are probably a bit better for plains-ish biomes
                //explanations of values in these lines
                //argument 1: x+actualChunkX to get the actual x value in world position of the chunk
                //argument 2: simplex noise method calls this Y, im using this to control how high i want the layers,
                // 0 to get the highest value, at the very button, 300 to get fairly low values resulting in layers at the bottom of the actual world
                // 100 to get an average of a 1 layer thick dirt layer, might change this to 20, looks nice, resulting in multiple layers of dirt, or 0
                // 0 it is, at least for smooth-ish plains
                // argument 3: z + actual z to get the actual z value in the world position of the chunk
                //argument 4: smoothness of the terrain, larger = less noisy
                //argument 5:max height of hills
                //argument 6: exponent, usefull for creating larger cliffs without changing too much on arg 4 & 5 (has the exact same effect though
                #endregion
                
                int stoneHeightBorder = 0;
                //make a new thread to calculate StoneHeightBorder...
                Thread t = new Thread(() =>
                    {
                        stoneHeightBorder = SimplexNoise((x + actualChunkX), 1, (z + actualChunkZ), 10, 3, 1.2f);//controls "hills" in the stone
                        stoneHeightBorder += SimplexNoise((x + actualChunkX), 3, (z + actualChunkZ), 20, 2, 0) + 1; // controls the main levels of stone
                        //stoneHeightBorder = SimplexNoise((x + actualChunkX), 100, (z + actualChunkZ), 10, 3, 1.2f);//controls "hills" in the stone
                        //stoneHeightBorder += SimplexNoise((x + actualChunkX), 300, (z + actualChunkZ), 20, 2, 0) + 10; // controls the main levels of stone
                    });
                t.Start();
                //while waiting for this line to be calculated (which is probably faster)
                int dirtHeightBorder = SimplexNoise((x + actualChunkX), 4, (z + actualChunkZ), 80, 1, 0) + 3;
                //int dirtHeightBorder = SimplexNoise((x + actualChunkX), 40, (z + actualChunkZ), 80, 10, 0) + 3;
                //and then joining them together
                t.Join();

                //this is all done to because both calculations are some fairly heavy calculations, doing them this way seperates them 
                //out onto different CPU cores... probably not any noticeable performance increase... i get the feeling it would actually
                //use more cpu power to generate the new thread than it would to run both calculations on the same core, but this is a 
                //attempt to make the simplex noise generation slightly more efficient

                #region fancy values
                /*
                quite nice values, look for better alternatives though: 
                int stoneHeightBorder = SimplexNoise((x+actualChunkX), 0, (z+actualChunkZ), 10, 3, 1.2f);
                stoneHeightBorder += SimplexNoise((x+actualChunkX), 300, (z+actualChunkZ), 20, 4, 0) + 10; // controls "hills"
                int dirtHeightBorder = SimplexNoise((x+actualChunkX), 100, (z+actualChunkZ), 50, 2, 0) + 1;
                 
                 */
                #endregion

                if (dirtHeightBorder < 1)
                    dirtHeightBorder = 1;
                
                //generate columns, over time so it doesn't freeze the entire pc while its generating
                StartCoroutine(CreateColumnsOverTime(stoneHeightBorder, dirtHeightBorder, (x + actualChunkX), (z + actualChunkZ), chunkObject));

                if (generateColumnsPerFrame) 
                    yield return new WaitForEndOfFrame();

            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        generatingChunk = false;
    }

    IEnumerator CreateColumnsOverTime(int stoneHeightBorder, int dirtHeightBorder, int x, int z, GameObject chunkObject)
    {
        GameObject objToMake;
        bool generate = false;
        int y = stoneHeightBorder+dirtHeightBorder+1;
        int maxPerFrame = 1;

        while (y > 0)
        {
            for (int spawned = 0; spawned < maxPerFrame && y > 0; spawned++)
            {
                if (y <= stoneHeightBorder)
                {
                    objToMake = stone;
                    float f = SimplexNoiseFloat(x, y, z, 10, 2, 0);
                    //1.7 seems reasonable for iron or coal veins
                    //the closer to 2 the rarer stuff is
                    //1.94 seems the highest rarity generated in a 3x3 area of chunks with a chunksize of 32
                    //that might be usefull for diamonds, and 1.95 for emeralds

                    if (f > 1.9)
                    {
                        //Debug.Log(f);
                        objToMake = emeraldOre;
                    }
                    generate = true;
                }
                else if (y <= stoneHeightBorder + dirtHeightBorder)
                {
                    objToMake = dirt;
                    generate = true;
                }
                else if (y <= stoneHeightBorder + dirtHeightBorder + 1)
                {
                    objToMake = grass;
                    generate = true;
                }
                else
                {
                    objToMake = null;
                    generate = false;
                }
                if (generate)
                {
                    if (chunkObject)
                    {
                        GameObject c = (GameObject)Instantiate(objToMake, new Vector3(x, y, z), Quaternion.identity);
                        c.transform.parent = chunkObject.transform;
                    }
                    else
                    {
                        //parent dissappeared, stop generating more
                        y = 0;
                        spawned = maxPerFrame;
                    }
                }
                y--;
            }
            float randomDelay = Random.Range(0.1f, 1f);
            yield return new WaitForSeconds(randomDelay);
            yield return new WaitForEndOfFrame();
        }

    }

    /// <summary>
    /// Simplex Noise generation method, calls SimplexNoiseFloat and rounds the result to nearest integer
    /// </summary>
    /// <param name="x">x coords</param>
    /// <param name="y">y coords</param>
    /// <param name="z">z coords</param>
    /// <param name="scale">controls how smooth the terrain is, lower values makes more noisy terrain, higher terrain makes smooth plains</param>
    /// <param name="height">controls the maximum relative height of mountains</param>
    /// <param name="power">usefull for generating noisier, tall mountain stuffs</param>
    /// <returns>the calculated simplex noise value as an integer</returns>
    public int SimplexNoise(int x, int y, int z, float scale, float height, float power)
    {
        float rValue = SimplexNoiseFloat(x, y, z, scale, height, power);
        return Mathf.RoundToInt(rValue);
    }
    //simplex noise generation method, returns a float
    public float SimplexNoiseFloat(int x, int y, int z, float scale, float height, float power)
    {
        float rValue;
        rValue = Noise.Generate(((float)x) / scale, ((float)y) / scale, ((float)z) / scale);
        rValue *= height;

        if (power != 0)
        {
            rValue = Mathf.Pow(rValue, power);
        }

        return rValue;
    }
}
