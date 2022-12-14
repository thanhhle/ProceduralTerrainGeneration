using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public const int mapChunkSize = 241;

    public bool autoUpdate;

    public enum MapType { NoiseMap, ColorMap, Mesh };
    public MapType mapType;

    public int seed;
    public float scale;

    public int octaves;
    [Range(0, 1)]
    public float persistence;
    public float lacunarity;

    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    [Range(0, 6)]
    public int levelOfDetail;


    public Landform[] landforms;


    public void GenerateMap()
    {
        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, scale, octaves, persistence, lacunarity, offset);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int x = 0; x < mapChunkSize; x++)
        {
            for (int y = 0; y < mapChunkSize; y++)
            {
                float noiseValue = noiseMap[x, y];
                for (int i = 0; i < landforms.Length; i++)
                {
                    if (noiseValue <= landforms[i].threshold)
                    {
                        colorMap[y * mapChunkSize + x] = landforms[i].color;
                        break;
                    }
                }
            }
        }

        MapRenderer mapRenderer = FindObjectOfType<MapRenderer>();
        if (mapType == MapType.NoiseMap)
        {
            mapRenderer.RenderMap(TextureGenerator.GenerateTextureFromNoiseMap(noiseMap));
        }
        else if (mapType == MapType.ColorMap)
        {
            mapRenderer.RenderMap(TextureGenerator.GenerateTextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
        else if (mapType == MapType.Mesh)
        {
            mapRenderer.RenderMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.GenerateTextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
    }


    private void OnValidate()
    {
        scale = scale <= 0 ? 0.1f : scale;
        lacunarity = lacunarity < 1 ? 1 : lacunarity;
        octaves = octaves < 0 ? 0 : octaves;
    }
}


[System.Serializable]
public struct Landform
{
    public string name;
    public float threshold;
    public Color color;
}
