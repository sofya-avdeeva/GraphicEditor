using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Geometry
{
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
        public Point(Point tmp)
        {
            X = tmp.X;
            Y = tmp.Y;
        }

        public static Point operator +(Point Left, Point Rigth) => new Point(Left.X + Rigth.X, Left.Y + Rigth.Y);
        public static Point operator -(Point Left, Point Rigth) => new Point(Left.X - Rigth.X, Left.Y - Rigth.Y);
        public static Point operator +(Point Left, double Rigth) => new Point(Left.X + Rigth, Left.Y + Rigth);

        public static double operator *(Point Left, Point Rigth) => Left.X * Rigth.X + Left.Y * Rigth.Y;
        public static Point operator *(Point Left, double Rigth) => new Point(Left.X * Rigth, Left.Y * Rigth);
        public static Point operator /(Point Left, double Rigth) => new Point(Left.X / Rigth, Left.Y / Rigth);
    }

    public struct Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }
        public string ToHexRGB() => (R | (((uint)G) << 8) | (((uint)B) << 16)).ToString("X");
        public string ToHexRGBA() => (R | (((uint)(G)) << 8) | (((uint)B) << 16) | (((uint)A) << 24)).ToString("X");

        public static Color Black => new Color { A = 255, R = 0, G = 0, B = 0 };
        public static Color Blue => new Color { A = 255, R = 112, G = 229, B = 255 };
    }
}
