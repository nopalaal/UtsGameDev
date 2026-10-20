using UnityEngine;

namespace Geprek.UI
{
    /// <summary>Palet dan ukuran baku UI, dikumpulkan supaya tampilan konsisten.</summary>
    public static class UIStyle
    {
        // === Gaya Cartoon: warna lebih cerah, jenuh, dan kontras ===
        public static readonly Color Ink        = new(0.10f, 0.07f, 0.05f);        // hitam kuat untuk outline
        public static readonly Color InkSoft    = new(0.30f, 0.24f, 0.18f);
        public static readonly Color Cream      = new(1.00f, 0.97f, 0.88f);        // krem lebih bersih
        public static readonly Color Panel      = new(1.00f, 0.97f, 0.88f, 0.98f);
        public static readonly Color PanelDark  = new(0.15f, 0.10f, 0.08f, 0.96f);
        public static readonly Color Wood       = new(0.72f, 0.44f, 0.18f);        // cokelat lebih hangat & jenuh
        public static readonly Color Chili      = new(0.95f, 0.22f, 0.18f);        // merah cerah cartoon
        public static readonly Color ChiliDark  = new(0.75f, 0.12f, 0.10f);
        public static readonly Color Leaf       = new(0.22f, 0.78f, 0.30f);        // hijau segar cartoon
        public static readonly Color Gold       = new(1.00f, 0.82f, 0.10f);        // kuning cerah cartoon
        public static readonly Color Sky        = new(0.20f, 0.62f, 0.95f);        // biru langit cerah
        public static readonly Color Dim        = new(0f,    0f,    0f,    0.55f);
        public static readonly Color NightTint  = new(0.12f, 0.15f, 0.40f, 0.45f);

        // Warna tambahan gaya cartoon
        public static readonly Color CartoonOutline = new(0.08f, 0.05f, 0.03f, 1f); // outline hitam tebal
        public static readonly Color CartoonShadow  = new(0.00f, 0.00f, 0.00f, 0.35f);
        public static readonly Color Tangerine      = new(1.00f, 0.55f, 0.10f);     // oranye untuk aksen
        public static readonly Color Rose           = new(0.98f, 0.35f, 0.55f);     // pink aksen kartun

        // === Font lebih besar agar terasa bold dan cartoon ===
        public const int FontTitle   = 52;   // naik dari 44
        public const int FontHeading = 30;   // naik dari 26
        public const int FontBody    = 20;   // naik dari 18
        public const int FontSmall   = 16;   // naik dari 15

        static Font _font;

        /// <summary>Font bawaan Unity. Nama aset berbeda antar versi, jadi dicoba berurutan.</summary>
        public static Font Font
        {
            get
            {
                if (_font != null) return _font;
                string[] candidates = { "LegacyRuntime.ttf", "Arial.ttf" };
                foreach (var name in candidates)
                {
                    _font = Resources.GetBuiltinResource<Font>(name);
                    if (_font != null) return _font;
                }
                _font = Font.CreateDynamicFontFromOSFont("Helvetica", FontBody);
                return _font;
            }
        }
    }
}
