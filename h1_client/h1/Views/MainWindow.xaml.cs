﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace h1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DBMethods.DebugMethod();
        }

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			//open new window here, remember to dispose and close it afterwards
			
            // Create a new window instance
			AddUserWindow newWindow = new AddUserWindow();

			// Show the new window
			newWindow.Show();
		}
	}
}