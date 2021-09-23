using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAxis : MonoBehaviour
{
    public bool Highlighted { get => _luminaire.activeSelf; set => _luminaire.SetActive(value); }
    public Vector3 Direction => _direction;

    [SerializeField] private Vector3 _direction;
    [SerializeField] private GameObject _luminaire;

    private void Awake()
    {
        _luminaire.SetActive(false);
    }
}
