using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MStack.Core.Entities
{
    public interface IEntity
    {

    }
    public interface IIdEntity<TKey> : IEntity
    {
        TKey Id { get; set; }
    }

    public interface ISoftDeleteEntity : IEntity
    {
        bool IsDeleted { get; set; }
    }
}
