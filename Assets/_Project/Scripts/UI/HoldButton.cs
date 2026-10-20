using Geprek.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Geprek.UI
{
    /// <summary>
    /// Tombol interaksi layar yang juga mendukung tahan, untuk menggeprek.
    /// Sekarang dilengkapi umpan balik visual: skala mengecil saat ditekan,
    /// cincin isi saat menahan (hold), dan label aksi.
    /// </summary>
    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // ---- umpan balik visual ----
        RectTransform _rt;
        Image _holdRing;
        Text _actionLabel;
        bool _pressed;
        float _pressScale = 0.88f;
        float _currentScale = 1f;
        float _holdProgress;
        bool _holdActive;

        void Awake()
        {
            _rt = GetComponent<RectTransform>();
            BuildHoldRing();
            BuildActionLabel();
        }

        /// <summary>Lingkaran isi di sekitar tombol, menunjukkan progres menahan.</summary>
        void BuildHoldRing()
        {
            var go = new GameObject("HoldRing", typeof(RectTransform));
            var rt = (RectTransform)go.transform;
            rt.SetParent(_rt, false);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(128f, 128f);

            _holdRing = go.AddComponent<Image>();
            _holdRing.sprite = UIFactory.Circle;
            _holdRing.type = Image.Type.Filled;
            _holdRing.fillMethod = Image.FillMethod.Radial360;
            _holdRing.fillOrigin = (int)Image.Origin360.Top;
            _holdRing.fillClockwise = true;
            _holdRing.fillAmount = 0f;
            _holdRing.color = new Color(1f, 1f, 1f, 0.45f);
            _holdRing.raycastTarget = false;

            // letakkan di bawah ikon tombol supaya tidak menutupinya
            rt.SetAsFirstSibling();
        }

        /// <summary>Label kecil di atas tombol yang menunjukkan aksi saat ini.</summary>
        void BuildActionLabel()
        {
            var go = new GameObject("ActionLabel", typeof(RectTransform));
            var rt = (RectTransform)go.transform;
            rt.SetParent(_rt, false);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0f, 6f);
            rt.sizeDelta = new Vector2(140f, 26f);

            _actionLabel = go.AddComponent<Text>();
            _actionLabel.font = UIStyle.Font;
            _actionLabel.fontSize = UIStyle.FontSmall;
            _actionLabel.color = UIStyle.Cream;
            _actionLabel.alignment = TextAnchor.MiddleCenter;
            _actionLabel.fontStyle = FontStyle.Bold;
            _actionLabel.horizontalOverflow = HorizontalWrapMode.Overflow;
            _actionLabel.raycastTarget = false;
            _actionLabel.text = "";

            // outline tipis supaya teks terbaca di atas latar terang
            var outline = go.AddComponent<Outline>();
            outline.effectColor = UIStyle.CartoonOutline;
            outline.effectDistance = new Vector2(1f, -1f);
        }

        // ---- input ----

        public void OnPointerDown(PointerEventData e)
        {
            _pressed = true;
            GeprekInput.Instance?.PressInteract();
        }

        public void OnPointerUp(PointerEventData e)
        {
            _pressed = false;
            GeprekInput.Instance?.ReleaseInteract();
        }

        void OnDisable()
        {
            _pressed = false;
            GeprekInput.Instance?.ReleaseInteract();
        }

        // ---- visual update ----

        void Update()
        {
            // --- animasi skala tombol ---
            float targetScale = _pressed ? _pressScale : 1f;
            _currentScale = Mathf.Lerp(_currentScale, targetScale, Time.unscaledDeltaTime * 18f);
            if (_rt != null)
                _rt.localScale = Vector3.one * _currentScale;

            // --- cincin hold ---
            if (_holdRing != null)
            {
                // geser fillAmount halus ke target
                float target = _holdActive ? _holdProgress : 0f;
                _holdRing.fillAmount = Mathf.Lerp(_holdRing.fillAmount, target, Time.unscaledDeltaTime * 12f);
                _holdRing.enabled = _holdRing.fillAmount > 0.01f;
            }
        }

        // ---- dipanggil UIRoot untuk sinkron visual ----

        /// <summary>Panggil setiap frame saat tombol digunakan untuk hold (geprek).</summary>
        public void SetHoldProgress(float progress01)
        {
            _holdActive = true;
            _holdProgress = Mathf.Clamp01(progress01);
        }

        /// <summary>Berhenti menampilkan progres hold.</summary>
        public void ClearHoldProgress()
        {
            _holdActive = false;
            _holdProgress = 0f;
        }

        /// <summary>Ubah teks label aksi di atas tombol.</summary>
        public void SetActionLabel(string text)
        {
            if (_actionLabel != null) _actionLabel.text = text ?? "";
        }
    }
}
