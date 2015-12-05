using System.Collections.Generic;
using System.Linq;

namespace System.CLI
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Parser<T> where T:class
    {
        private readonly Func<T, IEnumerable<string>, int> _defaultCommandHandler;
        private readonly Dictionary<string, Func<object, IEnumerable<string>, int>> _commands;
        private Func<Type, IEnumerable<string>, object> _deserializer; 

        internal Parser(Func<T, IEnumerable<string>, int> defaultCommandHandler, StringComparer propertyNameComparer = null)
        {
            if (defaultCommandHandler == null) throw new ArgumentNullException(nameof(defaultCommandHandler));

            _deserializer = (t, s) => null;
            _defaultCommandHandler = defaultCommandHandler;

            _commands = new Dictionary<string, Func<object, IEnumerable<string>, int>>(propertyNameComparer ??  StringComparer.InvariantCulture);
        }

        public int Execute(IEnumerable<string> args)
        {
            var candidateCommand = args.FirstOrDefault();

            if (candidateCommand != null && _commands.ContainsKey(candidateCommand))
            {
                return _commands[candidateCommand](null, args);
            }

            return _defaultCommandHandler(null, args);
        }

        public Parser<T> WithCommand<TC>(Func<TC, IEnumerable<string>, int> command) where TC : class
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            if (!typeof (TC).Name.EndsWith("Args"))
            {
                throw new ArgumentException($"command parameter Type name must end in 'Args', but '{typeof(TC).Name}' does not.");
            }

            var commandName = typeof (TC).Name.Substring(0, typeof (TC).Name.Length - 4);

            _commands.Add(commandName, (o,a) => command((TC) o, a));

            return this;
        }

        public Parser<T> WithDeserializer(Func<Type, IEnumerable<string>, object> deserializer)
        {
            if (deserializer == null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            _deserializer = deserializer;

            return this;
        }
    }
}
