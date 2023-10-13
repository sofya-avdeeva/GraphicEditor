using Core.Interfaces;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Core.Geometry
{
   public class Triangle : ReactiveObject, IFigure
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

      public Triangle(Point lt, Point rb)
      {
         ReflectionLeftRigth = ReactiveCommand.Create(() => { Angle = -Angle; });
         ReflectionUpDown = ReactiveCommand.Create(() => { Angle = 180 - Angle; });

         Init(lt, rb);
      }

      public string Id => "triangle";

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

      string IFigure.Name { get; }

      public List<Point> Vertexes { get; set; }

      public void Init(Point lt, Point rb)
      {
         Direction = new Point(rb.X - lt.X, 0);
         Width = Math.Abs(rb.X - lt.X);
         Height = Math.Abs(lt.Y - rb.Y);
         X = lt.X;
         Y = lt.Y;
         Vertexes = GetNodes();
      }

      public void GetCenter()
      {
         Point n = new Point();
         n.X = Direction.Y;
         n.Y = Direction.X;
         double norma = Math.Sqrt(n.X * n.X + n.Y * n.Y);
         n.X = n.X / norma * Height / 2;
         n.Y = n.Y / norma * Height / 2;

         Point DirectionCenterTmp = new Point(LeftTopPoint.X + Direction.X / 2, LeftTopPoint.Y + Direction.Y / 2);
         Center = DirectionCenterTmp + n;
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
         //ОТРИСОВАТЬ ПРЯМОУГОЛЬНИК КОНТУРА
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
         temp.Add(PointRotate(LeftTopPoint + n, Angle));
         temp.Add(PointRotate(LeftTopPoint + Direction / 2, Angle));
         temp.Add(PointRotate(LeftTopPoint + n + Direction, Angle));

         return temp;
      }

      public IFigure IntersectWith(IFigure figure)
      {
         throw new NotImplementedException();
      }

      public bool IsIn(Point p)
      {
         Point v1 = Vertexes[0];
         Point v2 = Vertexes[1];
         Point v3 = Vertexes[2];

         return (v3.Y - v2.Y) * p.X + (v2.X - v3.X) * p.Y + (v3.X * v2.Y - v2.X * v3.Y) <= 0 &&
                (v2.Y - v1.Y) * p.X + (v1.X - v2.X) * p.Y + (v2.X * v1.Y - v1.X * v2.Y) <= 0 &&
                (v1.Y - v3.Y) * p.X + (v3.X - v1.X) * p.Y + (v1.X * v3.Y - v3.X * v1.Y) <= 0;
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
         Triangle fig = new Triangle(LeftTopPoint, new Point(LeftTopPoint.X + Width, LeftTopPoint.Y + Height));
         fig.Angle = Angle;

         return fig;
      }
   }
}
