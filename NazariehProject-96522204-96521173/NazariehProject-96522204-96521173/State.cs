using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NazariehProject_96522204_96521173
{
    public class State
    {
        public string Name { get; set; }
        public Dictionary<string, int> NextStates;
        public List<string> States { get; set; }
        
        public State(string name, Dictionary<string, int> next, List<string> states)
        {
            Name = name;
            NextStates = next;
            States = states;
        }
    }
}
