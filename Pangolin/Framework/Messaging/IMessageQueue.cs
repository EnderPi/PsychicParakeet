namespace EnderPi.Framework.Messaging
{
    interface IMessageQueue
    {
        void SendMessage(Message message);
        Message GetNextMessage();
        Message PeekMessage();
        long Count();
    }
}
