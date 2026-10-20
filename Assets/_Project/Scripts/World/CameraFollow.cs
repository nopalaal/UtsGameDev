using UnityEngine;

namespace Geprek.World
{
    /// <summary>Kamera mengikuti pemain dengan pelembutan dan dibatasi kotak lokasi.</summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target & Gerakan")]
        [SerializeField] Transform target;
        [SerializeField] float smoothTime = 0.14f;
        [SerializeField] Vector2 offset = new(0f, 0.4f);

        [Header("Batas Gerak")]
        [SerializeField] bool clampToBounds = true;
        [SerializeField] bool clampScreenEdges = false;
        [SerializeField] Vector2 boundsMin = new(-8f, -6f);
        [SerializeField] Vector2 boundsMax = new(8f, 6f);

        Camera _cam;
        Vector3 _velocity;
        Vector3 _shakeOffset;
        float _shakeAmount, _shakeLeft, _shakeTotal;
        Vector3? _focusPoint;

        void Awake()
        {
            _cam = GetComponent<Camera>();
            FindTarget();
        }

        void Start()
        {
            FindTarget();
        }

        public void SetTarget(Transform t) => target = t;

        /// <summary>Cari target pemain secara otomatis jika belum di-assign di inspector.</summary>
        public void FindTarget()
        {
            if (target != null) return;

            var pc = Object.FindAnyObjectByType<Geprek.Player.PlayerController>();
            if (pc != null)
            {
                target = pc.transform;
                return;
            }

            var playerObj = GameObject.FindWithTag("Player") ?? GameObject.Find("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }

        /// <summary>
        /// Arahkan kamera ke satu titik, mengabaikan pemain. Dipakai adegan cerita.
        /// Kirim null untuk mengembalikannya mengikuti pemain.
        /// </summary>
        public void FocusOn(Vector3? worldPoint) => _focusPoint = worldPoint;

        public void SetBounds(Vector2 min, Vector2 max)
        {
            boundsMin = min; boundsMax = max;
        }

        public void SetClampToBounds(bool clamp) => clampToBounds = clamp;

        /// <summary>Atur zoom. Ruangan yang lebih kecil dipandang lebih dekat.</summary>
        public void SetSize(float orthographicSize)
        {
            if (_cam == null) _cam = GetComponent<Camera>();
            if (_cam != null) _cam.orthographicSize = Mathf.Max(1f, orthographicSize);
        }

        /// <summary>Getar sesaat. Dipakai untuk kejadian keras seperti ayam gosong.</summary>
        public void Shake(float amount, float duration)
        {
            _shakeAmount = Mathf.Max(_shakeAmount, amount);
            _shakeTotal = Mathf.Max(0.05f, duration);
            _shakeLeft = _shakeTotal;
        }

        void LateUpdate()
        {
            if (target == null && _focusPoint == null)
            {
                FindTarget();
                if (target == null) return;
            }

            Vector2 anchor = _focusPoint.HasValue ? (Vector2)_focusPoint.Value : (Vector2)target.position;
            Vector3 desired = Clamp(anchor + offset);
            desired.z = transform.position.z;

            float dampTime = Mathf.Max(0.001f, smoothTime);
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, dampTime);

            if (_shakeLeft > 0f)
            {
                _shakeLeft -= Time.unscaledDeltaTime;
                float falloff = Mathf.Clamp01(_shakeLeft / _shakeTotal);
                _shakeOffset = (Vector3)Random.insideUnitCircle * (_shakeAmount * falloff);
                transform.position += _shakeOffset;
                if (_shakeLeft <= 0f) _shakeAmount = 0f;
            }
        }

        public void SnapToTarget()
        {
            if (target == null && _focusPoint == null)
            {
                FindTarget();
                if (target == null) return;
            }

            Vector2 anchor = _focusPoint.HasValue ? (Vector2)_focusPoint.Value : (Vector2)target.position;
            Vector3 p = Clamp(anchor + offset);
            p.z = transform.position.z;
            transform.position = p;
            _velocity = Vector3.zero;
        }

        Vector2 Clamp(Vector2 p)
        {
            if (!clampToBounds) return p;

            if (_cam == null) _cam = GetComponent<Camera>();
            if (_cam == null) return p;

            float bMinX = Mathf.Min(boundsMin.x, boundsMax.x);
            float bMaxX = Mathf.Max(boundsMin.x, boundsMax.x);
            float bMinY = Mathf.Min(boundsMin.y, boundsMax.y);
            float bMaxY = Mathf.Max(boundsMin.y, boundsMax.y);

            if (clampScreenEdges)
            {
                float halfH = _cam.orthographicSize;
                float halfW = halfH * _cam.aspect;

                float minX = bMinX + halfW, maxX = bMaxX - halfW;
                float minY = bMinY + halfH, maxY = bMaxY - halfH;

                p.x = minX <= maxX ? Mathf.Clamp(p.x, minX, maxX) : Mathf.Clamp(p.x, bMinX, bMaxX);
                p.y = minY <= maxY ? Mathf.Clamp(p.y, minY, maxY) : Mathf.Clamp(p.y, bMinY, bMaxY);
            }
            else
            {
                p.x = Mathf.Clamp(p.x, bMinX, bMaxX);
                p.y = Mathf.Clamp(p.y, bMinY, bMaxY);
            }

            return p;
        }
    }
}
