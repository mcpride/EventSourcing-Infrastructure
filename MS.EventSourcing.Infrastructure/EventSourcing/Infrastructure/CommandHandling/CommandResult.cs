using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MS.EventSourcing.Infrastructure.CommandHandling
{
    public class CommandResult : ICommandResult
    {
        private bool _success;
        private bool _cancelled;

        public CommandResult()
        {
            Success = true;
            Errors = new List<string>();
            ErrorStackTrace = new List<string>();
            TimeStamp = DateTime.Now;
        }

        public CommandResult(string error) : this()
        {
            Success = false;
            Errors.Add(error);
        }

        public CommandResult(bool cancelled)
            : this()
        {
            Success = false;
            Cancelled = cancelled;
        }

        public CommandResult(bool cancelled, string error)
            : this()
        {
            Success = false;
            Cancelled = cancelled;
            Errors.Add(error);
        }

        public static CommandResult Successful
        {
            get
            {
                return new CommandResult();
            }
        }

        [DefaultValue(true)]
        public bool Success
        {
            get { return _success; }
            set
            {
                _success = value;
                if (value) _cancelled = false;
            }
        }

        [DefaultValue(false)]
        public bool Cancelled
        {
            get { return _cancelled; }
            set
            {
                _cancelled = value;
                if (value) _success = false;
            }
        }

        [DefaultValue(null)]
        public string ErrorType { get; set; }

        public DateTime TimeStamp { get; set; }

        public List<string> Errors { get; set; }

        public List<string> ErrorStackTrace { get; set; }
    }
}
