using UnityEngine;

public class ChunkLoader : MonoBehaviour {
    public Chunk[] ChunkPrefabs;
    public float ChunkHeight;
    public int ChunksToLoad;
    public Transform ChunksContainer;
    public NewPlayer Player;

    int LastChunkIndex;

    void Update() {
        // @todo: There is probably a much simpler way to get this range.
        // I did it this way because I needed the min and max specifically,
        // but don't anymore.
        float maxLoadHeight = Player.transform.position.y + (ChunkHeight * (ChunksToLoad / 2));
        maxLoadHeight = (int) maxLoadHeight + ((int) ChunkHeight - ((int) maxLoadHeight % (int) ChunkHeight));
        float minLoadHeight = maxLoadHeight - (ChunkHeight * ChunksToLoad);

        int chunkIndex = (int) ((minLoadHeight - transform.position.y) / ChunkHeight);
        if (chunkIndex != LastChunkIndex) {
            Chunk[] existingChunks = ChunksContainer.GetComponentsInChildren<Chunk>();
            
            for (int i = chunkIndex; i < chunkIndex + ChunksToLoad; i++) {
                Chunk existingChunk = null;
                for (int j = 0; j < existingChunks.Length; j++) {
                    Chunk c = existingChunks[j];
                    if (c.Index == i) {
                        existingChunk = c;
                        break;
                    }
                }

                if (!existingChunk) {
                    int prefabIndex = Random.Range(0, ChunkPrefabs.Length - 1);
                    
                    Chunk chunk = (Chunk) Instantiate(ChunkPrefabs[prefabIndex], ChunksContainer);
                    chunk.Index = i;
                    chunk.transform.position = new Vector3(0, chunk.Index * ChunkHeight, 0);
                }
            }

            for (int i = 0; i < existingChunks.Length; i++) {
                Chunk existingChunk = existingChunks[i];
                if (existingChunk.Index < chunkIndex || existingChunk.Index > chunkIndex + ChunksToLoad) {
                    Destroy(existingChunk.gameObject);
                }
            }

            LastChunkIndex = chunkIndex;
        }

        Debug.DrawLine(transform.position, transform.position + new Vector3(0, Player.transform.position.y, 0), Color.green);
        Debug.DrawLine(transform.position + new Vector3(0, minLoadHeight, 0), transform.position + new Vector3(0, maxLoadHeight, 0), Color.red);
    }
}