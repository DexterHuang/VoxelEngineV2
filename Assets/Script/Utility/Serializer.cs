using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class Serializer {

    public static void serializeAndSave<T>(object o, string filePath) {
        if (Directory.Exists(Path.GetDirectoryName(filePath)) == false) {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }
        //FileStream file = File.Create(filePath);
        //BinaryFormatter bf = new BinaryFormatter();
        //Timer.start("serializing " + o);
        //bf.Serialize(file, o);
        //Timer.endAndPrint("serializing " + o);
        //file.Close();
        //Timer.start("serializing json " + o);
        File.WriteAllText(filePath, JsonUtility.ToJson(o));
        //Timer.endAndPrint("serializing json " + o);
    }
    public static T loadAndDeserialize<T>(string filePath) {
        if (File.Exists(filePath)) {
            //FileStream file = File.OpenRead(filePath);
            //BinaryFormatter bf = new BinaryFormatter();
            //T o = (T)bf.Deserialize(file);
            //file.Close();
            return JsonUtility.FromJson<T>(File.ReadAllText(filePath));
        } else {
            return default(T);
        }
    }
}
