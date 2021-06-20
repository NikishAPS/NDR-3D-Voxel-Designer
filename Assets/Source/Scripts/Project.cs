using FileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Project : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public void Collect()
        {
            _gridSize = SceneData.grid.Size;

            _chunkSize = SceneData.chunk.Size;
        }

        public void Distribute()
        {
            SceneData.grid.Resize(_gridSize);


            SceneData.chunk.Resize(_chunkSize);
        }

        [SerializeField]
        private Vector3 _gridSize;

        [SerializeField]
        private Vector3Int _chunkSize;
    }

    [SerializeField]
    Data data = new Data();
    

    [SerializeField]
    private Panel exitPanel;

    private bool isExit = false;

    private void Awake()
    {
        Application.wantsToQuit += ExitProcessing;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitProcessing();
    }

    private bool ExitProcessing()
    {
        exitPanel.Open();
        return isExit;
    }

    public void ApplicationQuit()
    {
        isExit = true;
        Application.Quit();
    }

    public void Save(bool exit)
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save", Application.dataPath, "Name", "txt");
        
        if(!string.IsNullOrEmpty(path))
        {
            data.Collect();
            File.WriteAllText(path, JsonUtility.ToJson(data));


            if (exit)
                ApplicationQuit();
        }
    }

    public void Load()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Load", Application.dataPath, "txt", false);

        if(paths.Length > 0)
        {
            data = JsonUtility.FromJson<Data>(File.ReadAllText(paths[0]));
            data.Distribute();
        }
    }
}

