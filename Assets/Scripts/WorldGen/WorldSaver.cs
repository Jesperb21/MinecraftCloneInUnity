using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public static class WorldSaver {
    public static string SaveFolder = "MCInUnitySaves";
    public static string worldName = "World1";

    /// <summary>
    /// finds the location to put all the save data in of the specific world
    /// creates the directory if it doesn't exist
    /// </summary>
    /// <param name="WorldName">name of the world to save</param>
    /// <returns></returns>
    public static string SaveLocation(string WorldName)
    {
        string saveLocation = SaveFolder + "/" + WorldName +"/";

        if (!Directory.Exists(saveLocation))
        {
            Directory.CreateDirectory(saveLocation);
        }
        return saveLocation;
    }

    /// <summary>
    /// finds the name of the file of where to put/get the chunk
    /// </summary>
    /// <param name="pos">position of the chunk</param>
    /// <returns></returns>
    public static string saveFileName(ChunkPos pos)
    {
        string fileName = pos.x + "," + pos.z + ".bin";
        return fileName;
    }


    public static void SaveChunk(ChunkGenerator Chunk)
    {
        string saveFile = SaveLocation(worldName);
        ChunkPos CP = new ChunkPos((int)Chunk.actualChunkCoords.x / Chunk.GetChunkSize(), (int)Chunk.actualChunkCoords.z / Chunk.GetChunkSize());
        saveFile += saveFileName(CP);

        IFormatter formatter = new BinaryFormatter(); //new binary formatter to format serialized input to binary output (for bin save files)
        Stream stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.None); //filestream that writes into saveFile, creates it first ofc, and doesn't share it with anyone >=)

        formatter.Serialize(stream, Chunk.Blocks);

        stream.Close();//always close the stream! always!
    }

    public static bool LoadChunk(ChunkGenerator Chunk)
    {
        string saveFile = SaveLocation(worldName);
        ChunkPos CP = new ChunkPos((int)Chunk.actualChunkCoords.x / Chunk.GetChunkSize(), (int)Chunk.actualChunkCoords.z / Chunk.GetChunkSize());
        saveFile += saveFileName(CP);
        if (!File.Exists(saveFile))
        {
            return false;
        }
        //new binaryformatter to format binary input to serialized output(bin file -> useable stuff)
        IFormatter formatter = new BinaryFormatter();
        //filestream to read from saveFile, it opens it, reads from it, and blocks any other attempt to access it while its reading...
        //probably doesn't need to block access to it when its reading? but meh
        Stream stream = new FileStream(saveFile, FileMode.Open, FileAccess.Read, FileShare.None);

        Chunk.Blocks = (BlockData[, ,])formatter.Deserialize(stream);

        stream.Close(); //always close the stream! always!
        return true;
    }

}
