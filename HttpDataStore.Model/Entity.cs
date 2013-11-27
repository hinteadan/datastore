using System;
using System.Collections.Generic;

namespace HttpDataStore.Model
{
    public class Entity<T>
    {
        public Entity(Guid id, T data, Dictionary<string, object> meta)
        {
            this.Id = id;
            this.Data = data;
            this.Meta = meta;
        }
        public Entity()
            : this(new Guid(), default(T), new Dictionary<string, object>())
        { }
        public Entity(T data)
            : this(new Guid(), data, new Dictionary<string, object>())
        { }
        public Entity(Guid id, T data)
            : this(id, data, new Dictionary<string, object>())
        { }
        public Entity(T data, Dictionary<string, object> meta)
            : this(new Guid(), data, meta)
        { }


        public Guid Id { get; set; }
        public Dictionary<string, object> Meta { get; private set; }
        public T Data { get; set; }
    }
}
