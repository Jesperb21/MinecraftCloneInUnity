using UnityEngine;
using System.Collections;

//note, this doesn't extend MonoBehaviour, thats on purpose, it doesn't need it
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
    public enum Direction { north, east, south, west, up, down };


    public BlockData(bool visibility, BlockType type, Vector3 pos, bool block)
    {
        this.visibility = visibility;
        this.type = type;
        isSpawned = false;
        position = pos;
        isAnActualBlock = block;
    }
    public virtual bool IsSolid(Direction direction)
    {
        if (type != BlockType.air)
        {
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
            }
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

    protected virtual MeshData FaceDataUp(ChunkGenerator chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        //meshData.uv.AddRange(new Vector2(Direction.up));
        return meshData;
    }

    protected virtual MeshData FaceDataDown(ChunkGenerator chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        //meshData.uv.AddRange(FaceUVs(Direction.down));
        return meshData;
    }

    protected virtual MeshData FaceDataNorth(ChunkGenerator chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        //meshData.uv.AddRange(FaceUVs(Direction.north));
        return meshData;
    }

    protected virtual MeshData FaceDataEast(ChunkGenerator chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        //meshData.uv.AddRange(FaceUVs(Direction.east));
        return meshData;
    }

    protected virtual MeshData FaceDataSouth(ChunkGenerator chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        //meshData.uv.AddRange(FaceUVs(Direction.south));
        return meshData;
    }

    protected virtual MeshData FaceDataWest(ChunkGenerator chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        //meshData.uv.AddRange(FaceUVs(Direction.west));
        return meshData;
    }
    #endregion
}

