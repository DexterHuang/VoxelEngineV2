using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
[Serializable]
public class ItemDrop {
    public MyMaterial material = MyMaterial.STONE;

    public float minAmount = 1;

    public float maxAmount = 1;

    public float chance = 1f;

    public int getAmount() {
        return UnityEngine.Random.Range((int)minAmount, (int)maxAmount + 1);
    }
}
