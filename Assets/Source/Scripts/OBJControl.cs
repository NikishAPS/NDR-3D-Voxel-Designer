using UnityEngine;
using System.Collections.Generic;
using FileBrowser;
using System.IO;

public class OBJControl : MonoBehaviour
{
    private static OBJControl _this;
    public List<ImportedModel> Models = new List<ImportedModel>();
    public static Transform SelectedModel { get; private set; }

    [SerializeField]
    private Material _material;

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
        if (SelectedModel != null) SelectedModel.GetComponent<Collider>().enabled = true;
        SelectedModel = model;
        if (SelectedModel != null) SelectedModel.GetComponent<Collider>().enabled = false;
    }

    public static Vector3? OnMove(Vector3 dragValue)
    {
        SelectedModel.position += dragValue;
        return dragValue;
    }

    public static Vector3? OnScale(Vector3 scaleValue)
    {
        float sign = Mathf.Sign(InputEvent.MouseSpeed.y);
        SelectedModel.localScale += Vector3.one * scaleValue.magnitude * sign;
        return scaleValue;
    }

    public static void ResizeSelectedModel(Vector3 startSize, Vector3 offset)
    {

    }


    private void Awake()
    {
        _this = FindObjectOfType<OBJControl>();
    }

    public static bool TryDrag(DragTransform dragValue)
    {
        //drag position
        SelectedModel.position += dragValue.Position;

        //drag scale
        float sign = Mathf.Sign(InputEvent.MouseSpeed.y);
        SelectedModel.localScale += Vector3.one * dragValue.Scale.magnitude * sign;

        return true;
    }

}
