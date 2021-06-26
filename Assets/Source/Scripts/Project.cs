﻿using FileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Project : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public void Collect()
        {
            _chunkData = SceneData.chunk.GetData();
        }
        public void Distribute()
        {
            SceneData.chunk.SetData(_chunkData);
        }

        public string GetSavePath()
        {
            return _savePath;
        }

        public void SetSavePath(string path)
        {
            _savePath = path;
        }


        //project data
        [SerializeField]
        private string _savePath;

        //chunk data
        [SerializeField]
        private ChunkData _chunkData;
    }

    [SerializeField]
    Data _data = new Data();

    [SerializeField]
    private string _name;

    [SerializeField]
    private string _fileExtension = "nrd";

    [SerializeField]
    private bool _saved = false;

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

        if (Input.GetKeyDown(KeyCode.S))
        {
            Save(false);
        }
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

    public void SetName(InputField inputField)
    {
        _name = inputField.text;
    }

    public void SetSavePath(InputField inputField)
    {
        _data.SetSavePath(inputField.text);
    }

    public void StartNewProject()
    {
        SceneData.chunk.Resize();
        SceneData.modeControl.enabled = true;
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
            _data = JsonUtility.FromJson<Data>(File.ReadAllText(paths[0]));
            _data.Distribute();
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
        string path = StandaloneFileBrowser.SaveFilePanel("Save", _data.GetSavePath(), _name, _fileExtension);

        if (!string.IsNullOrEmpty(path))
        {
            _data.Collect();
            _data.SetSavePath(path);
            File.WriteAllText(path, JsonUtility.ToJson(_data));

            //_saved = true;

            return true;
        }

        return false;
    }
}

