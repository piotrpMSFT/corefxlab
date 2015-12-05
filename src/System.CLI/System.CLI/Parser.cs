using System.Collections.Generic;
using System.Linq;

namespace System.CLI
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Parser
    {
        private readonly Func<object, int> _defaultCommandHandler;
        private readonly Dictionary<string, Func<object, int>> _commands;
        private Func<Type, string[], object> _deserializer; 

        public Parser(Func<object, int> defaultCommandHandler, StringComparer propertyNameComparer = null)
        {
            if (defaultCommandHandler == null) throw new ArgumentNullException(nameof(defaultCommandHandler));

            _deserializer = (t, s) => null;
            _defaultCommandHandler = defaultCommandHandler;

            _commands = new Dictionary<string, Func<object, int>>(propertyNameComparer ??  StringComparer.InvariantCulture);
        }

        public int Execute(IEnumerable<string> args)
        {
            var candidateCommand = args.FirstOrDefault();

            if (candidateCommand != null && _commands.ContainsKey(candidateCommand))
            {
                return _commands[candidateCommand](null);
            }

            return _defaultCommandHandler(null);
        }

        public Parser WithCommand<T>(Func<T, int> command) where T : class
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            if (!typeof (T).Name.EndsWith("Args"))
            {
                throw new ArgumentException($"command parameter Type name must end in 'Args', but '{typeof(T).Name}' does not.");
            }

            var commandName = typeof (T).Name.Substring(0, typeof (T).Name.Length - 4);

            _commands.Add(commandName, a => command((T) a));

            return this;
        }

        public Parser WithDeserializer(Func<Type, string[], object> deserializer)
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
