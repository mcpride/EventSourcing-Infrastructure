using System;

namespace MS.EventSourcing.Infrastructure.CommandHandling
{
    public static class CommandResultExtensionMethods
    {
        public static CommandResult ToCommandResult(this Exception ex)
        {
            if (ex == null) return new CommandResult{ Success = false };

            var result = new CommandResult(ex.Message) { ErrorType = ex.GetType().FullName };

            var innerEx = ex.InnerException;
            while (innerEx != null)
            {
                result.Errors.Add(string.Format("{0}: {1}", innerEx.GetType().FullName, innerEx.Message));
                innerEx = innerEx.InnerException;
            }

            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                result.ErrorStackTrace.Add(ex.StackTrace);
            }

            return result;
        }
    }
}
