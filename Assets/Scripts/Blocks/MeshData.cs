using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//note, this doesn't extend MonoBehaviour, thats on purpose, it doesn't need it
public class MeshData
{
    //vertices are coordinates in a 3d space (from 0.0f to 1.0f)
    public List<Vector3> vertices = new List<Vector3>();
    //triangles is a list of indexes from the vertices list, ex if triangle[0] = 0, triangle[1] = 1, triangle[2] = 2; 
    //it means that the first triangle uses point 0, 1 and 2 specified in the vertices list; note: this list must always contain numbers in the power of 3
    //without it it cannot make a triange... cause a triangle needs 3 corners silly...
    public List<int> triangles = new List<int>();
    //each cordinate in this list corresponds to a coordinate in the vertices list, and the idea is that the triangles then use these cordinates to get 
    //textures from a texturemap... oh, the cordinates are from a file, ranging from 0.0f, to 1.0f; cordinates are used to get the actual texture for the 
    //mesh to use, fancy stuff... sounds complex in the start, but once you understand it its actually not that bad...
    public List<Vector2> uv = new List<Vector2>();

    public MeshData() { }

    public void AddQuadTriangles()
    {
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);

    }

    public void AddVertex(Vector3 vertex)
    {
        vertices.Add(vertex);

    }

    public void AddTriangle(int tri)
    {
        triangles.Add(tri);
    }
}
