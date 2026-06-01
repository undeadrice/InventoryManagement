using InventoryManagement.Domain.Customers;

namespace InventoryManagement.Domain.Orders.Services;

public class DiscountCalculator : IDiscountCalculator
{
    private readonly Func<DateTime> _getCurrentDate;

    public DiscountCalculator(Func<DateTime>? getCurrentDate = null)
    {
        _getCurrentDate = getCurrentDate ?? (() => DateTime.UtcNow);
    }

    public decimal CalculateDiscount(decimal basePrice, int totalQuantity, CustomerLocation location, DateTime orderDate)
    {
        var applicableDiscounts = new List<(string Name, decimal Percentage)>();

        var volumeDiscount = GetVolumeDiscount(totalQuantity);
        if (volumeDiscount > 0)
        {
            applicableDiscounts.Add(("Volume", volumeDiscount));
        }

        var seasonalDiscount = GetSeasonalDiscount(orderDate);
        if (seasonalDiscount > 0)
        {
            applicableDiscounts.Add(("Seasonal", seasonalDiscount));
        }

        var highestDiscount = applicableDiscounts.Any()
            ? applicableDiscounts.OrderByDescending(d => d.Percentage).First().Percentage
            : 0;

        var locationMultiplier = GetLocationMultiplier(location);
        var priceWithLocationAdjustment = basePrice * locationMultiplier;

        var discountAmount = priceWithLocationAdjustment * (highestDiscount / 100);
        var finalPrice = priceWithLocationAdjustment - discountAmount;

        return finalPrice;
    }

    private decimal GetVolumeDiscount(int quantity)
    {
        if (quantity >= 50)
        {
            return 30;
        }

        if (quantity >= 10)
        {
            return 20;
        }

        if (quantity >= 5)
        {
            return 10;
        }

        return 0;
    }

    private decimal GetSeasonalDiscount(DateTime orderDate)
    {
        var currentDate = _getCurrentDate();

        if (IsBlackFriday(currentDate))
        {
            return 25;
        }
        

        if (IsPolishHoliday(currentDate))
        {
            return 15;
        }
           
        return 0;
    }

    private bool IsBlackFriday(DateTime date)
    {
        if (date.Month != 11)
        {
            return false;
        }

        var thursdayCount = 0;

        for (int day = 1; day <= 30; day++)
        {
            var currentDay = new DateTime(date.Year, 11, day);
            if (currentDay.DayOfWeek == DayOfWeek.Thursday)
            {
                thursdayCount++;
                if (thursdayCount == 4)
                {
                    var blackFriday = currentDay.AddDays(1);
                    return date.Date == blackFriday.Date;
                }
            }
        }

        return false;
    }

    private bool IsPolishHoliday(DateTime date)
    {
        return (date.Month == 1 && date.Day == 1) ||   // New Year's Day
               (date.Month == 1 && date.Day == 6) ||   // Epiphany
               (date.Month == 5 && date.Day == 1) ||   // Labour Day
               (date.Month == 5 && date.Day == 3) ||   // Constitution Day
               (date.Month == 8 && date.Day == 15) ||  // Assumption of Mary
               (date.Month == 11 && date.Day == 1) ||  // All Saints' Day
               (date.Month == 11 && date.Day == 11) || // Independence Day
               (date.Month == 12 && date.Day == 25) || // Christmas Day
               (date.Month == 12 && date.Day == 26);   // Second Day of Christmas
    }

    private decimal GetLocationMultiplier(CustomerLocation location)
    {
        return location switch
        {
            CustomerLocation.US => 1.0m,
            CustomerLocation.EUROPE => 1.15m, // 15% VAT
            CustomerLocation.ASIA => 1.05m,   // 5% logistics cost
            _ => 1.0m
        };
    }
}
