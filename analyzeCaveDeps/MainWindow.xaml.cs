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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace analyzeCaveDeps
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DepAnalyzer depAnalyzer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BAnalyze_Click(object sender, RoutedEventArgs e)
        {
            depAnalyzer = new DepAnalyzer(TBaseDir.Text);
            FillLBProjects();
        }

        private void FillLBProjects()
        {
            LBProjects.Items.Clear();
            foreach (var project in depAnalyzer.Projects.Keys)
            {
                LBProjects.Items.Add(project);
            }            
        }
    }
}
