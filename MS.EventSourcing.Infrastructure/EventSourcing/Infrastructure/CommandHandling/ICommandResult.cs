using System.Collections.Generic;

namespace MS.EventSourcing.Infrastructure.CommandHandling
{
    public interface ICommandResult
    {
        bool Success { get; set; }
        List<string> Errors { get; }
    }
}