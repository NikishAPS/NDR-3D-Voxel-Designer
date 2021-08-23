using UnityEngine;
using System.Collections.Generic;
using FileBrowser;
using System.IO;

public class OBJControl : MonoBehaviour
{
    private static OBJControl _this;
    public List<ImportedModel> Models = new List<ImportedModel>();

    [SerializeField]
    private Material _material;
    private Transform _selectedModel;

    public static void Import()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Import OBJ", Application.dataPath, "obj", true);

        for (int i = 0; i < paths.Length; i++)
        {
            Transform imported = new OBJImporter(paths[i]).Import().transform;
            imported.position = ChunksManager.Center;

            _this.Models.Add(new ImportedModel(imported, _this._material));
        }
    }

    public static void SelectModel(Transform model)
    {
        _this._selectedModel = model;
    }

    public static void MoveSelectedModel(Vector3 startPoint, Vector3 offset)
    {
        _this._selectedModel.position = startPoint + offset;
        SceneData.DragSystem.OffsetPosition(offset);
    }

    public static void ResizeSelectedModel(Vector3 startSize, Vector3 offset)
    {

    }


    private void Awake()
    {
        _this = FindObjectOfType<OBJControl>();
    }
}
