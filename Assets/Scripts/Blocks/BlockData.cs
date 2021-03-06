﻿using UnityEngine;
using System.Collections;
using System;

//note, this doesn't extend MonoBehaviour, thats on purpose, it doesn't need it
[Serializable]//serializable so it can be serialized into a file
public class BlockData
{
    public BlockType type;

    //vector 3 surrogate, vector 3 wasn't serializable, so couldn't be saved
    [Serializable]
    public struct coordinates
    {
        public float x; 
        public float y; 
        public float z;

        public coordinates(float x, float y, float z){
            this.x = x;
            this.y = y;
            this.z = z;
        }
    };
    public coordinates position;
    public int Temperature;
    
    public enum BlockType
    {
        air, stone, dirt, grass, coalOre, lapisLazuliOre, emeraldOre, goldOre, redstoneOre, ironOre, tinOre
    }
    public enum Direction { north, east, south, west, up, down };
    
    //texture variables
    public struct Tile { public int x; public int y;}
    const float tileSize = 0.25f; //space between tiles. eg 1 (picture) /4(pictures adjacent) = 0.25f per picture

    public BlockData(BlockType type, Vector3 pos, int temp)
    {
        this.type = type;
        position.x = pos.x;
        position.y = pos.y;
        position.z = pos.z;
        Temperature = temp;
    }
    public BlockData()
    {
        type = BlockType.air;
    }

    public virtual bool IsSolid(Direction direction)
    {
        if (type != BlockType.air)
        {
            /*
            switch (direction)
            {
                case Direction.north:
                    return true;
                case Direction.east:
                    return true;
                case Direction.south:
                    return true;
                case Direction.west:
                    return true;
                case Direction.up:
                    return true;
                case Direction.down:
                    return true;
            }*/
            return true;
        }
        return false;
    }


    public virtual MeshData Blockdata
     (ChunkGenerator chunk, int x, int y, int z, MeshData meshData)
    {
        if (type != BlockType.air)
        {
            if (!chunk.getBlockAt(x, y + 1, z).IsSolid(Direction.down))
            {
                meshData = FaceDataUp(chunk, x, y, z, meshData);
            }

            if (!chunk.getBlockAt(x, y - 1, z).IsSolid(Direction.up))
            {
                meshData = FaceDataDown(chunk, x, y, z, meshData);
            }

            if (!chunk.getBlockAt(x, y, z + 1).IsSolid(Direction.south))
            {
                meshData = FaceDataNorth(chunk, x, y, z, meshData);
            }

            if (!chunk.getBlockAt(x, y, z - 1).IsSolid(Direction.north))
            {
                meshData = FaceDataSouth(chunk, x, y, z, meshData);
            }

            if (!chunk.getBlockAt(x + 1, y, z).IsSolid(Direction.west))
            {
                meshData = FaceDataEast(chunk, x, y, z, meshData);
            }

            if (!chunk.getBlockAt(x - 1, y, z).IsSolid(Direction.east))
            {
                meshData = FaceDataWest(chunk, x, y, z, meshData);
            }
        }
        return meshData;

    }

    #region add data about the different faces of the block
    public virtual Vector2[] FaceUVs(Direction direction)
    {
        Vector2[] UVs = new Vector2[4];
        Tile tilePos = TexturePosition(direction);

        UVs[0] = new Vector2(tileSize * tilePos.x + tileSize,
            tileSize * tilePos.y);
        UVs[1] = new Vector2(tileSize * tilePos.x + tileSize,
            tileSize * tilePos.y + tileSize);
        UVs[2] = new Vector2(tileSize * tilePos.x,
            tileSize * tilePos.y + tileSize);
        UVs[3] = new Vector2(tileSize * tilePos.x,
            tileSize * tilePos.y);

        return UVs;
    }
    public virtual Tile TexturePosition(Direction direction)
    {
        Tile tile = new Tile();
        tile.x = 0;
        tile.y = 0;

        switch (type)
        {
            case BlockType.dirt:
                tile.x = 1;
                break;
            case BlockType.emeraldOre:
                tile.x = 2;
                tile.y = 3;
                break;
            case BlockType.grass:
                switch (direction)
                {
                    case Direction.up:
                        if (Temperature < 0)
                        {
                            tile.y = 1;
                        }
                        else
                        {
                            tile.x = 2;
                        }
                        break;
                    case Direction.down:
                        tile.x = 1;
                        break;
                    default:
                        if (Temperature < 0)
                        {
                            tile.x = 1;
                            tile.y = 1;
                        }
                        else
                        {
                            tile.x = 3;
                        }
                        break;
                }
                break;
            case BlockType.coalOre:
                tile.y = 3;
                break;
            case BlockType.lapisLazuliOre:
                tile.y = 3;
                tile.x = 1;
                break;
            case BlockType.goldOre:
                tile.y = 3;
                tile.x = 3;
                break;
            case BlockType.redstoneOre:
                tile.y = 2;
                break;
            case BlockType.tinOre:
                tile.x = 1;
                tile.y = 2;
                break;
            case BlockType.ironOre:
                tile.x = 2;
                tile.y = 2;
                break;
        } 
        
        return tile;
    }

    protected virtual MeshData FaceDataUp(ChunkGenerator chunk, int x, int y, int z, MeshData meshData)
    {

        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.up));
        return meshData;
    }

    protected virtual MeshData FaceDataDown(ChunkGenerator chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.down));
        return meshData;
    }

    protected virtual MeshData FaceDataNorth(ChunkGenerator chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.north));
        return meshData;
    }

    protected virtual MeshData FaceDataEast(ChunkGenerator chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.east));
        return meshData;
    }

    protected virtual MeshData FaceDataSouth(ChunkGenerator chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.south));
        return meshData;
    }

    protected virtual MeshData FaceDataWest(ChunkGenerator chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.west));
        return meshData;
    }
    #endregion
}

