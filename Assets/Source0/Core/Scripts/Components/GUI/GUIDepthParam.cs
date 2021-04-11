namespace Source.Components.GUI
{
    using UnityEngine;
    using UnityEngine.UI;

    using Voxelator;

    [AddComponentMenu("Source/GUI/GUI Depth Param"), DisallowMultipleComponent]
    public sealed class GUIDepthParam : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputField _inputField = default;
        [SerializeField] private VoxelGrid _voxelGrid = default;

        private void OnValueChanged(string value)
        {
            if (int.TryParse(value, out int result))
            {
                if (result <= 0)
                {
                    result = 1;
                    _inputField.text = "1";
                }
                _voxelGrid.depth = result;
            }
        }

        private void Awake() => _inputField.onValueChanged.AddListener(OnValueChanged);

        private void Start()
        {
            if (_voxelGrid.depth > 0)
                _inputField.text = _voxelGrid.depth.ToString();
            else
                _inputField.text = "1";
        }
    }
}