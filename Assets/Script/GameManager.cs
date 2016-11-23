using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour {

    public GameObject mapHolder;
    public Database _database;
    public GameObject _chunkPrefab;
    public GameObject playerPrefab;
    public GameObject _chunkChildPrefab;
    public GameObject _reflectionProbePrefab;
    public GameObject reflectionProbeHolder;
    public GameObject recyclePoolHolder;
    public GameObject itemHolder;
    public GameObject _ballPrefab;
    public GameObject cubePrefab;
    public GameObject iconRederSet;
    public GameObject _itemEntityPrefab;
    public GameObject _BlcokDamageOverlayPrefab;
    public PhysicMaterial itemPhysicsMaterial;
    public GameObject _worldSpaceUIPrefab;
    public Transform _worldSpaceUIHolder;
    public int viewDistance = 5;
    public UIHandler _UIHandler;
    public static float deltaTime = 0;
    public Sprite debugSprite;
    public Material itemMaterial;
    public bool startGame = true;
    public bool toucControl = false;
    public WebHandler webHandler;

    public static Database database;
    public static Chunk[,,] chunks;
    private static Dictionary<ReuseableGameObject, List<Entity>> entityLists = new Dictionary<ReuseableGameObject, List<Entity>>();
    public static GameObject chunkPrefab;
    public static TerrainGenerator currentTerrainGenerator;
    private static GameManager _manager;
    public static Location spawnPoint;
    public static Thread mainThread;
    public static string path;
    public static UIHandler UIHandler;
    public static Save save;

    public static int maxChunkCount = 1000;
    public static int maxChunkCountHeight = 30;
    public static int chunkSize = 12;
    public static int chunkHalfSize = chunkSize / 2;
    public static int halfMaxChunkCount;
    public static bool useReflection = false;
    public static string saveFileName = "Default";

    void Start() {
        _manager = this;
        database = _database;
#if !UNITY_EDITOR
        toucControl = true;
#endif
        if (!toucControl)
            CrossPlatformInputManager.useHardwareInput();
        else {
            CrossPlatformInputManager.useTouchInput();
        }

        UIHandler = _UIHandler;
        path = Application.persistentDataPath;
        Debug.Log("Game started at " + path);
        mainThread = Thread.CurrentThread;
        Timer.start("Loading Materials");
        for (int i = 0; i <= Enum.GetValues(typeof(MyMaterial)).Length - 1; i++) {
            ((MyMaterial)i).loadMaterial();
        }
        Timer.endAndPrint("Loading Materials");
        halfMaxChunkCount = (maxChunkCount / 2);
        chunkPrefab = _chunkPrefab;
        chunks = new Chunk[maxChunkCount, 50, maxChunkCount];
        currentTerrainGenerator = new TerrainGenerator();


        //INITIATIONS
        ItemMeshCreator.init();
        IconRenderer.init();
        if (startGame) {
            //LOAD SAVE FILE
            save = SaveLoader.load(saveFileName);
            currentTerrainGenerator.initiateMap();
            //INITIALIZE PLAYER
            if (save.player == null) {
                save.player = new Player(spawnPoint);
            }
            save.player.init(playerPrefab);
            //INITIALIZE UI
            _UIHandler.init();
        }


        //START BACKGOUND THREAD
        if (startGame) {
            Thread thread = new Thread(Timer.doAsycTick);
            thread.IsBackground = true;
            thread.Start();
        }
    }
    void Update() {
        if (startGame) {
            //if (Time.frameCount % 30 == 0) {
            //    System.GC.Collect();
            //}
            deltaTime += Time.deltaTime;
            Timer.doSyncTick();
            //Cursor.visible = true;
            GameLogicHandler.onUpdate();
        }
    }
    public void OnDestroy() {
        Timer.isPlaying = false;
    }

    public void OnApplicationQuit() {
        if (startGame && save != null && save.player != null) {
            Timer.start("Saving data");
            SaveLoader.save(save, saveFileName);
            foreach (Chunk chunk in Chunk.displayedChunks.ToArray()) {
                chunk.unload(false);
            }
            Timer.endAndPrint("Saving data");
        }
    }

    public void spawnPlayer() {
        spawnPrefab(playerPrefab, new Vector3(chunkSize / 2, chunkSize, chunkSize / 2));
    }
    public static GameObject spawnPrefab(GameObject prefab, Vector3 position, ReuseableGameObject ReuseableGameObject, string customName = "") {
        GameObject o = ReuseGameObjectHandler.getObject(ReuseableGameObject + "", position);
        if (o == null) {
            o = spawnPrefab(prefab, position);
        }
        return o;
    }
    public static GameObject spawnPrefab(GameObject prefab, Vector3 position) {
        GameObject o = Instantiate<GameObject>(prefab);
        o.transform.position = position;
        return o;
    }
    public static Chunk getChunk(Vector vector) {
        int x = (int)vector.getX() + halfMaxChunkCount;
        int y = (int)vector.getY();
        int z = (int)vector.getZ() + halfMaxChunkCount;
        Chunk chunk = chunks[x, y, z];
        if (chunk == null) {
            //Debug.Log("creating new Chunk at " + x + ", " + y + ", " + z + "     " + chunk);
            chunks[x, y, z] = ChunkLoader.loadChunk(vector);
            chunk = chunks[x, y, z];
        }
        return chunk;
    }
    public static Block getBlock(Location location) {
        float x = location.getX();
        float y = location.getY();
        float z = location.getZ();
        //Vector locV = location.toVector();
        Vector chunkVector = new Vector((int)Math.Floor(x / chunkSize), (int)Math.Floor(y / chunkSize), (int)Math.Floor(z / chunkSize));
        Chunk chunk = getChunk(chunkVector);
        //Vector remain = locV.subtract(chunk.getChunkStartLocation());
        //Debug.Log("(" + x + ", " + y + ", " + z + " ) - " + new Vector((int)nfmod(x, chunkSize), (int)nfmod(y, chunkSize), (int)nfmod(z, chunkSize)));
        return chunk.getBlock(new Vector((int)nfmod(x, chunkSize), (int)nfmod(y, chunkSize), (int)nfmod(z, chunkSize)));
    }
    static float nfmod(float a, float b) {
        return a - b * (float)Math.Floor(a / b);
    }
    public static Block getHeighest(Location location) {
        Location l = location.clone();
        for (int i = maxChunkCountHeight * chunkSize; i >= 0; i--) {
            l.setY(i);
            Block b = getBlock(l);
            if (b.getType().Equals(MyMaterial.AIR) == false) {
                return b;
            }
        }
        return null;
    }
    public static GameManager getGameManager() {
        return _manager;
    }
    public static Player getLocalPlayer() {
        if (save != null) {
            return save.player;
        }
        return null;
    }
    public static List<Entity> getEntitiesByType(ReuseableGameObject ReuseableGameObject) {
        List<Entity> l;
        if (entityLists.TryGetValue(ReuseableGameObject, out l) == false) {
            l = new List<Entity>();
            entityLists.Add(ReuseableGameObject, l);
        }
        return l;
    }
    public static Item spawnItem(ItemStack item, Location location) {
        return new Item(item, location);
    }
    public static List<Entity> getEntityInRadius(ReuseableGameObject ReuseableGameObject, Vector3 pos, float radius) {
        List<Entity> entities = new List<Entity>();
        foreach (Entity e in GameManager.getEntitiesByType(ReuseableGameObject)) {
            if (e.reuseableGameObject == ReuseableGameObject) {
                if (Vector3.Distance(e.getLocation().getVector3(), pos) <= radius) {
                    entities.Add(e);
                }
            }
        }
        return entities;
    }
}
