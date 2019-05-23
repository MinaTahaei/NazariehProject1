using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NazariehProject1_96522204
{
    class Program
    {
        public static string[] ReadFile()
        {
            var lineCount = File.ReadLines(@"..\..\..\input.txt").Count();
            List<string> SaveList = new List<string>();

            using (StreamReader sr = new StreamReader(@"..\..\..\input.txt"))
            {

                string line;

                while ((line = sr.ReadLine()) != null)
                {

                    SaveList.Add(line);

                }
            }
            return SaveList.ToArray();
        }

        public static Dictionary<string, Dictionary<string, string>> MakeNFA(string [] data,out string InitState, out HashSet<string> FinalStates)
        {
            Dictionary<string, Dictionary<string, string>> NFA = new Dictionary<string, Dictionary<string, string>>();
            InitState = "0";
            FinalStates = new HashSet<string>();
            
            for(int i = 2; i<data.Length; i++)
            {
               
                var temp = data[i].Split(',', StringSplitOptions.RemoveEmptyEntries);

                if(temp[0].Substring(0,2)=="->")
                {
                    InitState = temp[0].Substring(2);

                    temp[0] = InitState;
                }

                if(temp[2][0]== '*')
                {

                    var Finalstate = temp[2].Substring(1);

                    FinalStates.Add(Finalstate);

                    temp[2] = Finalstate;
                }

                if(NFA.ContainsKey(temp[0]))
                {
                    NFA[temp[0]].Add(temp[1], temp[2]);

                }
                else
                {
                    NFA.Add(temp[0], new Dictionary<string, string>());

                    NFA[temp[0]].Add(temp[1], temp[2]);
                }
            }
            return NFA;
        }

        public static void ConvertToDFA (Dictionary<string, Dictionary<string, string>> NFA)
        {

        }

        static void Main()
        {
            var data = ReadFile();
            MakeNFA(data,out string InitState, out HashSet<string> FinalStates);
        }
    }
}
