using System.Collections.Generic;
using Core.Geometry;

namespace Core.Interfaces
{
   public interface IGraphics
   {
      double Step { get; }
      void SetPen(Color color, double thickness, LineTypes lineType);
      void SetFillColor(Color color);
      void DrawPolyLine(IEnumerable<Point> line, bool isclosed);
      void FillPoly(IEnumerable<Point> line);
      void DrawCircle(Point a, double rad, double angle);
      void Bar(Point lt, Point rb);
      void DrawTriangle(Point lt, Point rb);
      void DrawEllipse(Point a, double rx, double ry, double angle);
      void DrawText(Point center, string text, string font, double fontsize);
   }
}