using UnityEngine;
using System.Collections;
using SimplexNoise;
using System.Threading;
using System;
using System.Collections.Generic;

public class ChunkGenerator : MonoBehaviour {
    public GameObject grass;
    public GameObject stone;
    public GameObject dirt;
    public GameObject emeraldOre;
    public bool shouldUpdate = false;

    public enum BlockType{
        grass,stone,dirt,emeraldOre
    }
    public struct BlockData
    {
        public bool visibility;
        public bool isSpawned;
        public BlockType type;
        public Vector3 position;
        public bool isAnActualBlock;

        public BlockData(bool visibility, BlockType type, Vector3 pos, bool block)
        {
            this.visibility = visibility;
            this.type = type;
            isSpawned = false;
            position = pos;
            isAnActualBlock = block;
        }
    }

    //x,y,z
    public BlockData[,,] Blocks;
    
    private int chunkSize;

    public void init(int chunkSize, int actualChunkX, int actualChunkZ, bool generateColumnsPerFrame = false)
    {
        this.chunkSize = chunkSize;
        Blocks = new BlockData[chunkSize, 32, chunkSize];
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
        for (int x = 0; x < Blocks.GetLength(0); x++)
        {
            for (int z = 0; z < Blocks.GetLength(2); z++)
            {
                int blocksToShow = 10;

                for (int y = 0; y < Blocks.GetLength(1)-1; y++)
                {
                    BlockData block = Blocks[x, y, z];
                    /*block.visibility = false; // set visibility to false per default, resulting in this block not being spawned

                    //Debug.Log("x:" + x + "y:"+y + "z"+z);

                    //if, else if, else if... i dont like the look of the else if, but ultimately it results in less cpu cycles 
                    //(stopping after the first true, instead of calculating the rest of the if statements too...
                    //could do it all in a single if statement, but this way its slightly seperated

                    //if the block directly above it has a type of null (air),make this block visible
                    if (y + 1 < Blocks.GetLength(1) && !Blocks[x, y + 1, z].isAnActualBlock)
                    {
                        block.visibility = true;
                    }//if one of the adjacent blocks has a type of null (air) make this block visibile
                    else if ((x + 1 < Blocks.GetLength(0) && !Blocks[x + 1, y, z].isAnActualBlock) ||
                             (x - 1 >= 0 && !Blocks[x - 1, y, z].isAnActualBlock))
                    {
                        block.visibility = true;
                    }
                    else if ((z + 1 < Blocks.GetLength(2) && !Blocks[x, y, z + 1].isAnActualBlock) ||
                            (z - 1 >= 0 && !Blocks[x, y, z - 1].isAnActualBlock))
                    {
                        block.visibility = true;
                    }//if the block directly below this one has a type of null (air), make this block visible
                    else if (y - 1 > 0 && !Blocks[x, y - 1, z].isAnActualBlock)
                    {
                        block.visibility = true;
                    }

                    //if visible
                    if (block.visibility)
                        showBlock(block);
                    else
                        hideBlock(block);
                    */
                    if (blocksToShow > 0)
                    {
                        showBlock(block);
                        blocksToShow--;
                        yield return new WaitForEndOfFrame();
                    }
                }
                yield return new WaitForEndOfFrame();
            }

        }

    }

    public void showBlock(BlockData block)
    {
        //Debug.Log("show it");
        GameObject objToSpawn = dirt; //default value, wornt actually be spawned, the default case defined below prevents that
        bool generate = true;

        switch (block.type)
        {
            case BlockType.dirt:
                objToSpawn = dirt;
                break;
            case BlockType.emeraldOre:
                objToSpawn = emeraldOre;
                break;
            case BlockType.grass:
                objToSpawn = grass;
                break;
            case BlockType.stone:
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
                StartCoroutine(CreateColumnsOverTime(stoneHeightBorder, dirtHeightBorder, (x + actualChunkX), (z + actualChunkZ)));

                if (generateColumnsPerFrame)
                    yield return new WaitForEndOfFrame();

            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        //shouldUpdate = true;
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
        BlockType objToMake = BlockType.stone; //default
        bool generate = false;
        int y = stoneHeightBorder + dirtHeightBorder + 1;
        int maxPerFrame = 1;

        while (y > 0)
        {
            for (int spawned = 0; spawned < maxPerFrame && y > 0; spawned++)
            {
                if (y <= stoneHeightBorder)
                {
                    objToMake = BlockType.stone;
                    float f = SimplexNoiseFloat(x, y, z, 10, 2, 0);
                    //1.7 seems reasonable for iron or coal veins
                    //the closer to 2 the rarer stuff is
                    //1.94 seems the highest rarity generated in a 3x3 area of chunks with a chunksize of 32
                    //that might be usefull for diamonds, and 1.95 for emeralds

                    if (f > 1.9)
                    {
                        //Debug.Log(f);
                        objToMake = BlockType.emeraldOre;
                    }
                    generate = true;
                }
                else if (y <= stoneHeightBorder + dirtHeightBorder)
                {
                    objToMake = BlockType.dirt;
                    generate = true;
                }
                else if (y <= stoneHeightBorder + dirtHeightBorder + 1)
                {
                    objToMake = BlockType.grass;
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
                        if (generate)
                        {
                            showBlock(Blocks[_x%chunkSize, y, _z%chunkSize]);

                        }
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
