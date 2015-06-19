using UnityEngine;
using System.Collections;
using SimplexNoise;
using System.Collections.Generic;

//not really a Perlin Noise generator anymore
public class WorldGenerator : MonoBehaviour
{
    GameObject world;
    public GameObject grass;
    public GameObject stone;
    public GameObject dirt;
    public GameObject emeraldOre;

    public int chunkSize = 16;
    public int heightLimit = 128;
    public Vector2 WorldSize = new Vector2(2, 2);

    // Use this for initialization
    void Start()
    {

        for (int x = 0; x < WorldSize.x; x++)
        {
            for (int z = 0; z < WorldSize.y; z++)
            {
                GameObject chunk = new GameObject("chunk_x" + x + "_z" + z);
                chunk.transform.parent = transform;
                chunk.transform.position = new Vector3(x * chunkSize, 0f, z * chunkSize);
                GenerateChunk(x, z, chunk);
            }
        }
        GameObject manualChunk = new GameObject("chunk_x" + 10 + "_z" + 10);
        manualChunk.transform.parent = transform;
        manualChunk.transform.position = new Vector3(10 * chunkSize, 0f, 10 * chunkSize);
        GenerateChunk(10, 10, manualChunk);
    }

    void GenerateChunk(int chunkX, int chunkZ, GameObject chunkObject)
    {
        int actualChunkX = chunkX * chunkSize;
        int actualChunkZ = chunkZ * chunkSize;
        //float y;
        GameObject objToMake = stone; //default block stone, if nothing else is selected make this
        GameObject[] LineOfBlocks = new GameObject[512];
        float[] Layer = new float[3];
        bool generate = false;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                //these are probably a bit better for plains-ish biomes
                //explanations of values in these lines
                //argument 1: x+actualChunkX to get the actual in world position of the chunk
                //argument 2: simplex noise method calls this Y, im using this to control how high i want the layers,
                // 0 to get the highest value, at the very button, 300 to get fairly low values resulting in layers at the bottom of the actual world
                // 100 to get an average of a 1 layer thick dirt layer, might change this to 20, looks nice, resulting in multiple layers of dirt, or 0
                // 0 it is, at least for smooth-ish plains
                int stoneHeightBorder = SimplexNoise((x + actualChunkX), 100, (z + actualChunkZ), 10, 3, 1.2f);
                stoneHeightBorder += SimplexNoise((x + actualChunkX), 300, (z + actualChunkZ), 20, 2, 0) + 10; // controls "hills"
                int dirtHeightBorder = SimplexNoise((x + actualChunkX), 40, (z + actualChunkZ), 80, 10, 0) + 3;
                /*
                quite nice values, look for better alternatives though: 
                int stoneHeightBorder = SimplexNoise((x+actualChunkX), 0, (z+actualChunkZ), 10, 3, 1.2f);
                stoneHeightBorder += SimplexNoise((x+actualChunkX), 300, (z+actualChunkZ), 20, 4, 0) + 10; // controls "hills"
                int dirtHeightBorder = SimplexNoise((x+actualChunkX), 100, (z+actualChunkZ), 50, 2, 0) + 1;
                 
                 */
                if (dirtHeightBorder < 1)
                    dirtHeightBorder = 1;

                for (int y = 0; y < heightLimit; y++)
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
                            Debug.Log(f);
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
                        generate = false;
                    }
                    if (generate)
                    {
                        GameObject c = (GameObject)Instantiate(objToMake, new Vector3(actualChunkX + x, y, actualChunkZ + z), Quaternion.identity);
                        c.transform.parent = chunkObject.transform;
                    }
                }
            }
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
        float rValue;
        rValue = SimplexNoiseFloat(x, y, z, scale, height, power);
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
