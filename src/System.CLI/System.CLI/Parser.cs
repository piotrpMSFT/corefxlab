using System.Collections.Generic;

namespace System.CLI
{
    public class Parser : Parser<EmptyArgs>
    {
        public Parser(Func<IEnumerable<string>, int> defaultCommandHandler, StringComparer propertyNameComparer = null) 
            : base(WrapHandler(defaultCommandHandler), propertyNameComparer)
        {
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