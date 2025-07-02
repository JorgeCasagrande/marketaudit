using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Entities
{
    public class Entity : IEquatable<Entity>
    {
        public long Id { get; set; }

        public bool Equals(Entity other)
        {
            if (Id == other.Id)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            int idHash = Id.GetHashCode();

            return idHash;
        }
    }
}
