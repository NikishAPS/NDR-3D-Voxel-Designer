namespace Source.Components.Controllers
{
    using UnityEngine;

    using GUI;

    [AddComponentMenu("Source/Controllers/Camera Controller"), DisallowMultipleComponent]
    public sealed class CameraController : MonoBehaviour
    {
        [SerializeField, HideInInspector] private Transform _transform = default;

        [Header("Controller Settings")]
        [SerializeField] private float _rotationSpeed = default;
        [SerializeField] private float _movementSpeed = default;

        private Vector3 _rotationStart;

        private void Update()
        {
            if (!GUIBlock.isBlocked)
            {
                if (Input.GetMouseButtonDown(1))
                    _rotationStart = _transform.eulerAngles;
                else if (Input.GetMouseButton(1))
                {
                    _rotationStart.x -= _rotationSpeed * Input.GetAxis("Mouse Y");
                    _rotationStart.y += _rotationSpeed * Input.GetAxis("Mouse X");

                    _transform.eulerAngles = _rotationStart;
                }
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.W))
                _transform.position += _transform.forward * _movementSpeed * Time.deltaTime;
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.S))
                _transform.position -= _transform.forward * _movementSpeed * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKey(KeyCode.A))
                _transform.position -= _transform.right * _movementSpeed * Time.deltaTime;
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKey(KeyCode.D))
                _transform.position += _transform.right * _movementSpeed * Time.deltaTime;
        }

#if UNITY_EDITOR
        private void Reset() => _transform = transform;
#endif
    }
}