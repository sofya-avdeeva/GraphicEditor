using System;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Core.Geometry;
using System.Diagnostics;

namespace Vectorly.UI
{
   public struct RenderingUnit
   {
      public System.Windows.Media.Geometry Geometry { get; set; }
      public System.Windows.Media.Brush Brush { get; set; }
      public System.Windows.Media.Pen Pen { get; set; }
   }

   public class Graphics : IGraphics
   {
      public double Step => 0.001;

      VectorlyCanvas canvas;
      bool forSelected;

      public Graphics(VectorlyCanvas canvas, bool forSelected)
      {
         this.forSelected = forSelected;
         this.canvas = canvas;
      }

      public void Bar(Point lt, Point rb)
      {
         var geometry = new System.Windows.Media.RectangleGeometry();

         double w = Math.Abs(rb.X - lt.X);
         double h = Math.Abs(rb.Y - lt.Y);

         geometry.Rect = new System.Windows.Rect(lt.X, lt.Y, w, h);

         if (forSelected)
            canvas.AddTempRenderingUnit(geometry);
         else
            canvas.AddRenderingUnit(geometry);
      }

      public void DrawCircle(Point center, double rad, double angle)
      {
         DrawEllipse(center, rad, rad, angle);
      }

      public void DrawEllipse(Point center, double rx, double ry, double angle)
      {
         var geometry = new System.Windows.Media.EllipseGeometry();
         geometry.RadiusX = rx;
         geometry.RadiusY = ry;
         geometry.Center = ConvertPoint(center);


         var rotTrans = new System.Windows.Media.RotateTransform(angle, center.X, center.Y);

         geometry.Transform = rotTrans;

         if (forSelected)
            canvas.AddTempRenderingUnit(geometry);
         else
            canvas.AddRenderingUnit(geometry);
      }

      public void DrawTriangle(Point lt, Point rb)
      {
         Point lt1, rb1;
         if (lt.Y < rb.Y)
         {
            lt1 = new Point(lt);
            rb1 = new Point(rb);
         }
         else
         {
            lt1 = new Point(rb);
            rb1 = new Point(lt);
         }

         Point l1, l2, l3;
         List<Point> line;
         l2 = new Point(lt1.X, rb1.Y);
         l1 = new Point(rb1.X, rb1.Y);
         l3 = new Point((lt1.X + rb1.X) / 2, lt1.Y);

         line = new List<Point> { l1, l2, l3 };
         if (line == null)
            return;

         if (line.Count() == 0)
            return;

         var geometry = new System.Windows.Media.StreamGeometry();

         using (var geometryContext = geometry.Open())
         {
            geometryContext.BeginFigure(ConvertPoint(line.First()), false, true);

            foreach (var p in line.Skip(1))
               geometryContext.LineTo(ConvertPoint(p), true, true);
         }

         if (forSelected)
            canvas.AddTempRenderingUnit(geometry);
         else
            canvas.AddRenderingUnit(geometry);
      }

      public void DrawPolyLine(IEnumerable<Point> line, bool isclosed)
      {
         if (line == null)
            return;

         if (line.Count() == 0)
            return;

         var geometry = new System.Windows.Media.StreamGeometry();

         using (var geometryContext = geometry.Open())
         {
            geometryContext.BeginFigure(ConvertPoint(line.First()), false, isclosed);

            foreach (var p in line.Skip(1))
               geometryContext.LineTo(ConvertPoint(p), true, true);
         }

         if (forSelected)
            canvas.AddTempRenderingUnit(geometry);
         else
            canvas.AddRenderingUnit(geometry);
      }

      public void DrawText(Point center, string text, string font, double fontsize)
      {
         throw new NotImplementedException();

         //var text = new FormattedText { Text = "Hello" };
         //text.Typeface = new Typeface(new FontFamily("Arial"));
         //text.FontSize = 20.0;

         //context.DrawText(Brushes.Aqua, new Point(100, 100), text);
      }

      public void FillPoly(IEnumerable<Point> line)
      {
         if (line == null)
            return;

         if (line.Count() == 0)
            return;

         var geometry = new System.Windows.Media.StreamGeometry();

         using (var geometryContext = geometry.Open())
         {
            geometryContext.BeginFigure(ConvertPoint(line.First()), true, true);

            foreach (var p in line.Skip(1))
               geometryContext.LineTo(ConvertPoint(p), true, true);
         }

         if (forSelected)
            canvas.AddTempRenderingUnit(geometry);
         else
            canvas.AddRenderingUnit(geometry);
      }

      public void SetFillColor(Color color)
      {
         System.Windows.Media.Brush brush = new System.Windows.Media.SolidColorBrush(ConvertColor(color));
         canvas.Brush = brush;
      }

      public void SetPen(Color color, double thickness, LineTypes lineType)
      {
         var brush = new System.Windows.Media.SolidColorBrush(ConvertColor(color));
         var pen = new System.Windows.Media.Pen(brush, thickness);

         pen.DashStyle = ConvertLineType(lineType);

         canvas.Pen = pen;
      }

      System.Windows.Media.DashStyle ConvertLineType(LineTypes lineType)
      {
         System.Windows.Media.DashStyle style;

         switch (lineType)
         {
            case LineTypes.Solid:
               style = System.Windows.Media.DashStyles.Solid;
               break;

            case LineTypes.Dashed:
               style = System.Windows.Media.DashStyles.Dash;
               break;

            case LineTypes.Dotted:
               style = System.Windows.Media.DashStyles.Dot;
               break;

            default:
               style = System.Windows.Media.DashStyles.Solid;
               break;
         }

         return style;
      }

      System.Windows.Point ConvertPoint(Point p) => new System.Windows.Point(p.X, p.Y);

      System.Windows.Media.Color ConvertColor(Color color) => System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
   }
}
