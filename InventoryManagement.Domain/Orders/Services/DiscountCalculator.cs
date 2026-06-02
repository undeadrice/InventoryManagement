using InventoryManagement.Domain.Customers;

namespace InventoryManagement.Domain.Orders.Services;

public class DiscountCalculator : IDiscountCalculator
{
    public DiscountCalculator()
    {
    }

    public decimal CalculateDiscount(IEnumerable<OrderLineItem> items, CustomerLocation location, DateTime orderDate)
    {
        var lineItems = items.ToList();

        if (lineItems.Count == 0)
        {
            return 0;
        }

        var locationMultiplier = GetLocationMultiplier(location);

        var adjustedItems = lineItems
            .Select(i => new OrderLineItem(i.UnitPrice * locationMultiplier, i.Quantity))
            .ToList();

        var totalQuantity = adjustedItems.Sum(i => i.Quantity);
        var adjustedTotal = adjustedItems.Sum(i => i.UnitPrice * i.Quantity);

        var discounts = new List<(string Name, decimal Percentage)>();

        var volumeDiscount = GetVolumeDiscount(totalQuantity);
        if (volumeDiscount > 0)
        {
            discounts.Add(("Volume", volumeDiscount));
        }

        var (seasonalName, seasonalDiscount) = GetSeasonalDiscount(orderDate);
        if (seasonalDiscount > 0)
        {
            discounts.Add((seasonalName, seasonalDiscount));
        }

        if (!discounts.Any())
        {
            return adjustedTotal;
        }

        var best = discounts.OrderByDescending(d => d.Percentage).First();

        if (best.Name == "Holiday")
        {
            return CalculateHolidayDiscount(adjustedItems, adjustedTotal);
        }

        return adjustedTotal * (1 - best.Percentage / 100m);
    }

    private static decimal CalculateHolidayDiscount(List<OrderLineItem> adjustedItems, decimal adjustedTotal)
    {
        var mostExpensive = adjustedItems.MaxBy(i => i.UnitPrice)!;
        var lineTotalOfMostExpensive = mostExpensive.UnitPrice * mostExpensive.Quantity;
        var discountOnMostExpensive = lineTotalOfMostExpensive * 0.15m;
        return adjustedTotal - discountOnMostExpensive;
    }

    private static decimal GetVolumeDiscount(int quantity)
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

    private static (string Name, decimal Percentage) GetSeasonalDiscount(DateTime orderDate)
    {
        if (IsBlackFriday(orderDate))
        {
            return ("BlackFriday", 25);
        }

        if (IsPolishHoliday(orderDate))
        {
            return ("Holiday", 15);
        }

        return (string.Empty, 0);
    }

    private static bool IsBlackFriday(DateTime date)
    {
        if (date.Month != 11)
        {
            return false;
        }

        var thursdayCount = 0;
        for (int day = 1; day <= 30; day++)
        {
            var current = new DateTime(date.Year, 11, day);
            if (current.DayOfWeek == DayOfWeek.Thursday)
            {
                thursdayCount++;
                if (thursdayCount == 4)
                {
                    return date.Date == current.AddDays(1).Date;
                }
            }
        }

        return false;
    }

    private static bool IsPolishHoliday(DateTime date) =>
        (date.Month == 1 && date.Day == 1) || // New Year's Day
        (date.Month == 1 && date.Day == 6) || // Epiphany
        (date.Month == 5 && date.Day == 1) || // Labour Day
        (date.Month == 5 && date.Day == 3) || // Constitution Day
        (date.Month == 8 && date.Day == 15) || // Assumption of Mary
        (date.Month == 11 && date.Day == 1) || // All Saints' Day
        (date.Month == 11 && date.Day == 11) || // Independence Day
        (date.Month == 12 && date.Day == 25) || // Christmas Day
        (date.Month == 12 && date.Day == 26);   // Second Day of Christmas

    private static decimal GetLocationMultiplier(CustomerLocation location) =>
        location switch
        {
            CustomerLocation.US => 1.0m,
            CustomerLocation.EUROPE => 1.15m,
            CustomerLocation.ASIA => 1.05m,
            _ => 1.0m
        };
}
