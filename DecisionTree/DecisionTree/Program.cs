using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DecisionTree
{
    class Program
    {
        public static int varIndex = 0;
        public static double bestGini = 999; 
        static void Main(string[] args)
        {
            DataSet ds = new DataSet(@"C:\Users\student\Documents\Compusci\Homework 7\signal.dat");
            DataSet dsB = new DataSet(@"C:\Users\student\Documents\Compusci\Homework 7\background.dat");

            Level1();
        }

        public static void Level1()
        {
            DataSet ds = new DataSet(@"C:\Users\student\Documents\Compusci\Homework 7\signal.dat");
            DataSet dsB = new DataSet(@"C:\Users\student\Documents\Compusci\Homework 7\background.dat");

            double split = GetSplit(ds, dsB, ds.Points[0].Variables.Length);
            Console.WriteLine("Best Split: " + split);
            Console.WriteLine("Variable: " + varIndex);
            Console.WriteLine("Gini Score: " + bestGini);

            int i = 0;
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\student\Documents\Compusci\Homework 7\results.txt", true))
            {
                foreach (DataPoint p in dsB.Points)
                {
                    if (p.Variables[0] < split)
                    {
                        file.WriteLine(i + "," + 0);
                        i++;
                    }
                    else
                    {
                        file.WriteLine(i + "," + 1);
                        i++;
                    }
                }
                foreach (DataPoint p in ds.Points)
                {
                    if (p.Variables[0] < split)
                    {
                        file.WriteLine(i + "," + 0);
                        i++;
                    }
                    else
                    {
                        file.WriteLine(i + "," + 1);
                        i++;
                    }
                }
            }

        }

        public static double gini_index(Tuple<int, int> signal, Tuple<int, int> background)
        {
            double total1 = signal.Item1 + background.Item1;
            double giniIndex =0;
            if (total1 != 0)
            {
                double proportion1 = signal.Item1 / total1;
                double proportion2 = background.Item1 / (total1);
                giniIndex += proportion1 * proportion2; 
            }
            else
            {
                giniIndex = 0.25;
            }

            double total2 = signal.Item2 + background.Item2; 
            if(total2 != 0)
            {
                double proportion3 = signal.Item2 / total2;
                double proportion4 = background.Item2 / total2;
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
            foreach(DataPoint d in ds.Points)
            {
                if(d.Variables[variableIndex] < cut)
                {
                    left++;
                }
                else
                {
                    right++;
                }
            }
            return new Tuple<int, int>(left, right);
        }

        public static double GetSplit(DataSet signal, DataSet background, int variableMax)
        {
            double lowGini = 999;
            double bestSplit = 0; 
            for(int i = 0; i < variableMax; i++)
            {
                foreach (DataPoint d in signal.Points)
                {
                    Tuple<int, int> sigGroup = Split(i, d.Variables[i], signal);
                    Tuple<int, int> backGroup = Split(i, d.Variables[i], background);
                    double gini = gini_index(sigGroup, backGroup);
                    if(gini < lowGini)
                    {
                        bestGini = gini;
                        bestSplit = d.Variables[i];
                        lowGini = gini;
                        varIndex = i; 
                    }
                }
            }
            
            
            return bestSplit; 
        }

    }
}
