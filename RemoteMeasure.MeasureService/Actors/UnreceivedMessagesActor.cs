using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using RemoteMeasure.Common.Messages;

namespace RemoteMeasure.MeasureService.Actors
{
    public class UnreceivedMessagesActor : ReceivePersistentActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();
        private readonly List<DeadLetter> _unsendMessages = new List<DeadLetter>();
        private bool _isConnected = false;

        public UnreceivedMessagesActor()
        {
            Command<DeadLetter>(DeadLetterHandler, x => x.Message is MeasureData);
            Command<CheckUnreadMessages>(x => ResendMessages());
            Command<SendSuccess>(data => _isConnected = true);

            Recover<SnapshotOffer>(offer =>
            {
                var snapshot = offer.Snapshot as List<DeadLetter>;
                _unsendMessages.AddRange(snapshot);
            });
            Command<SaveSnapshotSuccess>(success =>
            {
                // delete all but the most current snapshot
                var criteria = new SnapshotSelectionCriteria(
                    success.Metadata.SequenceNr,
                    success.Metadata.Timestamp.AddMilliseconds(-1));
                DeleteSnapshots(criteria);
            });
        }

        public override string PersistenceId
        {
            get { return "UnreceivedMessages"; }
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

            base.PreStart();
        }

        private void DeadLetterHandler(DeadLetter letter)
        {
            _log.Debug("Receive Dead Letter");
            _isConnected = false;
            _unsendMessages.Add(letter);
            if (_unsendMessages.Count % 5 == 0)
            {
                SaveSnapshot(_unsendMessages);
            }
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
                SaveSnapshot(_unsendMessages);
            }
        }

        private class CheckUnreadMessages { }
    }
}
