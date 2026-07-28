using System;
using System.Drawing;

namespace Rotation
{
    public static class Tmatrix
    {
        // rotates each point around a given centre by angleDegrees, using the standard rotation matrix
        public static PointF[] matrixRotate(PointF[] points, float angleDegrees, PointF centre)
        {
            double radians = angleDegrees * Math.PI / 180.0;
            double cosA = Math.Cos(radians);
            double sinA = Math.Sin(radians);

            PointF[] result = new PointF[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                float xShift = points[i].X - centre.X;
                float yShift = points[i].Y - centre.Y;

                float xNew = (float)(xShift * cosA - yShift * sinA);
                float yNew = (float)(xShift * sinA + yShift * cosA);

                result[i] = new PointF(centre.X + xNew, centre.Y + yNew);
            }

            return result;
        }
    }
}