using System;
using System.Collections.Generic;
using System.Windows;

namespace UnitySC.Shared.ResultUI.HAZE
{
    public static class HazeUtils
    {
        public static float GetValue(float[] values, int width, int height, int x, int y)
        {
            if (values == null) return float.NaN;
            if (x < 0 || x >= width) return float.NaN;
            if (y < 0 || y >= height) return float.NaN;
            return values[y * width + x];
        }

        public static List<Point> InterBresenhamLine(int x0, int y0, int x1, int y1)
        {
            var indexList = new List<Point>();

            int x, y, e;
            int dx = x1 - x0; 
            int dy = y1 - y0;
            int tmp;

            if (Math.Abs(dy) <= Math.Abs(dx))
            {
                if (x1 < x0)
                {
                    tmp = x0; x0 = x1; x1 = tmp;
                    tmp = y0; y0 = y1; y1 = tmp;
                }

                dx = x1 - x0; dy = y1 - y0;
                if (y0 <= y1)
                {
                    e = -dx;
                    x = x0; y = y0;
                    for (int i = 0; i <= dx; i++)
                    {
                        indexList.Add(new Point(x, y));
                        x++; 
                        e += 2 * dy;
                        if (e >= 0)
                        {
                            y++;
                            e -= 2 * dx;
                        }
                    }
                }
                else
                {
                    e = dx;
                    x = x0; y = y0;
                    for (int i = 0; i <= dx; i++)
                    {
                        indexList.Add(new Point(x, y));
                        x++; 
                        e += 2 * dy;
                        if (e <= 0)
                        {
                            --y;
                            e += 2 * dx;
                        }
                    }
                }
            }
            else
            {
                if (y1 < y0)
                {
                    tmp = x0; x0 = x1; x1 = tmp;
                    tmp = y0; y0 = y1; y1 = tmp;
                }

                dx = x1 - x0; dy = y1 - y0;
                if (x0 <= x1)
                {
                    e = -dy;
                    x = x0; y = y0;
                    for (int i = 0; i <= dy; i++)
                    {
                        indexList.Add(new Point(x, y));
                        y++; 
                        e += 2 * dx;
                        if (e >= 0)
                        {
                            x++;
                            e -= 2 * dy;
                        }
                    }
                }
                else
                {
                    e = dy;
                    x = x0; y = y0;
                    for (int i = 0; i <= dy; i++)
                    {
                        indexList.Add(new Point(x, y));
                        y++; 
                        e += 2 * dx;
                        if (e <= 0)
                        {
                            --x;
                            e += 2 * dy;
                        }
                    }
                    // we need to revert point list in order to have x correctly ordered 
                    indexList.Reverse();
                }
            }

            return indexList;
        }
    }
}
