using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class ObjModelManager
{
    public static Vector3 SelectedModelPosition
    {
        get => SelectedModel.Position;
        set
        {
            SelectedModel.Position = value;
            UpdateModelEvent(SelectedModel);
        }
    }
    public static float SelectedModelSize
    {
        get => SelectedModel.Size;
        set
        {
            SelectedModel.Size = value;
            UpdateModelEvent?.Invoke(SelectedModel);
        }
    }
    public static ObjModel SelectedModel;
    public static Action<ObjModel> UpdateModelEvent;
    private static LinkedList<ObjModel> _models = new LinkedList<ObjModel>();

    public static void Import(string path, Vector3 position, float size)
    {
        if (!File.Exists(path)) return;

        GameObject obj = new OBJImporter(path).Import();
        ObjModel model = obj.AddComponent<ObjModel>();
        model.Path = path;
        model.Position = position;
        model.Size = size;
        _models.AddLast(model);

        return;
    }

    public static void Select(ObjModel model)
    {
        if (!_models.Contains(model)) return;
        SelectedModel = model;
    }

    public static void DeleteSelectedModel()
    {
        if (SelectedModel == null) return;
        SelectedModel.Dispose();
        _models.Remove(SelectedModel);
    }

    public static void DragSelectedModel(DragTransform dragValue, out DragTransform dragResult)
    {
        if(SelectedModel == null)
        {
            dragResult = new DragTransform();
            return;
        }

        SelectedModelPosition += dragValue.Position;
        SelectedModelSize += dragValue.Scale.x;
        dragResult = dragValue;
    }
}
