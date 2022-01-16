using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPanel : Panel
{
    public enum AnswerType { Indefinite, Confirm, Reject, Cancel }
    private AnswerType _answer = AnswerType.Indefinite;

    [SerializeField] private Text _panelTitle, _confirmTitle, _rejectTitle, _cancelTitle;
    private Action _confirmMethod, _rejectMethod, _cancelMethod;

    public override void OnOpen() => PanelManager.PinThePanel(this);
    public override void OnClose()
    {
        _answer = AnswerType.Indefinite;
        _confirmMethod = _rejectMethod = _cancelMethod = null;
        PanelManager.PinThePanel(null);
    }

    public void SetTitles(string panelTitle, string confirmTitle, string rejectTitle, string cancelTitle)
    {
        _panelTitle.text = panelTitle;
        _confirmTitle.text = confirmTitle;
        _rejectTitle.text = rejectTitle;
        _cancelTitle.text = cancelTitle;
    }

    public string ConfirmTitle { set => _confirmTitle.text = value; }
    public string RejectTitle { set => _rejectTitle.text = value; }
    public string CancelTitle { set => _cancelTitle.text = value; }

    public void SetConfirmMethod(Action confirmMethod) => _confirmMethod = confirmMethod;
    public void SetRejectMethod(Action rejectMethod) =>_rejectMethod = rejectMethod;
    public void SetCancelMethod(Action cancelMethod) => _cancelMethod = cancelMethod;

    public void OnConfirm()
    {
        _answer = AnswerType.Confirm;
        _confirmMethod?.Invoke();
        Close();
    }

    public void OnReject()
    {
        _answer = AnswerType.Reject;
        _rejectMethod?.Invoke();
        Close();
    }

    public void OnCancel()
    {
        _answer = AnswerType.Cancel;
        _cancelMethod?.Invoke();
        Close();
    }

    public async Task<AnswerType> GetAnswer()
    {
        return await Task.Run(()=>
        {
            while (_answer == AnswerType.Indefinite) { }
            return _answer;
        });
    }

}
