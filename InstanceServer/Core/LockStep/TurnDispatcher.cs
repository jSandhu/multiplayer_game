using System;
using System.Timers;

namespace InstanceServer.Core.LockStep
{
    /// <summary>
    /// Provides timer based events (used mainly as a game loop event provider).
    /// 
    /// Uses lock to prevent undesired race conditions between
    /// timer events.  Example: When debugger is halted at a break point System.Timers.Timer will continue
    /// to fire onTimerElapsed(...), causing unintended effects.
    /// </summary>
    public class TurnDispatcher
    {
        private readonly object _lock = new object();

        public event Action Updated;
        public event Action PostUpdateStarted;

        public int TurnNumber;

        private Timer timer;
        private bool isDispatching;

        public TurnDispatcher(int dispatchFrequencyMS)
        {
            timer = new Timer(dispatchFrequencyMS);
        }

        public void Start()
        {
            lock (_lock)
            {
                timer.Elapsed += onTimerElapsed;
                timer.Start();
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                timer.Stop();
                timer.Elapsed -= onTimerElapsed;
            }
        }

        private void onTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // lock to make sure another timer tick doesn't cause another
            // dispatch event to occur before the current one is completed.
            lock (_lock)
            {
                TurnNumber++;

                if (isDispatching) return;

                if (Updated == null)
                {
                    return;
                }

                isDispatching = true;
            }

            if (Updated != null)
            {
                Updated();
            }

            if (PostUpdateStarted != null)
            {
                PostUpdateStarted();
            }

            isDispatching = false;
        }
    }
}
