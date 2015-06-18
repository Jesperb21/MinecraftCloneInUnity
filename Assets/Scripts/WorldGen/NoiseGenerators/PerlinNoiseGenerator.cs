using UnityEngine;
using System.Collections;
using SimplexNoise;
using System.Collections.Generic;

public class PerlinNoiseGenerator : MonoBehaviour {
    GameObject world;
    public GameObject grass;
    public GameObject stone;
    public int chunkSize = 16;
    public int heightLimit = 16;
    public Vector2 WorldSize = new Vector2(2, 2);

	// Use this for initialization
	void Start () {
        int hillModifier = 50;

        for (int x = 0; x < WorldSize.x; x++)
        {
            for(int z = 0; z < WorldSize.y; z++){
                GameObject chunk = new GameObject("chunk_x"+x+"_z"+z);
                chunk.transform.parent = transform;
                chunk.transform.position = new Vector3(x * chunkSize,0f, z * chunkSize);
                GenerateChunk(x, z, chunk, hillModifier);
                hillModifier++;
            }
        }
	}

    void GenerateChunk(int chunkX, int chunkZ, GameObject chunkObject, int hillModifier)
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
                int stoneHeightBorder = SimplexNoise((x+actualChunkX), 0, (z+actualChunkZ), 10, 3, 1.2f);
                stoneHeightBorder += SimplexNoise((x+actualChunkX), 300, (z+actualChunkZ), 20, 2, 0) + 10; // controls "hills"
                int dirtHeightBorder = SimplexNoise((x+actualChunkX), 100, (z+actualChunkZ), 50, 3, 0) + 1;
                /*
                quite nice values, look for better alternatives though: 
                int stoneHeightBorder = SimplexNoise((x+actualChunkX), 0, (z+actualChunkZ), 10, 3, 1.2f);
                stoneHeightBorder += SimplexNoise((x+actualChunkX), 300, (z+actualChunkZ), 20, 4, 0) + 10; // controls "hills"
                int dirtHeightBorder = SimplexNoise((x+actualChunkX), 100, (z+actualChunkZ), 50, 2, 0) + 1;
                 
                 */

                for (int y = 0; y < heightLimit; y++)
                {
                    if (y <= stoneHeightBorder)
                    {
                        objToMake = stone;
                        generate = true;
                    }
                    else if (y <= stoneHeightBorder + dirtHeightBorder)
                    {
                        objToMake = grass;
                        generate = true;
                    }else{
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
    /// Simplex Noise generation method
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
        rValue = Noise.Generate(((float)x) / scale, ((float)y) / scale, ((float)z) / scale);
        rValue *= height;

        if (power != 0)
        {
            rValue = Mathf.Pow(rValue, power);
        }

        return (int)rValue;

    }
}
