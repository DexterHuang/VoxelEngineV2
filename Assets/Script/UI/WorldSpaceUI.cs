using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

public class WorldSpaceUI : MonoBehaviour {

    public Transform contentParent;
    public GameObject buttonPrefab;
    private List<UIButton> uiButtons = new List<UIButton>();
    void Start() {

    }
    void Update() {
        Quaternion q = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        transform.rotation = q;
    }
    public void addButton(string name, ThreadStart task) {
        GameObject o = Instantiate<GameObject>(buttonPrefab);
        o.transform.SetParent(contentParent, false);
        UIButton uiButton = o.GetComponent<UIButton>();
        uiButton.setText(name);
        uiButton.setTask(task);
        uiButtons.Add(uiButton);
    }
    public void removeAllButtons() {
        foreach (UIButton uiButton in uiButtons.ToArray()) {
            Destroy(uiButton.gameObject);
            uiButtons.Remove(uiButton);
        }
    }
}
