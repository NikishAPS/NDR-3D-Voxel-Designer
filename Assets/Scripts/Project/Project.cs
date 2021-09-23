using FileBrowser;
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
            _chunksManagerData = ChunksManager.GetData();
        }
        public void Distribute()
        {
            //SceneData.Chunk.SetData(_chunkData);
            ChunksManager.SetData(_chunksManagerData);
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
        private ChunksManagerData _chunksManagerData;
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

        ChunksManager.InitChunks();
        GridManager.Grids[Direction.Down].Size = new Vector3Int(ChunksManager.FieldSize.x, 1, ChunksManager.FieldSize.z);
        GridManager.Grids[Direction.Down].Active = true;
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
        if (!Saved)
        {
            Ouit();
        }
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

    private void OnDrawGizmos()
    {
        return;

        if(ChunksManager.Vertices != null)
        {
            for(int i = 0; i < ChunksManager.Vertices.Length;i ++)
            {
                if(ChunksManager.Vertices[i] != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(ChunksManager.Vertices[i].Position, 0.1f);
                }
            }
        }

        //return;
        //if(ChunksManager.Chunks != null)
        //{
        //    for(int i = 0; i < ChunksManager.Chunks.Length && ChunksManager.Chunks[i].Vertices != null; i++)
        //    {
        //        for(int j = 0; j < ChunksManager.Chunks[i].Vertices.Length; j++)
        //        {
        //            if(ChunksManager.Chunks[i].Vertices[j] != null)
        //            {
        //                Gizmos.color = Color.red;
        //                Gizmos.DrawSphere(ChunksManager.Chunks[i].Vertices[j].Position, 0.1f);
        //            }
        //        }
        //    }
        //}

        //return;

        //if (ChunksManager.Chunks == null) return;

        //Gizmos.color = Color.green;
        //for (int i = 0; i < ChunksManager.Chunks.Length; i++)
        //{
        //    Vector3Int pos = ChunksManager.Chunks[i].Position;
        //    Vector3Int size = ChunksManager.Chunks[i].Size;

        //    Gizmos.DrawWireCube(pos.Mul(ChunksManager.ChunkSize) + size.ToVector3() * 0.5f - Vector3.one * 0.5f, size);
        //}
    }
}

