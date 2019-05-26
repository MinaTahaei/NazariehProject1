using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NazariehProject_96522204_96521173
{
    class Program
    {
        static void Main()
        {
            var data = ReadFile();
            var nfa = MakeNFA(data, out string InitState, out HashSet<string> FinalState);

            var dfa = ConvertToDFA(nfa, InitState, FinalState, data[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));//add trap
            var dfaFinalStates = FinalStates(dfa, FinalState);
            var dfaToString = DFAToString(dfa, dfaFinalStates, data[1]);

            var minimizedDFA = DFAMinimizer(dfa, dfaFinalStates, data[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            var minimizedToString = MinimizedToString(minimizedDFA, dfaFinalStates.ToList(), data[1]);
        }

        public static string MinimizedToString(Tuple<List<Dictionary<string, int>>, int[]> dfa, List<int> finalStates, string alphabet)
        {
            var table = dfa.Item1;
            var stateInSet = dfa.Item2;
            string answer = "";
            answer += table.Count + "\n";
            answer += alphabet + "\n->";
            for(int i = 0; i < finalStates.Count; i++)
            {
                finalStates[i] = stateInSet[finalStates[i]];
            }
            for (int i = 0; i < dfa.Item1.Count; i++)
            {
                foreach(var state in table[i])
                {
                    if (finalStates.Contains(stateInSet[i]))
                    {
                        answer += "*";
                    }
                    answer += "g" + stateInSet[i] + "," + state.Key + ",";
                    if (finalStates.Contains(state.Value))
                    {
                        answer += "*";
                    }
                    answer += "g" + state.Value + "\n";
                }
            }
            return answer;
        }

        public static string DFAToString(List<State> dfa, HashSet<int> finalStates, string alphabet)
        {
            string answer = "";
            answer += dfa.Count + "\n";
            answer += alphabet + "\n->";
            for(int i = 0; i < dfa.Count; i++)
            {
                foreach(var state in dfa[i].NextStates)
                {
                    if(finalStates.Contains(i))
                    {
                        answer += "*";
                    }
                    answer += "q" + i + "," + state.Key + ","; 
                    if(finalStates.Contains(state.Value))
                    {
                        answer += "*";
                    }
                    answer += "q" + state.Value + "\n";
                }
            }
            return answer;
        }

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

        public static List<State> ConvertToDFA(Dictionary<string, Dictionary<string, List<string>>> NFA, string InitState, HashSet<string> FinalState, string[] alphabet)
        {
            var initstate = new State("", new Dictionary<string, int>(), new List<string>() { InitState });
            var states = new List<State>();
            states.Add(initstate);
            
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                for (int j = 0; j < state.States.Count; j++)
                {
                    if (NFA[state.States[j]].ContainsKey("_"))
                    {
                        List<string> s = new List<string>();
                        s.AddRange(NFA[state.States[j]]["_"]);
                        for (int k = 0; k < s.Count; k++)
                        {
                            if(!state.States.Contains(s[k]))
                                state.States.Add(s[k]);
                        }
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
                                List<string> nv = new List<string>();
                                nv.AddRange(ns.Value);
                                for (int j = 0; j < nv.Count; j++)
                                {
                                    if (!nextStates[ns.Key].States.Contains(nv[j]))
                                    {
                                        nextStates[ns.Key].States.Add(nv[j]);
                                    }
                                }
                            }
                            else
                            {
                                nextStates.Add(ns.Key, new State("", new Dictionary<string, int>(), new List<string>()));
                                nextStates[ns.Key].States.AddRange(ns.Value);
                            }
                        }
                    }
                }

                foreach (var ns in nextStates)
                {
                    var st = new State(null, null, null);
                    st = ns.Value;
                    for (int j = 0; j < st.States.Count; j++)
                    {
                        if (NFA[st.States[j]].ContainsKey("_"))
                        {
                            List<string> s = new List<string>();
                            s.AddRange(NFA[st.States[j]]["_"]);
                            for (int k = 0; k < s.Count; k++)
                            {
                                if (!st.States.Contains(s[k]))
                                    st.States.Add(s[k]);
                            }
                        }
                    }

                    var index = StateExists(states, st);
                    state.NextStates.Add(ns.Key, index);
                }
            }

            
            bool needTrap = false;
            for(int i = 0; i < states.Count; i++)
            {
                foreach(var a in alphabet)
                {
                    if(!states[i].NextStates.ContainsKey(a))
                    {
                        states[i].NextStates.Add(a, states.Count);
                        needTrap = true;
                    }
                }
            }
            if (needTrap)
            {
                states.Add(new State("", new Dictionary<string, int>(), new List<string>()));
                var last = states.Count - 1;
                foreach (var a in alphabet)
                {
                    states[last].NextStates.Add(a, last);
                }
            }
            return states;
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
                        if(!states[i].States.Contains(state.States[j]))
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

        public static HashSet<int> FinalStates(List<State> DFA, HashSet<string> nfaFinalState)
        {
            HashSet<int> dfaFinalStates = new HashSet<int>();
            for(int i = 0; i < DFA.Count; i++)
            {
                foreach(var state in DFA[i].States)
                {
                    if(nfaFinalState.Contains(state))
                    {
                        dfaFinalStates.Add(i);
                        break;
                    }
                }
            }
            return dfaFinalStates;
        }

        public static Tuple< List<Dictionary<string, int>>, int[]> DFAMinimizer (List<State> DFA, HashSet<int> dfaFinalStates,string[] Alphabet)
        {
            var sets = new List<List<int>>() { new List<int>()};
            //create 2 sets for final states and nonfinal states
            for(int i = 0; i < DFA.Count; i++)
            {
                if(dfaFinalStates.Contains(i))
                {
                    sets[0].Add(i);
                }
                else if(sets.Count == 1)
                {
                    sets.Add(new List<int>() { i });
                }
                else
                {
                    sets[1].Add(i);
                }
            }

            var dfaTable = ConvertDFAtoTable(DFA);
            var setsTable = new List<Dictionary<string, int>>();
            var stateInSet = new int[DFA.Count];
            while (true)
            {
                setsTable = new List<Dictionary<string, int>>();
            //defines each state with each alphabet goes relates to which set
                for (int i = 0; i < dfaTable.Count; i++)
                {
                    setsTable.Add(new Dictionary<string, int>());
                    foreach (var a in Alphabet)
                    {
                        for (int j = 0; j < sets.Count; j++)
                        {
                            if (sets[j].Contains(dfaTable[i][a]))
                            {
                                setsTable[i].Add(a, j);
                                break;
                            }
                        }
                    }
                }
                var newSets = new List<List<int>>();
                //makes new sets out of old sets
                for (int i = 0; i < sets.Count; i++)
                {
                    newSets.Add(new List<int>() { sets[i][0] });
                    stateInSet[sets[i][0]] = newSets.Count - 1;
                    for (int j = 1; j < sets[i].Count; j++)
                    {
                        bool isMatched = true;
                        for (int k = j - 1; k >= 0; k--)
                        {
                            isMatched = true;
                            foreach(var a in Alphabet)
                            {
                                if (setsTable[sets[i][j]][a] != setsTable[sets[i][k]][a])
                                {
                                    isMatched = false;
                                    break;
                                }
                            }
                            if (isMatched)
                            {
                                newSets[stateInSet[sets[i][k]]].Add(sets[i][j]);
                                stateInSet[sets[i][j]] = stateInSet[sets[i][k]];
                                break;
                            }
                        }
                        if (!isMatched)
                        {
                            newSets.Add(new List<int>() { sets[i][j] });
                            stateInSet[sets[i][j]] = newSets.Count - 1;
                        }
                    }
                }

                if(sets.Count == newSets.Count)
                {
                    break;
                }

                sets = newSets;
            }

            for(int i = 0; i < sets.Count; i++)
            {
                for(int j = 1; j < sets[i].Count; j++)
                {
                    setsTable.RemoveAt(sets[i][j]);
                }
            }

            return new Tuple<List<Dictionary<string, int>>, int[]>(setsTable,stateInSet);
        }
        
        public static List<Dictionary<string, int>> ConvertDFAtoTable(List<State> DFA)
        {
            List<Dictionary<string, int>> dfaTable = new List<Dictionary<string, int>>();
            for(int i = 0; i < DFA.Count; i++)
            {
                dfaTable.Add(new Dictionary<string, int>());
                foreach(var a in DFA[i].NextStates)
                {
                    dfaTable[i].Add(a.Key, a.Value);
                }
            }
            return dfaTable;
        }
    }
}