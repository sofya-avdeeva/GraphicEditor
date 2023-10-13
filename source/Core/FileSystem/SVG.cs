using Core.Geometry;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Core.FileSystem
{
   public class SVG : IGraphics
   {
      XmlTextWriter SVGWriter;

      LineTypes LineType;
      Color LineColor;
      Color FillColor;

      double Thickness;
      double Width;
      double Height;

      double dashedSize;

      public SVG(string filename, double width, double height)
      {
         SVGWriter = new XmlTextWriter(filename, Encoding.UTF8);
         SVGWriter.Formatting = Formatting.Indented;
         Width = width;
         Height = height;
      }

      public void Begin()
      {
         string cdlibraryVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

         SVGWriter.WriteStartDocument();
         SVGWriter.WriteDocType("svg", "-//W3C//DTD SVG 1.1//EN", "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd", null);
         SVGWriter.WriteStartElement("svg", "http://www.w3.org/2000/svg");
         SVGWriter.WriteAttributeString("version", "1.1");
         SVGWriter.WriteAttributeString("width", Width.ToString());
         SVGWriter.WriteAttributeString("height", Height.ToString());
      }

      public void End()
      {
         SVGWriter.WriteEndDocument();
         SVGWriter.Flush();
         SVGWriter.Dispose();
         SVGWriter = null;
      }

      public double Step { get; }

      public void SetPen(Color color, double thickness, LineTypes lineType)
      {
         LineColor = color;
         Thickness = thickness;
         LineType = lineType;
      }

      public void SetFillColor(Color color) => FillColor = color;

      public void Features(bool isclosed)
      {
         string linecolor = LineColor.ToHexRGB() == "0" ? "#000000" : "#" + LineColor.ToHexRGB();
         string fillcolor = FillColor.ToHexRGB() == "0" ? "#000000" : "#" + FillColor.ToHexRGB();
         linecolor = "#" + linecolor.Substring(5, 2) + linecolor.Substring(3, 2) + linecolor.Substring(1, 2);
         fillcolor = "#" + fillcolor.Substring(5, 2) + fillcolor.Substring(3, 2) + fillcolor.Substring(1, 2);
         dashedSize = 0.5;

         SVGWriter.WriteAttributeString("stroke", linecolor);
         SVGWriter.WriteAttributeString("stroke-opacity", (Convert.ToDouble(LineColor.A) / 255.0).ToString());
         SVGWriter.WriteAttributeString("stroke-width", Thickness.ToString());
         if (LineType.ToString() == "Dashed") SVGWriter.WriteAttributeString("stroke-dasharray", dashedSize.ToString() + "%");
         if (LineType.ToString() == "Dotted") SVGWriter.WriteAttributeString("stroke-dasharray", Thickness.ToString());
         if (isclosed)
         {
            SVGWriter.WriteAttributeString("fill", fillcolor);
            SVGWriter.WriteAttributeString("fill-opacity", (Convert.ToDouble(FillColor.A) / 255.0).ToString());
         }
      }

      public void DrawPolyLine(IEnumerable<Point> line, bool isclosed)
      {
         string temp = "";

         if (isclosed) SVGWriter.WriteStartElement("polygon");
         else SVGWriter.WriteStartElement("polyline");
         foreach (Point p in line)
         {
            temp += p.X.ToString();
            temp += ",";
            temp += p.Y.ToString();
            temp += " ";
         }
         SVGWriter.WriteAttributeString("points", temp);
         if (isclosed)
            Features(true);
         else Features(false);
         SVGWriter.WriteEndElement();
      }

      public void FillPoly(IEnumerable<Point> line)
      {
         DrawPolyLine(line, true);
      }

      public void DrawCircle(Point center, double rad, double angle)
      {
         DrawEllipse(center, rad, rad, angle);
      }

      public void Bar(Point lt, Point rb)
      {
         double width = rb.X - lt.X;
         double height = rb.Y - lt.Y;

         SVGWriter.WriteStartElement("rect");
         SVGWriter.WriteAttributeString("x", lt.X.ToString());
         SVGWriter.WriteAttributeString("y", lt.Y.ToString());
         SVGWriter.WriteAttributeString("width", width.ToString());
         SVGWriter.WriteAttributeString("height", height.ToString());
         Features(true);
         SVGWriter.WriteEndElement();
      }

      public void DrawEllipse(Point center, double rx, double ry, double angle)
      {
         string figureName = rx == ry ? "circle" : "ellipse";
         SVGWriter.WriteStartElement(figureName);
         if (figureName == "ellipse")
         {
            SVGWriter.WriteAttributeString("rx", rx.ToString());
            SVGWriter.WriteAttributeString("ry", ry.ToString());
         }
         else
            SVGWriter.WriteAttributeString("r", rx.ToString());
         SVGWriter.WriteAttributeString("cx", center.X.ToString());
         SVGWriter.WriteAttributeString("cy", center.Y.ToString());
         Features(true);
         SVGWriter.WriteEndElement();
      }

      public void DrawText(Point center, string text, string font, double fontsize)
      {
         SVGWriter.WriteStartElement("text");
         SVGWriter.WriteAttributeString("x", center.X.ToString());
         SVGWriter.WriteAttributeString("y", center.Y.ToString());
         SVGWriter.WriteAttributeString("font-family", font.ToString());
         SVGWriter.WriteAttributeString("font-size", fontsize.ToString());
         SVGWriter.WriteAttributeString("font-weight", "bold");
         SVGWriter.WriteAttributeString("font-style", "italic");
         SVGWriter.WriteAttributeString("text-decoration", "underlined");

         Features(true);
         SVGWriter.WriteString(text);
         SVGWriter.WriteEndElement();
      }

      public void DrawTriangle(Point lt, Point rb)
      {
         throw new NotImplementedException();
      }
   }
}
