using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;

    public GameObject Player;
    public float radius;
    public Vector3 noTerrainPosition;
    public LayerMask terrainLayer;
    PlayerController pc;
    public GameObject currentChunk;

    [Header("Optimizacion")]
    public List <GameObject> spawnedChunks;
    GameObject LastChunk;
    public float MaxDist;
    float chunkDistance;
    private int previousChunkIndex = -1;
   public float timer;
   public float deleteChunkTime;
    void Start()
    {
        pc = FindAnyObjectByType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        ChunkChecker();
        ChunkOptimization();
    }

    void ChunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }


        if (pc.moveDir.x >0 && pc.moveDir.y == 0) //derecha
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Derecha").position, radius, terrainLayer))
            {
                noTerrainPosition = currentChunk.transform.Find("Derecha").position;
                SpawnChunk();
            }
        }
       else  if (pc.moveDir.x < 0 && pc.moveDir.y == 0) //izquierda
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Izquierda").position, radius, terrainLayer))
            {
                noTerrainPosition = currentChunk.transform.Find("Izquierda").position;
                SpawnChunk();
            }
        }
        else if (pc.moveDir.x == 0 && pc.moveDir.y > 0) //arriba
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Arriba").position, radius, terrainLayer))
            {
                noTerrainPosition = currentChunk.transform.Find("Arriba").position;
                SpawnChunk();
            }
        }
        else if (pc.moveDir.x == 0 && pc.moveDir.y < 0) //abajo
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Abajo").position, radius, terrainLayer))
            {
                noTerrainPosition = currentChunk.transform.Find("Abajo").position;
                SpawnChunk();
            }
        }

        else if (pc.moveDir.x > 0 && pc.moveDir.y   > 0) //derecha arriba
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Derecha arriba").position, radius, terrainLayer))
            {
                noTerrainPosition = currentChunk.transform.Find("Derecha arriba").position;
                SpawnChunk();
            }
        }
        else if (pc.moveDir.x > 0 && pc.moveDir.y < 0) //derecha abajo
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Derecha abajo").position, radius, terrainLayer))
            {
                noTerrainPosition = currentChunk.transform.Find("Derecha abajo").position;
                SpawnChunk();
            }
        }
        else if (pc.moveDir.x < 0 && pc.moveDir.y > 0) //izquierda arriba
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("izquierda arriba").position, radius, terrainLayer))
            {
                noTerrainPosition = currentChunk.transform.Find("izquierda arriba").position;
                SpawnChunk();
            }
        }
        else if (pc.moveDir.x < 0 && pc.moveDir.y < 0) //izquierda abajo
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("izquierda abajo").position, radius, terrainLayer))
            {
                noTerrainPosition = currentChunk.transform.Find("izquierda abajo").position;
                SpawnChunk();
            }
        }
    }

    void SpawnChunk()
    {
        if (terrainChunks == null || terrainChunks.Count == 0)
        {
            Debug.LogWarning("No hay chunks configurados para generar.", this);
            return;
        }

        SimpleArraySet<int> candidates = new SimpleArraySet<int>(terrainChunks.Count);
        for (int i = 0; i < terrainChunks.Count; i++)
        {
            if (i != previousChunkIndex)
                candidates.Add(i);
        }

        // Con un solo chunk configurado no existe una alternativa posible.
        if (candidates.IsEmpty)
            candidates.Add(previousChunkIndex);

        int[] candidateIndices = candidates.ToArray();
        int selectedIndex = candidateIndices[Random.Range(0, candidateIndices.Length)];
        previousChunkIndex = selectedIndex;

        LastChunk = currentChunk = Instantiate(terrainChunks[selectedIndex], noTerrainPosition, Quaternion.identity);
        spawnedChunks.Add(LastChunk);
    }



    void ChunkOptimization()
    {
        timer -= Time.deltaTime;

        if( deleteChunkTime >= 0)
        {
            timer = deleteChunkTime;
        }
        else
        {
            return;
        }

        foreach (GameObject chunk in  spawnedChunks)
        {
            chunkDistance = Vector3.Distance(Player.transform.position,chunk.transform.position);
            if(chunkDistance > MaxDist)
            {
                chunk.SetActive(false);
            }
            else

            {
                chunk.SetActive(true);

            }
        }

       
    }
        
}
