using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation_of_wall_properties_as_an_insulator
{
    class Simulation
    {
        public double H { get; set; }
        public int N { get; set; }
        public int FunctionID { get; set; }

        private double MatBetterConduct { get;set; }
        private double MatWorseConduct { get; set; }
        private double DiffConstant { get; set; }
        private double Mat1Width { get; set; }

        public double[] PositionX { get; set; }


        public double[] ThermalConductivity{get; set;}
        public double[] DerivativeOfThermalConductivity { get; set;}

        private double[] AMinus {  get; set; }
        private double[] AZero{ get; set; }
        private double[] APlus { get; set; }
        private double[] RightSideOfDifferentialEquation { get; set; }
        public double[] TemperatureInsideOfWall { get; set; }

        private double[] Alpha { get; set; }
        private double[] Beta { get; set; }
        private double[] Gamma { get; set; }



        public Simulation(double h,int n,int functionID) 
        {
            this.N = n;
            this.H = h;
            this.FunctionID = functionID;
        }

        public void Calculate(double LeftSideTemperature,double RightSideTemperature, double matBetterConduct, double matWorseConduct)
        {
            this.MatBetterConduct = matBetterConduct;
            this.MatWorseConduct = matWorseConduct;
            DiffConstant = 5.0;
            Mat1Width = 0.5;



            PositionX = new double[N];
            ThermalConductivity = new double[N];
            DerivativeOfThermalConductivity = new double[N];
            AMinus = new double[N];
            AZero = new double[N];
            APlus = new double[N];
            RightSideOfDifferentialEquation = new double[N];
            TemperatureInsideOfWall = new double[N];
            Alpha = new double[N];
            Beta = new double[N];
            Gamma = new double[N];
            InitialCalculations(LeftSideTemperature, RightSideTemperature);
            GaussMethod();
        }

        private void InitialCalculations(double LeftSideTemperature,double RightSideTemperature)
        {
            TemperatureInsideOfWall[0] = LeftSideTemperature;    //Boundary Value
            TemperatureInsideOfWall[N-1] = RightSideTemperature; //

            for(int i = 0; i < N; i++)
            {
                PositionX[i] = H * i; // Calculate xs

                ThermalConductivity[i] = Function(PositionX[i]); //Calculate thermal conductivity distribution
                DerivativeOfThermalConductivity[i] = DerivativeOfFunction(PositionX[i]); // Calculate distribution of derivative of thermal conductivity 

                AMinus[i] = ThermalConductivity[i] - H * DerivativeOfThermalConductivity[i] / 2.0;
                AZero[i] = -2.0 * ThermalConductivity[i];
                APlus[i] = ThermalConductivity[i] + H * DerivativeOfThermalConductivity[i] / 2.0;
                RightSideOfDifferentialEquation[i] = RightSideOfDifferentialEquationFunction(PositionX[i]);
            }

        }
        private void GaussMethod()
        {
            Alpha[N - 2] = 0.0;
            Beta[N - 2] = TemperatureInsideOfWall[N - 1];

            for(int i = N-2; i>0; i--)
            {
                Gamma[i] = -1.0 / (AZero[i] + APlus[i] * Alpha[i]);
                Alpha[i - 1] = AMinus[i] * Gamma[i];
                Beta[i - 1] = (APlus[i] * Beta[i] - RightSideOfDifferentialEquation[i]) * Gamma[i];
            }

            for(int i = 0; i < N - 2; i++)
            {
                TemperatureInsideOfWall[i + 1] = Alpha[i] * TemperatureInsideOfWall[i] + Beta[i];
            }

        }
        private double Function(double x)
        {
            switch (this.FunctionID)
            {
                case 0:
                    return MatBetterConduct * x + MatWorseConduct;
                case 1:
                    return MatBetterConduct *( x*x +x ) + MatWorseConduct;
                case 2:
                    return MatBetterConduct * Math.Sin(x) + MatWorseConduct;
                case 3:
                    return MatBetterConduct * Math.Cos(x) + MatWorseConduct;
                case 4:
                    double y = MatBetterConduct / (1 + Math.Exp(x - Mat1Width / DiffConstant)) + MatWorseConduct;
                    return y;

                default:
                    return 1;
            }
            
        }
        private double DerivativeOfFunction(double x)
        {
            switch (this.FunctionID)
            {
                case 0:
                    return MatBetterConduct;
                case 1:
                    return MatBetterConduct * 2 * x + MatBetterConduct;
                case 2:
                    return MatBetterConduct * Math.Cos(x);
                case 3:
                    return -MatBetterConduct * Math.Sin(x);
                case 4:
                    double SupVal = (x - Mat1Width) / DiffConstant;
                    double y = Math.Exp(SupVal) * (1 - (MatBetterConduct / DiffConstant)) / Math.Pow(1 + Math.Exp(SupVal), 2);
                    return y;

                default:
                    return 0;
            }
        }
        private double RightSideOfDifferentialEquationFunction(double x)
        {
            return 0.0;
        }

        public double[] GetData(int DataID)
        {
            switch(DataID)
            {
                
                case 0:
                    return this.PositionX;
                case 1:
                    return this.ThermalConductivity;
                case 2:
                    return this.DerivativeOfThermalConductivity;
                case 3:
                    return this.TemperatureInsideOfWall;
                default: return null;
            }
        }

    }
}
