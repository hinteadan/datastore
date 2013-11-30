using System;
using System.Collections.Generic;

namespace HttpDataStore.Model
{
    public class Entity<T>
    {
        Guid id;

        public Entity(Guid id, T data, Dictionary<string, object> meta)
        {
            this.Id = id;
            this.Data = data;
            this.Meta = meta;
        }
        public Entity()
            : this(Guid.NewGuid(), default(T), new Dictionary<string, object>())
        { }
        public Entity(T data)
            : this(Guid.NewGuid(), data, new Dictionary<string, object>())
        { }
        public Entity(Guid id, T data)
            : this(id, data, new Dictionary<string, object>())
        { }
        public Entity(T data, Dictionary<string, object> meta)
            : this(Guid.NewGuid(), data, meta)
        { }


        public Guid Id
        {
            get
            {
                return id;
            }
            set
            {
                this.id = value.Equals(Guid.Empty) ? Guid.NewGuid() : value;
            }
        }
        public Dictionary<string, object> Meta { get; private set; }
        public T Data { get; set; }
    }
}
