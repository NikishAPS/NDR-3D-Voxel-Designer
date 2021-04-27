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

    private void OnAlpha1()
    {
        mode = 0;
    }

    private void OnAlpha2()
    {
        mode = 1;
    }

    private void OnAlpha3()
    {
        mode = 2;
    }

    private void Update()
    {
        // print(SceneData.debug);

            print(SceneData.chunk.gameObject.name);
        if (mode >= 0 && mode < employees.Length)
        {
            employees[mode].Invoke();
        }

        //print(SceneData.debug);
    }

    public void SetMode(int value)
    {
        mode = value;
    }
}
