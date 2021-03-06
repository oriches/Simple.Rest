using System;
using System.Runtime.Serialization;

namespace Simple.Rest.Tests.Dto
{
    [DataContract(Name = "Report", Namespace = "")]
    public class Report : IEquatable<Report>
    {
        [DataMember(IsRequired = true)] public int Id { get; set; }

        [DataMember(IsRequired = true)] public string Name { get; set; }

        public bool Equals(Report other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Report) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(Report left, Report right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Report left, Report right)
        {
            return !Equals(left, right);
        }
    }
}