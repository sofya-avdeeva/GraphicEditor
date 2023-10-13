using System;
using System.Collections.Generic;
using ReactiveUI;
using System.Reactive;
using Core.Interfaces;
using Core.Geometry;
using System.Reactive.Linq;
using System.Linq;

namespace Core.Model
{
   public class MainViewModel : ReactiveObject
   {
      public IMainWindow Window;
      #region Events
      public event EventHandler FullRedrawNeed;
      public event EventHandler SelectedRedrawNeed;
      #endregion

      #region Collections
      public List<IFigure> Figures { get; set; } = new List<IFigure>();
      public Dictionary<IFigure, FigureGraphicsParameters> Parameters = new Dictionary<IFigure, FigureGraphicsParameters>();
      #endregion

      #region Draw Commands
      public ReactiveCommand<IGraphics, Unit> RedrawFull { get; set; }
      public ReactiveCommand<IGraphics, Unit> RedrawSelected { get; set; }
      #endregion

      #region Mouse Commands
      public ReactiveCommand<Point, Unit> OnMouseDown { get; set; }
      public ReactiveCommand<Point, Unit> OnMouseMove { get; set; }
      public ReactiveCommand<Point, Unit> OnMouseUp { get; set; }
      #endregion

      #region Figure Commands
      public ReactiveCommand<Unit, Unit> DeleteSelectedFigure { get; set; }
      public ReactiveCommand<Unit, Unit> Copy { get; set; }
      public ReactiveCommand<Unit, Unit> Paste { get; set; }
      #endregion

      #region Save Commands

      #endregion

      public ToolPanelViewModel ToolPanel { get; set; }

      public bool mouseDown = false;

      DrawingObject drawingObject = null;
      DrawingObject DrawingObject { get => drawingObject; set => this.RaiseAndSetIfChanged(ref drawingObject, value); }

      IFigure copiedFigure = null;
      IFigure selectedFigure;
      public IFigure SelectedFigure { get => selectedFigure; set => this.RaiseAndSetIfChanged(ref selectedFigure, value); }


      ObservableAsPropertyHelper<FigureGraphicsParameters> selectedparameters;
      public FigureGraphicsParameters SelectedFigureParameters => selectedparameters?.Value;

      public MainViewModel(IMainWindow window)
      {
         Window = window;

         ToolPanel = new ToolPanelViewModel(this);

         RedrawFull = ReactiveCommand.Create<IGraphics>(RedrawFullImpl);
         RedrawSelected = ReactiveCommand.Create<IGraphics>(RedrawSelectedImpl);

         OnMouseDown = ReactiveCommand.Create<Point>(OnMouseDownImpl);
         OnMouseMove = ReactiveCommand.Create<Point>(OnMouseMoveImpl);
         OnMouseUp = ReactiveCommand.Create<Point>(OnMouseUpImpl);

         var canExecute = this.WhenAnyValue<MainViewModel, bool, IFigure>(t => t.SelectedFigure, (fig) => fig != null);
         DeleteSelectedFigure = ReactiveCommand.Create(DeleteSelectedFigureImpl, canExecute);
         DeleteSelectedFigure.Subscribe(_ => FullRedrawNeed?.Invoke(null, null));

         Copy = ReactiveCommand.Create(CopyImpl);
         Paste = ReactiveCommand.Create(PasteImpl);

         Paste.Subscribe(_ => FullRedrawNeed?.Invoke(null, null));

         this.WhenAnyValue(t => t.ToolPanel.CurrentMode).Select(t => t != Mode.Selection).Subscribe(_ => SelectedFigure = null);
         this.WhenAnyValue(t => t.DrawingObject.StartPoint, t => t.DrawingObject.EndPoint).Subscribe(_ => SelectedRedrawNeed?.Invoke(null, null));

         #region Selected Figure and Parameters Subscription
         this.WhenAnyValue(t => t.SelectedFigure).Subscribe(_ => SelectedRedrawNeed?.Invoke(null, null));
         this.WhenAnyValue(
             t => t.SelectedFigure.LeftTopPoint,
            t => t.SelectedFigure.Width,
            t => t.SelectedFigure.Height,
            t => t.SelectedFigure.Angle
            ).Subscribe(_ => FullRedrawNeed.Invoke(null, null));

         this.WhenAnyValue(
            t => t.SelectedFigureParameters.LineThickness,
            t => t.SelectedFigureParameters.LineType,
            t => t.SelectedFigureParameters.LineColor,
            t => t.SelectedFigureParameters.FillColor
            ).Subscribe(_ => FullRedrawNeed?.Invoke(null, null));

         this.WhenAnyValue(t => t.SelectedFigure)
            .Select(f =>
            {
               if (f == null) return null;
               if (Parameters.TryGetValue(f, out var p)) return p;
               return null;
            }).ToProperty(this, t => t.SelectedFigureParameters, out selectedparameters);

         #endregion
      }

      IFigure FindFigure(Point p)
      {
         foreach (var fig in Figures)
            if (fig.IsIn(p))
               return fig;

         return null;
      }

      public void RedrawFullImpl(IGraphics graphics)
      {
         foreach (var pair in Parameters)
            pair.Key.Draw(graphics, pair.Value);

         SelectedRedrawNeed?.Invoke(null, null);
      }

      void RedrawSelectedImpl(IGraphics graphics)
      {
         DrawingObject?.Draw(graphics, FigureGraphicsParameters.Default);

         if (SelectedFigure != null)
         {
            graphics.SetPen(Color.Blue, 1.0, LineTypes.Solid);
            graphics.DrawPolyLine(SelectedFigure.Vertexes, SelectedFigure.Id != "polyline");
         }
      }

      void OnMouseDownImpl(Point p)
      {
         mouseDown = true;

         switch (ToolPanel.CurrentMode)
         {
            case Mode.Selection:
               IFigure figure = FindFigure(p);
               SelectedFigure = figure;
               break;

            case Mode.Rectangle:
               DrawingObject = new RectangleDrawingObject(p);
               break;

            case Mode.Ellipse:
               DrawingObject = new EllipseDrawingObject(p);
               break;

            case Mode.Triangle:
               DrawingObject = new TriangleDrawingObject(p);
               break;

            case Mode.Line:
               DrawingObject = new LineDrawingObject(p);
               break;
            case Mode.Pencil:
               DrawingObject = new PencilDrawingObject(p);
               break;
         }
      }

      void OnMouseMoveImpl(Point p)
      {
         DrawingObject?.Update(p);

         if (ToolPanel.CurrentMode == Mode.OpenFile)
         {
            FullRedrawNeed.Invoke(null, null);
            ToolPanel.CurrentMode = Mode.Selection;
         }

         if (ToolPanel.CurrentMode == Mode.Selection && SelectedFigure != null && mouseDown)
         {
            p = new Point(SelectedFigure.PointRotate(p, -SelectedFigure.Angle));
            SelectedFigure.X = p.X - SelectedFigure.Width / 2;
            SelectedFigure.Y = p.Y - SelectedFigure.Height / 2;
         }
      }

      void OnMouseUpImpl(Point p)
      {
         mouseDown = false;

         if (DrawingObject != null)
         {
            IFigure fig = DrawingObject.GetFigure();
            Figures.Add(fig);
            Parameters.Add(fig, FigureGraphicsParameters.Default);

            Parameters[fig].fillR = 230;
            Parameters[fig].fillG = 230;
            Parameters[fig].fillB = 230;
            Parameters[fig].fillA = 255;

            Parameters[fig].strokeR = 0;
            Parameters[fig].strokeG = 0;
            Parameters[fig].strokeB = 0;
            Parameters[fig].strokeA = 255;

            DrawingObject = null;

            FullRedrawNeed?.Invoke(null, null);
         }
      }

      void DeleteSelectedFigureImpl()
      {
         Figures.Remove(SelectedFigure);
         Parameters.Remove(SelectedFigure);
         SelectedFigure = null;
      }

      void CopyImpl()
      {
         if (selectedFigure != null)
         {
            copiedFigure = selectedFigure;
         }
      }

      void PasteImpl()
      {
         if (copiedFigure != null)
         {
            IFigure figure = copiedFigure.Clone();

            figure.X += 50;
            figure.Y += 50;
            FigureGraphicsParameters param = Parameters[copiedFigure].Clone();

            Figures.Add(figure);
            Parameters.Add(figure, param);
         }
      }
   }
}
