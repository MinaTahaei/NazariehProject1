using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NazariehProject_96522204_96521173
{
    class Program
    {
        public static string[] ReadFile()
        {
            List<string> SaveList = new List<string>();

            using (StreamReader sr = new StreamReader(@"..\..\input.txt"))
            {

                string line;

                while ((line = sr.ReadLine()) != null)
                {

                    SaveList.Add(line);

                }
            }
            return SaveList.ToArray();
        }

        public static Dictionary<string, Dictionary<string, List<string>>> MakeNFA(string[] data, out string InitState, out HashSet<string> FinalStates)
        {
            Dictionary<string, Dictionary<string, List<string>>> NFA = new Dictionary<string, Dictionary<string, List<string>>>();
            InitState = "0";
            FinalStates = new HashSet<string>();

            for (int i = 2; i < data.Length; i++)
            {

                var temp = data[i].Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);

                if (temp[0].Substring(0, 2) == "->")
                {
                    InitState = temp[0].Substring(2);

                    temp[0] = InitState;
                }

                if (temp[2][0] == '*')
                {

                    var Finalstate = temp[2].Substring(1);

                    FinalStates.Add(Finalstate);

                    temp[2] = Finalstate;
                }
                if(temp[0][0] == '*')
                {
                    var Finalstate = temp[0].Substring(1);

                    FinalStates.Add(Finalstate);

                    temp[0] = Finalstate;
                }

                if (NFA.ContainsKey(temp[0]))
                {
                    if (NFA[temp[0]].ContainsKey(temp[1]))
                    {
                        NFA[temp[0]][temp[1]].Add(temp[2]);
                    }
                    else
                    {
                        NFA[temp[0]].Add(temp[1], new List<string>() { temp[2] });
                    }

                }
                else
                {
                    NFA.Add(temp[0], new Dictionary<string, List<string>>());

                    NFA[temp[0]].Add(temp[1], new List<string>() { temp[2]});
                }
            }
            return NFA;
        }

        public static void ConvertToDFA(Dictionary<string, Dictionary<string, List<string>>> NFA, string InitState, HashSet<string> FinalState)
        {
            var initstate = new State(InitState, new Dictionary<string, int>(), new List<string>() { InitState });
            var states = new List<State>();
            states.Add(initstate);
            
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                for (int j = 0; j < state.States.Count; j++)
                {
                    if (NFA[state.States[j]].ContainsKey("_"))
                    {
                        state.States.AddRange(NFA[state.States[j]]["_"]);
                    }
                }
                StateExists(states, state);
                var nextStates = new Dictionary<string, State>();
                foreach (var s in state.States)
                {
                    foreach (var ns in NFA[s])
                    {
                        if (ns.Key != "_")
                        {
                            if (nextStates.ContainsKey(ns.Key))
                            {
                                nextStates[ns.Key].States.AddRange(ns.Value);
                            }
                            else
                            {
                                nextStates.Add(ns.Key, new State("", new Dictionary<string, int>(), ns.Value));
                            }
                        }
                    }
                }

                foreach (var ns in nextStates)
                {
                    var index = StateExists(states, ns.Value);
                    state.NextStates.Add(ns.Key, index);
                }
            }
        }

        public static int StateExists(List<State> states, State state)
        {
            for(int i = 0; i < states.Count; i++)
            {
                if(states[i].States.Count == state.States.Count)
                {
                    bool found = true;
                    for(int j = 0; j < state.States.Count; j++)
                    {
                        if(state.States[j] != states[i].States[j])
                        {
                            found = false;
                        }
                    }
                    if(found)
                    {
                        return i;
                    }
                }
            }
            states.Add(state);
            return states.Count - 1;
        }

        static void Main()
        {
            var data = ReadFile();
            var nfa = MakeNFA(data, out string InitState, out HashSet<string> FinalStates);

            ConvertToDFA(nfa, InitState, FinalStates);
        }
    }
}