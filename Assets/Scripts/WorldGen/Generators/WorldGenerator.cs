using UnityEngine;
using System.Collections;
using SimplexNoise;
using System.Collections.Generic;
using System.Threading;
using System;

//not really a Perlin Noise generator anymore
public class WorldGenerator : MonoBehaviour
{
    GameObject world;
    public GameObject chunk;

    //dictionaries are fancy as fuck, pardon the language; They're kindda like hashmaps, but on the same amount of steroids as lists are in comparison to arrays (well arraylists)
    //a dictionary is basicly a...well a dictionary... someone has been smart when naming this one xD ...
    //2 important key differences this collection type has in comparison to hashmaps are: no casting needed, i like that; and significant performance increase
    // performance increase is always a good thing :)
    // why not an array? because i dont want to define the entire array at once, kindda ruins the whole idea of procedurally generated worlds
    // why not a list then? well, because i want mapping between the world position and the chunkgenerator... which i probably should rename to chunk
    // why not hashmap? because dictionaries are fancy... honestly i like defining what content it can have before i use it, sorta eliminating any misunderstandings
    //in the future, and no casting required all the time
    public Dictionary<ChunkPos, ChunkGenerator> chunkDictionary= new Dictionary<ChunkPos, ChunkGenerator>();

    public int chunkSize = 4;
    public int radiusToGenerateAroundPlayer = 4;

    private bool generatingChunk = false;
    private bool generatingWorld = false;

    private Vector3 PlayerPos;
    private bool PlayerPosHasChanged = true;

    private enum dir { xUp, yUp, xDown, yDown };

    private GameObject player;

    private bool DimensionActive;
    private bool updatingWorld;

    public int spiralsInProgress = 0;

    public bool loadedUp = true;//pre-loading the map didn't help the rendering speed

    // Use this for initialization
    void Start()
    {

        if (transform.Find("Player"))
        {
            player = transform.Find("Player").gameObject;
        }
        else
        {

            Debug.Log("requires a player");
        }
    }
    void Update()
    {
        if (DimensionActive)
        {
            if(!updatingWorld) //why not just call UpdateWorld you may ask? because i dont want to rerun every single frame, only once a second
                StartCoroutine(UpdateWorld());
        }
        else
        {
            DimensionActive = gameObject.activeInHierarchy;
        }
    }

    IEnumerator UpdateWorld()
    {
        updatingWorld = true;
        if (!player)
        {
            if (transform.Find("Player")) // if player hasn't been defined
            {
                player = transform.Find("Player").gameObject;
            }
        }
        // if player is defined, changed position, not generating chunks and not generating the world right now
        if (player && PlayerPosHasChanged && !generatingChunk && !generatingWorld) 
        {
            PlayerPosHasChanged = false;
            Debug.Log("world generation has been restarted");
            spiralsInProgress++;
            StartCoroutine(GenerateChunksInASpiral()); // <--------------- start generating new chunks
        }
        else if //if the players position has changed away from the position stored in PlayerPos 
            (player &&
            (int)(PlayerPos.x/chunkSize) != (int)(player.transform.position.x/chunkSize) &&
            (int)(PlayerPos.z/chunkSize) != (int)(player.transform.position.z/chunkSize))
        {
            //change the boolean PlayerPosHasChanged to true, to allow the world gen to know to stop all generation and restart it... 
            //might make issues when the player runs in a single direction too fast for the world to be generated fast enough, oh well
            PlayerPosHasChanged = true;
            Debug.Log("players position has changed, restart world gen");
            PlayerPos = player.transform.position;
        }
        else
        {
            yield return new WaitForSeconds(5f);
        }
        updatingWorld = false;
    }

    IEnumerator GenerateChunksInASpiral()
    {
        generatingWorld = true;
        Vector3 playerPos = player.transform.position;
        //position of the player in "Chunk coords"
        int playerChunkXPos = (int)(playerPos.x / chunkSize);
        int playerChunkZPos = (int)(playerPos.z / chunkSize);
        
        //spiral loop variables
        Vector2 loopPos = new Vector2(playerChunkXPos, playerChunkZPos);
        dir currentDir = dir.xUp;
        bool addAnotherIteration = false;

        //loop that every time it runs through changes direction, every 2nd time it runs through adds another block to the end of the line, 
        //and every 4th time ( thisloop/2 ) knows its made another layer
        //this results in something like http://gyazo.com/c8daf4c2a000e11d4ac004e00d9ad21b ... hopefully
        //gif from before fuckup #86 http://gyazo.com/1d8ffa8b586a99576c44f41491146eee
        for (int thisLoop = 0; (thisLoop/2) <= radiusToGenerateAroundPlayer; )
        {

            //create lines of chunks in the direction of 'currentDir', this is results in a spiral and player didn't change pos
            for (int delta = 0; delta <= thisLoop && !PlayerPosHasChanged; delta++)
            {
                #region generateChunkAtPos
                //wait while it finishes generating chunks
                while(generatingChunk)
                {
                    yield return new WaitForEndOfFrame();
                }
                ChunkPos pos = new ChunkPos((int)loopPos.x, (int)loopPos.y);
                if (loadedUp)// load the world up before rendering
                    CreateChunkAndRender(pos);
                else
                    CreateChunk(pos);

                    yield return new WaitForEndOfFrame();
                

                
                #endregion


                //change position of next chunk depending on the direction the loop wants it to spawn in
                switch (currentDir)
                {
                    case dir.xDown:
                        loopPos.x--;
                        break;
                    case dir.xUp:
                        loopPos.x++;
                        break;
                    case dir.yDown:
                        loopPos.y--;
                        break;
                    case dir.yUp:
                        loopPos.y++;
                        break;
                }
                //yield return new WaitForSeconds(0.2f);
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();

            //add another iteration to this loop every 2nd time it runs through
            if (addAnotherIteration)
            {
                thisLoop++;
                addAnotherIteration = false;
            }
            else
            {
                addAnotherIteration = true;
            }

            //switch direction of the loop
            if ((int)currentDir == 3)
                currentDir = 0;
            else
                currentDir++;
        }
        deleteOldChunks(playerChunkXPos, playerChunkZPos);

        yield return new WaitForSeconds(1f); //when the world has been generated around the player wait for a second before attempting to create it again
        if (!loadedUp)
            PlayerPosHasChanged = true;//make it re-run the loadworld when the map has been generated

        loadedUp = true;
        generatingWorld = false;
    }

    /// <summary>
    /// create and render the chunk
    /// </summary>
    /// <param name="pos">position of the chunk to create & render</param>
    public void CreateChunkAndRender(ChunkPos pos)
    {
        CreateChunk(pos);
        StartCoroutine(RenderChunk(pos));
        
    }


    //note i dont want this to be a co-routine because... well... its fairly simple
    /// <summary>
    /// create a chunk if it doesn't exist
    /// </summary>
    /// <param name="pos">position of the chunk</param>
    public void CreateChunk(ChunkPos pos)
    {
        ChunkGenerator chunkGen = null;

        chunkDictionary.TryGetValue(pos, out chunkGen);


        if (chunkGen == null)
        {

            //instantiate a chunk at the calculated position
            
            GameObject chunkObj = (GameObject)Instantiate(chunk, new Vector3(pos.x * chunkSize, 0f, pos.z * chunkSize), Quaternion.identity);
            chunkObj.transform.parent = transform;

            int actualChunkX = pos.x * chunkSize;
            int actualChunkZ = pos.z * chunkSize;

            //add the chunk object spawned to the dictionary... well the ChunkGenerator script part of the object anyways
            chunkDictionary.Add(pos, chunkObj.GetComponent<ChunkGenerator>());
            
            chunkObj.GetComponent<ChunkGenerator>().init(chunkSize, actualChunkX, actualChunkZ);

            chunkObj.name = "chunk_x" + pos.x + "_z" + pos.z; // set the name property to know where it is in the world through the inspector
//            Debug.Log("derped out around here, name:" + chunkObj.name);
        }
    }

    /// <summary>
    /// render the chunk at the given position, this assumes that the chunk has been generated beforehand, 
    /// if not it will call CreateChunk on it... as a safeguard...
    /// and it will create the chunks on each side of this, to be able to render the edges of this chunk properly
    /// </summary>
    /// <param name="pos">position of the chunk</param>
    public IEnumerator RenderChunk(ChunkPos pos)
    {
        ChunkGenerator chunk = null;
        chunkDictionary.TryGetValue(pos, out chunk);

        if (chunk == null)
        {
            CreateChunk(pos);
            chunkDictionary.TryGetValue(pos, out chunk);
        }
        
        //generate the chunks on each side of the chunk, no need to render them, just generate, this is needed to calculate the edges of a chunk
        List<ChunkPos> positionsAroundTheChunk = new List<ChunkPos>();//why a list? why not, faster than arrays, and pretty syntax
        
        positionsAroundTheChunk.Add(new ChunkPos(pos.x+1, pos.z));
        positionsAroundTheChunk.Add(new ChunkPos(pos.x-1, pos.z));
        positionsAroundTheChunk.Add(new ChunkPos(pos.x, pos.z+1));
        positionsAroundTheChunk.Add(new ChunkPos(pos.x, pos.z-1));
        
        
        foreach (ChunkPos posToGen in positionsAroundTheChunk)
        {
            //make a new thread for each chunk needed to be generated
            CreateChunk(posToGen);
        }


        yield return new WaitForSeconds(0.5f); //wait a short while 
        chunk.Render();
    }

    //should probably remake this function to use the dictionary instead, but it works for now
    //deletes chunks that got out of the range of the player
    void deleteOldChunks(int playerChunkXPos, int playerChunkZPos)
    {
        foreach (KeyValuePair<ChunkPos,ChunkGenerator> chunk in chunkDictionary)
        {

            GameObject obj = chunk.Value.gameObject;
            ChunkPos pos = chunk.Key;
            bool breakIt = true;

            //the spiral loop generates a bit more than the radiusToGenerateAroundPlayer allows, so added a +2 modifier to it when removing
            //stuff still gets rmeoved, and this way when the player runs back and forth a bit it doesnt have to generate entirely new chunks 
            //time he gets out of the range of chunks..
            //should probably look into why it does that, later, its of no immediate concern.  
            if ((pos.x >= playerChunkXPos - radiusToGenerateAroundPlayer - 2 && pos.x <= playerChunkXPos + radiusToGenerateAroundPlayer + 2))
            {
                if ((pos.z >= playerChunkZPos - radiusToGenerateAroundPlayer - 2 && pos.z <= playerChunkZPos + radiusToGenerateAroundPlayer + 2))
                {
                    breakIt = false;
                }
            }

            if (breakIt)
                obj.SetActive(false); // set the chunk as inactive, the "object pooling"-ish methods of ChunkDestroyer will take care of the rest

        }
    }
    

}
