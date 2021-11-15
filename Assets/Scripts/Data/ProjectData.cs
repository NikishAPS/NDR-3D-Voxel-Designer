using System;
using UnityEngine;

[Serializable]
public class ProjectData
{
    public void Collect()
    {
        _chunksManagerData = ChunkManager.GetData();
    }

    public void Distribute()
    {
        ChunkManager.SetData(_chunksManagerData);
    }

    public string GetSavePath()
    {
        return _saveLocation;
    }

    public void SetSaveLocation(string path)
    {
        _saveLocation = path;
    }

    //project data
    [SerializeField]
    private string _saveLocation;

    //chunk data
    [SerializeField]
    private ChunkManagerData _chunksManagerData;

}