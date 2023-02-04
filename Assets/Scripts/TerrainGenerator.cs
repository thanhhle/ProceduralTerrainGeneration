using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public const float maxViewDistance = 450;
    public Transform character;
    public Material mapMaterial;

    public static Vector2 characterPosition;
    static MapGenerator mapGenerator;
    private int chunkSize;
    private int visibleChunks;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleChunksLastUpdate = new List<TerrainChunk>();


    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        chunkSize = MapGenerator.mapChunkSize - 1;
        visibleChunks = Mathf.RoundToInt(maxViewDistance / chunkSize);
    }


    void Update()
    {
        characterPosition = new Vector2(character.position.x, character.position.z);
        UpdateVisibleChunks();
    }


    void OnDestroy()
    {
        visibleChunksLastUpdate.Clear();
    }


    void UpdateVisibleChunks()
    {
        int currentChunkCoordX = Mathf.RoundToInt(characterPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(characterPosition.y / chunkSize);

        for (int i = 0; i < visibleChunksLastUpdate.Count; i++)
        {
            visibleChunksLastUpdate[i].SetVisible(false);
        }

        visibleChunksLastUpdate.Clear();

        for (int x = -visibleChunks; x <= visibleChunks; x++)
        {
            for (int y = -visibleChunks; y <= visibleChunks; y++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + x, currentChunkCoordY + y);
                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    TerrainChunk terrainChunk = terrainChunkDictionary[viewedChunkCoord];
                    terrainChunk.Update();
                    if (terrainChunk.IsVisible())
                    {
                        visibleChunksLastUpdate.Add(terrainChunk);
                    }
                }
                else
                {
                    TerrainChunk terrainChunk = new TerrainChunk(viewedChunkCoord, chunkSize, transform, mapMaterial);
                    terrainChunkDictionary.Add(viewedChunkCoord, terrainChunk);
                }
            }
        }
    }


    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;

            SetVisible(false);

            mapGenerator.RequestMapData(OnMapDataReceived);
        }

        private void OnMapDataReceived(MapData mapData)
        {
            mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
        }

        private void OnMeshDataReceived(MeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }

        public void Update()
        {
            float characterDistancefromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(characterPosition));
            bool visible = characterDistancefromNearestEdge <= maxViewDistance;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }
}