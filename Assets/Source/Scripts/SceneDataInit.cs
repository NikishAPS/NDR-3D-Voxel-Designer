using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDataInit : MonoBehaviour
{
    private void Awake()
    {
        SceneData.mainCamera = Camera.main.transform;
        SceneData.chunk = FindObjectOfType<Chunk>();
        SceneData.grid = FindObjectOfType<Grid>();
        SceneData.extractor = FindObjectOfType<Extractor>();
        SceneData.controlGUI = FindObjectOfType<ControlGUI>();
    }

    private void Start()
    {
        Destroy(gameObject);
    }
}
