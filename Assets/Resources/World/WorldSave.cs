using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public static class WorldSave {

    private static string fileExtension = "world";
    private static string worldFolder = "world";

    public static void SaveWorld(ref World world)
    {
        string filePath = Application.persistentDataPath + "/" + worldFolder + "/" + world.WorldName() + "." + fileExtension;
        if (File.Exists(filePath))
        {
            File.Delete(filePath);

        }
        //File.WriteAllBytes(filePath, world.ToBytes());
    }
    public static void LoadWorld(string worldName)
    {
        string filePath = Application.persistentDataPath + "/" + worldFolder + "/" + worldName + "." + fileExtension;
        if (File.Exists(filePath))
        {

            byte[] byteArray = System.IO.File.ReadAllBytes(filePath);

            //GameManager.Get<WorldManager>().currentWorld = new World(byteArray);

        }
    }

    public static void DeleteWorld(string worldName)
    {
        string filePath = Application.persistentDataPath + "/" + worldFolder + "/" + worldName + "." + fileExtension;
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

}
