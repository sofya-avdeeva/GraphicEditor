using Core.FileSystem;
using Core.Geometry;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Windows.Input;

namespace Core.Model
{
    public enum Mode
    {
        Selection,
        Rectangle,
        Line,
        Ellipse,
        Triangle,
        Pencil,
        OpenFile
    }

    public class ToolPanelViewModel : ReactiveObject
    {
        MainViewModel viewModel;

        Mode currentMode;
        public Mode CurrentMode
        {
            get => currentMode;
            set
            {
                PreviousIndex = (int)currentMode;
                this.RaiseAndSetIfChanged(ref currentMode, value);
            }
        }

        public int PreviousIndex { get; set; }

        public ReactiveCommand<Unit, Unit> Open { get; set; }
        public ReactiveCommand<Unit, Unit> Save { get; set; }

        public ReactiveCommand<Unit, Unit> SaveToSVG { get; set; }
        public ReactiveCommand<Unit, Unit> SaveToEMF { get; set; }
        public ReactiveCommand<Unit, Unit> SaveToEPS { get; set; }

        public ReactiveCommand<Unit, Unit> Exit { get; set; }
        public ReactiveCommand<Unit, Unit> ExitWithoutSave { get; set; }

        public ReactiveCommand<Unit, Unit> Undo { get; set; }
        public ReactiveCommand<Unit, Unit> Redo { get; set; }

        public ReactiveCommand<Unit, Unit> SetSelectionMode { get; set; }
        public ReactiveCommand<Unit, Unit> SetRectangleMode { get; set; }
        public ReactiveCommand<Unit, Unit> SetLineMode { get; set; }
        public ReactiveCommand<Unit, Unit> SetEllipseMode { get; set; }
        public ReactiveCommand<Unit, Unit> SetTriangleMode { get; set; }
        public ReactiveCommand<Unit, Unit> SetPencilMode { get; set; }

        public ToolPanelViewModel(MainViewModel viewModel)
        {
            this.viewModel = viewModel;

            Open = ReactiveCommand.Create(OpenImpl);

            Save = ReactiveCommand.Create(SaveImpl);

            SaveToSVG = ReactiveCommand.Create(SaveToSVGImpl);
            SaveToEMF = ReactiveCommand.Create(SaveToEMFImpl);
            SaveToEPS = ReactiveCommand.Create(SaveToEPSImpl);

            Exit = ReactiveCommand.Create(ExitImpl);
            ExitWithoutSave = ReactiveCommand.Create(ExitWithoutSaveImpl);

            Undo = ReactiveCommand.Create(UndoImpl);
            Redo = ReactiveCommand.Create(RedoImpl);

            SetSelectionMode = ReactiveCommand.Create(() => { CurrentMode = Mode.Selection; });
            SetRectangleMode = ReactiveCommand.Create(() => { CurrentMode = Mode.Rectangle; });
            SetLineMode = ReactiveCommand.Create(() => { CurrentMode = Mode.Line; });
            SetEllipseMode = ReactiveCommand.Create(() => { CurrentMode = Mode.Ellipse; });
            SetTriangleMode = ReactiveCommand.Create(() => { CurrentMode = Mode.Triangle; });
            SetPencilMode = ReactiveCommand.Create(() => { CurrentMode = Mode.Pencil; });
     
        }

        void ExitWithoutSaveImpl()
        {
            Environment.Exit(0);
        }

        public string filename;

        private void ExitImpl()
        {
            SaveImpl();
            if (filename != "")
                ExitWithoutSaveImpl();
        }


        private void OpenImpl()
        {
            filename = viewModel.Window.OpenFileDialog("JSON формат (*.json)", "json");
            if (filename != "")
            {
                Saver saver = new JSONSaver();
                IEnumerable<(IFigure, FigureGraphicsParameters)> figs = saver.LoadFromFile(filename);
                viewModel.Parameters.Clear();
                foreach (var fig in figs)
                {
                    viewModel.Figures.Add(fig.Item1);
                    viewModel.Parameters.Add(fig.Item1, fig.Item2);
                }
                currentMode = Mode.OpenFile;
            }
        }

        private void SaveImpl()
        {
            filename = viewModel.Window.SaveFileDialog("JSON формат (*.json)", "json");
            if (filename != "")
            {
                Saver saver = new JSONSaver();

                double maxX = double.MinValue;
                double maxY = double.MinValue;

                foreach (var fig in viewModel.Figures)
                {
                    foreach (var v in fig.Vertexes)
                    {
                        if (maxX < v.X)
                            maxX = v.X + viewModel.Parameters[fig].LineThickness;

                        if (maxY < v.Y)
                            maxY = v.Y + viewModel.Parameters[fig].LineThickness;
                    }
                }

                List<(IFigure, FigureGraphicsParameters)> figs = new List<(IFigure, FigureGraphicsParameters)>();
                foreach (var pair in viewModel.Parameters)
                    figs.Add((pair.Key, pair.Value));

                saver.SaveToFile(filename, figs, maxX, maxY);

            }
        }

        void SaveToSVGImpl()
        {
            string filename = viewModel.Window.SaveFileDialog("SVG формат", "svg");
            if (filename != "")
            {
                double maxX = double.MinValue;
                double maxY = double.MinValue;

                foreach (var fig in viewModel.Figures)
                {
                    foreach (var v in fig.Vertexes)
                    {
                        if (maxX < v.X)
                            maxX = v.X + viewModel.Parameters[fig].LineThickness;

                        if (maxY < v.Y)
                            maxY = v.Y + viewModel.Parameters[fig].LineThickness;
                    }
                }

                Saver saver = new SVGSaver();

                List<(IFigure, FigureGraphicsParameters)> figs = new List<(IFigure, FigureGraphicsParameters)>();

                foreach (var pair in viewModel.Parameters)
                    figs.Add((pair.Key, pair.Value));

                saver.SaveToFile(filename, figs, maxX, maxY);
            }
        }

        private void SaveToEMFImpl()
        {
            throw new NotImplementedException();
        }

        private void SaveToEPSImpl()
        {
            throw new NotImplementedException();
        }

        private void UndoImpl()
        {
            throw new NotImplementedException();
        }

        private void RedoImpl()
        {
            throw new NotImplementedException();
        }
    }
}
