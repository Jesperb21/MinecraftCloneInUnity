using UnityEngine;
using System.Collections;
using SimplexNoise;
using System.Threading;
using System;
using System.Collections.Generic;

//require these components, will crash the game íf those components are not present
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]


public class ChunkGenerator : MonoBehaviour {
    public GameObject grass;
    public GameObject stone;
    public GameObject dirt;
    public GameObject emeraldOre;
    public bool shouldUpdate = false;
    public int airLimit = 64;

    //the actual X Y and Z coords of the chunk, not in "chunk coords"
    //note Y = 0;
    public ChunkPos actualChunkCoords;

    //x,y,z
    public BlockData[,,] Blocks;
    

    private MeshFilter filter;
    private MeshCollider collider;
    private WorldGenerator WG;//increased rendering speed by almost 10%

    private BlockData solid;
    private int tallestPoint;

    private int chunkSize;
    private List<Thread> chunkColumnThreads = new List<Thread>();

    void Update()
    {
        if (shouldUpdate)
        {
            shouldUpdate = false;
            //Render();
        }
    }

    #region creation of the chunk
    public void init(int chunkSize, int actualChunkX, int actualChunkZ, bool generateColumnsPerFrame = false)
    {
        tallestPoint = 0;
        solid = new BlockData(BlockData.BlockType.stone, new Vector3());

        this.chunkSize = chunkSize;
        actualChunkCoords = new ChunkPos(actualChunkX, actualChunkZ);
        filter = gameObject.GetComponent<MeshFilter>();
        collider = gameObject.GetComponent<MeshCollider>();

        WG = GetComponentInParent<WorldGenerator>();

        bool loaded = WorldSaver.LoadChunk(this);
        if (!loaded)
        {

            Blocks = new BlockData[chunkSize, airLimit, chunkSize];

            //fill the entire array of blocks with air
            for (int x = 0; x < chunkSize; x++)
            {

                for (int y = 0; y < airLimit; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        Blocks[x, y, z] = new BlockData(BlockData.BlockType.air, new Vector3((actualChunkX + x), y, (actualChunkZ + z)));
                    }
                }
            }
            CalculateChunk(actualChunkX, actualChunkZ);
        }
    }

    /// <summary>
    /// generates a chunk over time, with build in delays to ensure a playabole performance when generating the chunk
    /// </summary>
    /// <param name="actualChunkX">actual x coords of the chunk</param>
    /// <param name="actualChunkZ">actual z coords of the chunk</param>
    /// <param name="chunkObject">the gameObject to add all the blocks to</param>
    private void CalculateChunk(int actualChunkX, int actualChunkZ)
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

                /*if (actualChunkX < 0)
                    XModifier = -XModifier;
                if (actualChunkZ < 0)
                    ZModifier = -ZModifier;
                */

                int _Z = actualChunkZ + (z * ZModifier);
                int _X = actualChunkX + (x * XModifier);





                //make a new thread to calculate StoneHeightBorder...
                //Thread t = new Thread(() =>
                //{
                    stoneHeightBorder = SimplexNoise(_X, 100, _Z, 10, 3, 1.2f);//controls "hills" in the stone
                    stoneHeightBorder += SimplexNoise(_X, 300, _Z, 20, 2, 0) + 10; // controls the main levels of stone

                //});
                //t.Start();
                //while waiting for this line to be calculated (which is probably faster)
                int dirtHeightBorder = SimplexNoise(_X, 10, _Z, 80, 10, 0.9f) + 10;
                //int dirtHeightBorder = SimplexNoise((x + actualChunkX), 40, (z + actualChunkZ), 80, 10, 0) + 3;
                //and then joining them together
                //t.Join();

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

                if (stoneHeightBorder + dirtHeightBorder + 1 > tallestPoint)
                    tallestPoint = stoneHeightBorder + dirtHeightBorder + 1;

                CalculateChunkColumn(stoneHeightBorder, dirtHeightBorder, (x + actualChunkX), (z + actualChunkZ));

            }

        }
        if (chunkColumnThreads.Count != 0)
        {
            //didn't really change ther performance alot... oh well, i suppose its better than running it all on the UI thread?
            foreach (Thread cCThread in chunkColumnThreads)
            {
                cCThread.Join(); //gather all the columns back to the main thread
            }
            chunkColumnThreads.Clear();
        }


        WorldSaver.SaveChunk(this);
    }

    /// <summary>
    /// generates columns of blocks in each chunk
    /// </summary>
    /// <param name="stoneHeightBorder"how high to place stone</param>
    /// <param name="dirtHeightBorder">how high to place dirt</param>
    /// <param name="x">x position of the column</param>
    /// <param name="z">z position of the column</param>
    private void CalculateChunkColumn(int stoneHeightBorder, int dirtHeightBorder, int x, int z)
    {
        /*Thread th = new Thread(() =>
            {*/
        BlockData.BlockType objToMake = BlockData.BlockType.stone; //default
        bool generate = false;
        int y = stoneHeightBorder + dirtHeightBorder + 1;

        while (y > 0)
        {
            if (y <= stoneHeightBorder)
            {
                objToMake = BlockData.BlockType.stone;
                #region ore gen, should be its own function really, but meh, later
                float f = SimplexNoiseFloat(x, y, z, 10, 2, 0);
                //1.7 seems reasonable for iron or coal veins
                //the closer to 2 the rarer stuff is
                //1.94 seems the highest rarity generated in a 3x3 area of chunks with a chunksize of 32
                //that might be usefull for diamonds, and 1.95 for emeralds

                if (f > 1.9)
                {
                    objToMake = BlockData.BlockType.emeraldOre;
                }
                #endregion
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

            int _x = Mathf.Abs(x % chunkSize);
            int _z = Mathf.Abs(z % chunkSize);
            Blocks[_x, y, _z] = new BlockData(objToMake, new Vector3(x, y, z));
            y--;

        }
        //    });
        //th.Start();
    }
    
    #endregion

    #region rendering of the chunk

    public void Render()
    {
        MeshData meshData = generateMesh();
        renderMesh(meshData);
    }

    //in here lies the issue
    private MeshData generateMesh()
    {
        MeshData meshData = new MeshData();

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {

                for (int y = 0; y <= tallestPoint; y++)
                {
                    meshData = Blocks[x, y, z].Blockdata(this, x, y, z, meshData);

                }
            }
        }

        return meshData;
    }

    private void renderMesh(MeshData meshdata)
    {
        if (this != null && filter != null && collider != null)
        {
            filter.mesh.Clear();
            filter.mesh.vertices = meshdata.vertices.ToArray();
            filter.mesh.triangles = meshdata.triangles.ToArray();

            filter.mesh.uv = meshdata.uv.ToArray();
            filter.mesh.RecalculateNormals();

            collider.sharedMesh = null;
            Mesh mesh = new Mesh();
            mesh.vertices = meshdata.vertices.ToArray();
            mesh.triangles = meshdata.triangles.ToArray();
            mesh.RecalculateNormals();

            collider.sharedMesh = mesh;
        }
    }

    #endregion

    #region manipulation of the chunk
    public void setBlock(int x, int y, int z)
    {
        Blocks[x%chunkSize, y, z%chunkSize].type = BlockData.BlockType.air;
        Render();
        WorldSaver.SaveChunk(this);
    }

    #endregion

    #region getters and setters
    public int GetChunkSize()
    {
        return chunkSize;
    }

    public ChunkGenerator getChunk(int x, int z)
    {
        if (this != null)
        {
            int chunkX = (int)((x+actualChunkCoords.x) /chunkSize);
            int chunkZ = (int)((z+actualChunkCoords.z) / chunkSize);
        
            ChunkGenerator CG = null;

            WG.chunkDictionary.TryGetValue(new ChunkPos(chunkX, chunkZ), out CG);

            if (CG != null)
            {
                return CG;
            }

        }
        return null;
    }


    public BlockData getBlockAt(int x, int y, int z)
    {
        

        //if at the bottom of the game
        if (y < 0)
        {
            return solid;
        }
        else if (y >= airLimit)//if at the air limit of the game
        {
            return solid;
        }


        if (x >= 0 && z >= 0)
        {
            if (x < chunkSize && z < chunkSize)
            {
                if (this != null)
                {
                    int _x = Mathf.Abs(x % chunkSize);
                    int _z = Mathf.Abs(z % chunkSize);

                    return Blocks[_x, y, _z];
                }
            }
        }


        //get the chunk at the specified position //returns this if the block is in this chunk
        ChunkGenerator CG = getChunk(x, z);
        
        if (CG != null)
        {
            int _x = Mathf.Abs(x % chunkSize);
            int _z = Mathf.Abs(z % chunkSize);
            return CG.Blocks[_x, y, _z]; 
        }
        else //if the chunk wasn't found just return an airblock
        {
            return solid;
        }
    }

    #endregion


    #region simplex
    /// <summary>
    /// Simplex Noise generation method, calls SimplexNoiseFloat and rounds the result to nearest integer
    /// </summary>
    /// <param name="x">x coords</param>
    /// <param name="y">y coords</param>
    /// <param name="z">z coords</param>
    /// <param name="scale">controls how smooth the terrain is, lower values makes more noisy terrain, higher terrain makes smooth plains</param>
    /// <param name="height">controls the maximum relative height of mountains</param>
    /// <param name="power">usefull for generating noisier, tall mountain stuffs</param>
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

    #endregion
}
