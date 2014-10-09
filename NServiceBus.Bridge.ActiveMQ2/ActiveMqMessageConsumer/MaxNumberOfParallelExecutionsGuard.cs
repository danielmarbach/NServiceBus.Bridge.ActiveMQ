using System;
using System.Threading;

namespace NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer
{
    public class MaxNumberOfParallelExecutionsGuard
    {
        private readonly int maxNumberOfRunningActions;

        private int currentNumberOfRunningActions;

        public MaxNumberOfParallelExecutionsGuard(int maxNumberOfRunningActions)
        {
            this.maxNumberOfRunningActions = maxNumberOfRunningActions;
        }

        public void Process(Action action)
        {
            try
            {
                var value = Interlocked.Increment(ref this.currentNumberOfRunningActions);
                if (value <= this.maxNumberOfRunningActions)
                {
                    action();
                }
            }
            finally
            {
                Interlocked.Decrement(ref this.currentNumberOfRunningActions);
            }
        }
    }
}