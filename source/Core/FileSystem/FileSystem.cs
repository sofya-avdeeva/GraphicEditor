using Core.Geometry;
using Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.FileSystem
{
   abstract public class Saver
    {
        virtual public void SaveToFile(string filename, IEnumerable<(IFigure, FigureGraphicsParameters)>Figures, double width, double height)
        {
            CreateFile(filename, width, height);
            var Graphic = CreateGraphic();
            foreach (var fig in Figures)
                fig.Item1.Draw(Graphic, fig.Item2);
            CloseFile();
        }

        protected abstract IGraphics CreateGraphic();
        protected abstract void CloseFile();
        protected abstract void CreateFile(string filename, double w, double h);
        public abstract IEnumerable<(IFigure, FigureGraphicsParameters)> LoadFromFile(string filename);
    }

    public class SVGSaver : Saver
    {
        SVG save;

        public override IEnumerable<(IFigure, FigureGraphicsParameters)> LoadFromFile(string filename)
        {
            throw new System.NotImplementedException();
        }

        protected override void CloseFile()
        {
            save.End();
        }

        protected override void CreateFile(string filename, double w, double h)
        {
            save = new SVG(filename, w, h);
            save.Begin();
        }

        protected override IGraphics CreateGraphic()
        {
            return save;
        }
    }

    public class JSONSaver : Saver
    {
        override public void SaveToFile(string filename, IEnumerable<(IFigure, FigureGraphicsParameters)> Figures, double width, double height)
        {
            string res = "[";
            foreach (var el in Figures)
            {
                res += $"{{\"first\":{el.Item1.ToJson()}, \"second\":{el.Item2.ToJson()}}},";
            }
            res = res.Substring(0, res.Length - 1);
            res += "]";
            using (StreamWriter writer = new StreamWriter(new FileStream(filename, FileMode.Create)))
            {
                writer.Write(res);
            }
        } 

        public override IEnumerable<(IFigure, FigureGraphicsParameters)> LoadFromFile(string filename)
        {
            string jsZeroLevel;
            using (StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                jsZeroLevel = reader.ReadToEnd();
            }          

            List<Root> jsFirstLevel = new List<Root>();
            jsFirstLevel = JsonSerializer.Deserialize<List<Root>>(jsZeroLevel);

            List<(IFigure, FigureGraphicsParameters)> data = new List<(IFigure, FigureGraphicsParameters)>();

            foreach (var el in jsFirstLevel)
            {
                data.Add((FigureAssistant.CreateFigFromJson(el.first), FigureAssistant.CreateMatFromJson(el.second)));
            }

            return data;
        }

        protected override void CloseFile()
        {
            throw new System.NotImplementedException();
        }

        protected override void CreateFile(string filename, double w, double h)
        {
            throw new System.NotImplementedException();
        }

        protected override IGraphics CreateGraphic()
        {
            throw new System.NotImplementedException();
        }
    }


}
