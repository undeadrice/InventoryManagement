using FluentAssertions;
using InventoryManagement.Domain.Customers;
using InventoryManagement.Domain.Orders.Services;
using Xunit;

namespace InventoryManagement.Domain.Tests.Orders.Services;

public class DiscountCalculatorTests
{
    private static DiscountCalculator CreateWithDate(DateTime currentDate)
        => new DiscountCalculator(() => currentDate);

    private static readonly DateTime NeutralDate = new DateTime(2024, 6, 15); // Saturday, June 15

    #region No Discount Scenarios

    [Fact]
    public void CalculateDiscount_WithUSLocationAndNoDiscounts_ShouldReturnBasePrice()
    {
        // Arrange
        var calculator = CreateWithDate(NeutralDate);

        // Act
        var result = calculator.CalculateDiscount(100m, 1, CustomerLocation.US, NeutralDate);

        // Assert
        result.Should().Be(100);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    public void CalculateDiscount_WithQuantityBelowVolumeThreshold_ShouldApplyNoVolumeDiscount(int quantity)
    {
        // Arrange
        var calculator = CreateWithDate(NeutralDate);

        // Act
        var result = calculator.CalculateDiscount(100m, quantity, CustomerLocation.US, NeutralDate);

        // Assert
        result.Should().Be(100);
    }

    #endregion

    #region Volume Discounts

    [Theory]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(9)]
    public void CalculateDiscount_WithQuantityBetween5And9_ShouldApply10PercentVolumeDiscount(int quantity)
    {
        // Arrange
        var calculator = CreateWithDate(NeutralDate);

        // Act
        var result = calculator.CalculateDiscount(100m, quantity, CustomerLocation.US, NeutralDate);

        // Assert
        result.Should().Be(90);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(49)]
    public void CalculateDiscount_WithQuantityBetween10And49_ShouldApply20PercentVolumeDiscount(int quantity)
    {
        // Arrange
        var calculator = CreateWithDate(NeutralDate);

        // Act
        var result = calculator.CalculateDiscount(100m, quantity, CustomerLocation.US, NeutralDate);

        // Assert
        result.Should().Be(80);
    }

    [Theory]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(200)]
    public void CalculateDiscount_WithQuantity50OrMore_ShouldApply30PercentVolumeDiscount(int quantity)
    {
        // Arrange
        var calculator = CreateWithDate(NeutralDate);

        // Act
        var result = calculator.CalculateDiscount(100m, quantity, CustomerLocation.US, NeutralDate);

        // Assert
        result.Should().Be(70);
    }

    #endregion

    #region Location Multipliers

    [Fact]
    public void CalculateDiscount_WithUSLocation_ShouldApplyNoLocationMultiplier()
    {
        // Arrange
        var calculator = CreateWithDate(NeutralDate);

        // Act
        var result = calculator.CalculateDiscount(100m, 1, CustomerLocation.US, NeutralDate);

        // Assert
        result.Should().Be(100);
    }

    [Fact]
    public void CalculateDiscount_WithEuropeLocation_ShouldApply15PercentVATMultiplier()
    {
        // Arrange
        var calculator = CreateWithDate(NeutralDate);

        // Act
        var result = calculator.CalculateDiscount(100m, 1, CustomerLocation.EUROPE, NeutralDate);

        // Assert
        result.Should().Be(115);
    }

    [Fact]
    public void CalculateDiscount_WithAsiaLocation_ShouldApply5PercentLogisticsMultiplier()
    {
        // Arrange
        var calculator = CreateWithDate(NeutralDate);

        // Act
        var result = calculator.CalculateDiscount(100m, 1, CustomerLocation.ASIA, NeutralDate);

        // Assert
        result.Should().Be(105);
    }

    // 100 * 1.15 = 115, then 20% off -> 115 * 0.80 = 92
    [Fact]
    public void CalculateDiscount_WithEuropeLocationAndVolumeDiscount_ShouldApplyMultiplierBeforeDiscount()
    {
        // Arrange
        var calculator = CreateWithDate(NeutralDate);

        // Act
        var result = calculator.CalculateDiscount(100m, 10, CustomerLocation.EUROPE, NeutralDate);

        // Assert
        result.Should().Be(92m);
    }

    // 100 * 1.05 = 105, then 20% off -> 105 * 0.80 = 84
    [Fact]
    public void CalculateDiscount_WithAsiaLocationAndVolumeDiscount_ShouldApplyMultiplierBeforeDiscount()
    {
        // Arrange
        var calculator = CreateWithDate(NeutralDate);

        // Act
        var result = calculator.CalculateDiscount(100m, 10, CustomerLocation.ASIA, NeutralDate);

        // Assert
        result.Should().Be(84m);
    }

    #endregion

    #region Seasonal Discounts — Polish Holidays

    [Theory]
    [InlineData(1, 1)]   // New Year's Day
    [InlineData(1, 6)]   // Epiphany
    [InlineData(5, 1)]   // Labour Day
    [InlineData(5, 3)]   // Constitution Day
    [InlineData(8, 15)]  // Assumption of Mary
    [InlineData(11, 1)]  // All Saints' Day
    [InlineData(11, 11)] // Independence Day
    [InlineData(12, 25)] // Christmas Day
    [InlineData(12, 26)] // Second Day of Christmas
    public void CalculateDiscount_OnPolishHoliday_ShouldApply15PercentSeasonalDiscount(int month, int day)
    {
        // Arrange
        var holiday = new DateTime(2024, month, day);
        var calculator = CreateWithDate(holiday);

        // Act
        var result = calculator.CalculateDiscount(100m, 1, CustomerLocation.US, holiday);

        // Assert
        result.Should().Be(85m);
    }

    #endregion

    #region Seasonal Discounts — Black Friday

    [Fact]
    public void CalculateDiscount_OnBlackFriday2024_ShouldApply25PercentSeasonalDiscount()
    {
        var blackFriday = new DateTime(2024, 11, 29);
        var calculator = CreateWithDate(blackFriday);

        // Act
        var result = calculator.CalculateDiscount(100m, 1, CustomerLocation.US, blackFriday);

        // Assert
        result.Should().Be(75);
    }

    [Fact]
    public void CalculateDiscount_OnBlackFriday2023_ShouldApply25PercentSeasonalDiscount()
    {
        var blackFriday = new DateTime(2023, 11, 24);
        var calculator = CreateWithDate(blackFriday);

        // Act
        var result = calculator.CalculateDiscount(100m, 1, CustomerLocation.US, blackFriday);

        // Assert
        result.Should().Be(75);
    }

    [Fact]
    public void CalculateDiscount_OnFridayInNovemberThatIsNotBlackFriday_ShouldNotApplyBlackFridayDiscount()
    {
        var thirdFriday = new DateTime(2024, 11, 15);
        var calculator = CreateWithDate(thirdFriday);

        // Act
        var result = calculator.CalculateDiscount(100m, 1, CustomerLocation.US, thirdFriday);

        // Assert
        result.Should().Be(100);
    }

    #endregion

    #region Discount Priority — Only Highest Applies

    // Polish holiday gives 15%, quantity 50 gives 30% → 30% wins
    [Fact]
    public void CalculateDiscount_WhenVolumeDiscountHigherThanSeasonal_ShouldApplyVolumeDiscount()
    {
        // Arrange
        var holiday = new DateTime(2024, 1, 1);
        var calculator = CreateWithDate(holiday);

        // Act
        var result = calculator.CalculateDiscount(100m, 50, CustomerLocation.US, holiday);

        // Assert
        result.Should().Be(70);
    }

    //  Black Friday gives 25%, quantity 5 gives 10% → 25% wins
    [Fact]
    public void CalculateDiscount_WhenSeasonalDiscountHigherThanVolume_ShouldApplySeasonalDiscount()
    {
        // Arrange
        var blackFriday = new DateTime(2024, 11, 29);
        var calculator = CreateWithDate(blackFriday);

        // Act
        var result = calculator.CalculateDiscount(100m, 5, CustomerLocation.US, blackFriday);

        // Assert
        result.Should().Be(75);
    }

    // Polish holiday gives 15%, quantity 5 gives 10% → 15% wins (higher)
    [Fact]
    public void CalculateDiscount_WhenBothDiscountsAreEqual_ShouldApplyThatDiscountOnce()
    {
        // Arrange
        var holiday = new DateTime(2024, 5, 1);
        var calculator = CreateWithDate(holiday);

        // Act
        var result = calculator.CalculateDiscount(100m, 5, CustomerLocation.US, holiday);

        // Assert
        result.Should().Be(85);
    }

    // Black Friday gives 25%, quantity 50 gives 30% → 30% wins
    [Fact]
    public void CalculateDiscount_WhenBlackFridayAndHighVolumeDiscount_ShouldApplyHighestDiscount()
    {
        // Arrange
        var blackFriday = new DateTime(2024, 11, 29);
        var calculator = CreateWithDate(blackFriday);

        // Act
        var result = calculator.CalculateDiscount(100m, 50, CustomerLocation.US, blackFriday);

        // Assert
        result.Should().Be(70);
    }

    #endregion

    #region Combined Location + Discount

    // 100 * 1.15 = 115, then 25% off → 115 * 0.75 = 86.25
    [Fact]
    public void CalculateDiscount_WithEuropeLocationOnBlackFriday_ShouldApplyMultiplierThenDiscount()
    {
        // Arrange
        var blackFriday = new DateTime(2024, 11, 29);
        var calculator = CreateWithDate(blackFriday);

        // Act
        var result = calculator.CalculateDiscount(100m, 1, CustomerLocation.EUROPE, blackFriday);

        // Assert
        result.Should().Be(86.25m);
    }

    // 100 * 1.05 = 105, then 15% off → 105 * 0.85 = 89.25
    [Fact]
    public void CalculateDiscount_WithAsiaLocationOnPolishHoliday_ShouldApplyMultiplierThenDiscount()
    {
        // Arrange
        var holiday = new DateTime(2024, 12, 25); // Christmas
        var calculator = CreateWithDate(holiday);

        // Act
        var result = calculator.CalculateDiscount(100m, 1, CustomerLocation.ASIA, holiday);

        // Assert
        result.Should().Be(89.25m);
    }

    #endregion
}
