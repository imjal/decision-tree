using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    public class Leaf
    {
        private Leaf output1 = null;
        private Leaf output2 = null;
        private double split;
        private int variable;

        private int nBackground = 0;
        private int nSignal = 0;

        public Leaf() :
            this(0, 0)
        { }

        public Leaf(int variable, double split)
        {
            this.variable = variable;
            this.split = split;
        }

        public void WriteToFile(string filename)
        {
            var bw = new BinaryWriter(File.Create(filename));
            write(bw);
            bw.Close();
        }

        private void write(BinaryWriter bw)
        {
            bw.Write(variable);
            bw.Write(split);
            bw.Write(nSignal);
            bw.Write(nBackground);

            bool isfinal = IsFinal();
            bw.Write(isfinal);
            if (!isfinal)
            {
                output1.write(bw);
                output2.write(bw);
            }
        }

        public Leaf(string filename)
        {
            var br = new BinaryReader(File.OpenRead(filename));
            read(br);
        }

        private Leaf(BinaryReader sr)
        {
            read(sr);
        }

        private void read(BinaryReader br)
        {
            variable = br.ReadInt32();
            split = br.ReadDouble();
            nSignal = br.ReadInt32();
            nBackground = br.ReadInt32();

            bool fin = br.ReadBoolean();
            if (!fin)
            {
                output1 = new Leaf(br);
                output2 = new Leaf(br);
            }
        }

        public bool IsFinal()
        {
            return output1 == null || output2 == null;
        }

        public double GetPurity()
        {

            return nSignal/(nSignal + 0.0 + nBackground);
        }

        public double RunDataPoint(DataPoint dataPoint)
        {
            if (IsFinal())
            {
                return GetPurity();
            }

            if (doSplit(dataPoint))
            {
                return output1.RunDataPoint(dataPoint);
            }
            else
            {
                return output2.RunDataPoint(dataPoint);
            }
        }

        private bool doSplit(DataPoint dataPoint)
        {
            return dataPoint.Variables[variable] <= split;
        }

        public void Train(DataSet signal, DataSet background)
        {
            nSignal = signal.Points.Count;
            nBackground = background.Points.Count;

            bool branch = chooseVariable(signal, background);

            if (branch)
            {
                output1 = new Leaf();
                output2 = new Leaf();

                DataSet signalLeft = new DataSet();
                DataSet signalRight = new DataSet();
                DataSet backgroundLeft = new DataSet();
                DataSet backgroundRight = new DataSet();

                foreach (var dataPoint in signal.Points)
                {
                    if (doSplit(dataPoint))
                    {
                        signalLeft.AddDataPoint(dataPoint);
                    }
                    else
                    {
                        signalRight.AddDataPoint(dataPoint);
                    }
                }

                foreach (var dataPoint in background.Points)
                {
                    if (doSplit(dataPoint))
                    {
                        backgroundLeft.AddDataPoint(dataPoint);
                    }
                    else
                    {
                        backgroundRight.AddDataPoint(dataPoint);
                    }
                }

                output1.Train(signalLeft, backgroundLeft);
                output2.Train(signalRight, backgroundRight);
            }
        }

        private bool chooseVariable(DataSet signal, DataSet background)
        {
            double lowGini = 999;
            Tuple<int, int> sigGroup = new Tuple<int, int>(0,0);
            Tuple<int, int> backGroup = new Tuple<int, int>(0, 0);
            double signalLeft = 0;
            double signalRight = 0;
            double backgroundLeft = 0;
            double backgroundRight = 0; 

            for (int i = 0; i < signal.Points[0].Variables.Length; i++)
            {
                for (int j = 0; j < signal.Points.Count; j+=10)
                {
                    sigGroup = Split(i, signal.Points[j].Variables[i], signal);
                    backGroup = Split(i, signal.Points[j].Variables[i], background);
                    if (sigGroup.Item2 < 50 || sigGroup.Item1 < 50 || backGroup.Item1 < 50 || backGroup.Item2 < 50)
                    {
                        continue;
                    }
                    double gini = gini_index(sigGroup, backGroup);
                    if (gini < lowGini)
                    {
                        signalLeft = sigGroup.Item1;
                        signalRight = sigGroup.Item2;
                        backgroundLeft = backGroup.Item1;
                        backgroundRight = backGroup.Item2; 

                        split = signal.Points[j].Variables[i];
                        lowGini = gini;
                        variable = i;
                    }
                }
                
           }
            if (signalLeft < 50 || signalRight < 50 || backgroundLeft < 50 || backgroundRight < 50)
            {
                return false;
            }
            return true;
        }

        public static double gini_index(Tuple<int, int> signal, Tuple<int, int> background)
        {
            double total1 = signal.Item1 + background.Item1 +0.0;
            double giniIndex = 0;
            if (total1 != 0)
            {
                double proportion1 = signal.Item1 / (total1+0.0);
                double proportion2 = background.Item1 / (total1*1.0);
                giniIndex += proportion1 * proportion2;
            }
            else
            {
                giniIndex = 0.25;
            }

            double total2 = signal.Item2 + background.Item2;
            if (total2 != 0)
            {
                double proportion3 = signal.Item2 / (total2*1.0);
                double proportion4 = background.Item2 / (total2*1.0);
                giniIndex += proportion3 * proportion4;
            }
            else
            {
                giniIndex = 0.25;
            }

            return giniIndex;
        }

        public static Tuple<int, int> Split(int variableIndex, double cut, DataSet ds)
        {
            int left = 0;
            int right = 0;
            foreach (DataPoint d in ds.Points)
            {
                if (d.Variables[variableIndex] < cut)
                {
                    left += (int)Math.Round(d.Weight); 
                }
                else
                {
                    right += (int)Math.Round(d.Weight);
                }
            }
            return new Tuple<int, int>(left, right);
        }

    }
}
