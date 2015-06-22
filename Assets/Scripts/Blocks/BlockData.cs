using UnityEngine;
using System.Collections;

public class BlockData
{
    public bool visibility;
    public bool isSpawned;
    public BlockType type;
    public Vector3 position;
    public bool isAnActualBlock;

    public enum BlockType
    {
        air, grass, stone, dirt, emeraldOre
    }


    public BlockData(bool visibility, BlockType type, Vector3 pos, bool block)
    {
        this.visibility = visibility;
        this.type = type;
        isSpawned = false;
        position = pos;
        isAnActualBlock = block;
    }
}

