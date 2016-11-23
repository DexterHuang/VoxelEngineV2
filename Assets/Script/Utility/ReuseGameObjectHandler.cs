using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReuseGameObjectHandler {
    private static Dictionary<string, Queue<GameObject>> recyclePool = new Dictionary<string, Queue<GameObject>>();

    public static GameObject getObject(string type, Vector3 position) {
        GameObject o = null;
        if (getObjectPool(type).Count > 0) {
            o = getObjectPool(type).Dequeue();
            o.transform.localPosition = position;
            o.SetActive(true);
        }
        return o;
    }
    public static void putToRecycle(ReuseableGameObject type, GameObject o, string customName = "") {
        o.transform.SetParent(GameManager.getGameManager().recyclePoolHolder.transform);
        o.transform.position = Vector3.zero;
        o.SetActive(false);
        getObjectPool(type + customName).Enqueue(o);
    }
    private static Queue<GameObject> getObjectPool(string type) {
        Queue<GameObject> ol;
        if (recyclePool.ContainsKey(type + "") == false) {
            recyclePool.Add(type + "", new Queue<GameObject>());
        }
        recyclePool.TryGetValue(type + "", out ol);
        return ol;
    }
}
