using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Core.Model;
using ReactiveUI;

namespace Vectorly.UI
{

   /// <summary>
   /// Логика взаимодействия для ToolPanel.xaml
   /// </summary>
   public partial class ToolPanel : UserControl, IViewFor<ToolPanelViewModel>
   {
      ToolPanelViewModel viewModel = null;
      object IViewFor.ViewModel { get => viewModel; set => throw new NotImplementedException(); }
      public ToolPanelViewModel ViewModel { get => viewModel; set => throw new NotImplementedException(); }

      public ToolPanel()
      {
         InitializeComponent();

         this.WhenActivated(disposable =>
         {
            viewModel = (ToolPanelViewModel)DataContext;
         });
      }

      private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         ListBox listbox = (ListBox)sender;

         if (listbox.SelectedIndex == -1)
         {
            listbox.SelectedIndex = ViewModel.PreviousIndex;
            ViewModel.CurrentMode = (Mode)ViewModel.PreviousIndex;
         }
      }

	}
}
