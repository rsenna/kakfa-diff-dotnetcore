using System;
using JetBrains.Annotations;

namespace Kafka.Diff.Common.Log
{
    // TODO: rename Common.Log to Common
    public class SubmitKey : IEquatable<SubmitKey>
    {
        // TODO: use enum instead
        public const string Left = "left";
        public const string Right = "right";

        public const string Separator = ":";

        public string Id { get; }
        public string Side { get; }

        [Pure]
        public override string ToString() => $"{Id}{Separator}{Side}";

        public SubmitKey([NotNull]string id, [NotNull]string side)
        {
            Id = id;
            Side = side;
        }

        public static SubmitKey FromString([NotNull]string text)
        {
            var parts = text.Split(Separator);

            if (parts?.Length != 2)
            {
                throw new ArgumentException($"Invalid ${text}", nameof(text));
            }

            return new SubmitKey(parts[0], parts[1]);
        }

        public bool Equals(SubmitKey other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Id, other.Id) && string.Equals(Side, other.Side);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((SubmitKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ Side.GetHashCode();
            }
        }

        public static bool operator ==(SubmitKey left, SubmitKey right) => Equals(left, right);
        public static bool operator !=(SubmitKey left, SubmitKey right) => !Equals(left, right);
    }
}
