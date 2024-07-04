using FluentAssertions;
using Names;

namespace Domain.Tests;

public class DataTypeTests
{
    [Fact]
    public void ShouldCreateBoolDataType()
    {
        var dataType = DataType.Bool();

        dataType.Should().BeOfType<BoolDataType>();
    }

    [Fact]
    public void ShouldCreateStringDataType()
    {
        var dataType = DataType.String();

        dataType.Should().BeOfType<StringDataType>();
    }

    [Fact]
    public void ShouldCreateDateTimeDataType()
    {
        var dataType = DataType.DateTime();

        dataType.Should().BeOfType<DateTimeDataType>();
    }

    [Fact]
    public void ShouldCreateDateDataType()
    {
        var dataType = DataType.Date();

        dataType.Should().BeOfType<DateDataType>();
    }

    [Fact]
    public void ShouldCreateTimeDataType()
    {
        var dataType = DataType.Time();

        dataType.Should().BeOfType<TimeDataType>();
    }

    [Fact]
    public void ShouldCreateDateTimeOffsetDataType()
    {
        var dataType = DataType.DateTimeOffset();

        dataType.Should().BeOfType<DateTimeOffsetDataType>();
    }

    [Fact]
    public void ShouldCreateByteDataType()
    {
        var dataType = DataType.Byte();

        dataType.Should().BeOfType<ByteDataType>();
    }

    [Fact]
    public void ShouldCreateInt16DataType()
    {
        var dataType = DataType.Int16();

        dataType.Should().BeOfType<Int16DataType>();
    }

    [Fact]
    public void ShouldCreateInt32DataType()
    {
        var dataType = DataType.Int32();

        dataType.Should().BeOfType<Int32DataType>();
    }

    [Fact]
    public void ShouldCreateInt64DataType()
    {
        var dataType = DataType.Int64();

        dataType.Should().BeOfType<Int64DataType>();
    }

    [Fact]
    public void ShouldCreateDecimalDataType()
    {
        var dataType = DataType.Decimal();

        dataType.Should().BeOfType<DecimalDataType>();
    }

    [Fact]
    public void ShouldCreateUniqueIdentifierDataType()
    {
        var dataType = DataType.UniqueIdentifier();

        dataType.Should().BeOfType<UniqueIdentifierDataType>();
    }

    [Fact]
    public void ShouldCreateObjectDataType()
    {
        var dataType = DataType.Object();

        dataType.Should().BeOfType<ObjectDataType>();
    }

    [Fact]
    public void ShouldCreateLatLongDataType()
    {
        var dataType = DataType.LatLong();

        dataType.Should().BeOfType<LatLongType>();
    }

    [Fact]
    public void ShouldCreateEntityDataType()
    {
        var dataType = DataType.Entity(new NonEmptyString("TestEntity"));

        dataType.Should().BeOfType<EntityDataType>();
    }
}