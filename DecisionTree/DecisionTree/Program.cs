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
            Level1();

        }

        public static void Level3()
        {
            DataSet ds = new DataSet(@"C:\Users\student\Documents\Compusci\Homework 7\signal.dat");
            DataSet dsB = new DataSet(@"C:\Users\student\Documents\Compusci\Homework 7\background.dat");
            List<double> alpha = new List<double>();
            var tree = new Leaf();

            for (int i = 0; i < 10; i++)
            {
                tree = new Leaf();
                tree.Train(ds, dsB);

                tree.WriteToFile(@"C:\Users\student\Documents\Compusci\Homework7.2\tree" + i + ".txt");
                Console.WriteLine("Tree #" + i + " is finished.");
                int count = 0;
                foreach (var point in ds.Points)
                {
                    double purity = tree.RunDataPoint(point);
                    if (purity < .5)
                    {
                        count++;
                    }
                }
                foreach (var point in dsB.Points)
                {
                    double purity = tree.RunDataPoint(point);
                    if (purity > 0.5)
                    {
                        count++;
                    }
                }
                Console.WriteLine("Finished Calculating Weights for tree " + i);
                double r = count / (ds.Points.Count + dsB.Points.Count + 0.0);
                double weight = ((1 - r) / r) + 20;
                Console.WriteLine(weight);
                foreach (var point in ds.Points)
                {
                    double purity = tree.RunDataPoint(point);
                    if (purity < .5)
                    {
                        point.Weight = weight;
                        point.FinalWeight += Math.Log(weight);
                    }
                    else
                    {
                        point.Weight = 1;
                    }
                }
                foreach (var point in dsB.Points)
                {
                    double purity = tree.RunDataPoint(point);
                    if (purity > 0.5)
                    {
                        point.Weight = weight;
                        point.FinalWeight += Math.Log(weight);
                    }
                    else
                    {
                        point.Weight = 1;
                    }
                }
                alpha.Add(weight);

            }
            alpha.Add(29.2511532547412);
            alpha.Add(31.0992135511192);
            alpha.Add(29.416666667);
            alpha.Add(31.0992135511192);
            alpha.Add(29.416666667);
            alpha.Add(31.0992135511192);
            alpha.Add(29.416666667);
            alpha.Add(31.0992135511192);
            alpha.Add(29.416666667);
            alpha.Add(31.0992135511192);
            alpha.Add(29.416666667);
            DataSet dtd = new DataSet(@"C:\Users\student\Documents\Compusci\Homework 7\decisionTreeData.dat");
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\student\Documents\Compusci\Homework7.2\results.txt", true))
            {
                foreach (var point in dtd.Points)
                {
                    double total = 0;
                    for (int i = 0; i < 10; i++)
                    {
                        var treeNum = new Leaf(@"C:\Users\student\Documents\Compusci\Homework7.2\tree" + i + ".txt");
                        double purity = treeNum.RunDataPoint(point);
                        total = Math.Log(alpha[i]) * purity;
                        file.WriteLine(total);

                    }
                }
            }
        }


        public static void Level2()
        {
            DataSet ds = new DataSet(@"C:\Users\student\Documents\Compusci\Homework 7\decisionTreeData.dat");

            var tree = new Leaf(@"C:\Users\student\Documents\Compusci\Homework 7\treeBasic.txt");
            /**
            tree.Train(ds, dsB);
            tree.WriteToFile(@"C:\Users\student\Documents\Compusci\Homework 7\tree1.txt");
    **/
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\student\Documents\Compusci\Homework 7\level2results.txt", true))
            {
                foreach (var point in ds.Points)
                {
                    file.WriteLine(tree.RunDataPoint(point));
                }
            }
            

           
        }


        public static void Level1()
        {
            DataSet ds = new DataSet(@"C:\Users\student\Documents\Compusci\Homework 7\signal.dat");
            DataSet dsB = new DataSet(@"C:\Users\student\Documents\Compusci\Homework 7\background.dat");

            double split = GetSplit(ds, dsB, ds.Points[0].Variables.Length);
            Console.WriteLine("Best Split: " + split);
            Console.WriteLine("Variable: " + varIndex);
            Console.WriteLine("Gini Score: " + bestGini);

            DataSet dstd = new DataSet(@"C:\Users\student\Documents\Compusci\Homework 7\decisionTreeData.dat");


            int i = 0;
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\student\Documents\Compusci\Homework 7\results.txt", true))
            {
                foreach (DataPoint p in dstd.Points)
                {
                    if (p.Variables[0] < split)
                    {
                        file.WriteLine("0");
                        i++;
                    }
                    else
                    {
                        file.WriteLine("1");
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
