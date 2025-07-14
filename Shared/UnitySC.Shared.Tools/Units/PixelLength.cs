namespace UnitySC.Shared.Tools.Units
{
    static public class PixelLength
    {
        public static double GetPixels(this Length length, Length pixelSize)
        {
            return length.Micrometers / pixelSize.Micrometers;
        }
    }
}
