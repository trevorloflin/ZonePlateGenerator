using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZonePlateGenerator
{
    class ZoneGenerator
    {
        public int Size { get; set; }
        public int Scale { get; set; }
        public Bitmap Result { get; internal set; }

        public event EventHandler<int> ProgressUpdate;
        protected virtual void OnProgressUpdate(int progress)
        {
            EventHandler<int> handler = ProgressUpdate;
            if (handler != null) { handler(this, progress); }
        }

        public bool Generate()
        {
            if (Size == 0 || Scale == 0) { return false; }

            Result = new Bitmap(Size, Size, PixelFormat.Format24bppRgb);

            BitmapData pntResult = Result.LockBits(new Rectangle(0, 0, Result.Height, Result.Width), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int bytes  = Math.Abs(pntResult.Stride) * Result.Height;
            byte[] resultArray = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(pntResult.Scan0, resultArray, 0, bytes);

            double center = Size / 2D;

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    double f = 20000000D; // focus, 2cm
                    double n = 1D; // phase

                    double r = Math.Sqrt(Math.Pow(x + 0.5D - center, 2) + Math.Pow(y + 0.5D - center, 2)) * Scale;
                    Func<int, double> opacity = (l => (1 + Math.Cos(Math.PI * r * r * (n - 0.5D) / (n * l * f + n * n * l * l / 4D))) / 2D);
                    double rLight = ObserverData.GetR(opacity);
                    double gLight = ObserverData.GetG(opacity);
                    double bLight = ObserverData.GetB(opacity);

                    resultArray[x * 3 + 0 + y * pntResult.Stride] = (byte)(rLight * 255);
                    resultArray[x * 3 + 1 + y * pntResult.Stride] = (byte)(gLight * 255);
                    resultArray[x * 3 + 2 + y * pntResult.Stride] = (byte)(bLight * 255);

                    if (x + y * Size % Size * Size / 260 == 0)
                    {
                        OnProgressUpdate(x + y * Size);
                    }
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(resultArray, 0, pntResult.Scan0, bytes);
            Result.UnlockBits(pntResult);
            
            return true;
        }
    }
}
