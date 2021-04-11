using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using FileBrowser; //Нативный файловый браузер для автономных платформ Unity
//using OBJImporter;
//using Dummiesman;

public class ProjectControl : MonoBehaviour
{
    public Material obj_material;
    public ProjectParameters projectParameters = new ProjectParameters();
    public PartParametrs partParametrs = new PartParametrs();

    //public MessageSystem messageSystem;

    private string GetPath(string title)
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Load", Application.dataPath, "json", false);

        if (paths.Length > 0)
        {
            return new System.Uri(paths[0]).AbsoluteUri;
        }

        return string.Empty;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl))
        {
            if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
                return;
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                Load();
                return;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                ExportPart();
                return;
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                ImportPart();
                return;
            }
        }
    }

    public void Save()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save", Application.dataPath, "Name", "dubp");

        if (!string.IsNullOrEmpty(path))
        {
            projectParameters.Set();
            File.WriteAllText(path, JsonUtility.ToJson(projectParameters));
        }
        Collider g;
    }

    public void Load()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Load", Application.dataPath, "dubp", false);

        if (paths.Length > 0)
        {
            string path = paths[0];
            projectParameters = JsonUtility.FromJson<ProjectParameters>(File.ReadAllText(path));
            projectParameters.Load();
        }
    }

    public void ImportPart()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Import part", Application.dataPath, "dubp(part)", false);

        if (paths.Length > 0)
        {
            string path = paths[0];

            partParametrs = JsonUtility.FromJson<PartParametrs>(File.ReadAllText(path));
            partParametrs.Load();
        }
    }

    public void ExportPart()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Export part", Application.dataPath, "Part name", "dubp(part)");

        if (!string.IsNullOrEmpty(path))
        {
            partParametrs.Set();
            File.WriteAllText(path, JsonUtility.ToJson(partParametrs));
        }
    }


    public void LoadOBJ()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Import model", Application.dataPath, "obj", false); // массив ссылок на выбранные файлы
        OBJImporter importer;

        for(int i = 0; i < paths.Length; i++)
        {
            string path = paths[i]; // уже готовая ссылка на файл obj

            importer = new OBJImporter(path);
            Transform imported = importer.Import().transform;

            for(int j = 0; j < imported.childCount; j++)
            {

                Transform curChild = imported.GetChild(i);
                curChild.tag = "Imported";
                curChild.SetParent(null);

                Destroy(imported.gameObject);


                curChild.gameObject.AddComponent<MeshCollider>().sharedMesh = curChild.gameObject.GetComponent<MeshFilter>().mesh;

                curChild.position = GameObject.Find("Voxelator").transform.Find("Grid").GetComponent<Voxelator.GridControl>().center;

                //curChild.GetComponent<MeshRenderer>().material.SetOverrideTag("RenderingMode", "Cutout");
                //curChild.GetComponent<MeshRenderer>().material.SetFloat("_Mode", 2f);


                MeshRenderer meshRenderer = curChild.GetComponent<MeshRenderer>();
                Material[] materials = new Material[meshRenderer.materials.Length];
                for (int l = 0; l < meshRenderer.materials.Length; l++)
                {
                    materials[l] = obj_material;
                }
                meshRenderer.materials = materials;

                //meshRenderer.gameObject.AddComponent<SetterMaterial>().material = obj_material;
                

                //StandardShaderUtils.ChangeRenderingMode(curChild.GetComponent<MeshRenderer>().material,
                //    StandardShaderUtils.BlendMode.Fade);
            }

            //GetComponent<ImporterOBJ>().ReadFile(path); //вызов моего импортера
            break;
        }
    }
}
