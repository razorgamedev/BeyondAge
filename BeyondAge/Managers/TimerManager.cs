using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Managers
{
    class Timer
    {
        public Action Callback;
        public float Time;
    }

    class TimerManager
    {
        private static TimerManager self;
        private List<Timer> timers;

        private TimerManager()
        {
            timers = new List<Timer>();
        }

        public Timer AddTimer(Timer timer)
        {
            timers.Add(timer);
            return timer;
        }

        public void Update(GameTime time)
        {
            for (int i = timers.Count - 1; i >= 0; i--)
            {
                var timer = timers[i];
                timer.Time -= (float)time.ElapsedGameTime.TotalSeconds;
                if (timer.Time <= 0)
                {
                    timer?.Callback();
                    timers.Remove(timer);
                }
            }
        }

        public static TimerManager Self {
            get { 
                if (self == null) self = new TimerManager();
                return self;
            }
        }
    }
}
