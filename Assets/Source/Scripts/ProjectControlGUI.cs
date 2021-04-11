using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO; //Для работы с файловым потоком
//using Dummiesman;

using MyGUI;

public class ProjectControlGUI : MonoBehaviour
{
    public ProjectParameters projectParameters = new ProjectParameters();

    public InputField inputFieldName;
    public InputField inputFieldPath;
    public Text error;

    private int mode;
    public string nameSave, path;

    private CursorPanelsGUI cursorPanelsGUI;

    void Start()
    {
        //inputFieldName.text = "VoxelSave";
        //inputFieldPath.text = @"C:\Users\lenovo\AppData\UnityProject\MagicaVoxel\Assets";
        cursorPanelsGUI = Camera.main.GetComponent<CursorPanelsGUI>();
    }

    void Update()
    {

    }

    public void MainF(int index)
    {
        for (int i = 0; i < cursorPanelsGUI.windowGUI[1].caption.Length; i++)
        {
            cursorPanelsGUI.windowGUI[1].caption[i].gameObject.SetActive(false);
        }

        cursorPanelsGUI.windowGUI[1].panel.gameObject.SetActive(true);
        cursorPanelsGUI.windowGUI[1].caption[index].gameObject.SetActive(true);

        mode = index;
    }

    public void Perform()
    {
        path = inputFieldPath.text;
        switch (mode)
        {
            case 0: Save(); break;
            case 1: Load(); break;
            case 2: ImportOBJ(); break;
            case 3: SavePart(); break;
            case 4: ImportPart(); break;
        }
    }

    public void Cancel()
    {
        path = "";
        error.text = "";

        cursorPanelsGUI.windowGUI[1].panel.gameObject.SetActive(false);
        for (int i = 0; i < cursorPanelsGUI.windowGUI[1].caption.Length; i++)
        {
            cursorPanelsGUI.windowGUI[1].caption[i].gameObject.SetActive(false);
        }
    }

    public void Save()
    {
        nameSave = inputFieldName.text;
        path = inputFieldPath.text + @"\" + nameSave + ".json";


        if (!File.Exists(path))
        {
            File.Create(path);
        }
        if (File.Exists(path))
        {
            projectParameters.Set();

            File.WriteAllText(path, JsonUtility.ToJson(projectParameters));

            Cancel();
        }
        else
        {
            error.text = "File doesn't exist!";
        }
    }

    public void Load()
    {
        path = inputFieldPath.text;

        if (File.Exists(path))
        {

            projectParameters = JsonUtility.FromJson<ProjectParameters>(File.ReadAllText(path));
            projectParameters.Load();
            Cancel();
        }
        else
        {
            error.text = "File doesn't exist!";
        }

    }

    public void ImportOBJ()
    {
        if (File.Exists(path))
        {
            error.text = "";
            //GameObject loadedObject = new OBJLoader().Load(path);
          //  ObjectSettings(loadedObject);

            Cancel();
        }
        else
        {
            error.text = "File doesn't exist!";
        }


        /*
        Mesh holderMesh = new Mesh();
        ObjImporter newMesh = new ObjImporter();
        holderMesh = newMesh.ImportFile("C:/Users/cvpa2/Desktop/ng/output.obj");

        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = holderMesh;

        */

    }

    public void SavePart()
    {

    }

    public void ImportPart()
    {
    }

    private void ObjectSettings(GameObject go)
    {
        GameObject d = go;
        go = d.transform.GetChild(0).gameObject;
        go.transform.parent = null;
        Destroy(d);

        go.transform.position = new Vector3(1, 1, 1) * 0.5f;
        go.tag = "Imported";
        go.AddComponent<MeshCollider>();
    }
}




