using System;
using System.Collections.Generic;
using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;
using NServiceBus.Config;

namespace NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer
{
    public class ActiveMqListener
    {
        private readonly List<string> queues;

        private readonly IConnectionFactory connectionFactory;

        private readonly ActiveMqExceptionHandler messageProcessor;

        private readonly string errorQueue;

        public ActiveMqListener(
            IConnectionFactory connectionFactory, 
            ActiveMqExceptionHandler messageProcessor,
            string replyToQueue,
            string errorQueue)
        {
            this.connectionFactory = connectionFactory;
            this.messageProcessor = messageProcessor;
            this.errorQueue = errorQueue;

            this.queues = new List<string> { replyToQueue };
            this.AddQueuesFromEventsComingFromActiveMQ();
            this.AddQueuesFromEndpointMappingsConfiguration();
        }

        public void Run()
        {
            using (var connection = this.connectionFactory.CreateConnection())
            using (var session = connection.CreateSession())
            using (var errorMessageProducer = session.CreateProducer(new ActiveMQQueue(this.errorQueue)))
            {
                connection.Start();
                foreach (var queue in this.queues)
                {
                    using (var consumer = session.CreateConsumer(session.GetQueue(queue)))
                    {
                        Apache.NMS.IMessage amqMessage;
                        while ((amqMessage = consumer.Receive(TimeSpan.FromMilliseconds(50))) != null)
                        {
                            this.messageProcessor.ProcessMessage(amqMessage, errorMessageProducer);
                            amqMessage.Acknowledge();
                        }
                    }
                }
            }
        }
    
        private void AddQueuesFromEventsComingFromActiveMQ()
        {
            ////foreach (var type in typeof(ActiveMQ1ContractType).Assembly.GetTypes().Union(typeof(ActiveMQ2ContractType).Assembly.GetTypes())
            ////    .Where(t => true /* again your conventions */))
            ////{
            ////    this.queues.Add("Consumer.activeMQBridge.VirtualTopic." + type.FullName);
            ////}
        }

        private void AddQueuesFromEndpointMappingsConfiguration()
        {
            foreach (MessageEndpointMapping mapping in
                Configure.ConfigurationSource.GetConfiguration<UnicastBusConfig>().MessageEndpointMappings)
            {
                if (mapping.Endpoint.StartsWith("someprefix.", StringComparison.Ordinal))
                {
                    this.queues.Add(mapping.Endpoint.Split('@')[0]);
                }
            }
        }
    }
}