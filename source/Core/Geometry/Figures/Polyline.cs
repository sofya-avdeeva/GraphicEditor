using Core.Geometry;
using Core.Interfaces;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Geometry
{
   public class Polyline : ReactiveObject, IFigure
   {
      public void Scale(double x, double y)
      {
         throw new NotImplementedException();
      }
      bool isClosed = false;
      bool isRefLR = false;
      bool isRefUD = false;

      double FirstWidth, FirstHeight;

      public List<Point> Vertexes { get; set; }
      [JsonIgnore]
      public List<Point> VertexesRect { get; set; }
      [JsonIgnore]
      public List<Point> DistanceByLtVert { get; set; }
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

      public Polyline(List<Point> Vert, bool IsClosed)
      {
         ReflectionLeftRigth = ReactiveCommand.Create(() =>
         {
            Angle = -Angle;
            isRefLR = !isRefLR;
            double len = Center.X - LeftTopPoint.X;
            Direction.X = -Direction.X;

            for (int i = 0; i < DistanceByLtVert.Count(); i++)
            {
               DistanceByLtVert[i] = new Point(-DistanceByLtVert[i].X, DistanceByLtVert[i].Y);
            }
            LeftTopPoint = new Point(Center.X + len, LeftTopPoint.Y);
         });

         ReflectionUpDown = ReactiveCommand.Create(() =>
         {
            Angle = -Angle;
            GetCenter();
            isRefUD = !isRefUD;
            double len = Center.Y - LeftTopPoint.Y;

            for (int i = 0; i < DistanceByLtVert.Count(); i++)
            {
               DistanceByLtVert[i] = new Point(DistanceByLtVert[i].X, -DistanceByLtVert[i].Y);
            }
            LeftTopPoint = new Point(LeftTopPoint.X, Center.Y + len);
         });

         isClosed = IsClosed;
         Init(Vert);
      }

      public string Id => "polyline";

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
         get => angle; set
         {
            GetCenter();
            this.RaiseAndSetIfChanged(ref angle, value);
         }
      }

      Point Center;
      Point Direction;
      Point Diagonal;

      string IFigure.Name { get; }


      public void Init(List<Point> Vert)
      {
         DistanceByLtVert = new List<Point>();
         Vertexes = Vert;

         double Xmax, Xmin, Ymax, Ymin;
         Xmax = Xmin = Vertexes[0].X;
         Ymax = Ymin = Vertexes[0].Y;
         for (int i = 1; i < Vertexes.Count(); i++)
         {
            if (Vertexes[i].X < Xmin) Xmin = Vertexes[i].X;
            if (Vertexes[i].X > Xmax) Xmax = Vertexes[i].X;
            if (Vertexes[i].Y < Ymin) Ymin = Vertexes[i].Y;
            if (Vertexes[i].Y > Ymax) Ymax = Vertexes[i].Y;
         }
         LeftTopPoint = new Point(Xmin, Ymin);
         Direction = new Point(Xmax - Xmin, 0);
         Height = Ymax - Ymin;
         Width = Xmax - Xmin;
         FirstHeight = Height;
         FirstWidth = Width;
         X = lt.X;
         Y = lt.Y;

         foreach (var cur in Vertexes)
         {
            DistanceByLtVert.Add(cur - LeftTopPoint);
         }
         VertexesRect = GetNodes();

      }

      public void GetCenter()
      {
         if (Vertexes != null)
         {
            Diagonal.X = VertexesRect[2].X - VertexesRect[0].X;
            Diagonal.Y = VertexesRect[2].Y - VertexesRect[0].Y;

            Center.X = VertexesRect[0].X + Diagonal.X / 2;
            Center.Y = VertexesRect[0].Y + Diagonal.Y / 2;

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
         Vertexes = GetNodesPoly();
         VertexesRect = GetNodes();
         if (isClosed)
         {
            screen.FillPoly(Vertexes);
            screen.DrawPolyLine(Vertexes, true);
         }
         else
         {
            screen.DrawPolyLine(Vertexes, false);
         }
         //screen.DrawPolyLine(VertexesRect, true);
      }

      public Point FindNormalDirection()
      {
         Point n = new Point();
         n.X = Direction.Y;
         n.Y = Direction.X;
         double norma = Math.Sqrt(n.X * n.X + n.Y * n.Y);
         n.X = n.X / norma * Height;
         n.Y = n.Y / norma * Height;

         if (isRefLR)
            n.Y = -n.Y;

         if (isRefUD)
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
      public List<Point> GetNodesPoly()
      {
         for (int i = 0; i < Vertexes.Count(); i++)
         {
            Point newDist = new Point();
            newDist.X = DistanceByLtVert[i].X / FirstWidth * Width;
            newDist.Y = DistanceByLtVert[i].Y / FirstHeight * Height;

            Vertexes[i] = PointRotate(LeftTopPoint + newDist, Angle);

         }


         //double y_max = Vertexes[0].Y, y_min = Vertexes[0].Y;
         ////проверим реальную высоту 
         //for (int i = 1; i < Vertexes.Count(); i++)
         //   if (Vertexes[i].Y > y_max) y_max = Vertexes[i].Y;
         //   else if (Vertexes[i].Y < y_min) y_min = Vertexes[i].Y;

         ////увеличим на текущим хейтом
         //Point vect = new Point(0, Height - (y_max - y_min));
         ////Scale(vect, 2);
         return Vertexes;
      }
      public IFigure IntersectWith(IFigure figure)
      {
         throw new NotImplementedException();
      }

      public bool IsIn(Point p)
      {
         if (isClosed == true)
         {
            bool c = false;
            for (int i = 0, j = Vertexes.Count() - 1; i < Vertexes.Count(); j = i++)
            {
               Point i_tmp = Vertexes[i];
               Point j_tmp = Vertexes[j];

               if (((i_tmp.Y < j_tmp.Y) && (i_tmp.Y <= p.Y) && (p.Y <= j_tmp.Y) &&
                   ((j_tmp.Y - i_tmp.Y) * (p.X - i_tmp.X) == (j_tmp.X - i_tmp.X) * (p.Y - i_tmp.Y))) ||
                   ((i_tmp.Y > j_tmp.Y) && (j_tmp.Y <= p.Y) && (p.Y <= i_tmp.Y) &&
                       ((j_tmp.Y - i_tmp.Y) * (p.X - i_tmp.X) == (j_tmp.X - i_tmp.X) * (p.Y - i_tmp.Y))))
                  return true;

               if (((i_tmp.Y < j_tmp.Y) && (i_tmp.Y <= p.Y) && (p.Y <= j_tmp.Y) &&
                   ((j_tmp.Y - i_tmp.Y) * (p.X - i_tmp.X) > (j_tmp.X - i_tmp.X) * (p.Y - i_tmp.Y))) ||
                   ((i_tmp.Y > j_tmp.Y) && (j_tmp.Y <= p.Y) && (p.Y <= i_tmp.Y) &&
                   ((j_tmp.Y - i_tmp.Y) * (p.X - i_tmp.X) < (j_tmp.X - i_tmp.X) * (p.Y - i_tmp.Y))))
                  c = !c;
            }
            return c;
         }
         else //для незамкнутой
         {
            for (int i = 0; i < Vertexes.Count() - 1; i++)
            {
               Point first = Vertexes[i];
               Point second = Vertexes[i + 1];
               double a = second.Y - first.Y;
               double b = -second.X + first.X;
               double c = -a * first.X - b * first.Y;

               double aaa = Math.Abs(a * p.X + b * p.Y + c);

               if (aaa <= 2000 && ((p.X >= first.X && p.X <= second.X) || (p.X <= first.X && p.X >= second.X)))
                  return true;
            }
            return false;
         }
      }
      public void MoveByVector(Point v)
      {
         throw new NotImplementedException();
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
         List<Point> points = new List<Point>();

         foreach (var v in Vertexes)
            points.Add(v);

         Polyline fig = new Polyline(points, isClosed);
         fig.Angle = Angle;

         return fig;
      }
   }
}