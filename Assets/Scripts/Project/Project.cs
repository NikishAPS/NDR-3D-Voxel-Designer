using System;
using System.IO;
using FileBrowser;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Project : MonoBehaviour
{
    private static StatisticsPanel _statisticsPanel;

    public static bool Saved { get; set; }

    private static ProjectData _projectData = new ProjectData();
    private static string _name;
    private static string _fileExtension = "nrd";

    private void Awake()
    {
        Application.wantsToQuit += ExitProcessing;
    }

    private void Start()
    {
        _statisticsPanel = PanelManager.GetPanel<StatisticsPanel>();
    }

    public static void Init(string name, string saveLocation)
    {
        Saved = true;

        _name = name;
        _projectData.SetSaveLocation(saveLocation);
    }

    public static void Ouit()
    {
        Application.Quit();
    }

    public static void Quit()
    {
        if (!Saved)
        {
            if (TryToSave())
            {
                Application.Quit();
            }
        }
        else
        {
            Application.Quit();
        }
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
        _projectData.SetSaveLocation(path);
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
        string path = StandaloneFileBrowser.SaveFilePanel("Save", _projectData.GetSavePath(), _name, _fileExtension);

        if (!string.IsNullOrEmpty(path))
        {
            _projectData.Collect();
            _projectData.SetSaveLocation(path);
            File.WriteAllText(path, JsonUtility.ToJson(_projectData));

            Saved = true;

            return true;
        }

        return false;
    }

    public static bool TryToLoad()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Load", _projectData.GetSavePath(), _fileExtension, false);
        if (paths.Length > 0)
        {
            _projectData = JsonUtility.FromJson<ProjectData>(File.ReadAllText(paths[0]));
            _projectData.Distribute();

            return true;
        }

        return false;
    }

}

