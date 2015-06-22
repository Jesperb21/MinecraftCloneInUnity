using UnityEngine;
using System.Collections;
using SimplexNoise;
using System.Threading;
using System;
using System.Collections.Generic;

//require these components, will automatically get added if they're not on this object already
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]


public class ChunkGenerator : MonoBehaviour {
    public GameObject grass;
    public GameObject stone;
    public GameObject dirt;
    public GameObject emeraldOre;
    public bool shouldUpdate = false;
    public int airLimit = 32;

    public Vector3 actualChunkCoords;

    //x,y,z
    public BlockData[,,] Blocks;
    

    private MeshFilter filter;
    private MeshCollider collider;

    private int chunkSize;

    public void init(int chunkSize, int actualChunkX, int actualChunkZ, bool generateColumnsPerFrame = false)
    {
        filter = gameObject.GetComponent<MeshFilter>();
        collider = gameObject.GetComponent<MeshCollider>();

        this.chunkSize = chunkSize;
        Blocks = new BlockData[chunkSize, airLimit, chunkSize];
        for (int x = 0; x < chunkSize; x++ )
        {
            for (int y = 0; y < airLimit; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    Blocks[x, y, z] = new BlockData(false, BlockData.BlockType.air, new Vector3((actualChunkX + x), y, (actualChunkZ + z)), false);
                }
            }
        }
        actualChunkCoords = new Vector3(actualChunkX, 0, actualChunkZ);
        StartCoroutine(CreateChunksOverTime(actualChunkX, actualChunkZ, generateColumnsPerFrame));
    }

    void Update() { 
        if(shouldUpdate)
        {
            shouldUpdate = false;
            StartCoroutine(updateChunk());
        }
    }
    

    IEnumerator updateChunk()
    {
        MeshData meshData = new MeshData();
        for (int x = 0; x < Blocks.GetLength(0); x++)
        {
            for (int z = 0; z < Blocks.GetLength(2); z++)
            {

                for (int y = 0; y < Blocks.GetLength(1); y++)
                {
                    BlockData block = Blocks[x, y, z];
                    
                    meshData = block.Blockdata(this, x, y, z, meshData);
                    
                }
            }

        }
        yield return new WaitForEndOfFrame();
        renderMesh(meshData);
    }

    private void renderMesh(MeshData meshdata)
    {
        filter.mesh.Clear();
        filter.mesh.vertices = meshdata.vertices.ToArray();
        filter.mesh.triangles = meshdata.triangles.ToArray();

        filter.mesh.uv = meshdata.uv.ToArray();
        filter.mesh.RecalculateNormals();

        collider.sharedMesh = filter.mesh;
        collider.sharedMesh.RecalculateNormals();

    }

    public ChunkGenerator getChunk(int x, int z)
    {
        //if x & z coords are out of the range of this chunk, get the chunk needed
        if ((x < actualChunkCoords.x || x >= actualChunkCoords.x + chunkSize) ||
            (z < actualChunkCoords.z || x >= actualChunkCoords.z + chunkSize)){
                try
                {
                    return transform.parent.Find("chunk_x" + ((int)x % chunkSize) + "_z" + ((int)z % chunkSize)).gameObject.GetComponent<ChunkGenerator>();
                }
                catch
                {
                    return null;
                }
        }else{
            return this;
        }
    }


    public BlockData getBlockAt(int x, int y, int z)
    {
        int _x = Mathf.Abs(x % chunkSize);
        int _z = Mathf.Abs(z % chunkSize);
        if (y < 0)
        {
            return new BlockData(false, BlockData.BlockType.air, new Vector3(x, y, z), false);
        }
        else if (y >= airLimit)
        {
            return new BlockData(false, BlockData.BlockType.air, new Vector3(x, y, z), false);
        }

        //get the chunk at the specified position //returns this if the block is in this chunk
        ChunkGenerator CG = getChunk(x, z);
        
        if (CG != null)
        {
            return CG.Blocks[_x, y, _z]; 
        }
        else //if the chunk wasn't found just return an airblock
        {
            return new BlockData(false, BlockData.BlockType.air, new Vector3(x, y, z), false);
        }
    }

    public void showBlock(BlockData block)
    {
        //Debug.Log("show it");
        GameObject objToSpawn = dirt; //default value, wornt actually be spawned, the default case defined below prevents that
        bool generate = true;

        switch (block.type)
        {
            case BlockData.BlockType.dirt:
                objToSpawn = dirt;
                break;
            case BlockData.BlockType.emeraldOre:
                objToSpawn = emeraldOre;
                break;
            case BlockData.BlockType.grass:
                objToSpawn = grass;
                break;
            case BlockData.BlockType.stone:
                objToSpawn = stone;
                break;
            default:
                generate = false;
                break;
        }

        if (!block.isSpawned && generate)
        {
            //spawn new block here
            GameObject c = (GameObject)Instantiate(objToSpawn, block.position, Quaternion.identity);
            c.name = "b_x" + block.position.x + "_y" + block.position.y + "_z" + block.position.z;
            c.transform.parent = gameObject.transform;
            block.isSpawned = true;
        }

    }
    public void hideBlock(BlockData block)
    {
        if(block.isSpawned)
        {
            //break block

            //mercilessly destroy the gameobject!! muahahaha!!.. no really... i judged that implementing the same primitive object pooling thing
            //i did for chunks on block level would require too many resources, and this is after all single blocks we're talking about, so 
            //re instantiating it would probably not require that many resources
            Destroy(transform.Find("b_x" + block.position.x + "_y" + block.position.y + "_z" + block.position.z).gameObject);
            block.isSpawned = false;
        }
    }


    /// <summary>
    /// generates a chunk over time, with build in delays to ensure a playabole performance when generating the chunk
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
    IEnumerator CreateChunksOverTime(int actualChunkX, int actualChunkZ, bool generateColumnsPerFrame = false)
    {
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
                int XModifier = 1; //converted to negative if the chunk's X is a negative value
                int ZModifier = 1; //converted to negative if the chunk's Z is a negative value

                if (actualChunkX < 0)
                    XModifier = -XModifier;
                if (actualChunkZ < 0)
                    ZModifier = -ZModifier;
                
                int _Z = ZModifier*(z + Mathf.Abs(actualChunkZ));
                int _X = XModifier*(x + Mathf.Abs(actualChunkX));

                //make a new thread to calculate StoneHeightBorder...
                Thread t = new Thread(() =>
                {
                    stoneHeightBorder = SimplexNoise(_X, 100, _Z, 10, 3, 1.2f);//controls "hills" in the stone
                    stoneHeightBorder += SimplexNoise(_X, 300, _Z, 20, 2, 0) + 10; // controls the main levels of stone
                    //stoneHeightBorder = SimplexNoise((x + actualChunkX), 100, (z + actualChunkZ), 10, 3, 1.2f);//controls "hills" in the stone
                    //stoneHeightBorder += SimplexNoise((x + actualChunkX), 300, (z + actualChunkZ), 20, 2, 0) + 10; // controls the main levels of stone
                });
                t.Start();
                //while waiting for this line to be calculated (which is probably faster)
                int dirtHeightBorder = SimplexNoise(_X, 40, _Z, 80, 10, 0) + 3;
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
                StartCoroutine(CreateColumnsOverTime(stoneHeightBorder, dirtHeightBorder, (x + actualChunkX), (z + actualChunkZ)));

                if (generateColumnsPerFrame)
                    yield return new WaitForEndOfFrame();

            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        shouldUpdate = true;
    }

    /// <summary>
    /// generates columns of blocks in each chunk
    /// </summary>
    /// <param name="stoneHeightBorder"how high to place stone</param>
    /// <param name="dirtHeightBorder">how high to place dirt</param>
    /// <param name="x">x position of the column</param>
    /// <param name="z">z position of the column</param>
    /// <param name="chunkObject">which chunk object to add the column to</param>
    /// <returns>nothing of note, only there to be able to use delays in here</returns>
    IEnumerator CreateColumnsOverTime(int stoneHeightBorder, int dirtHeightBorder, int x, int z)
    {
        BlockData.BlockType objToMake = BlockData.BlockType.stone; //default
        bool generate = false;
        int y = stoneHeightBorder + dirtHeightBorder + 1;
        int maxPerFrame = 1;

        while (y > 0)
        {
            for (int spawned = 0; spawned < maxPerFrame && y > 0; spawned++)
            {
                if (y <= stoneHeightBorder)
                {
                    objToMake = BlockData.BlockType.stone;
                    float f = SimplexNoiseFloat(x, y, z, 10, 2, 0);
                    //1.7 seems reasonable for iron or coal veins
                    //the closer to 2 the rarer stuff is
                    //1.94 seems the highest rarity generated in a 3x3 area of chunks with a chunksize of 32
                    //that might be usefull for diamonds, and 1.95 for emeralds

                    if (f > 1.9)
                    {
                        //Debug.Log(f);
                        objToMake = BlockData.BlockType.emeraldOre;
                    }
                    generate = true;
                }
                else if (y <= stoneHeightBorder + dirtHeightBorder)
                {
                    objToMake = BlockData.BlockType.dirt;
                    generate = true;
                }
                else if (y <= stoneHeightBorder + dirtHeightBorder + 1)
                {
                    objToMake = BlockData.BlockType.grass;
                    generate = true;
                }
                else
                {
                    generate = false;
                }
                //if (generate)
                //{
                    if (gameObject)//probably not necessary with this if, this i used before i seperated the chunk gen to a different class
                    {
                        int _x = Mathf.Abs(x);
                        int _z = Mathf.Abs(z);
                        Blocks[_x%chunkSize,y,_z%chunkSize] = new BlockData(false, objToMake, new Vector3(x,y,z), generate);
                        /*if (generate)
                        {
                            showBlock(Blocks[_x%chunkSize, y, _z%chunkSize]);

                        }*/
                        //GameObject c = (GameObject)Instantiate(objToMake, new Vector3(x, y, z), Quaternion.identity);
                        //c.transform.parent = gameObject.transform;
                    }
                    else
                    {
                        //parent dissappeared, stop generating more
                        y = 0;
                        spawned = maxPerFrame;
                    }
                //}
                y--;
            }
            float randomDelay = UnityEngine.Random.Range(0.1f, 1f);
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
