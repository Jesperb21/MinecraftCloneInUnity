using UnityEngine;
using System.Collections;

//note, this is not a class, this is a struct
//why an entire file for a struct? no reason really, just didn't want to scroll through 30 lines each time scrolled through the WorldGen... 
//...which really is getting huge...
public struct ChunkPos
{
    public int x, z;

    public ChunkPos(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    //Add this function:
    public override bool Equals(object obj)
    {
        if (!(obj is ChunkPos))
            return false;

        ChunkPos pos = (ChunkPos)obj;
        if (pos.x != x || pos.z != z)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}