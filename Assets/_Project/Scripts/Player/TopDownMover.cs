using UnityEngine;

namespace Geprek.Player
{
    /// <summary>Gerak top-down berbasis Rigidbody2D supaya tabrakan dengan perabot tetap benar.</summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class TopDownMover : MonoBehaviour
    {
        [SerializeField] float speed = 4.2f;

        [Tooltip("Seberapa cepat mencapai kecepatan penuh saat mulai bergerak (lebih tinggi = lebih responsif).")]
        [SerializeField] float acceleration = 60f;

        [Tooltip("Seberapa cepat berhenti saat tidak ada input (lebih tinggi = berhenti lebih tajam).")]
        [SerializeField] float deceleration = 80f;

        Rigidbody2D _rb;
        Vector2 _input;

        public Vector2 Velocity => _rb != null ? _rb.linearVelocity : Vector2.zero;
        public float Speed { get => speed; set => speed = Mathf.Max(0f, value); }

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            // Jika prefab lama tidak memiliki nilai deceleration (terdeserialisasi sebagai 0), beri nilai aman
            if (deceleration <= 0f) deceleration = Mathf.Max(acceleration * 2f, 80f);
        }

        /// <summary>Arah gerak yang diinginkan, panjang 0..1.</summary>
        public void SetInput(Vector2 input)
        {
            _input = input.sqrMagnitude > 1f ? input.normalized : input;
        }

        public void Stop()
        {
            _input = Vector2.zero;
            if (_rb != null) _rb.linearVelocity = Vector2.zero;
        }

        void FixedUpdate()
        {
            Vector2 current = _rb.linearVelocity;
            Vector2 target  = _input * speed;

            float decelRate = deceleration > 0f ? deceleration : Mathf.Max(acceleration * 2f, 80f);
            float rate = _input.sqrMagnitude > 0.001f ? acceleration : decelRate;

            _rb.linearVelocity = Vector2.MoveTowards(current, target, rate * Time.fixedDeltaTime);

            // Jika tidak ada input dan pergerakan sudah sangat pelan, langsung hentikan total agar kembali ke idle
            if (_input == Vector2.zero && _rb.linearVelocity.sqrMagnitude < 0.01f)
            {
                _rb.linearVelocity = Vector2.zero;
            }
        }
    }
}
