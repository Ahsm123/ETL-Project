namespace ETLDomain.Resolver
{
    public class TypeHandlerResolver<T>
    {

        private readonly IEnumerable<T> _handlers;
        private readonly Func<T, Type, bool> _canHandle;

        public TypeHandlerResolver(IEnumerable<T> handlers, Func<T, Type, bool> canHandle)
        {
            _handlers = handlers;
            _canHandle = canHandle;
        }

        public T? Resolve(Type type)
        {
            return _handlers.FirstOrDefault(h => _canHandle(h, type));
        }
    }
}