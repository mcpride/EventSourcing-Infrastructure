namespace MS.EventSourcing.Infrastructure.EF.MySql.IntegrationTests.Snaphots
{
    public class Customer
    {
        private readonly CustomerAddress _address = new CustomerAddress();
        public string Name { get; set; }

        public CustomerAddress Address
        {
            get { return _address; }
        }
    }
}
