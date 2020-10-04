using System;
using EnderPi.Framework.Messaging;
using System.Collections.Generic;

namespace EnderPi.Framework.Interfaces
{

    /// <summary>
    /// The interface for the event manager.
    /// </summary>
    public interface IEventManager
    {
        /// <summary>
        /// Send messages of type T to this delegate.
        /// </summary>
        /// <typeparam name="T">The type of event to subscribe to</typeparam>
        /// <param name="eventProcessor">The delegate that will be called with the event when it occurs.</param>
        /// <remarks>
        /// Probably shouldn't subscribe an anonymous delegate to an event, as this will make cancelling the subscription difficult.
        /// </remarks>
        void Subscribe<T>(Action<T> eventProcessor) where T : EventMessage;

        /// <summary>
        /// Stop sending messages of type T to this delegate.
        /// </summary>
        /// <typeparam name="T">The type of event to unsubscribe from.</typeparam>
        /// <param name="eventProcessor">The delegate to stop receiving events.</param>
        void Unsubscribe<T>(Action<T> eventProcessor) where T : EventMessage;

        /// <summary>
        /// Publish this event message for every subscriber, at the given priority.
        /// </summary>
        /// <param name="eventMessage">The event message to publish,</param>
        /// <param name="priority">The priority to publish at.</param>
        /// <remarks>
        /// Most messages should probably be published at normal priority, high should be reserved for cache invalidation events
        /// and other such events where processing in a timely fashion is beneficial.
        /// </remarks>
        void PublishEvent(EventMessage eventMessage, MessagePriority priority);


        /// <summary>
        /// Publish this event message for every subscriber, at normal priority.
        /// </summary>
        /// <param name="eventMessage">The event message to publish,</param>        
        void PublishEvent(EventMessage eventMessage);

        /// <summary>
        /// Deserializes a string into an event message.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        EventMessage DeserializeEventMessage(string xml);

        /// <summary>
        /// Gets all the application subscriptions.  Might not be bad to split data access out into a separate class, but it's OK for now.
        /// </summary>
        /// <returns></returns>
        List<ApplicationSubscription> GetAllSubscriptions();

    }

}
