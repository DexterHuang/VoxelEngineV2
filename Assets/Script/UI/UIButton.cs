using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.EventSystems;
using System;

public class UIButton : MonoBehaviour {

    public Text text;
    ThreadStart task;
    void Start() {
    }
    public void setText(string str) {
        text.text = str;
    }
    public void onPressed() {
        if (task != null) {
            task.Invoke();
        }
    }
    public void setTask(ThreadStart task) {
        this.task = task;
    }
}
