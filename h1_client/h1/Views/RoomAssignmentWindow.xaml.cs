using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace h1.Views
{
	/// <summary>
	/// Interaction logic for RoomAssignmentWindow.xaml
	/// </summary>
	public partial class RoomAssignmentWindow : Window
	{
		public RoomAssignmentWindow()
		{
			InitializeComponent();
		}

		private void ManualRoomAssignmentButton_Click(object sender, RoutedEventArgs e)
		{
			// Create and set the content for View 1
			ManualAssignmentView view1 = new ManualAssignmentView();
			contentContainer.Content = view1;
		}

		private void AutoRoomAssignmentButton_Click(object sender, RoutedEventArgs e)
		{
			// Create and set the content for View 1
			//AutoAssignmentView view1 = new AutoAssignmentView();
			//contentContainer.Content = view1;
		}
	}
}
