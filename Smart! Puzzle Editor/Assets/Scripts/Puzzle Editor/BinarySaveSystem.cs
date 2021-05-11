using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System;

public static class BinarySaveSystem
{
    public static void SaveFile(string path, object obj)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream filestream = new FileStream(path, FileMode.Create);
        formatter.Serialize(filestream, obj);
        filestream.Close();
    }

    public static T LoadFile<T>(string path)
    {
        if (File.Exists(path)) 
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream filestream = new FileStream(path, FileMode.Open);
            T t = (T)formatter.Deserialize(filestream);
            filestream.Close();

            return t;
        }
        else
        {
            Debug.LogError("No such path exists. Path tried: " + path);
            return default(T);
        }
    }

    public static string BiteArrayToString(byte[] binaryData)
    {
        return System.Text.Encoding.ASCII.GetString(binaryData);
    }

    public static byte[] ObjectToByteArray(object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public static object ByteArrayToObject(byte[] arrBytes)
    {
        using (var memStream = new MemoryStream(arrBytes))
        {
            var binForm = new BinaryFormatter();
            object obj = binForm.Deserialize(memStream);
            return obj;
        }
    }
}
