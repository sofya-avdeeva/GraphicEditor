using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Geometry;
using Core.Interfaces;
using Core.Model;
using ReactiveUI;

namespace Vectorly.UI
{
	public class VectorlyCanvas : Canvas, IViewFor<MainViewModel>
	{
		MainViewModel viewModel;
		public MainViewModel ViewModel { get => viewModel; set => throw new NotImplementedException(); }
		object IViewFor.ViewModel { get => viewModel; set => throw new NotImplementedException(); }

		List<RenderingUnit> scene = new List<RenderingUnit>();
		List<RenderingUnit> tempScene = new List<RenderingUnit>();

		public System.Windows.Media.Brush Brush { get; set; }
		public System.Windows.Media.Pen Pen { get; set; }
      
      public VectorlyCanvas()
		{
			MouseDown += OnMouseDownImpl;
			MouseMove += OnMouseMoveImpl;
			MouseUp += OnMouseUpImpl;

			Brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 230, 230, 230));
			Pen = new System.Windows.Media.Pen(Brush, 1.0);

			Focusable = true;
			ClipToBounds = true;

         this.WhenActivated(d =>
         {
            viewModel = (MainViewModel)DataContext;
            viewModel.FullRedrawNeed += RedrawFull;
            viewModel.SelectedRedrawNeed += RedrawSelected;
         });
      }

      void RedrawFull(object sender, EventArgs e)
		{
			scene.Clear();
			ViewModel.RedrawFull.Execute(CreateGraphicsInteface()).Subscribe();
			InvalidateVisual();
		}

      void RedrawSelected(object sender, EventArgs e)
      {
			tempScene.Clear();
			ViewModel.RedrawSelected.Execute(CreateGraphicsInteface(true)).Subscribe();
			InvalidateVisual();
		}

		IGraphics CreateGraphicsInteface(bool forSelected = false) => new Graphics(this, forSelected);

		public void AddRenderingUnit(System.Windows.Media.Geometry geometry)
		{
			scene.Add(new RenderingUnit
			{
				Geometry = geometry,
				Brush = Brush,
				Pen = Pen
			});
		}

		public void AddTempRenderingUnit(System.Windows.Media.Geometry geometry)
      {
			tempScene.Add(new RenderingUnit
			{
				Geometry = geometry,
				Brush = Brush,
				Pen = Pen
			});
		}

		void OnMouseDownImpl(object sender, MouseButtonEventArgs e)
		{
			Point p = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);
			ViewModel.OnMouseDown.Execute(p).Subscribe();
		}

		void OnMouseMoveImpl(object sender, MouseEventArgs e)
		{
			Point p = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);
			ViewModel.OnMouseMove.Execute(p).Subscribe();
		}

		void OnMouseUpImpl(object sender, MouseButtonEventArgs e)
      {
			Point p = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);
			ViewModel.OnMouseUp.Execute(p).Subscribe();
		}

		protected override void OnRender(System.Windows.Media.DrawingContext dc)
		{
			base.OnRender(dc);

			foreach (var unit in scene)
				dc.DrawGeometry(unit.Brush, unit.Pen, unit.Geometry);

			foreach (var unit in tempScene)
				dc.DrawGeometry(unit.Brush, unit.Pen, unit.Geometry);
		}
   }
}
