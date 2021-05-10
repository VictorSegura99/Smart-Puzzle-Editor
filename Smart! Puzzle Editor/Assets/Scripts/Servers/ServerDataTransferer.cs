using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;
using System.IO;

public class ServerDataTransferer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string path = Path.Combine(Application.persistentDataPath, "Data", "Test" + ".puzzle");
        byte[] rawData = File.ReadAllBytes(path);
        var file = new UnityGoogleDrive.Data.File { Name = "Test.puzzle", Content = rawData, Parents = "10RDxlMAvqf1stif1L_TIg4UjHBh_cJ6O" };
        GoogleDriveFiles.Create(file).Send();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
