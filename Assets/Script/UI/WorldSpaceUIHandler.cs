using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WorldSpaceUIHandler {

    static WorldSpaceUI currentUI;

    public static WorldSpaceUI showUI(Vector3 position) {
        position = position + new Vector3(0, 2, 0);
        if (currentUI == null) {
            GameObject o = GameManager.spawnPrefab(GameManager.getGameManager()._worldSpaceUIPrefab, position, ReuseableGameObject.WORLD_SPACE_UI);
            currentUI = o.GetComponent<WorldSpaceUI>();
            //o.transform.SetParent(GameManager.getGameManager()._worldSpaceUIHolder, false);
        } else {
            currentUI.transform.position = position;
        }
        currentUI.removeAllButtons();
        return currentUI;
    }
    public static void hideUI() {
        ReuseGameObjectHandler.putToRecycle(ReuseableGameObject.WORLD_SPACE_UI, currentUI.gameObject);
        currentUI = null;
    }

}
