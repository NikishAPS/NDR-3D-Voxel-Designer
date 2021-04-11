using System.Runtime.CompilerServices;

namespace Source.Components.Voxelator
{
    using UnityEngine;

    [AddComponentMenu("Source/Voxelator/Voxel"), RequireComponent(typeof(MeshRenderer), typeof(Collider)), DisallowMultipleComponent]
    public sealed class Voxel : MonoBehaviour
    {
        [SerializeField, HideInInspector] private Collider _collider = default;
        [SerializeField, HideInInspector] private MeshRenderer _meshRenderer = default;

        [Header("References")]
        [SerializeField] private Material _material = default;
        [SerializeField] private Material _materialSelection = default;

        private bool _isSelected;

        public bool isTrigger
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _collider.enabled;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _collider.enabled = value;
        }

        public bool isSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    _meshRenderer.sharedMaterial = _isSelected ? _materialSelection : _material;
                }
            }
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _collider = GetComponent<Collider>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }
#endif
    }
}