using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        float halfWidth = (width - 1) / 2f;
        float halfHeight = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.vertices[vertexIndex] = new Vector3(x - halfWidth, noiseMap[x, y], y - halfHeight);
                meshData.uv[vertexIndex] = new Vector2(1f - x / (float)width, 1f - y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width, vertexIndex + width + 1);
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;

    }
}


public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uv;

    private int triangleIndex;

    public MeshData(int width, int height)
    {
        vertices = new Vector3[width * height];
        triangles = new int[(width - 1) * (height - 1) * 6];
        uv = new Vector2[width * height];
    }

    public void AddTriangle(int vertex1, int vertex2, int vertex3)
    {
        triangles[triangleIndex] = vertex1;
        triangles[triangleIndex + 1] = vertex2;
        triangles[triangleIndex + 2] = vertex3;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        return mesh;
    }
}
