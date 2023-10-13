using Core.Interfaces;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Geometry
{
   public class Rectangle : ReactiveObject, IFigure
   {
      public void Scale(double x, double y)
      {
         throw new NotImplementedException();
      }

      bool isRefLR = false;

      Point lt;
      [JsonIgnore]
      public Point LeftTopPoint { get => lt; set => this.RaiseAndSetIfChanged(ref lt, value); }

      double x;
      public double X
      {
         get => x;
         set
         {
            this.RaiseAndSetIfChanged(ref x, value);
            LeftTopPoint = new Point(x, LeftTopPoint.Y);
         }
      }

      double y;
      public double Y
      {
         get => y;
         set
         {
            this.RaiseAndSetIfChanged(ref y, value);
            LeftTopPoint = new Point(LeftTopPoint.X, y);
         }
      }

      [JsonIgnore]
      public ReactiveCommand<Unit, Unit> ReflectionLeftRigth { get; set; }
      [JsonIgnore]
      public ReactiveCommand<Unit, Unit> ReflectionUpDown { get; set; }

      public Rectangle(Point lt, Point rb)
      {
         ReflectionLeftRigth = ReactiveCommand.Create(() => { Angle = -Angle; });
         ReflectionUpDown = ReactiveCommand.Create(() => { Angle = 180 - Angle; });

         Init(lt, rb);
      }

      public string Id => "rectangle";

      double w;
      public double Width
      {
         get => w;
         set
         {
            if (value > 0)
            {
               double norm = Math.Sqrt(Direction.X * Direction.X + Direction.Y * Direction.Y);
               Direction = Direction / norm * value;
               this.RaiseAndSetIfChanged(ref w, value);
            }
         }
      }

      double h;
      public double Height
      {
         get => h;
         set
         {
            if (value > 0)
               this.RaiseAndSetIfChanged(ref h, value);
         }
      }

      double angle;
      public double Angle
      {
         get => angle;
         set
         {
            GetCenter();
            this.RaiseAndSetIfChanged(ref angle, value);
         }
      }

      Point Center;
      Point Direction;
      Point Diagonal;

      string IFigure.Name { get; }
      public List<Point> Vertexes { get; set; }

      public void Init(Point lt, Point rb)
      {
         Direction = new Point(rb.X - lt.X, 0);
         Width = Math.Abs(rb.X - lt.X);
         Height = Math.Abs(lt.Y - rb.Y);
         X = lt.X;
         Y = lt.Y;
         Center = new Point((lt.X + rb.X) / 2, (lt.Y + rb.Y) / 2);
         Vertexes = GetNodes();
      }

      public void GetCenter()
      {
         if (Vertexes != null)
         {
            Diagonal.X = Vertexes[2].X - Vertexes[0].X;
            Diagonal.Y = Vertexes[2].Y - Vertexes[0].Y;

            Center.X = Vertexes[0].X + Diagonal.X / 2;
            Center.Y = Vertexes[0].Y + Diagonal.Y / 2;
         }
      }

      public IFigure CloneWithPoints(IEnumerable<Point> nodes)
      {
         throw new NotImplementedException();
      }

      public void Draw(IGraphics screen, FigureGraphicsParameters param)
      {
         screen.SetFillColor(param.FillColor);
         screen.SetPen(param.LineColor, param.LineThickness, param.LineType);
         Vertexes = GetNodes();
         screen.FillPoly(Vertexes);
         screen.DrawPolyLine(Vertexes, true);
      }

      public Point FindNormalDirection()
      {
         Point n = new Point();
         n.X = Direction.Y;
         n.Y = -Direction.X;
         double norma = Math.Sqrt(n.X * n.X + n.Y * n.Y);
         n.X = n.X / norma * Height;
         n.Y = n.Y / norma * Height;
         n.Y = -n.Y;
         if (isRefLR)
            n.Y = -n.Y;

         return n;
      }

      public List<Point> GetNodes()
      {
         List<Point> temp = new List<Point>();

         Point n = FindNormalDirection();
         temp.Add(PointRotate(LeftTopPoint, Angle));
         temp.Add(PointRotate(new Point(LeftTopPoint.X + Direction.X, LeftTopPoint.Y + Direction.Y), Angle));
         temp.Add(PointRotate(new Point(LeftTopPoint.X + Direction.X + n.X, LeftTopPoint.Y + Direction.Y + n.Y), Angle));
         temp.Add(PointRotate(new Point(LeftTopPoint.X + n.X, LeftTopPoint.Y + n.Y), Angle));

         return temp;
      }

      public IFigure IntersectWith(IFigure figure)
      {
         throw new NotImplementedException();
      }

      public bool IsIn(Point p)
      {
         Point v4 = Vertexes[0];
         Point v3 = Vertexes[1];
         Point v2 = Vertexes[2];
         Point v1 = Vertexes[3];

         return
             (v1.Y - v2.Y) * p.X + (v2.X - v1.X) * p.Y + (v1.X * v2.Y - v2.X * v1.Y) <= 0 &&
             (v2.Y - v3.Y) * p.X + (v3.X - v2.X) * p.Y + (v2.X * v3.Y - v3.X * v2.Y) <= 0 &&
             (v3.Y - v4.Y) * p.X + (v4.X - v3.X) * p.Y + (v3.X * v4.Y - v4.X * v3.Y) <= 0 &&
             (v4.Y - v1.Y) * p.X + (v1.X - v4.X) * p.Y + (v4.X * v1.Y - v1.X * v4.Y) <= 0;
      }

      public Point PointRotate(Point p, double Angle)
      {
         Angle *= Math.PI / 180.0;
         var x = new Point(p.X - Center.X, p.Y - Center.Y);
         var sinf = Math.Sin(Angle);
         var cosf = Math.Cos(Angle);
         var x1 = new Point(cosf * x.X - sinf * x.Y, sinf * x.X + cosf * x.Y);

         return new Point(x1.X + Center.X, x1.Y + Center.Y);
      }

      public double CosAngleBetweenVectors(Point V1, Point V2)
      {
         double normV1 = Math.Sqrt(V1.X * V1.X + V1.Y * V1.Y);
         double normV2 = Math.Sqrt(V2.X * V2.X + V2.Y * V2.Y);

         return (V1 * V2) / (normV1 * normV2);
      }

      public IFigure Subtract(IFigure figure)
      {
         throw new NotImplementedException();
      }
      public IFigure UnionWith(IFigure figure)
      {
         throw new NotImplementedException();
      }

      void IFigure.ResetFigureByPoints(IEnumerable<Point> vertex)
      {
         throw new NotImplementedException();
      }

      string IFigure.ToJson()
      {
         return JsonSerializer.Serialize(this);
      }

      public IFigure Clone()
      {
         IFigure fig = new Rectangle(LeftTopPoint, new Point(LeftTopPoint.X + Width, LeftTopPoint.Y + Height));
         fig.Angle = Angle;

         return fig;
      }
   }
}
