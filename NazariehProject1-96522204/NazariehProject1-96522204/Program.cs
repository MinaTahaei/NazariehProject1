using System;
using System.IO;
using System.Linq;

namespace NazariehProject1_96522204
{
    class Program
    {
        public string[] ReadFile()
        {
            var lineCount = File.ReadLines(@"..\..\input.txt").Count();
            string[] SaveArray = new string[lineCount];

            using (StreamReader sr = new StreamReader(@"..\..\input.txt"))
            {

                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    for (int i = 0; i <= lineCount; i++)
                    {
                        SaveArray[i] = sr.ReadLine();
                    }
                }
            }
            return SaveArray;
        }

        static void Main()
        {

        }
    }
}
