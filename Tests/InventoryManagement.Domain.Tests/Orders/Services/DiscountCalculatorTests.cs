using FluentAssertions;
using InventoryManagement.Domain.Customers;
using InventoryManagement.Domain.Orders.Services;
using Xunit;

namespace InventoryManagement.Domain.Tests.Orders.Services;

public class DiscountCalculatorTests
{
    private static readonly DiscountCalculator Calculator = new DiscountCalculator();

    private static readonly DateTime NeutralDate = new DateTime(2024, 6, 15);


    private static IEnumerable<OrderLineItem> SingleItem(decimal unitPrice, int quantity = 1)
        => [new OrderLineItem(unitPrice, quantity)];

    private static IEnumerable<OrderLineItem> Items(params (decimal price, int quantity)[] lines)
        => lines.Select(l => new OrderLineItem(l.price, l.quantity));

    #region No Discount Scenarios

    [Fact]
    public void CalculateDiscount_WithUSLocationAndNoDiscounts_ShouldReturnBasePrice()
    {

        var result = Calculator.CalculateDiscount(SingleItem(100m), CustomerLocation.US, NeutralDate);

        result.Should().Be(100m);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    public void CalculateDiscount_WithQuantityBelowVolumeThreshold_ShouldApplyNoVolumeDiscount(int quantity)
    {

        var result = Calculator.CalculateDiscount(SingleItem(100m, quantity), CustomerLocation.US, NeutralDate);

        result.Should().Be(100 * quantity);
    }

    #endregion

    #region Volume Discounts

    [Theory]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(9)]
    public void CalculateDiscount_WithQuantityBetween5And9_ShouldApply10PercentVolumeDiscount(int quantity)
    {
        var result = Calculator.CalculateDiscount(SingleItem(100m, quantity), CustomerLocation.US, NeutralDate);

        result.Should().Be(100m * quantity * 0.90m);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(49)]
    public void CalculateDiscount_WithQuantityBetween10And49_ShouldApply20PercentVolumeDiscount(int quantity)
    {
        var result = Calculator.CalculateDiscount(SingleItem(100m, quantity), CustomerLocation.US, NeutralDate);

        result.Should().Be(100 * quantity * 0.80m);
    }

    [Theory]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(200)]
    public void CalculateDiscount_WithQuantity50OrMore_ShouldApply30PercentVolumeDiscount(int quantity)
    {

        var result = Calculator.CalculateDiscount(SingleItem(100m, quantity), CustomerLocation.US, NeutralDate);

        result.Should().Be(100 * quantity * 0.70m);
    }

    #endregion

    #region Location Multipliers

    [Fact]
    public void CalculateDiscount_WithUSLocation_ShouldApplyNoLocationMultiplier()
    {
        var result = Calculator.CalculateDiscount(SingleItem(100m), CustomerLocation.US, NeutralDate);

        result.Should().Be(100);
    }

    [Fact]
    public void CalculateDiscount_WithEuropeLocation_ShouldApply15PercentVATMultiplier()
    {
        var result = Calculator.CalculateDiscount(SingleItem(100m), CustomerLocation.EUROPE, NeutralDate);

        result.Should().Be(115);
    }

    [Fact]
    public void CalculateDiscount_WithAsiaLocation_ShouldApply5PercentLogisticsMultiplier()
    {

        var result = Calculator.CalculateDiscount(SingleItem(100m), CustomerLocation.ASIA, NeutralDate);

        result.Should().Be(105);
    }

    // 100 * 1.15 = 115, then 20% off total (10 units) › 115 * 10 * 0.80 = 920
    [Fact]
    public void CalculateDiscount_WithEuropeLocationAndVolumeDiscount_ShouldApplyMultiplierBeforeDiscount()
    {
        var result = Calculator.CalculateDiscount(SingleItem(100m, 10), CustomerLocation.EUROPE, NeutralDate);

        result.Should().Be(920);
    }

    // 100 * 1.05 = 105, then 20% off total (10 units) › 105 * 10 * 0.80 = 840
    [Fact]
    public void CalculateDiscount_WithAsiaLocationAndVolumeDiscount_ShouldApplyMultiplierBeforeDiscount()
    {
        var result = Calculator.CalculateDiscount(SingleItem(100m, 10), CustomerLocation.ASIA, NeutralDate);

        result.Should().Be(840);
    }

    #endregion

    #region Seasonal Discounts — Polish Holidays (15% off most expensive product)

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
    public void CalculateDiscount_OnPolishHolidaySingleProduct_ShouldApply15PercentToThatProduct(int month, int day)
    {
        var holiday = new DateTime(2024, month, day);

        var result = Calculator.CalculateDiscount(SingleItem(100m), CustomerLocation.US, holiday);

        result.Should().Be(85);
    }

    // Multi-product order: 15% applies ONLY to the most expensive product's line total.
    // Products: $200 × 1 (most expensive), $50 × 2
    // Total before discount = 200 + 100 = 300
    // Holiday discount = 15% × (200 × 1) = 30
    // Final = 300 - 30 = 270
    [Fact]
    public void CalculateDiscount_OnPolishHolidayMultipleProducts_ShouldApply15PercentOnlyToMostExpensiveProduct()
    {
        var holiday = new DateTime(2024, 1, 1); // New Year's Day

        var result = Calculator.CalculateDiscount(
            Items((200m, 1), (50m, 2)),
            CustomerLocation.US,
            holiday);

        result.Should().Be(270);
    }

    // Most expensive product has multiple units — 15% applies to the full line (unit × qty).
    // Products: $100 × 3 (most expensive unit price, line = 300), $40 × 2 (line = 80)
    // Total = 380
    // Holiday discount = 15% × 300 = 45
    // Final = 380 - 45 = 335
    [Fact]
    public void CalculateDiscount_OnPolishHolidayMostExpensiveHasMultipleUnits_ShouldApply15PercentToFullLineTotal()
    {
        var holiday = new DateTime(2024, 5, 1); // Labour Day

        var result = Calculator.CalculateDiscount(
            Items((100m, 3), (40m, 2)),
            CustomerLocation.US,
            holiday);

        result.Should().Be(335);
    }

    #endregion

    #region Seasonal Discounts — Black Friday (25% off entire order)

    [Fact]
    public void CalculateDiscount_OnBlackFriday2024_ShouldApply25PercentToEntireOrder()
    {
        var blackFriday = new DateTime(2024, 11, 29);

        var result = Calculator.CalculateDiscount(SingleItem(100m), CustomerLocation.US, blackFriday);

        result.Should().Be(75);
    }

    [Fact]
    public void CalculateDiscount_OnBlackFriday2023_ShouldApply25PercentToEntireOrder()
    {
        var blackFriday = new DateTime(2023, 11, 24);

        var result = Calculator.CalculateDiscount(SingleItem(100m), CustomerLocation.US, blackFriday);

        result.Should().Be(75);
    }

    [Fact]
    public void CalculateDiscount_OnFridayInNovemberThatIsNotBlackFriday_ShouldNotApplyBlackFridayDiscount()
    {
        var thirdFriday = new DateTime(2024, 11, 15);

        var result = Calculator.CalculateDiscount(SingleItem(100m), CustomerLocation.US, thirdFriday);

        result.Should().Be(100);
    }

    // Products: $200 × 1, $50 × 2 › total = 300, 25% off › 225
    [Fact]
    public void CalculateDiscount_OnBlackFridayMultipleProducts_ShouldApply25PercentToEntireOrder()
    {

        var blackFriday = new DateTime(2024, 11, 29);

        var result = Calculator.CalculateDiscount(
            Items((200m, 1), (50m, 2)),
            CustomerLocation.US,
            blackFriday);

        result.Should().Be(225);
    }

    #endregion

    #region Discount Priority — Only Highest Applies

    // Polish holiday gives 15% (holiday, on most expensive), quantity 50 gives 30% (full order) › 30% wins
    // 50 units at $100 = $5000 base; volume 30% › $3500
    [Fact]
    public void CalculateDiscount_WhenVolumeDiscountHigherThanSeasonal_ShouldApplyVolumeDiscount()
    {
        var holiday = new DateTime(2024, 1, 1); // New Year's Day

        var result = Calculator.CalculateDiscount(SingleItem(100m, 50), CustomerLocation.US, holiday);

        result.Should().Be(3500);
    }

    // Black Friday gives 25%, quantity 5 gives 10% › 25% wins
    // 5 units at $100 = $500 base; Black Friday 25% › $375
    [Fact]
    public void CalculateDiscount_WhenSeasonalDiscountHigherThanVolume_ShouldApplySeasonalDiscount()
    {
        var blackFriday = new DateTime(2024, 11, 29);

        var result = Calculator.CalculateDiscount(SingleItem(100m, 5), CustomerLocation.US, blackFriday);

        result.Should().Be(375);
    }

    // Polish holiday gives 15% (on most expensive item), quantity 5 gives 10% (on full order)
    // 15% > 10%, so holiday wins — and it applies only to the most expensive product.
    // Single product: 5 units × $100 = $500 total; holiday takes 15% of most expensive line ($500) › saves $75 › $425
    // Holiday wins (15% > 10%); applied to only the most expensive product line = 5×100 = 500 › 15% off = 75 saved › 425
    [Fact]
    public void CalculateDiscount_WhenHolidayDiscountHigherThanVolume_ShouldApplyHolidayDiscount()
    {
        var holiday = new DateTime(2024, 5, 1); // Labour Day

        var result = Calculator.CalculateDiscount(SingleItem(100m, 5), CustomerLocation.US, holiday);

        result.Should().Be(425);
    }

    // Black Friday gives 25%, quantity 50 gives 30% › 30% wins
    // 50 units × $100 = $5000; volume 30% › $3500
    [Fact]
    public void CalculateDiscount_WhenBlackFridayAndHighVolumeDiscount_ShouldApplyHighestDiscount()
    {
        var blackFriday = new DateTime(2024, 11, 29);

        var result = Calculator.CalculateDiscount(SingleItem(100m, 50), CustomerLocation.US, blackFriday);

        result.Should().Be(3500);
    }

    #endregion

    #region Combined Location + Discount

    // 100 * 1.15 = 115 (Europe), then Black Friday 25% off › 115 * 0.75 = 86.25
    [Fact]
    public void CalculateDiscount_WithEuropeLocationOnBlackFriday_ShouldApplyMultiplierThenDiscount()
    {
        var blackFriday = new DateTime(2024, 11, 29);

        var result = Calculator.CalculateDiscount(SingleItem(100m), CustomerLocation.EUROPE, blackFriday);

        result.Should().Be(86.25m);
    }

    // 100 * 1.05 = 105 (Asia), holiday 15% off most expensive (only item) › 105 * 0.85 = 89.25
    [Fact]
    public void CalculateDiscount_WithAsiaLocationOnPolishHoliday_ShouldApplyMultiplierThenDiscount()
    {
        var holiday = new DateTime(2024, 12, 25); // Christmas

        var result = Calculator.CalculateDiscount(SingleItem(100m), CustomerLocation.ASIA, holiday);

        result.Should().Be(89.25m);
    }

    // Europe + holiday + multi-product:
    // Products: $200 × 1, $50 × 2
    // After Europe multiplier: $230 × 1, $57.5 × 2 › total = $345
    // Holiday 15% on most expensive line ($230 × 1 = $230) › saves $34.5
    // Final = 345 - 34.5 = 310.5
    [Fact]
    public void CalculateDiscount_WithEuropeLocationOnHolidayMultipleProducts_ShouldApplyMultiplierThenHolidayOnMostExpensive()
    {
        var holiday = new DateTime(2024, 1, 1); // New Year's Day

        var result = Calculator.CalculateDiscount(
            Items((200m, 1), (50m, 2)),
            CustomerLocation.EUROPE,
            holiday);

        result.Should().Be(310.5m);
    }

    #endregion
}
