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
            // TODO: Finish this function so that it returns the purity of the leaf
            throw new NotImplementedException();
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
            // TODO set the values of variable and split here		
            // Return true if you were able to find a useful variable, and false if you were not and want to stop calculation here

            // If you are going to branch, you should end with:
            // variable = 3; // The index number of the variable you want
            // split = 2.55; // The value of the cut
            // return true;

            // Or if you cannot split usefully, you should
            // return false;
            // Make sure to do this or your code will run forever!

            throw new NotImplementedException();
        }

    }
}
