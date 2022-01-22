using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class ObjModelManager
{
    public static ObjModel SelectedModel;

    private static LinkedList<ObjModel> _models = new LinkedList<ObjModel>();

    public static bool TryImport(string path)
    {
        if (!File.Exists(path)) return false;

        GameObject model = new OBJImporter(path).Import();
        _models.AddLast(new ObjModel(model, Voxelator.Center, 1));

        return false;
    }

    public static void Select(ObjModel model)
    {
        SelectedModel = model;
    }

    public static bool Delete(ObjModel model)
    {
        if (!_models.Contains(model)) return false;
        _models.Remove(model);
        return true;
    }

}