using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OneMap
{
    static class ColorExtensions
    {
        /// <summary>
        /// Get a colour that is suitable to use as a foreground against the specified background Color. 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Color DeriveForegroundColour(this Color c)
        {
            // A bit of trial and error to determine this value!
            const double threshold = 0.35;

            // from https://stackoverflow.com/questions/3116260/given-a-background-color-how-to-get-a-foreground-color-that-makes-it-readable-o
            var r = Math.Pow(c.R / 255.0, 2.2);
            var g = Math.Pow(c.G / 255.0, 2.2);
            var b = Math.Pow(c.B / 255.0, 2.2);

            var brightness = (0.2126 * r) + (0.7151 * g) + (0.0721 * b);

            return brightness > threshold ? Colors.Black : Colors.WhiteSmoke;
        }


        /// <summary>
        /// Adjust the brightness of a Color. 
        /// </summary>
        /// <remarks>
        /// Adapted from https://www.pvladov.com/2012/09/make-color-lighter-or-darker.html
        /// </remarks>
        /// <param name="c">The colour being adjusted</param>
        /// <param name="factor">How much to adjust the colour by - negative values are darker, positive for brighter</param>
        /// <returns></returns>
        public static Color AdjustBrightness(this Color c, float factor)
        {
            if (factor < -1.0 || factor > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(factor), "must be in the range [-1.0, 1.0]");
            }

            float red = (float)c.R;
            float green = (float)c.G;
            float blue = (float)c.B;

            if (factor < 0)
            {
                factor = 1 + factor;
                red *= factor;
                green *= factor;
                blue *= factor;
            }
            else
            {
                red = (255 - red) * factor + red;
                green = (255 - green) * factor + green;
                blue = (255 - blue) * factor + blue;
            }

            return Color.FromArgb(
                c.A,
                Convert.ToByte(red),
                Convert.ToByte(green),
                Convert.ToByte(blue));
        }
    }
}
