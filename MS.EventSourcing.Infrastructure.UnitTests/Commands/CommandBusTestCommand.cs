namespace MS.EventSourcing.Infrastructure.UnitTests.Commands
{
    public class CommandBusTestCommand
    {
        public string Name { get; set; }
        public bool ShouldThrowException { get; set; }
    }
}