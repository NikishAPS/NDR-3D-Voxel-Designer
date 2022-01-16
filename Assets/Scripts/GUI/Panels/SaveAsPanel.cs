using NDR.UI;
using FileBrowser;
using UnityEngine;

public class SaveAsPanel : Panel
{
    [SerializeField] private InputField _nameField;
    [SerializeField] private InputField _rootPathField;

    public override void OnOpen() => PanelManager.PinThePanel(this);
    public override void OnClose() => PanelManager.PinThePanel(null);

    public void OnSetSaveDirectoryPath()
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Set save location (the project folder will be created here)", Application.dataPath, false);

        if (paths.Length > 0)
        {
            _rootPathField.SetValue(paths[0]);
        }
    }

    public async void OnSaveAs()
    {
        if (Project.Exists(_nameField.String, _rootPathField.String))
        {
            QuestionPanel questionPanel = PanelManager.GetPanel<QuestionPanel>();
            questionPanel.Title.TextValue = "A project with that name already exists.Overwrite it?";
            questionPanel.ConfirmTitle = "Yes";
            questionPanel.RejectTitle = "No";
            questionPanel.CancelTitle = "Cancel";
            questionPanel.Open();

            if (await questionPanel.GetAnswer() != QuestionPanel.AnswerType.Confirm) return;
        }

        Project.TrySaveAs(_nameField.String, _rootPathField.String);
        Close();
    }

}