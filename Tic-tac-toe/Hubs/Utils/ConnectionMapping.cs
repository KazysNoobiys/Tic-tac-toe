using System.Collections.Generic;
using System.Linq;

namespace Tic_tac_toe.Hubs.Utils
{
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _dictionary = new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                lock (_dictionary)
                {
                    return _dictionary.Count;
                }
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_dictionary)
            {
                HashSet<string> connection;
                if (!_dictionary.TryGetValue(key, out connection))
                {
                    connection = new HashSet<string>();
                    _dictionary.Add(key, connection);
                }

                lock (connection)
                {
                    connection.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetAllConnections(T key)
        {
            lock (_dictionary)
            {
                HashSet<string> connection;
                if (_dictionary.TryGetValue(key, out connection))
                {
                    return connection;
                }
            }

            return Enumerable.Empty<string>();
        }

        public IEnumerable<T> GetAllKeys()
        {
            lock (_dictionary)
            {
                return _dictionary.Keys;
            }
        }

        public void Remove(T key, string connectionId, bool noRemove = false)
        {
            lock (_dictionary)
            {
                HashSet<string> connection;
                if (!_dictionary.TryGetValue(key, out connection))
                {
                    return;
                }

                lock (connection)
                {
                    connection.Remove(connectionId);
                    if (connection.Count == 0)
                    {
                        if (!noRemove)
                        {
                            _dictionary.Remove(key);
                        }
                    }
                }
            }
        }
        public void Remove(T key)
        {
            lock (_dictionary)
            {
                HashSet<string> connection;
                if (!_dictionary.TryGetValue(key, out connection))
                {
                    return;
                }

                lock (connection)
                {
                    if (connection.Count == 0)
                    {
                        _dictionary.Remove(key);
                    }
                }
            }
        }
    }
}