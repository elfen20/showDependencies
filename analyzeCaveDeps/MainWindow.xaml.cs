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
            TBaseDir.Text = @"c:\";
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

        private void LBProjects_Selected(object sender, RoutedEventArgs e)
        {
            LBDpendsOn.Items.Clear();
            string pName = LBProjects.SelectedItem.ToString();
            foreach (var dep in depAnalyzer.Projects[pName].dependsOn)
            {
                LBDpendsOn.Items.Add(dep);
            }
        }
    }
}
