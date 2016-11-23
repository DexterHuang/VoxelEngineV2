using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrackingHandler {

    public static void onGenerateNearbyChunks() {
        //Timer.start("tracking player");
        Player player = GameManager.getLocalPlayer();
        Vector l = player.getLocation().getChunkVector();
        int viewDistance = GameManager.getGameManager().viewDistance;
        for (int d = 0; d <= viewDistance; d++) {
            for (int x = -d + (int)l.getX(); x <= d + (int)l.getX(); x++) {
                for (int z = -d + (int)l.getZ(); z <= d + (int)l.getZ(); z++) {
                    for (int y = -d + (int)l.getY(); y <= d + (int)l.getY(); y++) {
                        if (y < 0) {
                            y = 0;
                        } else if (y > GameManager.maxChunkCountHeight * GameManager.chunkSize) {
                            y = GameManager.maxChunkCountHeight * GameManager.chunkSize;
                        }
                        Chunk chunk = GameManager.getChunk(new Vector(x, y, z));
                        try {
                            if (chunk.update && chunk.hasBlock && ChunkMeshCreator.nearbyChunksToRender.Contains(chunk) == false) {
                                ChunkMeshCreator.nearbyChunksToRender.Enqueue(chunk);
                                goto end;
                            }
                        } catch (System.InvalidOperationException e) {
                            Debug.LogError(e);
                        }

                    }
                }
            }
        }

        end:;
        //Timer.endAndPrint("tracking player");
    }
    public static void onUnloadFarChunks() {
        Vector l = GameManager.getLocalPlayer().getLocation().getChunkVector();
        int x = (int)l.getX();
        int z = (int)l.getZ();
        int viewD = GameManager.getGameManager().viewDistance + 3;
        foreach (Chunk chunk in Chunk.displayedChunks.ToArray()) {
            Vector chunkLoc = chunk.getChunkLocation();
            if (Math.Abs(chunkLoc.getX() - x) > viewD || Math.Abs(chunkLoc.getZ() - z) > viewD) {

                chunk.unload(true);
            }
        }
    }
}
