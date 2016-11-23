using UnityEngine;
using System.Collections;
public class BlockDamageOverlay : MonoBehaviour {

    MeshRenderer myRenderer;
    public int stages;

    public void setStage(int damagedValue, int maxHealth) {
        float stage = (float)((int)(((float)damagedValue / maxHealth) * 10)) / 10f;
        if (myRenderer == null) {
            myRenderer = GetComponentInChildren<MeshRenderer>();
        }
        myRenderer.material.mainTextureOffset = new Vector2(0, stage);
    }
}
