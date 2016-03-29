using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using RemoteMeasure.Common.Messages;

namespace RemoteMeasure.MeasureService.Actors
{
    public class UnreceivedMessagesActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();
        private readonly IList<DeadLetter> _unsendMessages = new List<DeadLetter>();
        private bool _isConnected = false;

        public UnreceivedMessagesActor()
        {
            Receive<DeadLetter>(DeadLetterHandler, x => x.Message is MeasureData);
            Receive<CheckUnreadMessages>(x => ResendMessages());
            Receive<SendSuccess>(data => _isConnected = true);
        }

        protected override void PreStart()
        {
            // Subscribe to receive Dead Letter notification
            Context.System.EventStream.Subscribe(Self, typeof(DeadLetter));
            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromSeconds(15),
                Self,
                new CheckUnreadMessages(),
                Self);
        }

        private void DeadLetterHandler(DeadLetter letter)
        {
            _log.Debug("Receive Dead Letter");
            _isConnected = false;
            _unsendMessages.Add(letter);
        }

        private void ResendMessages()
        {
            if (_isConnected && _unsendMessages.Count > 0)
            {
                foreach (DeadLetter deadLetter in _unsendMessages)
                {
                    deadLetter.Sender.Tell(deadLetter.Message);
                }
                _unsendMessages.Clear();
            }
        }

        private class CheckUnreadMessages { }
    }
}
