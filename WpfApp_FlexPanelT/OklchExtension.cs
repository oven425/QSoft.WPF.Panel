using System.Windows.Markup;
using System.Windows.Media;

namespace WpfApp_FlexPanelT
{
    public class OklchExtension : MarkupExtension
    {
        // 核心屬性：接收 Oklch 分量
        public double L { get; set; } // Lightness (0.0 to 100.0)
        public double C { get; set; } // Chroma (0.0 to ~0.37)
        public double H { get; set; } // Hue (0.0 to 360.0)

        // 建構函式 (用於位置參數，例如: {local:Oklch 50, 0.1, 90})
        public OklchExtension(double l, double c, double h)
        {
            L = l;
            C = c;
            H = h;  
        }

        public OklchExtension() { }
        // 必須覆寫的核心方法：返回 XAML 屬性的值
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            try
            {
                return OklchConverter.ToSrgb(L, C, H);
            }
            catch(Exception)
            {

            }
            return Colors.Transparent;
        }
    }

    public static class OklchConverter
    {
        // 矩陣 M1: Oklab to Linear LMS
        private static readonly double[,] M1 = new double[,]
        {
            { 1.0, 0.3963377774, 0.2158037573 },
            { 1.0, -0.1055613458, -0.0638541728 },
            { 1.0, -0.0894841775, -1.2914855480 }
        };

        // 矩陣 M2: Linear LMS to Linear sRGB (M2 是 M1 矩陣的逆矩陣的立方根版本)
        private static readonly double[,] M2 = new double[,]
        {
            { 4.0722, -3.3072, 0.2309 },
            { -1.2638, 2.6033, -0.3396 },
            { -0.0041, -0.0772, 1.0773 }
        };

        // 主要轉換方法：Oklch -> sRGB Color
        public static Color ToSrgb(double l, double c, double h)
        {
            // 步驟 1: Oklch -> Oklab (L, a, b)
            // L 必須從百分比 (0-100) 轉換為 (0-1)
            double L_oklab = l / 100.0;
            double h_rad = h * (Math.PI / 180.0);

            // 避免 Chroma 為 0 時的浮點數錯誤
            double a = c * Math.Cos(h_rad);
            double b = c * Math.Sin(h_rad);

            // 步驟 2: Oklab -> Linear LMS (l, m, s)
            double l_lin = M1[0, 0] * L_oklab + M1[0, 1] * a + M1[0, 2] * b;
            double m_lin = M1[1, 0] * L_oklab + M1[1, 1] * a + M1[1, 2] * b;
            double s_lin = M1[2, 0] * L_oklab + M1[2, 1] * a + M1[2, 2] * b;

            // 步驟 3: Linear LMS -> Nonlinear LMS (Lc, Mc, Sc)
            double Lc = l_lin * l_lin * l_lin;
            double Mc = m_lin * m_lin * m_lin;
            double Sc = s_lin * s_lin * s_lin;

            // 步驟 4: Nonlinear LMS -> Linear sRGB (R_lin, G_lin, B_lin)
            double R_lin = M2[0, 0] * Lc + M2[0, 1] * Mc + M2[0, 2] * Sc;
            double G_lin = M2[1, 0] * Lc + M2[1, 1] * Mc + M2[1, 2] * Sc;
            double B_lin = M2[2, 0] * Lc + M2[2, 1] * Mc + M2[2, 2] * Sc;

            // 步驟 5: Gamma 校正 (Linear sRGB -> sRGB)
            // 並將結果截斷到 [0, 1] 範圍，以處理超出 sRGB 色域的值

            double ConvertToSrgb(double v)
            {
                // 截斷到 [0, 1] 範圍
                v = Math.Max(0.0, Math.Min(1.0, v));

                if (v <= 0.0031308)
                {
                    return 12.92 * v;
                }
                else
                {
                    return 1.055 * Math.Pow(v, 1.0 / 2.4) - 0.055;
                }
            }

            double R_srgb = ConvertToSrgb(R_lin);
            double G_srgb = ConvertToSrgb(G_lin);
            double B_srgb = ConvertToSrgb(B_lin);

            // 步驟 6: 量化並返回 Color
            byte R = (byte)Math.Round(R_srgb * 255);
            byte G = (byte)Math.Round(G_srgb * 255);
            byte B = (byte)Math.Round(B_srgb * 255);

            // 預設不透明 (Alpha = 255)
            return Color.FromArgb(255, R, G, B);
        }
    }
}

