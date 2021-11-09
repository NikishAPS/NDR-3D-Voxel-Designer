﻿using FileBrowser;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Project : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public void Collect()
        {
            _chunksManagerData = ChunkManager.GetData();
        }
        public void Distribute()
        {
            //SceneData.Chunk.SetData(_chunkData);
            ChunkManager.SetData(_chunksManagerData);
            Saved = true;
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

    private static Project _this;
    private static StatisticsPanel _statisticsPanel;

    public static bool Saved { get; set; }

    public Material ChunkMaterial => _chunkMaterial;
    public Material SelectedChunkMaterial => _selectedChunkMaterial;

    [SerializeField]
    Data _data = new Data();

    [SerializeField]
    private string _name;

    [SerializeField]
    private string _fileExtension = "nrd";

    [SerializeField]
    private Material _chunkMaterial;
    [SerializeField]
    private Material _selectedChunkMaterial;

    private void Awake()
    {
        _this = FindObjectOfType<Project>();
        Application.wantsToQuit += ExitProcessing;
    }

    private void Start()
    {
        _statisticsPanel = PanelManager.GetPanel<StatisticsPanel>();
    }

    private void OnDisable()
    {
        //ChunksManager.Release();
    }



    public void Init(string name, string saveLocation)
    {
        Saved = true;

        _name = name;
        _data.SetSaveLocation(saveLocation);

        ChunkManager.InitChunks();
        //GridManager.Grids[Direction.Down].Size = new Vector3Int(ChunkManager.FieldSize.x, 1, ChunkManager.FieldSize.z);
        //GridManager.Grids[Direction.Down].Active = true;

        //foreach (Grid grid in GridManager.Grids)
        //    grid.Size = new Vector3Int(ChunkManager.FieldSize.x, 1, ChunkManager.FieldSize.z);
        //GridManager.Grids[Direction.Down].Active = true;

        //GridManager.Grids[Direction.Down].Active = true;
        GridManager.Size = ChunkManager.FieldSize;
    }

    public static void Ouit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetSaved()
    {
        Saved = true;
    }

    public void SetName(string name)
    {
        _name = name;
    }

    public void SetSavePath(string path)
    {
        _data.SetSaveLocation(path);
    }

    public void Create()
    {
        if (Saved)
        {
            Restart();
        }
        else
        {
            if(TryToSave())
            {
                Restart();
            }
        }
    }

    public static string GetFolderPath(string title)
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel(title, Application.dataPath, false);
        if (paths.Length > 0)
        {
            return paths[0];
        }

        return null;
        //return @"C:\";
    }

    private bool ExitProcessing()
    {
        if (!Saved || true)
        {
            Ouit();
        }
        return true;
        return Saved;
    }

    public static bool TryToSave()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save", _this._data.GetSavePath(), _this._name, _this._fileExtension);

        if (!string.IsNullOrEmpty(path))
        {
            _this._data.Collect();
            _this._data.SetSaveLocation(path);
            File.WriteAllText(path, JsonUtility.ToJson(_this._data));

            Saved = true;

            return true;
        }

        return false;
    }

    public static bool TryToLoad()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Load", _this._data.GetSavePath(), _this._fileExtension, false);
        if (paths.Length > 0)
        {
            _this._data = JsonUtility.FromJson<Data>(File.ReadAllText(paths[0]));
            _this._data.Distribute();

            return true;
        }

        return false;
    }
   
}

