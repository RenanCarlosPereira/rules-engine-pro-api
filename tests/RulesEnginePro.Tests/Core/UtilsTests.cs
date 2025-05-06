using Bogus;
using FluentAssertions;
using Utils = RulesEnginePro.Core.Utils;

namespace RulesEnginePro.Tests.Core
{
    public class UtilsTests
    {
        [Fact]
        public void IntArray_ShouldReturnCorrectArray()
        {
            var result = Utils.IntArray(1, 2, 3);
            result.Should().BeEquivalentTo([1, 2, 3]);
        }

        [Fact]
        public void LongArray_ShouldReturnCorrectArray()
        {
            var result = Utils.LongArray(100L, 200L);
            result.Should().BeEquivalentTo([100L, 200L]);
        }

        [Fact]
        public void FloatArray_ShouldReturnCorrectArray()
        {
            var result = Utils.FloatArray(1.1f, 2.2f);
            result.Should().BeEquivalentTo([1.1f, 2.2f]);
        }

        [Fact]
        public void DoubleArray_ShouldReturnCorrectArray()
        {
            var result = Utils.DoubleArray(1.23, 4.56);
            result.Should().BeEquivalentTo([1.23, 4.56]);
        }

        [Fact]
        public void DecimalArray_ShouldReturnCorrectArray()
        {
            var result = Utils.DecimalArray(10.5m, 20.75m);
            result.Should().BeEquivalentTo([10.5m, 20.75m]);
        }

        [Fact]
        public void StringArray_ShouldReturnCorrectArray()
        {
            var result = Utils.StringArray("a", "b", "c");
            result.Should().BeEquivalentTo("a", "b", "c");
        }

        [Fact]
        public void BoolArray_ShouldReturnCorrectArray()
        {
            var result = Utils.BoolArray(true, false, true);
            result.Should().BeEquivalentTo([true, false, true]);
        }

        [Fact]
        public void CharArray_ShouldReturnCorrectArray()
        {
            var result = Utils.CharArray('x', 'y');
            result.Should().BeEquivalentTo(['x', 'y']);
        }

        [Fact]
        public void DateTimeArray_ShouldParseStringsCorrectly()
        {
            var dates = new[] { "2023-01-01", "2024-05-06" };
            var result = Utils.DateTimeArray(dates);

            result.Should().HaveCount(2);
            result[0].Should().Be(DateTime.Parse("2023-01-01"));
            result[1].Should().Be(DateTime.Parse("2024-05-06"));
        }

        [Fact]
        public void GuidArray_ShouldParseStringsCorrectly()
        {
            var g1 = Guid.NewGuid();
            var g2 = Guid.NewGuid();

            var result = Utils.GuidArray(g1.ToString(), g2.ToString());

            result.Should().BeEquivalentTo([g1, g2]);
        }

        [Fact]
        public void StringArray_WithBogusData_ShouldWork()
        {
            var faker = new Faker();
            var names = new[] { faker.Name.FirstName(), faker.Name.LastName() };
            var result = Utils.StringArray(names);

            result.Should().BeEquivalentTo(names);
        }
    }
}