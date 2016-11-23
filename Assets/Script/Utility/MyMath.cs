using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MyMath {
    public static int mod(float a, float b) {
        return (int)(a - b * Math.Floor(a / b));
    }
}
