using System;
using System.Runtime.InteropServices;

namespace MS.Infrastructure
{
    [StructLayout(LayoutKind.Explicit)]
    public struct UuidInteger
    {
        [FieldOffset(0)]
        public Int32 I1;     //mit Vorzeichen
        [FieldOffset(4)]
        public Int32 I2;
        [FieldOffset(8)]
        public Int32 I3;
        [FieldOffset(12)]
        public Int32 I4;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UuidCardinal
    {
        [FieldOffset(0)]
        public UInt32 C1;    //vorzeichenlos
        [FieldOffset(4)]
        public UInt32 C2;
        [FieldOffset(8)]
        public UInt32 C3;
        [FieldOffset(12)]
        public UInt32 C4;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UuidLong
    {
        [FieldOffset(0)]
        public Int64 L1;       //2 BIGINTS
        [FieldOffset(8)]
        public Int64 L2;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UuidShort
    {
        [FieldOffset(0)]
        public Int16 S1;     //mit Vorzeichen
        [FieldOffset(2)]
        public Int16 S2;
        [FieldOffset(4)]
        public Int16 S3;
        [FieldOffset(6)]
        public Int16 S4;
        [FieldOffset(8)]
        public Int16 S5;
        [FieldOffset(10)]
        public Int16 S6;
        [FieldOffset(12)]
        public Int16 S7;
        [FieldOffset(14)]
        public Int16 S8;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UuidBytes
    {
        [FieldOffset(0)]
        public Byte B1;       //16 Bytes
        [FieldOffset(1)]
        public Byte B2;
        [FieldOffset(2)]
        public Byte B3;
        [FieldOffset(3)]
        public Byte B4;
        [FieldOffset(4)]
        public Byte B5;
        [FieldOffset(5)]
        public Byte B6;
        [FieldOffset(6)]
        public Byte B7;
        [FieldOffset(7)]
        public Byte B8;
        [FieldOffset(8)]
        public Byte B9;
        [FieldOffset(9)]
        public Byte B10;
        [FieldOffset(10)]
        public Byte B11;
        [FieldOffset(11)]
        public Byte B12;
        [FieldOffset(12)]
        public Byte B13;
        [FieldOffset(13)]
        public Byte B14;
        [FieldOffset(14)]
        public Byte B15;
        [FieldOffset(15)]
        public Byte B16;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Uuid : IComparable, IFormattable, IEquatable<Uuid>
    {
        [FieldOffset(0)]
        public Guid AsGuid;
        [FieldOffset(0)]
        public UuidLong AsLong;
        [FieldOffset(0)]
        public UuidCardinal AsCardinal;
        [FieldOffset(0)]
        public UuidInteger AsInteger;
        [FieldOffset(0)]
        public UuidShort AsShort;
        [FieldOffset(0)]
        public UuidBytes AsByte;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Uuid && Equals((Uuid) obj);
        }

        public bool Equals(Uuid other)
        {
            return AsGuid.Equals(other.AsGuid);
        }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyFieldInGetHashCode
            return AsGuid.GetHashCode();
            // ReSharper restore NonReadonlyFieldInGetHashCode
        }

        public string ToString(string format)
        {
            return AsGuid.ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return AsGuid.ToString(format, formatProvider);
        }

        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }
            if (!(value is Uuid))
            {
                throw new ArgumentException(StringResources.ErrArgumentMustBeOfTypeUuid());
            }
            var g = ((Uuid)value).AsGuid;
            return AsGuid.CompareTo(g);
        }

        public static bool operator ==(Uuid a, Uuid b)
        {
            return (a.AsGuid == b.AsGuid);
        }

        public static bool operator !=(Uuid a, Uuid b)
        {
            return !(a == b);
        }

        public static Uuid NewId()
        {
            return new Uuid { AsGuid = Guid.NewGuid() };
        }

        public static Uuid Empty()
        {
            return new Uuid { AsGuid = Guid.Empty };
        }

        public static class StringResources
        {
            public static Func<string> ErrArgumentMustBeOfTypeUuid = () => "Argument must be of type 'Uuid'!";
        }

    }
}
