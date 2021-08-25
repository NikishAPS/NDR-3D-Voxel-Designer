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

    public static bool Saved
    {
        get => _saved;
        set
        {
            _saved = value;
            if(_saved)
            {
                Project project = FindObjectOfType<Project>();
                project.ChangeText.color = Color.green;
                project.ChangeText.text = "Saved";
            }
            else
            {
                Project project = FindObjectOfType<Project>();
                project.ChangeText.color = Color.red;
                project.ChangeText.text = "Not Saved";
            }
        }
    }

    public UnityEngine.UI.Text ChangeText;
    public Material ChunkMaterial => _chunkMaterial;
    public Material SelectedChunkMaterial => _selectedChunkMaterial;

    private static bool _saved;

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
        Application.wantsToQuit += ExitProcessing;
    }

    private void Update()
    {
        //int step = 1;
        //if(Input.GetKeyDown(KeyCode.A)) ChunksManager.MoveSelectedVoxels(Vector3.zero, new Vector3(-step, 0, 0));
        //if(Input.GetKeyDown(KeyCode.D)) ChunksManager.MoveSelectedVoxels(Vector3.zero, new Vector3(step, 0, 0));
        //if (Input.GetKeyDown(KeyCode.LeftControl)) ChunksManager.MoveSelectedVoxels(Vector3.zero, new Vector3(0, -step, 0));
        //if (Input.GetKeyDown(KeyCode.Space)) ChunksManager.MoveSelectedVoxels(Vector3.zero, new Vector3(0, step, 0));
        //if (Input.GetKeyDown(KeyCode.S)) ChunksManager.MoveSelectedVoxels(Vector3.zero, new Vector3(0, 0, -step));
        //if (Input.GetKeyDown(KeyCode.W)) ChunksManager.MoveSelectedVoxels(Vector3.zero, new Vector3(0, 0, step));

        //if (Input.GetKeyDown(KeyCode.Escape))
        //    ExitProcessing();

        if (Input.GetKeyDown(KeyCode.S))
        {
            //Save(false);
        }
    }

    private void OnDisable()
    {
        //ChunksManager.Release();
    }


    public void BaseInit()
    {
        SceneData.ModeControl.Init();
        ChunksManager.InitChunks();
        GridManager.Grids[Direction.Down].Size = new Vector3Int(ChunksManager.FieldSize.x, 1, ChunksManager.FieldSize.z);
        GridManager.Grids[Direction.Down].Active = true;
    }

    public void Init(string name, string saveLocation)
    {
        Saved = true;

        _name = name;
        _data.SetSaveLocation(saveLocation);

        BaseInit();

        //SceneData.ModeControl.Init();
        //GridManager.Grids[Direction.Down].Size = new Vector3Int(10, 1, 10);
        //GridManager.Grids[Direction.Down].Active = true;
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

    public void StartNewProject()
    {
        SceneData.ModeControl.Init();
        ChunksManager.InitChunks();
        GridManager.Grids[Direction.Down].Size = new Vector3Int(ChunksManager.FieldSize.x, 1, ChunksManager.FieldSize.z);
        GridManager.Grids[Direction.Down].Active = true;
    }

    public void Create()
    {
        if (_saved)
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

    public void Load()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Load", _data.GetSavePath(), _fileExtension, false);

        if(paths.Length > 0)
        {
            _data = JsonUtility.FromJson<Data>(File.ReadAllText(paths[0]));
            _data.Distribute();
        }
    }

    public void LoadAndClose(Panel panel)
    {
        if(TryToLoad())
        {
            panel.Close();
        }
    }

    public string GetFolderPath(string title)
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel(title, Application.dataPath, false);
        if (paths.Length > 0)
        {
            return paths[0];
        }

        return @"C:\";
    }



    private void OnChange()
    {
        //_saved = false;
    }

    private bool ExitProcessing()
    {
        if (!_saved)
        {
            Invoker.Execute(Commands.AppExitCommand);
        }
        return _saved;
    }

    public bool TryToSave()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save", _data.GetSavePath(), _name, _fileExtension);

        if (!string.IsNullOrEmpty(path))
        {
            _data.Collect();
            _data.SetSaveLocation(path);
            File.WriteAllText(path, JsonUtility.ToJson(_data));

            //_saved = true;

            return true;
        }

        return false;
    }

    public bool TryToLoad()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Load", _data.GetSavePath(), _fileExtension, false);
        if (paths.Length > 0)
        {
            _data = JsonUtility.FromJson<Data>(File.ReadAllText(paths[0]));
            _data.Distribute();

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

