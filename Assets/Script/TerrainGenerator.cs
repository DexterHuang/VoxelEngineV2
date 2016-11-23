using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator {
    public static int initialMapDistance = 1;
    public static float scale = 0.025f;
    public void populateChunk(Chunk chunk) {
        Vector startLoc = chunk.getChunkStartLocation();
        chunk.blocks = new Block[GameManager.chunkSize * GameManager.chunkSize * GameManager.chunkSize];
        bool hasBlock = false;
        for (int xx = 0; xx < GameManager.chunkSize; xx++) {
            int x = xx + (int)startLoc.getX();
            for (int zz = 0; zz < GameManager.chunkSize; zz++) {
                int z = zz + (int)startLoc.getZ();
                float i = Mathf.PerlinNoise(x * scale, z * scale) * GameManager.chunkSize;
                i += Mathf.PerlinNoise((x + 999) * (scale / 3), (z + 999) * (scale / 3)) * GameManager.chunkSize;
                i += Mathf.PerlinNoise((x + 333) * (scale / 6), (z + 333) * (scale / 6)) * 16;
                for (int yy = 0; yy < GameManager.chunkSize; yy++) {
                    int y = yy + (int)startLoc.getY();
                    MyMaterial m = MyMaterial.AIR;
                    if (y < i) {
                        m = MyMaterial.GRASS;
                        hasBlock = true;
                    }
                    chunk.setBlock(new Vector(xx, yy, zz), new Block(chunk, new Vector(xx, yy, zz), m));
                }
            }
        }
        chunk.hasBlock = hasBlock;
    }

    public void initiateMap() {
        Timer.start("initiate map");
        GameManager.spawnPoint = GameManager.getHeighest(new Location(GameManager.chunkSize / 2, 0, GameManager.chunkSize / 2)).getLocation().add(0, 50, 0);
        Vector v = new Vector(0, 0, 0);
        if (GameManager.getLocalPlayer() != null) {
            v = GameManager.getLocalPlayer().getLocation().getChunkVector();
            for (int x = -initialMapDistance; x <= initialMapDistance; x++) {
                for (int y = 0; y <= GameManager.getGameManager().viewDistance; y++) {
                    for (int z = -initialMapDistance; z <= initialMapDistance; z++) {
                        Vector cv = new Vector(x, y, z).add(v);
                        if (cv.getY() >= 0) {
                            Chunk chunk = GameManager.getChunk(cv);
                            ChunkMeshCreator.pushMeshData(chunk);
                            chunk.display();
                        }
                    }
                }
            }
        } else {
            for (int x = -initialMapDistance; x <= initialMapDistance; x++) {
                for (int y = 0; y <= GameManager.getGameManager().viewDistance; y++) {
                    for (int z = -initialMapDistance; z <= initialMapDistance; z++) {
                        Chunk chunk = GameManager.getChunk(new Vector(x, y, z).add(v));
                        ChunkMeshCreator.pushMeshData(chunk);
                        chunk.display();
                    }
                }
            }
        }
        Timer.endAndPrint("initiate map");
    }
}
