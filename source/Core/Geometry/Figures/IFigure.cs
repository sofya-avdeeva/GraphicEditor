using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Interfaces;
using ReactiveUI;

namespace Core.Geometry
{
   public enum LineTypes
   {
      Solid,
      Dashed,
      Dotted
   }

   public class FigureGraphicsParameters : ReactiveObject
   {
      public byte strokeR = 0, strokeG = 0, strokeB = 0, strokeA = 255;
      public byte fillR = 230, fillG = 230, fillB = 230, fillA = 255;
      [JsonIgnore]
      public byte StrokeR
      {
         get => strokeR;
         set
         {
            this.RaiseAndSetIfChanged(ref strokeR, value);
            LineColor = new Color { A = LineColor.A, R = strokeR, G = LineColor.G, B = LineColor.B };
         }
      }
      [JsonIgnore]
      public byte StrokeG
      {
         get => strokeG;
         set
         {
            this.RaiseAndSetIfChanged(ref strokeG, value);
            LineColor = new Color { A = LineColor.A, R = LineColor.R, G = strokeG, B = LineColor.B };
         }
      }
      [JsonIgnore]
      public byte StrokeB
      {
         get => strokeB;
         set
         {
            this.RaiseAndSetIfChanged(ref strokeB, value);
            LineColor = new Color { A = LineColor.A, R = LineColor.R, G = LineColor.G, B = strokeB };
         }
      }
      [JsonIgnore]
      public byte StrokeA
      {
         get => strokeA;
         set
         {
            this.RaiseAndSetIfChanged(ref strokeA, value);
            LineColor = new Color { A = strokeA, R = LineColor.R, G = LineColor.G, B = LineColor.B };
         }
      }

      Color lineColor;
      public Color LineColor { get => lineColor; set => this.RaiseAndSetIfChanged(ref lineColor, value); }
      [JsonIgnore]
      public byte FillR
      {
         get => fillR;
         set
         {
            this.RaiseAndSetIfChanged(ref fillR, value);
            FillColor = new Color { A = FillColor.A, R = fillR, G = FillColor.G, B = FillColor.B };
         }
      }
      [JsonIgnore]
      public byte FillG
      {
         get => fillG;
         set
         {
            this.RaiseAndSetIfChanged(ref fillG, value);
            FillColor = new Color { A = FillColor.A, R = FillColor.R, G = fillG, B = FillColor.B };
         }
      }
      [JsonIgnore]
      public byte FillB
      {
         get => fillB;
         set
         {
            this.RaiseAndSetIfChanged(ref fillB, value);
            FillColor = new Color { A = FillColor.A, R = FillColor.R, G = FillColor.G, B = fillB };
         }
      }
      [JsonIgnore]
      public byte FillA
      {
         get => fillA;
         set
         {
            this.RaiseAndSetIfChanged(ref fillA, value);
            FillColor = new Color { A = fillA, R = FillColor.R, G = FillColor.G, B = FillColor.B };
         }
      }

      Color fillColor;
      public Color FillColor { get => fillColor; set => this.RaiseAndSetIfChanged(ref fillColor, value); }

      double lineThickness;
      public double LineThickness
      {
         get => lineThickness;
         set
         {
            if (value > 0)
               this.RaiseAndSetIfChanged(ref lineThickness, value);
         }
      }

      LineTypes lineType;
      public LineTypes LineType { get => lineType; set => this.RaiseAndSetIfChanged(ref lineType, value); }

      public static FigureGraphicsParameters Default => new FigureGraphicsParameters
      {
         LineColor = Color.Black,
         FillColor = new Color { A = 255, R = 230, G = 230, B = 230 },
         LineThickness = 1.0,
         LineType = LineTypes.Solid
      };

      public string ToJson()
      {
         return JsonSerializer.Serialize(this);
      }

      public FigureGraphicsParameters Clone()
      {
         FigureGraphicsParameters param = new FigureGraphicsParameters();
         param.LineThickness = LineThickness;
         param.LineType = LineType;
         param.LineColor = LineColor;
         param.FillColor = FillColor;

         return param;
      }
   }

   public interface IFigure
   {
      string Name { get; }
      string Id { get; }
      void Draw(IGraphics graphic, FigureGraphicsParameters parameters);
      void Scale(double x, double y);
      Point PointRotate(Point p, double Angle);
      double Width { get; set; }
      double Height { get; set; }
      double Angle { get; set; }
      double X { get; set; }
      double Y { get; set; }
      Point LeftTopPoint { get; set; }
      bool IsIn(Point p);
      List<Point> Vertexes { get; set; } // 4 точки
      void ResetFigureByPoints(IEnumerable<Point> vertex);
      string ToJson();

      IFigure Clone();
   }

   //СЮДА ЛУЧШЕ НЕ СМОТРЕТЬ

   public class Changing
   {
   }

   public class Changed
   {
   }

   public class ThrownExceptions
   {
   }

   public class GeometryParams
   {
      public double X { get; set; }
      public double Y { get; set; }
      public string Id { get; set; }
      public double Width { get; set; }
      public double Height { get; set; }
      public double Angle { get; set; }
      public List<Point> Vertexes { get; set; }
      public Changing Changing { get; set; }
      public Changed Changed { get; set; }
      public ThrownExceptions ThrownExceptions { get; set; }
   }


   public class LineColor
   {
      public int R { get; set; }
      public int G { get; set; }
      public int B { get; set; }
      public int A { get; set; }
   }

   public class FillColor
   {
      public int R { get; set; }
      public int G { get; set; }
      public int B { get; set; }
      public int A { get; set; }
   }

   public class MaterialParams
   {
      public Color LineColor { get; set; }
      public Color FillColor { get; set; }
      public int LineThickness { get; set; }
      public int LineType { get; set; }
      public Changing Changing { get; set; }
      public Changed Changed { get; set; }
      public ThrownExceptions ThrownExceptions { get; set; }
   }

   public class Root
   {
      public GeometryParams first { get; set; }
      public MaterialParams second { get; set; }
   }
   // НЕ СМОТРЕТЬ!!!

   public static class FigureAssistant
   {
      public static FigureGraphicsParameters CreateMatFromJson(MaterialParams param)
      {
         FigureGraphicsParameters fpr = new FigureGraphicsParameters();
         fpr.LineColor = param.LineColor;
         fpr.LineThickness = param.LineThickness;
         fpr.LineType = (LineTypes)param.LineType;
         fpr.FillColor = param.FillColor;
         return fpr;
      }
      public static IFigure CreateFigFromJson(GeometryParams param)
      {
         IFigure fig = null;
         if (param.Id == "rectangle")
         {
            fig = new Rectangle(new Point(param.X, param.Y), new Point(param.X + param.Width, param.Y + param.Height));
            fig.Angle = param.Angle;
         }
         else if (param.Id == "triangle")
         {
            fig = new Triangle(new Point(param.X, param.Y), new Point(param.X + param.Width, param.Y + param.Height));
            fig.Angle = param.Angle;
         }
         else if (param.Id == "ellipse")
         {
            fig = new Ellipse(new Point(param.X, param.Y), new Point(param.X + param.Width, param.Y + param.Height));
            fig.Angle = param.Angle;
         }
         else if (param.Id == "polyline")
         {
            fig = new Polyline(param.Vertexes, false); //closed
            fig.Angle = param.Angle;
         }

         return fig;
      }
      public static IFigure CreateByName(string typename, string name, List<Point> parametres)
      {
         // if (typename == "rectagle") {
         Rectangle fig = new Rectangle(parametres[0], parametres[1]);
         //}

         return fig;
      }
      public static IEnumerable<string> AvailableFigures() => throw new NotImplementedException();
      public static Func<double, Point> Circle(double R)
      {
         return t => new Point { X = R * Math.Cos(2 * Math.PI * t), Y = R * Math.Sin(2 * Math.PI * t) };
      }
      public static Func<double, Point> Parabolic()
      {
         return t => new Point { X = t, Y = t * t };
      }
   }
}


