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
using System.Windows.Shapes;

namespace CSharp_Poker
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : Window
    {

        public Image image;

        public MainScreen()
        {
            InitializeComponent();
        }

        private void StartLocalGame(object sender, RoutedEventArgs e)
        {
            GameWindow screen = new GameWindow();
            screen.Show();
            this.Close();
        }
    }
}
