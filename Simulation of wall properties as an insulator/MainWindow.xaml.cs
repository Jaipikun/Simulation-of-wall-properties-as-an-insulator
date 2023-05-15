using ScottPlot;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private bool Initalized = false;

        public MainWindow()
        {
            InitializeComponent();
            Initalized = true;
            this.RightSideTemperature = RightTempSlider.Value;
            this.LeftSideTemperature = LeftTempSlider.Value;

            FunctionList.Items.Clear();
            AddComboBoxItems();

            TauLabel.Visibility = Visibility.Collapsed;
            WidthLabel.Visibility = Visibility.Collapsed;
            TauTxtBox.Visibility = Visibility.Collapsed;
            WidthTxtBox.Visibility = Visibility.Collapsed;
            TauSlider.Visibility = Visibility.Collapsed;
            WidthSlider.Visibility = Visibility.Collapsed;

        }

        private void AddComboBoxItems()
        {

            FunctionList.Items.Add("y = D1*x + D2");
            FunctionList.Items.Add("y = D1(x^2 + x) + D2");
            FunctionList.Items.Add("y = D1*sin(x) + D2");
            FunctionList.Items.Add("y = D1*cos(x) + D2");
            FunctionList.Items.Add("y = D1 / {1 + exp[(x - b) / τ ]} + D2");
            FunctionList.Items.Add("y = 1");


            GraphDataX.Items.Add("Pozycja X");
            GraphDataX.Items.Add("Przewodność cieplna ");
            GraphDataX.Items.Add("Pochodna przewodności ciepła");
            GraphDataX.Items.Add("Temperatura wew. ściany");


            GraphDataY.Items.Add("Pozycja X");
            GraphDataY.Items.Add("Przewodność cieplna ");
            GraphDataY.Items.Add("Pochodna przewodności ciepła");
            GraphDataY.Items.Add("Temperatura wew. ściany");

        }

        private async void Plot()
        {
            WPF1.Plot.Clear();
            if(GraphDataX.SelectedIndex != -1 && GraphDataY.SelectedIndex != -1 && double.TryParse(D1.Text,out double BetterMatConductivity) && double.TryParse(D2.Text, out double WorseMatConductivity))
            {
                int xID = GraphDataX.SelectedIndex;
                int yID = GraphDataY.SelectedIndex;
                bool TaskFinished = false;
                
                if(FunctionList.SelectedIndex == 4 && double.TryParse(WidthTxtBox.Text,out double Width) && double.TryParse(TauTxtBox.Text, out double Tau))
                {

                    wall.Mat1Width = Width;
                    wall.DiffConstant = Tau;

                    await Task.Run(async () =>
                    {
                        wall.Calculate(LeftSideTemperature, RightSideTemperature, BetterMatConductivity, WorseMatConductivity);
                        TaskFinished = true;
                    });
    
                }
                else if(FunctionList.SelectedIndex != 4)
                {
                    await Task.Run(async () =>
                    {
                        wall.Calculate(LeftSideTemperature, RightSideTemperature, BetterMatConductivity, WorseMatConductivity);
                        TaskFinished = true;
                    });
                    
                    
                }
                if (TaskFinished)
                {
                    try
                    {
                        WPF1.Plot.AddScatter(wall.GetData(xID), wall.GetData(yID));
                        WPF1.Refresh();
                    }
                    catch (Exception)
                    {
                        //MessageBox.Show("Błędne dane.");
                        
                    }
                    
                }
                

            }

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
            if (Initalized)
            {
                this.RightSideTemperature = RightTempSlider.Value;
                DataTextBox.Text = $" T - lewa:   {Math.Round(LeftSideTemperature,5)} \n T - prawa: {Math.Round(RightSideTemperature,5)}";
                if (isRunning)
                {
                    Plot();
                }
            }
            
            
        }

        private void LeftTempSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Initalized)
            {
                this.LeftSideTemperature = LeftTempSlider.Value;
                DataTextBox.Text = $" T - lewa:   {Math.Round(LeftSideTemperature, 5)} \n T - prawa: {Math.Round(RightSideTemperature, 5)}";
                if (isRunning)
                {

                    Plot();
                }
            }
            


            
        }

        private void FunctionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(FunctionList.SelectedIndex != 4)
            {
                TauLabel.Visibility = Visibility.Collapsed;
                WidthLabel.Visibility = Visibility.Collapsed;
                TauTxtBox.Visibility = Visibility.Collapsed;
                WidthTxtBox.Visibility = Visibility.Collapsed;
                TauSlider.Visibility = Visibility.Collapsed;
                WidthSlider.Visibility = Visibility.Collapsed;


            }
            else
            {
                TauLabel.Visibility = 0;
                WidthLabel.Visibility = 0;
                TauTxtBox.Visibility = 0;
                WidthTxtBox.Visibility = 0;
                WidthSlider.Visibility = 0;
                TauSlider.Visibility = 0;
            }
        }

        private void WidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            WidthTxtBox.Text = Math.Round(WidthSlider.Value,4).ToString();
            if (isRunning)
            {
                Plot();
            }
        }

        private void TauSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TauTxtBox.Text = Math.Round(TauSlider.Value,4).ToString();
            if (isRunning)
            {
                Plot();
            }

        }

        private void WidthTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Initalized)
            {
                if (WidthTxtBox.Text != WidthSlider.Value.ToString() && double.TryParse(WidthTxtBox.Text, out double value))
                {
                    WidthSlider.Value = value;
                    if (isRunning)
                    {
                        Plot();
                    }
                }
            }
            
        }

        private void TauTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Initalized)
            {
                if (TauTxtBox.Text != TauSlider.Value.ToString() && double.TryParse(TauTxtBox.Text, out double value))
                {
                    TauSlider.Value = value;
                    if (isRunning)
                    {
                        Plot();
                    }
                }
            }
        }

        private void GraphDataXY_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isRunning)
            {
                Plot();
            }
            
        }

        
    }
}
