using System.IO;
using UnityEngine;
using Dummiesman;


public class OBJImporter
{
    private string _objectPath;
    private GameObject _importedObject;

    public OBJImporter(string path)
    {
        _objectPath = path;
    }

    public GameObject Import()
    {
        if (File.Exists(_objectPath))
        {
            if (_importedObject != null)
                Object.Destroy(_importedObject);

            _importedObject = new OBJLoader().Load(_objectPath);

            return _importedObject;
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }

}
