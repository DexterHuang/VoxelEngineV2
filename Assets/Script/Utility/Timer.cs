using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Timer {
    static float deltaTime;
    public static bool isPlaying = true;
    static Dictionary<String, DateTime> nameStartTime = new Dictionary<string, DateTime>();
    public static void start(String name) {
        if (nameStartTime.ContainsKey(name) == false) {
            nameStartTime.Add(name, DateTime.Now);
        } else {
            nameStartTime[name] = DateTime.Now;
        }
    }
    public static void endAndPrint(String name) {
        DateTime startTime;
        if (nameStartTime.ContainsKey(name)) {
            nameStartTime.TryGetValue(name, out startTime);
            double diff = ((DateTime.Now - startTime).TotalMilliseconds);
            if (diff > 1000) {
                Debug.Log(name + " compeleted in " + (diff / 1000) + " seconds");
            } else {
                Debug.Log(name + " compeleted in " + diff + " ms");
            }
        } else {
            Debug.LogError("cannot find timer with name: " + name);
        }
    }
    static float deltaTime2 = 0;
    public static void doSyncTick() {
        try {
            Scheduler.onAsycTick();
            Scheduler.onSyncTick();
            deltaTime += Time.deltaTime;
            deltaTime2 += Time.deltaTime;
            ChunkMeshCreator.onImmediateDisplayChunksTick();
            if (GameManager.deltaTime >= 0.1f) {
                ChunkMeshCreator.onDisplayChunkTick();
                PlayerTrackingHandler.onUnloadFarChunks();
                BlockLogicHandler.onTick();
                GameManager.deltaTime = 0;


                foreach (Entity e in GameManager.getEntitiesByType(ReuseableGameObject.ITEM_ENTITY).ToArray()) {
                    if (e.isRemoved() == false && e.isInitiated()) {
                        e.onTick();
                    }
                }
            }
            if (deltaTime >= 1 && Chunk.changed > 3) {
                //StaticBatchingUtility.Combine(Chunk.childList.ToArray(), GameManager.getGameManager().mapHolder);
                //Chunk.changed = 0;
                deltaTime = 0;
            }
        } catch (Exception e) {
            Debug.LogException(e);
        }

    }
    public static void doAsycTick() {
        while (isPlaying) {
            //0.5s timer;
            //if (GameManager.deltaTime >= 0.5f) {
            ChunkMeshCreator.onImmediateConstractMeshTick();
            PlayerTrackingHandler.onGenerateNearbyChunks();
            ChunkMeshCreator.onConstractMeshTick();
            //    GameManager.deltaTime = 0;
            //}

            Thread.Sleep(200);
        }

        Debug.Log("Asyc Thread ended!");

    }
}
