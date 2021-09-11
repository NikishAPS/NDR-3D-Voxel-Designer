using UnityEngine;
using UnityEngine.UI;

public class QuestionPanel : Panel
{
    [SerializeField] private Text _panelTitle, _confirmTitle, _rejectTitle, _cancelTitle;

    private Void _confirm;
    private Void _reject;
    private Void _cancel;



    public override void OnOpen()
    {
        PanelManager.PinPanel(this);
    }

    public override void OnClose()
    {
        PanelManager.PinPanel(null);
    }

    public void SetTitles(string panelTitle, string confirmTitle, string rejectTitle, string cancelTitle)
    {
        _panelTitle.text = panelTitle;
        _confirmTitle.text = confirmTitle;
        _rejectTitle.text = rejectTitle;
        _cancelTitle.text = cancelTitle;
    }

    public void SetConfirmMethod(Void confirmMethod)
    {
        _confirm = confirmMethod;
    }

    public void SetRejectMethod(Void rejectMethod)
    {
        _reject = rejectMethod;
    }

    public void SetCancelMethod(Void cancelMethod)
    {
        _cancel = cancelMethod;
    }

    public void OnConfirm()
    {
        _confirm?.Invoke();
    }

    public void OnReject()
    {
        _reject?.Invoke();
    }

    public void OnCancel()
    {
        _cancel?.Invoke();
    }
}
