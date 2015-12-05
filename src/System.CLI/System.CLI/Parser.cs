using System.Collections.Generic;

namespace System.CLI
{
    public static class Parser
    {
        public static Parser<T> WithDefaultCommand<T>(Func<T, IEnumerable<string>, int> defaultCommandHandler, StringComparer propertyNameComparer = null) where T : class
        {
            return new Parser<T>(defaultCommandHandler, propertyNameComparer);
        }

        public static Parser<EmptyArgs> WithDefaultCommand(Func<IEnumerable<string>, int> defaultCommandHandler, StringComparer propertyNameComparer = null)
        {
            return new Parser<EmptyArgs>(WrapHandler(defaultCommandHandler), propertyNameComparer);
        }
        
        private static Func<object, IEnumerable<string>, int> WrapHandler(Func<IEnumerable<string>, int> defaultCommandHandler)
        {
            if (defaultCommandHandler == null)
            {
                return null;
            }
            else
            {
                return (a, r) => defaultCommandHandler(r);
            }
        }
    }
}