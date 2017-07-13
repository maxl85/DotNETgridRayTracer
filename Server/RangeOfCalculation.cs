using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Server
{
    public class RangeOfCalculation
    {
        public int start;
        public int stop;
        public string adress;
        public CurrentState state;
        public RangeOfCalculation(int Start, int Stop ,CurrentState State)
        {
            start = Start;
            stop = Stop;
            state = State;
        }
        public enum CurrentState
        {
            notComputing,
            computing,
            completing,
        };
        //public void Timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    this.state = CurrentState.notComputing;
        //}
    }
}
