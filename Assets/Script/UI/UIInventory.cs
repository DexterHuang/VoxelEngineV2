using UnityEngine;
using System.Collections;

public class UIInventory : MonoBehaviour {
    [SerializeField]
    Inventory inv;
    public Transform gridParent;
    public void setInventory(Inventory inv) {
        this.inv = inv;
        updateInventory();
    }
    public void updateInventory() {
        if (inv != null) {
            for (int i = 0; i < inv.getSize(); i++) {
                GameObject o = Instantiate<GameObject>(GameManager.UIHandler.UISlotPrefab);
                UISlot UISlot = o.GetComponent<UISlot>();
                UISlot.setParentSlot(inv.getSlot(i));
                UISlot.setInventoryType(inv.getInventoryType());
                o.transform.SetParent(gridParent);
            }
        }
    }
}
