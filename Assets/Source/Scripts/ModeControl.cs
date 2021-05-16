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



    private void Start()
    {
        mode = 0;
        employees = new Mode[]
        {
            new Builder(),
            new Selector(),
            new Editor()
        };

        SceneData.eventInput.alpha1 += OnAlpha1;
        SceneData.eventInput.alpha2 += OnAlpha2;
        SceneData.eventInput.alpha3 += OnAlpha3;

        SceneData.eventInput.tab += OnTab;
    }

    void OnTab()
    {

    }

   

    public void SwitchMode(int value)
    {
        employees[mode].Disable();
        mode = value;
        employees[mode].Enable();

        switcherMode.SwitchMode(value);
    }

    private void Update()
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
        switcherMode.UpdateButtons();
    }

    private void OnAlpha2()
    {
        SwitchMode(1);
        switcherMode.UpdateButtons();
    }

    private void OnAlpha3()
    {
        SwitchMode(2);
        switcherMode.UpdateButtons();
    }

    private void UpdateSwitch()
    {
        switcherMode.UpdateButtons();
    }
}
