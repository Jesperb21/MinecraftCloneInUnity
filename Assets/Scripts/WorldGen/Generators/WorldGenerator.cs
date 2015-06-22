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

    public int chunkSize = 4;
    public int radiusToGenerateAroundPlayer = 4;

    private bool generatingChunk = false;
    private bool generatingWorld = false;

    private Vector3 PlayerPos;
    private bool PlayerPosHasChanged = false;

    private enum dir { xUp, yUp, xDown, yDown };

    private GameObject player;

    private bool DimensionActive;
    private bool updatingWorld;

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
            if(!updatingWorld)
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
            if (transform.Find("Player"))
            {
                player = transform.Find("Player").gameObject;
            }
        }
        if (player && PlayerPosHasChanged && !generatingChunk && !generatingWorld)
        {
            PlayerPosHasChanged = false;
            Debug.Log("world generation has been restarted");
            StartCoroutine(GenerateChunksOverTime());
        }
        else if //if the players position has changed away from the position stored in PlayerPos 
            (player &&
            (int)(PlayerPos.x) != (int)(player.transform.position.x) &&
            (int)(PlayerPos.z) != (int)(player.transform.position.z))
        {
            //change the boolean PlayerPosHasChanged to true, to allow the world gen to know to stop all generation and restart it... 
            //might make issues when the player runs in a single direction too fast for the world to be generated fast enough, oh well
            PlayerPosHasChanged = true;
            Debug.Log("players position has changed, restart world gen");
            PlayerPos = player.transform.position;
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
        updatingWorld = false;
    }

    IEnumerator GenerateChunksOverTime()
    {
        generatingWorld = true;
        Vector3 playerPos = player.transform.position;
        int playerChunkXPos = (int)(playerPos.x / chunkSize);
        int playerChunkZPos = (int)(playerPos.z / chunkSize);
        
        //spiral loop variables
        Vector2 loopPos = new Vector2(playerChunkXPos, playerChunkZPos);
        dir currentDir = dir.xUp;
        bool addAnotherIteration = false;

        //loop that every time it runs through changes direction, every 2nd time it runs through adds another block to the end of the line, 
        //and every 4th time ( thisloop/2 ) knows its made another layer
        //this results in something like http://gyazo.com/c8daf4c2a000e11d4ac004e00d9ad21b ... hopefully
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

                if (!transform.Find("chunk_x" + loopPos.x + "_z" + loopPos.y) && !PlayerPosHasChanged)//stop generating the same chunk twice && dont generate if player pos has changed
                {
                    //instantiate a chunk at the calculated position
                    GameObject chunkObj = (GameObject)Instantiate(chunk, new Vector3(loopPos.x * chunkSize, 0f, loopPos.y * chunkSize), Quaternion.identity);
                    chunkObj.transform.parent = transform;

                    int actualChunkX = (int)loopPos.x * chunkSize;
                    int actualChunkZ = (int)loopPos.y * chunkSize;

                    chunkObj.GetComponent<ChunkGenerator>().init(chunkSize, actualChunkX, actualChunkZ);
                    chunkObj.name = "chunk_x" + loopPos.x + "_z" + loopPos.y;

                    //StartCoroutine(CreateChunksOverTime(actualChunkX, actualChunkZ, chunk));
                }
                else if (!PlayerPosHasChanged && !transform.Find("chunk_x" + loopPos.x + "_z" + loopPos.y).gameObject.activeInHierarchy)
                    transform.Find("chunk_x" + loopPos.x + "_z" + loopPos.y).gameObject.SetActive(true);

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

            //switch direction
            if ((int)currentDir == 3)
                currentDir = 0;
            else
                currentDir++;
        }
        #region square chunk generation
        /*
            for (int x = playerChunkXPos - radiusToGenerateAroundPlayer; x <= playerChunkXPos + radiusToGenerateAroundPlayer; x++)
            {
                for (int z = playerChunkZPos - radiusToGenerateAroundPlayer; z <= playerChunkZPos + radiusToGenerateAroundPlayer; z++)
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
        */
        #endregion
        deleteOldChunks(playerChunkXPos, playerChunkZPos);

        yield return new WaitForSeconds(1f); //when the world has been generated around the player wait for a second before attempting to create it again
        generatingWorld = false;
    }

    //deletes chunks that got out of the range of the player
    void deleteOldChunks(int playerChunkXPos, int playerChunkZPos)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            if (obj.tag == "Chunk")
            {
                int x = (int)obj.transform.position.x / chunkSize;
                int z = (int)obj.transform.position.z / chunkSize;
                bool breakIt = true;
                
                //the spiral loop generates a bit more than the radiusToGenerateAroundPlayer allows, so added a +2 modifier to it when removing
                //stuff still gets rmeoved, and this way when the player runs back and forth a bit it doesnt have to generate entirely new chunks 
                //time he gets out of the range of chunks...  
                if ((x >= playerChunkXPos - radiusToGenerateAroundPlayer -2 && x <= playerChunkXPos + radiusToGenerateAroundPlayer +2))
                {
                    if ((z >= playerChunkZPos - radiusToGenerateAroundPlayer -2&& z <= playerChunkZPos + radiusToGenerateAroundPlayer +2))
                    {
                        breakIt = false;
                    }
                }

                if (breakIt)
                    obj.SetActive(false);
                    //Destroy(obj);

            }
        }
    }
    

}
