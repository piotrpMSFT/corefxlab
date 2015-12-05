using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Its.Recipes;
using Xunit;

namespace System.CLI.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Given_a_Parser
    {

        [Fact]
        public void When_constructed_with_null_defaultCommand_it_throws_ArgumentNullException()
        {
            Action action = () => new Parser(null);

            action.ShouldThrow<ArgumentNullException>("Because a default command must always be registered")
                .WithMessage("Value cannot be null.\r\nParameter name: defaultCommandHandler");
        }

        [Fact]
        public void When_called_without_registered_command_then_defaultCommand_is_invoked()
        {
            var expectedExitCode = Any.Int();
            var args = Any.Sequence(x => Any.String());

            new Parser(r => expectedExitCode)
                .Execute(args)
                .Should().Be(expectedExitCode, "Because the defaultCommand is called");
        }

        [Fact]
        public void When_adding_a_null_command_then_it_throws_ArgumentNullException()
        {
            Action action = () => new Parser(r => Any.Int()).WithCommand<Command1Args>(null);

            action.ShouldThrow<ArgumentNullException>("Because a null command cannot be registered.")
                .WithMessage("Value cannot be null.\r\nParameter name: command");
        }

        [Fact]
        public void When_adding_a_command_whose_type_does_not_match_convention_then_it_throws_ArgumentNullException()
        {
            Action action = () => new Parser(r => Any.Int()).WithCommand<string>((o, a) => Any.Int());

            action.ShouldThrow<ArgumentException>("Because command parameter Types must follow the convention '[CommandName]Args'.")
                .WithMessage("command parameter Type name must end in 'Args', but 'String' does not.");
        }

        [Fact]
        public void When_called_with_empty_args_then_defaultCommand_is_invoked()
        {
            var expectedExitCode = Any.Int();
            var args = Enumerable.Empty<string>();

            new Parser(r => expectedExitCode)
                .WithCommand<Command1Args>((o, a) => Any.Int())
                .Execute(args)
                .Should().Be(expectedExitCode, "Because the defaultCommand is called");
        }

        [Fact]
        public void When_called_without_matching_command_then_defaultCommand_is_invoked()
        {
            var expectedExitCode = Any.Int();
            var args = Any.Sequence(x => Any.String());

            new Parser(r => expectedExitCode)
                .WithCommand<Command1Args>((o, a) => Any.Int())
                .Execute(args)
                .Should().Be(expectedExitCode, "Because the defaultCommand is called");
        }

        [Fact]
        public void When_called_with_matching_command_then_that_handler_is_invoked()
        {
            var expectedExitCode = Any.Int();
            var args = (new [] {"Command1"}).Concat(Any.Sequence(x => Any.String()));

            new Parser(r => Any.Int())
                .WithCommand<Command1Args>((o, a) => expectedExitCode)
                .Execute(args)
                .Should().Be(expectedExitCode, "Because the defaultCommand is called");
        }

        [Fact]
        public void When_called_with_matching_command_then_StringComparison_respected()
        {
            var expectedExitCode = Any.Int();
            var args = (new[] { "Command1".ToLower() }).Concat(Any.Sequence(x => Any.String()));

            new Parser(r => Any.Int(), StringComparer.OrdinalIgnoreCase)
                .WithCommand<Command1Args>((o, a) => expectedExitCode)
                .Execute(args)
                .Should().Be(expectedExitCode, "Because the defaultCommand is called");
        }

        [Fact]
        public void When_setting_Deserializer_to_null_then_ArgumentNullException_is_thrown()
        {
            Action action = () => new Parser(r => Any.Int()).WithDeserializer(null);

            action.ShouldThrow<ArgumentException>("Because command parameter Types must follow the convention '[CommandName]Args'.")
                .WithMessage("Value cannot be null.\r\nParameter name: deserializer");
        }
    }
}
