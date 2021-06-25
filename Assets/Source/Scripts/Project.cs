﻿using FileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
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
            chunkData = SceneData.chunk.GetData();
        }

        public void Distribute()
        {
            SceneData.chunk.SetData(chunkData);
        }

        //chunk data
        [SerializeField]
        private ChunkData chunkData;
    }

    [SerializeField]
    Data data = new Data();

    [SerializeField]
    private bool _saved = false;

    [SerializeField]
    private string _fileName = "Name";
    [SerializeField]
    private string _fileExtension = "nrd";

    [SerializeField]
    private Panel _exitPanel;

    private bool isExit = false;

    private void Awake()
    {
        Application.wantsToQuit += ExitProcessing;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitProcessing();

        if(Input.GetKeyDown(KeyCode.S))
        {
            Save(false);
        }
    }

    private void OnChange()
    {
        _saved = false;
    }

    private bool ExitProcessing()
    {
        _exitPanel.Open();
        return isExit;
    }

    private bool TryToSave()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save", Application.dataPath, _fileName, _fileExtension);

        if (!string.IsNullOrEmpty(path))
        {
            data.Collect();
            File.WriteAllText(path, JsonUtility.ToJson(data));

            //_saved = true;

            return true;
        }

        return false;
    }

    public void ApplicationQuit()
    {
        isExit = true;
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    public void Save(bool exit)
    {
        if(TryToSave())
        {
            if (exit)
                ApplicationQuit();
        }
    }

    public void Load()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Load", Application.dataPath, _fileExtension, false);

        if(paths.Length > 0)
        {
            data = JsonUtility.FromJson<Data>(File.ReadAllText(paths[0]));
            data.Distribute();
        }
    }
}

