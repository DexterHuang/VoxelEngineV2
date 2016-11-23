using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
public class SaveLoader {

    public static void save(Save save, string saveName) {
        string filePath = getSaveFilePath(saveName);
        Serializer.serializeAndSave<Save>(save, filePath);
    }
    public static Save load(string saveName) {
        string filePath = getSaveFilePath(saveName);
        Save save = Serializer.loadAndDeserialize<Save>(filePath);
        if (save == null) {
            save = new Save();
        }
        return save;
    }
    private static string getSaveFilePath(string saveName) {
        return GameManager.path + "/" + saveName + "/SaveData/save.txt";
    }
}
