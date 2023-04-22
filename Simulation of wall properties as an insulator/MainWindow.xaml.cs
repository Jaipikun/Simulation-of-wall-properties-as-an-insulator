using ScottPlot;
using Syncfusion.UI.Xaml.Charts;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Simulation_of_wall_properties_as_an_insulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double LeftSideTemperature;
        private double RightSideTemperature;
        private double H;
        private int N;
        private Simulation wall;
        private bool isRunning = false;
        private Dictionary<string,int> AvailableFunction;
        private Dictionary<string, int> PossibleGraph;

        public MainWindow()
        {
            InitializeComponent();
            this.RightSideTemperature = RightTempSlider.Value;
            this.LeftSideTemperature = LeftTempSlider.Value;

            FunctionList.Items.Clear();
            AddComboBoxItems();

        }

        private void AddComboBoxItems()
        {

            FunctionList.Items.Add("y = D1*x + D2");
            FunctionList.Items.Add("y = D1(x^2 + x) + D2");
            FunctionList.Items.Add("y = D1*sin(x) + D2");
            FunctionList.Items.Add("y = D1*cos(x) + D2");
            FunctionList.Items.Add("y = D1 / {1 + exp[(x - b) / τ ]} + D2");


            GraphDataX.Items.Add("Pozycja X");
            GraphDataX.Items.Add("Przewodność cieplna ");
            GraphDataX.Items.Add("Pochodna przewodności ciepła");
            GraphDataX.Items.Add("Temperatura wew. ściany");


            GraphDataY.Items.Add("Pozycja X");
            GraphDataY.Items.Add("Przewodność cieplna ");
            GraphDataY.Items.Add("Pochodna przewodności ciepła");
            GraphDataY.Items.Add("Temperatura wew. ściany");

        }

        private void Plot()
        {
            WPF1.Plot.Clear();
            if(GraphDataX.SelectedIndex != -1 && GraphDataY.SelectedIndex != -1 && double.TryParse(D1.Text,out double BetterMatConductivity) && double.TryParse(D2.Text, out double WorseMatConductivity))
            {
                int xID = GraphDataX.SelectedIndex;
                int yID = GraphDataY.SelectedIndex;

                wall.Calculate(LeftSideTemperature, RightSideTemperature, BetterMatConductivity, WorseMatConductivity);

                WPF1.Plot.AddScatter(wall.GetData(xID), wall.GetData(yID));
                WPF1.Refresh();
            }
            WPF1.Refresh();

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                int functionID = FunctionList.SelectedIndex;
                if (int.TryParse(NumberOfPointsInput.Text, out this.N) && double.TryParse(GridConstantInput.Text, out this.H))
                {
                    this.wall = new Simulation(H, N, functionID);
                    this.isRunning = true;
                    this.StartStopButton.Content = "Stop";

                    this.RightSideTemperature = RightTempSlider.Value;
                    this.LeftSideTemperature = LeftTempSlider.Value;

                    Plot();
                }

                
            }
            else
            {
                this.StartStopButton.Content = "Start";
                isRunning = false;
            }
            
            
        }

        private void RightSideSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.RightSideTemperature = RightTempSlider.Value;
            
            if (isRunning)
            {
                DataTextBox.Text = $" T - lewa:   {LeftSideTemperature} \n T - prawa: {RightSideTemperature}";
                Plot();
            }
            
        }

        private void LeftTempSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            this.LeftSideTemperature = LeftTempSlider.Value;
            
            if (isRunning)
            {
                DataTextBox.Text = $" T - lewa:   {LeftSideTemperature} \n T - prawa: {RightSideTemperature}";
                Plot();
            }


            
        }
    }
}
