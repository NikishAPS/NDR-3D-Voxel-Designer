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
    [SerializeField]
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
        if (_this._selectedModel != null) _this._selectedModel.GetComponent<Collider>().enabled = true;
        _this._selectedModel = model;
        if (_this._selectedModel != null) _this._selectedModel.GetComponent<Collider>().enabled = false;
    }

    public static Vector3? OnMove(Vector3 dragValue)
    {
        _this._selectedModel.position += dragValue;
        return dragValue;
    }

    public static Vector3? OnScale(Vector3 scaleValue)
    {
        float sign = Mathf.Sign(InputEvent.MouseSpeed.y);
        _this._selectedModel.localScale += Vector3.one * scaleValue.magnitude * sign;
        return scaleValue;
    }

    public static void ResizeSelectedModel(Vector3 startSize, Vector3 offset)
    {

    }


    private void Awake()
    {
        _this = FindObjectOfType<OBJControl>();
    }
}
