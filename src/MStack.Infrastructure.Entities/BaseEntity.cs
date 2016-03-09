using MStack.Core.Entities;
using System;

namespace MStack.Infrastructure.Entities
{
    public class BaseEntity : IEntity, IIdEntity<Guid>
    {
        public Guid Id { get; set; }
        public BaseEntity()
        {
            this.Id = Guid.NewGuid();
        }
    }
}