using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class Dragable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler {

    Vector2 originalPos;
    Transform originalParent;
    UISlot UISlot;
    Image image;

    void Start() {
        UISlot = transform.parent.GetComponent<UISlot>();
        image = GetComponent<Image>();
        originalParent = this.transform.parent;
    }
    public void OnBeginDrag(PointerEventData eventData) {
        if (UISlot.getSlot().getItem() != null) {
            originalPos = Vector2.zero;
            this.transform.SetParent(GameManager.UIHandler.cursorLayerParent);
            image.raycastTarget = false;
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (UISlot.getSlot().getItem() != null) {
            transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (UISlot.getSlot().getItem() != null) {
            this.transform.SetParent(originalParent);
            transform.localPosition = originalPos;
            RaycastResult ray = eventData.pointerCurrentRaycast;
            if (ray.gameObject != null && ray.gameObject.name.Equals("ItemIcon")) {
                UISlot otherUISlot = ray.gameObject.GetComponent<Dragable>().UISlot;
                UIHandler.onDragAndDropEvent(this.UISlot.getSlot(), otherUISlot.getSlot());
            } else if (ray.gameObject.name.Equals("Slot")) {

            } else {
                ItemStack item = this.UISlot.parentSlot.getItem();
                this.UISlot.parentSlot.setItem(null);
                GameManager.getLocalPlayer().dropItem(item);
            }
            image.raycastTarget = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (this.UISlot.inventoryType == InventoryType.PLAYER_HOTBAR) {
            GameManager.getLocalPlayer().setSelectedHotbarIndex(this.UISlot.getSlot().id);
        }
    }
}
