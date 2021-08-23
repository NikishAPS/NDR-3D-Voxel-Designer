using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeControl : MonoBehaviour
{
    [SerializeField]
    private int mode = -1;

    [SerializeField]
    private Switcher switcherMode;


    private Mode[] employees = new Mode[0];



    public void Init()
    {
        mode = 0;
        employees = new Mode[]
        {
            new BuildMode(),
            new SelectMode(),
            new EditMode(),
            new ObjMode()
        };

        employees[mode].Enable();

        SceneData.EventInput.alpha1 += OnAlpha1;
        SceneData.EventInput.alpha2 += OnAlpha2;
        SceneData.EventInput.alpha3 += OnAlpha3;

        SceneData.EventInput.tab += OnTab;
    }

    public void Enable()
    {
        employees[mode].Enable();
    }

    public void Disable()
    {
        employees[mode].Disable();
    }

    void OnTab()
    {

    }

    public void SwitchMode(int value)
    {
        if (mode < 0 || mode >= employees.Length) return;

        employees[mode].Disable();
        mode = value;
        employees[mode].Enable();
    }

    private void Update1()
    {
        if (mode >= 0 && mode < employees.Length)
        {
            employees[mode].Tick();
        }

        //print(SceneData.debug);
    }

    private void OnAlpha1()
    {
        SwitchMode(0);
    }

    private void OnAlpha2()
    {
        SwitchMode(1);
    }

    private void OnAlpha3()
    {
        SwitchMode(2);
    }

}
