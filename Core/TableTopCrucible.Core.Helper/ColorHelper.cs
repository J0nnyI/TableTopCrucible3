using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTopCrucible.Core.Helper;

public static  class ColorHelper
{
    public static string ToHexString(this Color c, bool adedPrefix=true) 
        => adedPrefix?"#":string.Empty + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
    public static System.Drawing.Color ToDrawingColor(this Color c)
        => System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
    public static Color ToMediaColor(this System.Drawing.Color c)
        => Color.FromArgb(c.A, c.R, c.G, c.B);

    public static string ToRgbString(this Color c) 
        => "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
}
