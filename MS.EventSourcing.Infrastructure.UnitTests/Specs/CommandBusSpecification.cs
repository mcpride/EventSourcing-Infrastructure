using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MS.QualityTools.UnitTestFramework.Specifications;
using MS.EventSourcing.Infrastructure.CommandHandling;
using MS.EventSourcing.Infrastructure.MassTransit;
using MS.EventSourcing.Infrastructure.UnitTests.Commands;
using MS.EventSourcing.Infrastructure.UnitTests.Handlers;

namespace MS.EventSourcing.Infrastructure.UnitTests.Specs
{
    [TestClass]
    [SpecificationDescription("Command bus specification")]
    public class CommandBusSpecification : Specification
    {
        [TestMethod]
        [ScenarioDescription("Create and initialize command bus with loopback")]        
        public void Create_and_initialize_command_bus_with_loopback()
        {
            Given("a command bus", testContext => testContext.State.CommandBus = new CommandBus())
            .Then("the command bus should be initializable", testContext =>
            {
                var commandBus = (CommandBus) testContext.State.CommandBus;
                commandBus.Initialize(null, sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue")));
            });
        }

        [TestMethod]
        [ScenarioDescription("Dispose created command bus")]
        public void Dispose_created_command_bus()
        {
            CommandBus commandBus = null;
            Given("a created command bus", testContext => commandBus = new CommandBus())
            .Then("the command bus should be disposable", testContext => commandBus.Dispose());
        }

        [TestMethod]
        [ScenarioDescription("Dispose initialized command bus")]
        public void Dispose_initialized_command_bus()
        {
            CommandBus commandBus = null;
            Given("a created command bus", testContext => commandBus = new CommandBus())
            .And("the command bus is initialized", testContext => commandBus.Initialize(null, sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue"))))
            .Then("the command bus should be disposable", testContext => commandBus.Dispose());
        }

        [TestMethod]
        [ScenarioDescription("Initialize command bus with service bus")]
        public void Iinitialize_command_bus_with_ServiceBus()
        {
            Given("a command bus", testContext => testContext.State.CommandBus = new CommandBus())
            .And("another service bus", testContext => testContext.State.ServiceBus = ServiceBusFactory.New(sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue"))))
            .Then("the command bus should be initializable with this another service bus", testContext =>
            {
                var commandBus = (CommandBus)testContext.State.CommandBus;
                var serviceBus = (IServiceBus)testContext.State.ServiceBus;
                commandBus.InitializeWithServiceBus(serviceBus);
            });
        }

        [TestMethod]
        [ScenarioDescription("Initialize command bus with unassigned service bus parameter throws error")]
        public void Initialize_command_bus_with_unassigned_ServiceBus_parameter_throws_error()
        {
            Action action = null;
            CommandBus commandBus = null;

            Given("a command bus", testContext => commandBus = new CommandBus())
            .When("the command bus will be initialized with unassigned service bus parameter", testContext => action = () => commandBus.InitializeWithServiceBus(null))
            .Then("the command bus should throw an ArgumentNullException", testContext => action.ShouldThrow<ArgumentNullException>());
        }

        [TestMethod]
        [ScenarioDescription("Initialize command bus with MSMQ and unassigned queue name parameter throws error")]
        public void Initialize_command_bus_with_MSMQ_and_unassigned_ServiceBus_parameter_throws_error()
        {
            Action action = null;
            CommandBus commandBus = null;

            Given("a command bus", testContext => commandBus = new CommandBus())
            .When("the command bus will be initialized with MSMQ and unassigned queue name parameter", testContext => action = () => commandBus.InitializeMsmq(null))
            .Then("the command bus should throw an ArgumentNullException", testContext => action.ShouldThrow<ArgumentNullException>());
        }

        [TestMethod]
        [ScenarioDescription("Initialize command bus with RabbitMQ and unassigned queue name parameter throws error")]
        public void Initialize_command_bus_with_RabbitMQ_and_unassigned_ServiceBus_parameter_throws_error()
        {
            Action action = null;
            CommandBus commandBus = null;

            Given("a command bus", testContext => commandBus = new CommandBus())
            .When("the command bus will be initialized with RabbitMQ and unassigned queue name parameter", testContext => action = () => commandBus.InitializeRabbitMq(null, null, null, null))
            .Then("the command bus should throw an ArgumentNullException", testContext => action.ShouldThrow<ArgumentNullException>());
        }

        [TestMethod]
        [ScenarioDescription("Command bus initialized twice just returns the service bus instance")]
        public void Command_bus_initialized_twice_just_returns_the_service_bus_instance()
        {
            Given("a command bus", testContext => testContext.State.CommandBus = new CommandBus())
            .And("the command bus is initialized", testContext =>
            {
                var commandBus = (CommandBus)testContext.State.CommandBus;
                testContext.State.ServiceBus1 = commandBus.Initialize(null, sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue")));
            })
            .When("the command bus is initialized a second time", testContext =>
            {
                var commandBus = (CommandBus)testContext.State.CommandBus;
                testContext.State.ServiceBus2 = commandBus.Initialize(null, sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/otherqueue")));
            })
            .Then("just the existing service bus instance should be returned", testContext =>
            {
                var serviceBus1 = (IServiceBus)testContext.State.ServiceBus1;
                var serviceBus2 = (IServiceBus)testContext.State.ServiceBus2;
                serviceBus2.Should().NotBeNull();
                serviceBus2.Should().BeSameAs(serviceBus1);
            });
        }

        [TestMethod]
        [ScenarioDescription("Initialized command bus without consumers should cancel sending command after a given time")]
        public void Initialized_command_bus_without_consumers_should_cancel_sending_command_after_a_given_time()
        {
            CommandResult cmdResult = null;
            var taskCancelled = false;
            CommandBus commandBus = null;
            Task<CommandResult> sendTask;

            Given("a command bus", testContext => commandBus = new CommandBus())
                .And("the command bus is initialized with loopback",
                    testContext =>
                        commandBus.Initialize(null,
                            sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue")))
                        //commandBus.InitializeMsmq(
                            //"test_commandbus_cancel_sending")
                )
                .And("the command bus has no consumers", testContext => { })
                .When("A command will be send over command bus asynchonously with timeout", testContext =>
                {
                    sendTask = commandBus.SendAsyncWithTimeout(new CommandBusTestCommand {Name = "Test"},
                        result =>
                        {
                            cmdResult = result;
                        }, TimeSpan.FromSeconds(2));
                    sendTask.ContinueWith(task =>
                    {
                        taskCancelled = true;
                    }, TaskContinuationOptions.OnlyOnCanceled);
                })
                .Then("the send process should be cancelled", testContext =>
                {
                    for (var i = 0; i < 30; i++)
                    {
                        if (cmdResult != null) break;
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(500);
                    cmdResult.Should().NotBeNull();
                    cmdResult.Cancelled.Should().BeTrue();
                })
                .And("the result shouldn't be succesful", testContext =>
                    cmdResult.Success.Should().BeFalse())
                .And("the result should contain an error message", testContext =>
                {
                    cmdResult.Errors.Should().NotBeEmpty();
                    foreach (var error in cmdResult.Errors)
                    {
                        Console.WriteLine(error);
                    }
                })
                .And("the task should be marked as cancelled", testContext =>
                    taskCancelled.Should().BeTrue())
            .And("the results error type should be timout exception", testContext =>
                cmdResult.ErrorType.Should().BeEquivalentTo(typeof (TimeoutException).FullName));
        }

        [TestMethod]
        [ScenarioDescription("Sending command asynchronously over command bus with consumer receives a successful command result")]
        public void Sending_command_asynchronously_over_command_bus_with_consumer_receives_a_successful_command_result()
        {
            CommandResult cmdResult = null;
            var taskCancelled = false;
            CommandBus commandBus = null;
            Task<CommandResult> sendTask;

            Given("a command bus", testContext => commandBus = new CommandBus())
                .And("the command bus is initialized with loopback",
                    testContext =>
                        commandBus.Initialize(ssbsc => ssbsc.Consumer(() => new CommandBusTestConsumer()).Permanent(),
                            sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue")))
                )
                .And("the command bus has a valid consumer", testContext => { })
                .When("A command will be send over command bus asynchonously", testContext =>
                {
                    sendTask = commandBus.SendAsync(new CommandBusTestCommand { Name = "Test" },
                        result =>
                        {
                            cmdResult = result;
                        });
                    sendTask.ContinueWith(task =>
                    {
                        taskCancelled = true;
                    }, TaskContinuationOptions.OnlyOnCanceled);
                })
                .Then("the result should be successful", testContext =>
                {
                    for (var i = 0; i < 30; i++)
                    {
                        if (cmdResult != null) break;
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(500);
                    cmdResult.Should().NotBeNull();
                    cmdResult.Success.Should().BeTrue();
                })
                .And("the result should not contain an error message",
                    testContext => cmdResult.Errors.Should().BeEmpty())
                .And("the task shouldn't be marked as cancelled", testContext =>
                    taskCancelled.Should().BeFalse());
        }

        [TestMethod]
        [ScenarioDescription("Sending command asynchronously over command bus interface with consumer receives a successful command result")]
        public void Sending_command_asynchronously_over_command_bus_interface_with_consumer_receives_a_successful_command_result()
        {
            CommandResult cmdResult = null;
            var taskCancelled = false;
            CommandBus commandBus = null;
            Task<CommandResult> sendTask;

            Given("a command bus", testContext => commandBus = new CommandBus())
                .And("the command bus is initialized with loopback",
                    testContext =>
                        commandBus.Initialize(ssbsc => ssbsc.Consumer(() => new CommandBusTestConsumer()).Permanent(),
                            sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue")))
                )
                .And("the command bus has a valid consumer", testContext => { })
                .When("A command will be send over command bus asynchonously", testContext =>
                {
                    sendTask = ((MassTransit.ICommandBus)commandBus).Send(new CommandBusTestCommand { Name = "Test" },
                        result =>
                        {
                            cmdResult = result;
                        });
                    sendTask.ContinueWith(task =>
                    {
                        taskCancelled = true;
                    }, TaskContinuationOptions.OnlyOnCanceled);
                })
                .Then("the result should be successful", testContext =>
                {
                    for (var i = 0; i < 30; i++)
                    {
                        if (cmdResult != null) break;
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(500);
                    cmdResult.Should().NotBeNull();
                    cmdResult.Success.Should().BeTrue();
                })
                .And("the result should not contain an error message",
                    testContext => cmdResult.Errors.Should().BeEmpty())
                .And("the task shouldn't be marked as cancelled", testContext =>
                    taskCancelled.Should().BeFalse());
        }

        [TestMethod]
        [ScenarioDescription("Sending command asynchronously with timeout over command bus interface with consumer receives a successful command result")]
        public void Sending_command_asynchronously_with_timeout_over_command_bus_interface_with_consumer_receives_a_successful_command_result()
        {
            CommandResult cmdResult = null;
            var taskCancelled = false;
            CommandBus commandBus = null;
            Task<CommandResult> sendTask;

            Given("a command bus", testContext => commandBus = new CommandBus())
                .And("the command bus is initialized with loopback",
                    testContext =>
                        commandBus.Initialize(ssbsc => ssbsc.Consumer(() => new CommandBusTestConsumer()).Permanent(),
                            sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue")))
                )
                .And("the command bus has a valid consumer", testContext => { })
                .When("A command will be send over command bus asynchonously with timeout", testContext =>
                {
                    sendTask = ((MassTransit.ICommandBus)commandBus).Send(new CommandBusTestCommand { Name = "Test" },
                        result =>
                        {
                            cmdResult = result;
                        }, 5.Seconds());
                    sendTask.ContinueWith(task =>
                    {
                        taskCancelled = true;
                    }, TaskContinuationOptions.OnlyOnCanceled);
                })
                .Then("the result should be successful", testContext =>
                {
                    for (var i = 0; i < 50; i++)
                    {
                        if (cmdResult != null) break;
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(500);
                    cmdResult.Should().NotBeNull();
                    cmdResult.Success.Should().BeTrue();
                })
                .And("the result should not contain an error message",
                    testContext => cmdResult.Errors.Should().BeEmpty())
                .And("the task shouldn't be marked as cancelled", testContext =>
                    taskCancelled.Should().BeFalse());
        }

        [TestMethod]
        [ScenarioDescription("Sending anonymous command asynchronously over command bus with consumer receives a successful command result")]
        public void Sending_anonymous_command_asynchronously_over_command_bus_with_consumer_receives_a_successful_command_result()
        {
            CommandResult cmdResult = null;
            var taskCancelled = false;
            TestCommandBus commandBus = null;
            Task<CommandResult> sendTask;

            Given("a command bus", testContext => commandBus = new TestCommandBus())
                .And("the command bus is initialized with loopback",
                    testContext =>
                        commandBus.Initialize(ssbsc => ssbsc.Consumer(() => new CommandBusTestConsumer()).Permanent(),
                            sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue")))
                )
                .And("the command bus has a valid consumer", testContext => { })
                .When("an anonymous command will be send over command bus asynchonously", testContext =>
                {
                    object command = new CommandBusTestCommand { Name = "Test" };
                    sendTask = commandBus.SendCommand(command,
                        result =>
                        {
                            cmdResult = result;
                        });
                    sendTask.ContinueWith(task =>
                    {
                        taskCancelled = true;
                    }, TaskContinuationOptions.OnlyOnCanceled);
                })
                .Then("the result should be successful", testContext =>
                {
                    for (var i = 0; i < 30; i++)
                    {
                        if (cmdResult != null) break;
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(500);
                    cmdResult.Should().NotBeNull();
                    cmdResult.Success.Should().BeTrue();
                })
                .And("the result should not contain an error message",
                    testContext => cmdResult.Errors.Should().BeEmpty())
                .And("the task shouldn't be marked as cancelled", testContext =>
                    taskCancelled.Should().BeFalse());
        }
        
        [TestMethod]
        [ScenarioDescription("Sending anonymous command asynchronously with timeout over command bus with consumer receives a successful command result")]
        public void Sending_anonymous_command_asynchronously_with_timeout_over_command_bus_with_consumer_receives_a_successful_command_result()
        {
            CommandResult cmdResult = null;
            var taskCancelled = false;
            TestCommandBus commandBus = null;
            Task<CommandResult> sendTask;

            Given("a command bus", testContext => commandBus = new TestCommandBus())
                .And("the command bus is initialized with loopback",
                    testContext =>
                        commandBus.Initialize(ssbsc => ssbsc.Consumer(() => new CommandBusTestConsumer()).Permanent(),
                            sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue")))
                )
                .And("the command bus has a valid consumer", testContext => { })
                .When("an anonymous command will be send over command bus asynchonously with timeout", testContext =>
                {
                    object command = new CommandBusTestCommand {Name = "Test"};
                    sendTask = commandBus.SendCommand(command ,
                        result =>
                        {
                            cmdResult = result;
                        }, 5.Seconds());
                    sendTask.ContinueWith(task =>
                    {
                        taskCancelled = true;
                    }, TaskContinuationOptions.OnlyOnCanceled);
                })
                .Then("the result should be successful", testContext =>
                {
                    for (var i = 0; i < 50; i++)
                    {
                        if (cmdResult != null) break;
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(500);
                    cmdResult.Should().NotBeNull();
                    cmdResult.Success.Should().BeTrue();
                })
                .And("the result should not contain an error message",
                    testContext => cmdResult.Errors.Should().BeEmpty())
                .And("the task shouldn't be marked as cancelled", testContext =>
                    taskCancelled.Should().BeFalse());
        }

        [TestMethod]
        [ScenarioDescription("Sending command over command bus with consumer that throws an exception receives an error command result")]
        public void Sending_command_over_command_bus_with_consumer_that_throws_an_exception_receives_an_error_command_result()
        {
            CommandResult cmdResult = null;
            var taskCancelled = false;
            var taskFailed = false;
            CommandBus commandBus = null;
            Task<CommandResult> sendTask;

            Given("a command bus", testContext => commandBus = new CommandBus())
                .And("the command bus is initialized with loopback",
                    testContext =>
                        commandBus.Initialize(ssbsc => ssbsc.Consumer(() => new CommandBusTestConsumer()),
                            sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue")))
                )
                .And("the command bus has a valid consumer", testContext => { })
                .When("A command will be send over command bus asynchonously with timeout", testContext =>
                {
                    sendTask = ((MassTransit.ICommandBus)commandBus).Send(new CommandBusTestCommand { Name = "Test", ShouldThrowException = true },
                        result =>
                        {
                            cmdResult = result;
                        }, TimeSpan.FromSeconds(3));
                    sendTask.ContinueWith(task =>
                        {
                            if (task.IsFaulted) taskFailed = true;
                            if (task.IsCanceled) taskCancelled = true;
                        });
                })
                .Then("the result shouldn't be successful", testContext =>
                {
                    for (var i = 0; i < 30; i++)
                    {
                        if (cmdResult != null) break;
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(500);
                    cmdResult.Should().NotBeNull();
                    cmdResult.Success.Should().BeFalse();
                })
                .And("the result should contain an error message",
                    testContext =>
                    {
                        cmdResult.Errors.Should().NotBeEmpty();
                        Console.Write("Expected error of type '{0}': ", cmdResult.ErrorType);
                        foreach (var error in cmdResult.Errors)
                        {
                            Console.WriteLine(error);
                        }
                        foreach (var trace in cmdResult.ErrorStackTrace)
                        {
                            Console.WriteLine(trace);
                        }
                    })
                .And("the task shouldn't be marked as cancelled", testContext =>
                    taskCancelled.Should().BeFalse())
                .And("the task shouldn't be marked as failed", testContext =>
                    taskFailed.Should().BeFalse());
        }

        [TestMethod]
        [ScenarioDescription("Sending command synchronously over command bus with consumer receives a successful command result")]
        public void Sending_command_synchronously_over_command_bus_with_consumer_receives_a_successful_command_result()
        {
            CommandResult cmdResult = null;
            CommandBus commandBus = null;
            Task<CommandResult> sendTask;

            Given("a command bus", testContext => commandBus = new CommandBus())
                .And("the command bus is initialized with loopback",
                    testContext =>
                        commandBus.Initialize(ssbsc => ssbsc.Consumer(() => new CommandBusTestConsumer()).Permanent(),
                            sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue")))
                )
                .And("the command bus has a valid consumer", testContext => { })
                .When("A command will be send over command bus synchonously", testContext =>
                {
                    Console.WriteLine("Thread Id (Test Start): " + Thread.CurrentThread.ManagedThreadId);
                    sendTask = commandBus.SendAsyncWithTimeout(new CommandBusTestCommand {Name = "Test"},
                        result =>
                        {
                            Console.WriteLine("Thread Id (Test handleResult): " + Thread.CurrentThread.ManagedThreadId);
                            cmdResult = result;
                        }, 20.Seconds());
                    sendTask.Wait();
                })
                .Then("the result should be successful", testContext =>
                {
                    for (var i = 0; i < 30; i++)
                    {
                        if (cmdResult != null) break;
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(500);
                    cmdResult.Should().NotBeNull();
                    cmdResult.Success.Should().BeTrue();
                })
                .And("the result should not contain an error message",
                    testContext => cmdResult.Errors.Should().BeEmpty());
        }

        [TestMethod]
        [ScenarioDescription("Sending command synchronously over command bus with consumer throws exception")]
        public void Sending_command_synchronously_over_command_bus_without_consumer_throws_exception()
        {
            CommandResult cmdResult = null;
            CommandBus commandBus = null;
            Task<CommandResult> sendTask;
            Exception exception = null;

            Given("a command bus", testContext => commandBus = new CommandBus())
                .And("the command bus is initialized with loopback",
                    testContext =>
                        commandBus.Initialize(configurator => { },
                            sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/queue")))
                )
                .And("the command bus has no consumer", testContext => { })
                .When("A command will be send over command bus synchonously", testContext =>
                {
                    Console.WriteLine("Thread Id (Test Start): " + Thread.CurrentThread.ManagedThreadId);
                    try
                    {
                        sendTask = commandBus.SendAsyncWithTimeout(new CommandBusTestCommand {Name = "Test"},
                            result =>
                            {
                                Console.WriteLine("Thread Id (Test handleResult): " +
                                                  Thread.CurrentThread.ManagedThreadId);
                                cmdResult = result;
                            }, 5.Seconds());
                        sendTask.Wait();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                })
                .Then("an exception should be thrown", testContext =>
                {
                    exception.Should().NotBeNull();
                });
        }
    }
}