using System;
using System.IO;
using Akka.Actor;
using Akka.Event;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console.
    /// Also responsible for calling <see cref="ActorSystem.Shutdown"/>.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {
        public const string StartCommand = "start";
        public const string ExitCommand = "exit";
        private readonly ILoggingAdapter log = Context.GetLogger();

        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
            {
                log.Info("Start Command seen");
                DoPrintInstructions();
            }

            GetAndValidateInput();


        }


        #region Internal methods
        private void DoPrintInstructions()
        {
            Console.WriteLine("Please provide the URI of a log file on disk.\r\n");
        }


        /// <summary>
        /// Reads input from console, validates it, then signals appropriate response
        /// (continue processing, error, success, etc.).
        /// </summary>
        private void GetAndValidateInput()
        {
            var message = Console.ReadLine();
            if (!string.IsNullOrEmpty(message) && String.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                // if user typed ExitCommand, shut down the entire actor system (allows the process to exit)
                Context.System.Shutdown();
                return;
            }

            // otherwise, just send the message off for validation
            Context.ActorSelection("akka://MyActorSystem/user/validationActor").Tell(message);
        }
        #endregion
    }
}