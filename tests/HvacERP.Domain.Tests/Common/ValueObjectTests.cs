using HvacERP.Domain.Common.ValueObjects;

namespace HvacERP.Domain.Tests.Common;

public sealed class ValueObjectTests
{
    [Fact]
    public void Equals_WithMatchingComponents_ReturnsTrue()
    {
        TestValueObject first = new("MAIN", 1);
        TestValueObject second = new("MAIN", 1);

        Assert.Equal(first, second);
        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equals_WithDifferentComponents_ReturnsFalse()
    {
        TestValueObject first = new("MAIN", 1);
        TestValueObject second = new("NORTH", 2);

        Assert.NotEqual(first, second);
        Assert.False(first == second);
        Assert.True(first != second);
    }

    [Fact]
    public void GetHashCode_WithMatchingComponents_ReturnsSameHashCode()
    {
        TestValueObject first = new("MAIN", 1);
        TestValueObject second = new("MAIN", 1);

        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentValueObjectType_ReturnsFalse()
    {
        TestValueObject value = new("MAIN", 1);
        DifferentTestValueObject different = new("MAIN", 1);

        Assert.False(value.Equals(different));
    }

    private sealed class TestValueObject : ValueObject
    {
        private readonly string _code;
        private readonly int _number;

        public TestValueObject(string code, int number)
        {
            _code = code;
            _number = number;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return _code;
            yield return _number;
        }
    }

    private sealed class DifferentTestValueObject : ValueObject
    {
        private readonly string _code;
        private readonly int _number;

        public DifferentTestValueObject(string code, int number)
        {
            _code = code;
            _number = number;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return _code;
            yield return _number;
        }
    }
}
