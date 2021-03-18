using EnderPi.Framework.BackgroundWorker;
using EnderPi.Framework.Messaging;
using EnderPi.Framework.Services;
using System;
using System.Threading;

namespace EnderPi.Framework.Simulation
{
    /// <summary>
    /// Simple simulation
    /// </summary>
    public class TestSimulation : LongRunningTask
    {
        protected override void InitializeInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            var messageQueue = provider.GetService<IMessageQueue>();
            messageQueue.SendMessage(new Message(1, "INITIALIZE", DateTime.Now, MessagePriority.Normal));
        }

        protected override void StartInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            var messageQueue = provider.GetService<IMessageQueue>();
            messageQueue.SendMessage(new Message(1, "START", DateTime.Now, MessagePriority.Normal));
        }

        protected override void StoreFinalResults(ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            var messageQueue = provider.GetService<IMessageQueue>();
            messageQueue.SendMessage(new Message(1, "STORE", DateTime.Now, MessagePriority.Normal));
        }
    }
}
