using System;

namespace NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer
{
    public class ActiveMqListenerTaskScheduler : IWantToRunWhenBusStartsAndStops
    {
        private readonly MaxNumberOfParallelExecutionsGuard maxNumberOfParallelExecutionsGuard;

        private readonly ActiveMqListener activeMqListener;

        private readonly int pollingInterval;

        public ActiveMqListenerTaskScheduler(
            ActiveMqListener activeMqListener,
            MaxNumberOfParallelExecutionsGuard maxNumberOfParallelExecutionsGuard, 
            int pollingInterval)
        {
            this.activeMqListener = activeMqListener;
            this.maxNumberOfParallelExecutionsGuard = maxNumberOfParallelExecutionsGuard;
            this.pollingInterval = pollingInterval;
        }

        public void Start()
        {
            Schedule.Every(TimeSpan.FromMilliseconds(this.pollingInterval)).Action(this.Run);
        }

        public void Stop()
        {
        }

        private void Run()
        {
            this.maxNumberOfParallelExecutionsGuard.Process(() => this.activeMqListener.Run());
        }
    }
}