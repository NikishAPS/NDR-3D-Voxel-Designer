using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //Для преобразования класса SaveParameters в файл JSON
using System.IO; //Для работы с файлами

public class SaveProject : MonoBehaviour
{
    public SaveParameters saveParameters = new SaveParameters();

    private string path;



    public bool b1;
    public bool b2;

    private void Start()
    {
        path = Path.Combine(Application.dataPath, "VoxelSave.json");
    }

    void Load()
    {
        if(File.Exists(path))
        {
            saveParameters = JsonUtility.FromJson<SaveParameters>(File.ReadAllText(path));
        }
    }

    public string ggg;

    public void Save()
    {
        if (!File.Exists(path))
        {
            File.Create(path);
        }
        if (File.Exists(path))
        {
            File.WriteAllText(path, JsonUtility.ToJson(saveParameters));
        }
    }

    void Update()
    {
        if(b1)
        {
            Save();
            b1 = false;
        }

        if (b2)
        {
            Load();
            b2 = false;
        }
    }

    public void LoadAndSave()
    {

    }
}


[Serializable]
public class SaveParameters
{
    public string name;
    public int g;
}
