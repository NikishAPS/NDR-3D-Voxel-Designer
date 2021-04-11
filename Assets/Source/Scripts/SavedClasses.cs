using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO; //Для работы с файловым потоком
//using Dummiesman;

using MyGUI;




//Класс параметров вокселя
[Serializable]
public struct VoxelParam
{
    public Vector3 pos; //Координаты вокселя
    public Vector3[] offset; //Координаты вершин вокселя
    public Material material;
}



//Класс, хранящий параметры проекта (для загрузки всей сцены)
[Serializable]
public class ProjectParameters
{
    public string gridWidth, gridLength, gridHeight; //Параметры сетки
    public int voxelSettingsMode; //Режим шага вершин (1, 4, 8, 16, 32)
    public VoxelParam[] voxelParam;

    public Color color;

    //Метод класса, отвечающий за заполнение полей класса
    public void Set()
    {
        CursorPanelsGUI cursorPanelsGUI = Camera.main.transform.GetComponent<CursorPanelsGUI>();
        voxelSettingsMode = cursorPanelsGUI.windowGUI[0].caption[1].transform.Find("Switch").GetComponent<MySwitch>().mode;

        gridWidth = GameObject.Find("Width").transform.Find("InputField").GetComponent<InputField>().text;
        gridLength = GameObject.Find("Depth").transform.Find("InputField").GetComponent<InputField>().text;
        gridHeight = GameObject.Find("Height").transform.Find("InputField").GetComponent<InputField>().text;

        GameObject[] go_Voxels = GameObject.FindGameObjectsWithTag("Voxel");
        voxelParam = new VoxelParam[go_Voxels.Length];

        for (int i = 0; i < voxelParam.Length; i++)
        {
            voxelParam[i].pos = go_Voxels[i].transform.position;


            Voxelator.VoxelMeshGenerator voxelMeshGenerator = go_Voxels[i].GetComponent<Voxelator.VoxelMeshGenerator>();
            voxelParam[i].offset = new Vector3[voxelMeshGenerator.offset.Length];

            for (int j = 0; j < voxelMeshGenerator.offset.Length; j++)
            {
                voxelParam[i].offset[j] = voxelMeshGenerator.offset[j];
            }

            //Сохранение материала
            voxelParam[i].material = go_Voxels[i].GetComponent<Voxelator.VoxelMaterials>().standard;
        }
    }

    //Метод класса, отвечающий за применение полей класса к сцене (воспроизведение сохранения)
    public void Load()
    {
        Camera.main.GetComponent<Voxelator.VoxelsControl>().Clear();

        CursorPanelsGUI cursorPanelsGUI = Camera.main.transform.GetComponent<CursorPanelsGUI>();
        cursorPanelsGUI.windowGUI[0].caption[1].transform.Find("Switch").GetComponent<MySwitch>().mode = voxelSettingsMode;
        Camera.main.GetComponent<Voxelator.VoxelsControl>().BlockSwitch();

        GameObject.Find("Width").transform.Find("InputField").GetComponent<InputField>().text = gridWidth;
        GameObject.Find("Depth").transform.Find("InputField").GetComponent<InputField>().text = gridLength;
        GameObject.Find("Height").transform.Find("InputField").GetComponent<InputField>().text = gridHeight;

        GameObject.Find("Grid").GetComponent<Voxelator.GridControl>().UpdateSize();


        Voxelator.CreateInstanceVoxel createInstanceVoxel = GameObject.Find("CreateInstanceVoxel").GetComponent<Voxelator.CreateInstanceVoxel>();


        for (int i = 0; i < voxelParam.Length; i++)
        {
            Voxelator.VoxelMeshGenerator voxelMeshGenerator = createInstanceVoxel.Create(voxelParam[i].pos);

            for (int j = 0; j < voxelMeshGenerator.offset.Length; j++)
            {

                voxelMeshGenerator.offset[j] = voxelParam[i].offset[j];
            }

            //Загрузка материала
            voxelMeshGenerator.GetComponent<Voxelator.VoxelMaterials>().SetStandard(voxelParam[i].material);
        }
    }
}

//Класс, хранящий параметры вокселей (для загрузки части другого проекта)
[Serializable]
public class PartParametrs
{
    public VoxelParam[] voxelParam;

    //Метод класса, отвечающий за заполнение полей класса
    public void Set()
    {
        //Сохранение массива выделенных выкселей
        Voxelator.VoxelsControl voxelsControl = Camera.main.transform.GetComponent<Voxelator.VoxelsControl>();


        //Сохранение позиции и координат вершин выделенных вокселей
        voxelParam = new VoxelParam[voxelsControl.selectedVoxels.Count];

        for (int i = 0; i < voxelParam.Length; i++)
        {
            //Сохранение позиции
            voxelParam[i].pos = voxelsControl.selectedVoxels[i].transform.position;

            //Сохранение координат вершин
            Voxelator.VoxelMeshGenerator voxelMeshGenerator = voxelsControl.selectedVoxels[i].GetComponent<Voxelator.VoxelMeshGenerator>();
            voxelParam[i].offset = new Vector3[voxelMeshGenerator.offset.Length];

            for (int j = 0; j < voxelMeshGenerator.offset.Length; j++)
            {
                voxelParam[i].offset[j] = voxelMeshGenerator.offset[j];

            }

            //Сохранение материала
            voxelParam[i].material = voxelsControl.selectedVoxels[i].GetComponent<Voxelator.VoxelMaterials>().standard;
        }
    }

    //Метод класса, отвечающий за применение полей класса к вокселям (загрузка только вокселей)
    public void Load()
    {
        //Подключение скрипта по управлению вокселями
        Voxelator.VoxelsControl voxelsControl = Camera.main.transform.GetComponent<Voxelator.VoxelsControl>();

        //Сбрасывание выделения с вокселей
        voxelsControl.DeselectAll();


        //CreateInstanceVoxel - Создатель вокселей
        Voxelator.CreateInstanceVoxel createInstanceVoxel = GameObject.Find("CreateInstanceVoxel").GetComponent<Voxelator.CreateInstanceVoxel>();


        //Загрузка вокселей
        for (int i = 0; i < voxelParam.Length; i++)
        {
            //Создание вокселя
            Voxelator.VoxelMeshGenerator voxelMeshGenerator = createInstanceVoxel.Create(voxelParam[i].pos);

            //Назначаем текущий воксель в массив выделенных вокселей
            voxelsControl.selectedVoxels.Add(voxelMeshGenerator.gameObject);

            for (int j = 0; j < voxelMeshGenerator.offset.Length; j++)
            {
                //Загрузка вершин
                voxelMeshGenerator.offset[j] = voxelParam[i].offset[j];
            }

            //Загрузка материала
            voxelsControl.selectedVoxels[i].GetComponent<Voxelator.VoxelMaterials>().SetStandard(voxelParam[i].material);
        }

        //Назачение материала для выделенных (импортированных только что) вокселей
        if (voxelsControl.mode == 2)
            voxelsControl.SetMaterialsVoxels(true);
    }
}
