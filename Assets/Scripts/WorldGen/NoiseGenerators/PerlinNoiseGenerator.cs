using UnityEngine;
using System.Collections;

public class PerlinNoiseGenerator : MonoBehaviour {
    GameObject world;
    public GameObject block;
    public int chunkSize = 16;

	// Use this for initialization
	void Start () {
        int hillModifier = 50;

        for (int x = 0; x < 4; x++)
        {
            for(int z = 0; z < 4; z++){
                GameObject chunk = new GameObject("chunk_x"+x+"_z"+z);
                chunk.transform.parent = transform;
                chunk.transform.position = new Vector3(x * chunkSize,0f, z * chunkSize);
                GenerateChunk(x, z, chunk, hillModifier);
                hillModifier++;
            }
        }
	}

    void GenerateChunk(int chunkX,int chunkZ, GameObject chunkObject, int hillModifier)
    {
        int actualChunkX = chunkX * chunkSize;
        int actualChunkZ = chunkZ * chunkSize;
        float y;
        //for (int layers = 0; layers < 5; layers++)
        //{
            for (int x = 0; x < chunkSize; x++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    y = Mathf.PerlinNoise((float)(x + actualChunkX) / hillModifier/**layers*/, (float)(actualChunkZ + z) / hillModifier/**layers*/);
                    y *= 25/*layers**/;

                    GameObject c = (GameObject)Instantiate(block, new Vector3(actualChunkX + x, y, actualChunkZ + z), Quaternion.identity);
                    c.transform.parent = chunkObject.transform;
                }
            }
        //}
    }
}
