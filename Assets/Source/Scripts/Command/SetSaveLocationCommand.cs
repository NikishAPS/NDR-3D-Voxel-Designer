using UnityEngine;
using NDR.UI;

public class SetSaveLocationCommand : Command
{
    [SerializeField]
    private InputField _saveLocation;

    public override void Execute()
    {
        _saveLocation.String = VoxelatorManager.Project.GetFolderPath("Save Location");
    }
}
