using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDataInit : MonoBehaviour
{
    private void OnEnable()
    {
        SceneData.camera = Camera.main;
        SceneData.mainCamera = Camera.main.transform;
        SceneData.chunk = FindObjectOfType<Chunk>();
        SceneData.grid = FindObjectOfType<Grid>();
        SceneData.extractor = FindObjectOfType<Extractor>();
        SceneData.controlGUI = FindObjectOfType<ControlGUI>();
        SceneData.eventInput = FindObjectOfType<EventInput>();
        SceneData.dragSystem = FindObjectOfType<DragSystem>();

        SceneData.colorTest = FindObjectOfType<ColorTest>();

        SceneData.voxelVertices = new Vector3[]
        {
            //left
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),

            //right
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),

            //bottom
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),

            //top
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(+0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),

            //rear
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),

            //front
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f)
    };

    }

    private void Start()
    {
        //Destroy(gameObject);
    }
}
