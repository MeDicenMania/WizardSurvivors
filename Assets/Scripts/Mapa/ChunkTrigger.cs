using UnityEngine;

public class ChunkTrigger : MonoBehaviour
{
   [SerializeField] GameObject targetChunk;
    MapController mc;


    private void Start()
    {
        mc = FindAnyObjectByType<MapController>();
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            mc.currentChunk = targetChunk;
        }
    }


    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (mc.currentChunk == targetChunk)
            {
                mc.currentChunk = null;
            }
        }
    }
}