using System;
using FluentAssertions;
using NUnit.Framework;

namespace StkCommon.Data.Test
{
    [TestFixture]
    public class EquatableByKeyTest
    {
        private class ModelWithValueTypeKey : EquatableByKey<ModelWithValueTypeKey, int>
        {
            public ModelWithValueTypeKey(int key) : base(key)
            {
            }
        }

        [Test]
        public void ShouldValueType()
        {
            var int0Val1 = new ModelWithValueTypeKey(0);
            var int0Val2 = new ModelWithValueTypeKey(0);
            var int0Val3 = new ModelWithValueTypeKey(3);

            int0Val1.GetHashCode().Should().Be(0);
            int0Val1.Equals(int0Val1).Should().BeTrue();
            int0Val1.Equals(int0Val2).Should().BeTrue();
            int0Val1.Equals(int0Val3).Should().BeFalse();
            int0Val1.Equals(new object()).Should().BeFalse();
            int0Val1.Equals(null).Should().BeFalse();
        }


        private class ReferenceKey : IEquatable<ReferenceKey>
        {
            public ReferenceKey(int keyVal)
            {
                KeyVal = keyVal;
            }

            private int KeyVal { get; }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            /// <param name="other">An object to compare with this object.</param>
            public bool Equals(ReferenceKey other)
            {
                if (ReferenceEquals(other, null))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;

                return KeyVal == other.KeyVal;
            }
        }

        private class ModelWithReferenceTypeKey : EquatableByKey<ModelWithReferenceTypeKey, ReferenceKey>
        {
            public ModelWithReferenceTypeKey(ReferenceKey key) : base(key)
            {
            }
        }

        [Test]
        public void ShouldReferenceType()
        {
            Assert.Throws<ArgumentNullException>(() => new ModelWithReferenceTypeKey(null));

            var keyVal1 = new ReferenceKey(0);
            var int0Val1 = new ModelWithReferenceTypeKey(keyVal1);
            var int0Val2 = new ModelWithReferenceTypeKey(new ReferenceKey(0));
            var int0Val3 = new ModelWithReferenceTypeKey(new ReferenceKey(1));

            int0Val1.GetHashCode().Should().Be(keyVal1.GetHashCode());
            int0Val1.Equals(int0Val1).Should().BeTrue();
            int0Val1.Equals(int0Val2).Should().BeTrue();
            int0Val1.Equals(int0Val3).Should().BeFalse();
            int0Val1.Equals(new object()).Should().BeFalse();
            int0Val1.Equals(null).Should().BeFalse();
        }

    }
}