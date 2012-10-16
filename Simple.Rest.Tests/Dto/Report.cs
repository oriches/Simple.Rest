namespace Simple.Rest.Tests.Dto
{
    using System;

    public class Report : IEquatable<Report>
    {
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
            if (obj.GetType() != this.GetType()) return false;
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

        public int Id { get; set; }

        public string Name { get; set; }
    }
}