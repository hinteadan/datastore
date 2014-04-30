﻿using System;
using System.Collections.Generic;

namespace HttpDataStore.Model
{
    public class Entity<T>
    {
        Guid id, checkTag;
        DateTime lastModifiedOn = DateTime.MinValue;

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
        public Guid CheckTag 
        {
            get
            {
                EnsureCheckTag();
                return checkTag;
            }
        }
        public DateTime LastModifiedOn
        {
            get
            {
                return lastModifiedOn;
            }
        }
        public Dictionary<string, object> Meta { get; private set; }
        public T Data { get; set; }

        private void EnsureCheckTag()
        {
            if (checkTag != Guid.Empty)
            {
                return;
            }
            checkTag = Guid.NewGuid();
        }

        public void ValidateAlterOperation(Guid alteredEntityCheckTag)
        {
            if (alteredEntityCheckTag == checkTag)
            {
                return;
            }

            throw new InvalidOperationException("You don't have the latest version of this entity; It has already been altered from some other source. Try fetching the latest version and apply your changes again.");
        }

        public void ValidateAlterOperation(Entity<object> alteredEntity)
        {
            ValidateAlterOperation(alteredEntity.CheckTag);
        }

        public void PinAlterPoint()
        {
            checkTag = Guid.NewGuid();
            lastModifiedOn = DateTime.Now;
        }
    }
}
