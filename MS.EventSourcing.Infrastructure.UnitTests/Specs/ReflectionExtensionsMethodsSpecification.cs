using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MS.Infrastructure;
using MS.QualityTools.UnitTestFramework.Specifications;
using MS.EventSourcing.Infrastructure.UnitTests.Events;

namespace MS.EventSourcing.Infrastructure.UnitTests.Specs
{
    public class GenObject
    {
        public object Obj;

        public void Apply<T>(T obj)
        {
            Obj = obj;
        }
    }
    
    [TestClass]
    [SpecificationDescription("ReflectionExtensionsMethods specification")]
    public class ReflectionExtensionsMethodsSpecification : Specification
    {
        [TestMethod]
        [ScenarioDescription("InvokeGenericMethod should call the specified generic method of an object without errors")]
        public void InvokeGenericMethod_should_call_the_specified_generic_method_of_an_object_without_errors()
        {
            Given("An object with a generic method", context => context.State.GenObject = new GenObject())
            .Then("InvokeGenericMethod with the name of the generic method should be callable without errors",
                context =>
                {
                    var genObject = (GenObject)(context.State.GenObject);
                    // ReSharper disable RedundantCast
                    ((GenObject)genObject)
                        .InvokeGenericMethod("Apply", typeof(CustomerCreated), new CustomerCreated());
                    // ReSharper restore RedundantCast
                    genObject.Obj.Should().BeOfType<CustomerCreated>();
                });

        }
    }
}
