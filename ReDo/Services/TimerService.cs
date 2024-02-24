using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReDo.Services
{
    public class TimerService
    {
        private Stopwatch stopwatch;
        public TimerService()
        {
            this.stopwatch = new Stopwatch();
        }

        public void StartTimer()
        {
            stopwatch.Start();
        }

        public TimeSpan StopTimer()
        {
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public TimeSpan? ToggleTimerState()
        {

            if (!stopwatch.IsRunning)
            {
                StartTimer();
                return null;
            }
            else
            {
                return StopTimer();
            }
        }
    }


}
