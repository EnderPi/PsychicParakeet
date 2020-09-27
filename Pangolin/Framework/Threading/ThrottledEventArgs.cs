using System;

namespace EnderPi.Framework.Threading
{
    /// <summary>
    /// Throttled message listener event POCO
    /// </summary>
    public class ThrottledEventArgs<T> where T : class
    {
        public ThrottledEventArgs(T message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }

        public ThrottledEventArgs(T message)
        {
            Message = message;
        }

        public ThrottledEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public ThrottledEventArgs()
        {
        }

        public T Message { set; get; }
        public Exception Exception { set; get; }
    }
}
