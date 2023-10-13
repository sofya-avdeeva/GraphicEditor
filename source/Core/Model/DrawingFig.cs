using Core.Geometry;
using Core.Interfaces;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace Core.Model
{
   public abstract class DrawingObject : ReactiveObject
   {
      public abstract void Draw(IGraphics graphics, FigureGraphicsParameters param);
      public abstract IFigure GetFigure();

      Point startPoint;
      Point endPoint;

      public Point StartPoint { get => startPoint; set => this.RaiseAndSetIfChanged(ref startPoint, value); }
      public Point EndPoint { get => endPoint; set => this.RaiseAndSetIfChanged(ref endPoint, value); }
      public abstract void Update(Point p);
   }

   public class RectangleDrawingObject : DrawingObject
   {
      Point lt, rb;
      public RectangleDrawingObject(Point startPoint)
      {
         lt = startPoint;
         rb = startPoint;
         StartPoint = startPoint;
         EndPoint = startPoint;
      }

      public override void Update(Point p)
      {
         EndPoint = p;

         double x1 = StartPoint.X;
         double x2 = EndPoint.X;

         double y1 = StartPoint.Y;
         double y2 = EndPoint.Y;

         if (x1 > x2)
            (x1, x2) = (x2, x1);

         if (y1 > y2)
            (y1, y2) = (y2, y1);

         lt = new Point(x1, y1);
         rb = new Point(x2, y2);
      }

      public override void Draw(IGraphics graphics, FigureGraphicsParameters param)
      {
         graphics.SetFillColor(param.FillColor);
         graphics.SetPen(param.LineColor, param.LineThickness, param.LineType);
         graphics.Bar(lt, rb);
      }

      public override IFigure GetFigure() => new Rectangle(lt, rb);
   }

   public class EllipseDrawingObject : DrawingObject
   {
      Point lt, rb;
      public EllipseDrawingObject(Point startPoint)
      {
         lt = startPoint;
         rb = startPoint;
         StartPoint = startPoint;
         EndPoint = startPoint;
      }

      public override void Update(Point p)
      {
         EndPoint = p;

         double x1 = StartPoint.X;
         double x2 = EndPoint.X;

         double y1 = StartPoint.Y;
         double y2 = EndPoint.Y;

         if (x1 > x2)
            (x1, x2) = (x2, x1);

         if (y1 > y2)
            (y1, y2) = (y2, y1);

         lt = new Point(x1, y1);
         rb = new Point(x2, y2);
      }

      public override void Draw(IGraphics graphics, FigureGraphicsParameters param)
      {
         graphics.SetFillColor(param.FillColor);
         graphics.SetPen(param.LineColor, param.LineThickness, param.LineType);
         graphics.DrawEllipse((StartPoint + EndPoint) / 2, (EndPoint.X - StartPoint.X) / 2, (EndPoint.Y - StartPoint.Y) / 2, 0);
      }

      public override IFigure GetFigure() => new Ellipse(lt, rb);
   }

   public class TriangleDrawingObject : DrawingObject
   {
      Point lt, rb;
      public TriangleDrawingObject(Point startPoint)
      {
         lt = startPoint;
         rb = startPoint;
         StartPoint = startPoint;
         EndPoint = startPoint;
      }

      public override void Update(Point p)
      {
         EndPoint = p;

         double x1 = StartPoint.X;
         double x2 = EndPoint.X;

         double y1 = StartPoint.Y;
         double y2 = EndPoint.Y;

         if (x1 > x2)
            (x1, x2) = (x2, x1);

         if (y1 > y2)
            (y1, y2) = (y2, y1);

         lt = new Point(x1, y1);
         rb = new Point(x2, y2);
      }

      public override void Draw(IGraphics graphics, FigureGraphicsParameters param)
      {
         graphics.SetFillColor(param.FillColor);
         graphics.SetPen(param.LineColor, param.LineThickness, param.LineType);
         graphics.DrawTriangle(StartPoint, EndPoint);
      }

      public override IFigure GetFigure() => new Triangle(lt, rb);
   }

   public class PencilDrawingObject : DrawingObject
   {
      List<Point> line = new List<Point>();
      public PencilDrawingObject(Point startPoint)
      {
         StartPoint = startPoint;
         EndPoint = startPoint;

         line.Add(startPoint);
      }

      public override void Update(Point p)
      {
         line.Add(p);
         EndPoint = p;
         StartPoint = EndPoint;
      }

      public override void Draw(IGraphics graphics, FigureGraphicsParameters param)
      {
         graphics.SetFillColor(param.FillColor);
         graphics.SetPen(param.LineColor, param.LineThickness, param.LineType);
         graphics.DrawPolyLine(line, false);
      }

      public override IFigure GetFigure() => new Polyline(line, false);
   }

   public class LineDrawingObject : DrawingObject
   {
      Point rb, lt;
      List<Point> line;
      public LineDrawingObject(Point startPoint)
      {
         rb = startPoint;
         lt = startPoint;
         StartPoint = startPoint;
         EndPoint = startPoint;
      }

      public override void Update(Point p)
      {
         EndPoint = p;
         rb = p;
         if (lt.X > rb.X)
            line = new List<Point> { lt, rb };
         else
            line = new List<Point> { rb, lt };
      }

      public override void Draw(IGraphics graphics, FigureGraphicsParameters param)
      {
         graphics.SetFillColor(param.FillColor);
         graphics.SetPen(param.LineColor, param.LineThickness, param.LineType);
         graphics.DrawPolyLine(line, false);
      }

      public override IFigure GetFigure() => new Polyline(line, false);
   }
}
