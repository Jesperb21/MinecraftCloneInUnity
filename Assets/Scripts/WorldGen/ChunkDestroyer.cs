using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ChunkGenerator))]
public class ChunkDestroyer : MonoBehaviour {
    public float timeframe = 10f;

    void OnEnable()
    {
        //prevent it from destroying if it was enabled within the timeframe allowed
        CancelInvoke();
    }

    void OnDisable()
    {
        //destroy after the timeframe allowed unless enabled again
        Invoke("Destroy", timeframe);
    }


    void Destroy()
    {
        CancelInvoke(); //safe guard to make sure it only destroys once
        ChunkGenerator CG = GetComponent<ChunkGenerator>();
        ChunkPos v = CG.actualChunkCoords;
        int x = (int)v.x / CG.GetChunkSize();
        int z = (int)v.z / CG.GetChunkSize();
        ChunkPos CP = new ChunkPos(x, z);

        GetComponentInParent<WorldGenerator>().chunkDictionary.Remove(CP);
        Destroy(gameObject);
    }
}
