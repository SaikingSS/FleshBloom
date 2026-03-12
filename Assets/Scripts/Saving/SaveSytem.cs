using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSytem 
{
    public static void SaveGame(Progress progress, int saveFile)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/mgs.save" + saveFile;
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(progress);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadGame(int saveFile)
    {
        string path = Application.persistentDataPath + "/mgs.save" + saveFile;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }
}
