using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeControl : MonoBehaviour
{
    [SerializeField]
    private int mode = -1;

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

   

    public void ChangeMode(int value)
    {
        employees[mode].Disable();
        mode = value;
        employees[mode].Enable();
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
        ChangeMode(0);
    }

    private void OnAlpha2()
    {
        ChangeMode(1);
    }

    private void OnAlpha3()
    {
        ChangeMode(2);
    }

    public void SetMode(int value)
    {
        mode = value;
    }
}
