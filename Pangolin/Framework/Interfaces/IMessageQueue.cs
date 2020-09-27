namespace EnderPi.Framework.Messaging
{

    /// <summary>
    /// Interface for a basic message queue.
    /// </summary>
    public interface IMessageQueue
    {
        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="message"></param>
        void SendMessage(Message message);

        /// <summary>
        /// Pulls the next message off the queue
        /// </summary>
        /// <returns></returns>
        Message GetNextMessage();

        /// <summary>
        /// Retrieves a copy of the next message on the queue.
        /// </summary>
        /// <returns></returns>
        Message PeekMessage();

        /// <summary>
        /// How many messages are in the queue?
        /// </summary>
        /// <returns></returns>
        long Count();

        /// <summary>
        /// What is the name of the queue?
        /// </summary>
        string Name { get; }
    }
}
