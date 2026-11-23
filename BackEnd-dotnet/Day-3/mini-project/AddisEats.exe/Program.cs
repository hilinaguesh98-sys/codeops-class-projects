            
using System;
using System.Collections.Generic;
using System.Linq;

namespace AddisEats.Catalog
{
    //  1
    public static class Delivery
    {
        public static decimal DeliveryFee(string area, bool isVip = false)
        {
            if (isVip)
                return 0m;

            return area switch
            {
                "Bole" => 50m,
                "Kazanchis" => 40m,
                "Lideta" => 60m,
                "Piassa" => 45m,
                _ => 70m
            };
        }
    }

    //2
    public static class Descriptions
    {
        public static void Describe(int value)
        {
            Console.WriteLine($"Integer overload: {value}");
        }

        public static void Describe(string value)
        {
            Console.WriteLine($"String overload: {value}");
        }

        public static void Describe(decimal value)
        {
            Console.WriteLine($"Decimal overload: {value}");
        }

        /*
        FOURTH OVERLOAD (INVALID):

        public static string Describe(int value)
        {
            return value.ToString();
        }

        COMPILER ERROR: CS0111

        C# does not allow method overloading
        using only a different return type.
        */
    }

    // 3
    public class Restaurant
    {
        public int Id { get; init; }

        public string Name { get; set; } = "";

        public string Area { get; set; } = "";

        public string DisplayName => $"{Name} ({Area})";
    }

    // 4
    public record Customer(string Name, string PhoneNumber);

    // Customer converted into a normal class
    public class CustomerClass
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public CustomerClass(string name, string phoneNumber)
        {
            Name = name;
            PhoneNumber = phoneNumber;
        }
    }

    // 5
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Area { get; set; }
        public decimal AmountETB { get; set; }
        public DateTime CreatedAt { get; set; }

        public Order(
            int id,
            string customerName,
            string area,
            decimal amountETB,
            DateTime createdAt)
        {
            Id = id;
            CustomerName = customerName;
            Area = area;
            AmountETB = amountETB;
            CreatedAt = createdAt;
        }
    }

    //6
    public interface IDiscountPolicy
    {
        decimal Apply(decimal amount);
    }

    // First implementation: 10% discount
    public class TenPercentDiscount : IDiscountPolicy
    {
        public decimal Apply(decimal amount)
        {
            return amount * 0.90m;
        }
    }

    // Second implementation: 50 ETB discount
    public class FiftyBirrDiscount : IDiscountPolicy
    {
        public decimal Apply(decimal amount)
        {
            return Math.Max(0m, amount - 50m);
        }
    }

    // Third implementation: 5% discount
    public class FivePercentDiscount : IDiscountPolicy
    {
        public decimal Apply(decimal amount)
        {
            return amount * 0.95m;
        }
    }

    // One method works with all discount policies
    public static class DiscountCalculator
    {
        public static decimal ApplyDiscount(
            decimal amount,
            IDiscountPolicy policy)
        {
            ArgumentNullException.ThrowIfNull(policy);

            return policy.Apply(amount);
        }
    }

    // 7
    public static class PaymentValidator
    {
        public static void ValidateAmount(decimal amountETB)
        {
            if (amountETB <= 0m)
            {
                throw new ArgumentException(
                    "ETB amount must be greater than zero.",
                    nameof(amountETB));
            }
        }
    }

    // 8
    internal class Program
    {
        static void Main(string[] args)
        {
            
            // Number 1

            Console.WriteLine("Number 1 ");

            // Positional call using default parameter
            Console.WriteLine(
                $"Bole: {Delivery.DeliveryFee("Bole")} ETB");

            // Named argument
            Console.WriteLine(
                $"Kazanchis VIP: {Delivery.DeliveryFee("Kazanchis", isVip: true)} ETB");

            // Positional call with both arguments
            Console.WriteLine(
                $"Lideta: {Delivery.DeliveryFee("Lideta", false)} ETB");


          
            // Number 2
           

            Console.WriteLine("\n Number 2 ");

            Descriptions.Describe(25);
            Descriptions.Describe("Doro Wot");
            Descriptions.Describe(125.75m);


           
            // Number 3
            

            Console.WriteLine("\n Number 3 ");

            Restaurant restaurant = new Restaurant
            {
                Id = 1,
                Name = "Addis Kitchen",
                Area = "Bole"
            };

            Console.WriteLine($"ID: {restaurant.Id}");
            Console.WriteLine($"Name: {restaurant.Name}");
            Console.WriteLine($"Display Name: {restaurant.DisplayName}");

            // Name and Area can be changed
            restaurant.Name = "Habesha Restaurant";
            restaurant.Area = "Kazanchis";

            Console.WriteLine($"Updated Name: {restaurant.Name}");
            Console.WriteLine($"Updated Display Name: {restaurant.DisplayName}");

            // restaurant.Id = 2;
            // Error: Id is init-only.


            // 
            //  4: 
            // 

            Console.WriteLine("\n Number 4 ");

            Customer customer1 =
                new Customer("Abebe", "0911000000");

            Customer customer2 =
                new Customer("Abebe", "0911000000");

            Console.WriteLine(
                $"Record equality: {customer1 == customer2}");

            CustomerClass classCustomer1 =
                new CustomerClass("Abebe", "0911000000");

            CustomerClass classCustomer2 =
                new CustomerClass("Abebe", "0911000000");

            Console.WriteLine(
                $"Class equality: {classCustomer1 == classCustomer2}");

            Console.WriteLine(
                "Records compare values; normal classes compare references by default.");


            // 
            // 5:
            // 

            Console.WriteLine("\n Number 5 ");

            List<Order> orders = new List<Order>
            {
                new Order(1, "Abebe", "Bole", 750m,
                    new DateTime(2026, 11, 10)),

                new Order(2, "Betty", "Kazanchis", 1200m,
                    new DateTime(2026, 11, 12)),

                new Order(3, "Dawit", "Bole", 450m,
                    new DateTime(2026, 11, 13)),

                new Order(4, "Sara", "Kazanchis", 800m,
                    new DateTime(2026, 11, 15)),

                new Order(5, "Mimi", "Lideta", 300m,
                    new DateTime(2026, 11, 16)),

                new Order(6, "Abebe", "Bole", 900m,
                    new DateTime(2026, 11, 17)),

                new Order(7, "Kalkidan", "Kazanchis", 650m,
                    new DateTime(2026, 11, 18)),

                new Order(8, "Hana", "Bole", 1100m,
                    new DateTime(2026, 11, 9)),

                new Order(9, "Yonas", "Lideta", 500m,
                    new DateTime(2026, 11, 8)),

                new Order(10, "Meron", "Kazanchis", 400m,
                    new DateTime(2026, 11, 7)),

                new Order(11, "Tigist", "Bole", 620m,
                    new DateTime(2026, 11, 6)),

                new Order(12, "Robel", "Kazanchis", 950m,
                    new DateTime(2026, 11, 5)),

                new Order(13, "Selam", "Bole", 510m,
                    new DateTime(2026, 11, 4)),

                new Order(14, "Biruk", "Kazanchis", 700m,
                    new DateTime(2026, 11, 3)),

                new Order(15, "Liya", "Lideta", 250m,
                    new DateTime(2026, 11, 2))
            };


            // 1. Bole orders over 500 ETB

            var boleOrders = orders
                .Where(o => o.Area == "Bole" && o.AmountETB > 500m)
                .ToList();

            Console.WriteLine("\nBole Orders Over 500 ETB:");

            foreach (var order in boleOrders)
            {
                Console.WriteLine(
                    $"Order {order.Id}: {order.AmountETB} ETB");
            }


            // 2. Customer names sorted alphabetically

            var customerNames = orders
                .Select(o => o.CustomerName)
                .OrderBy(name => name)
                .ToList();

            Console.WriteLine("\nCustomer Names Alphabetically:");

            foreach (var name in customerNames)
            {
                Console.WriteLine(name);
            }


            // 3. Total revenue for Kazanchis

            decimal kazanchisRevenue = orders
                .Where(o => o.Area == "Kazanchis")
                .Sum(o => o.AmountETB);

            Console.WriteLine(
                $"\nKazanchis Revenue: {kazanchisRevenue} ETB");


            // 4. Page 3, 10 orders per page, newest first

            int pageNumber = 3;
            int pageSize = 10;

            var pageThree = orders
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            Console.WriteLine("\nPage 3 - Newest First:");

            foreach (var order in pageThree)
            {
                Console.WriteLine(
                    $"{order.CreatedAt:yyyy-MM-dd} | " +
                    $"{order.CustomerName} | {order.AmountETB} ETB");
            }


            // 
            // Number 6
            // 

            Console.WriteLine("\n Number 6 ");

            decimal amount = 500m;

            ShowDiscount(
                "10% Discount",
                amount,
                new TenPercentDiscount());

            ShowDiscount(
                "50 ETB Discount",
                amount,
                new FiftyBirrDiscount());

            ShowDiscount(
                "5% Discount",
                amount,
                new FivePercentDiscount());


            // 
            // Number 7 
            // 

            Console.WriteLine("\n Number 7 ");

            // Valid amount
            TestPayment(250m);

            // Invalid amount
            TestPayment(0m);


            // 
            // COMPLETION
            // 

            Console.WriteLine("\nAll Numbers completed!");
        }


        // Accepts any class implementing IDiscountPolicy
        static void ShowDiscount(
            string label,
            decimal amount,
            IDiscountPolicy policy)
        {
            decimal result =
                DiscountCalculator.ApplyDiscount(amount, policy);

            Console.WriteLine(
                $"{label}: {amount} ETB -> {result} ETB");
        }


        // Demonstrates try, catch, and finally
        static void TestPayment(decimal amountETB)
        {
            Console.WriteLine(
                $"\nChecking payment: {amountETB} ETB");

            try
            {
                PaymentValidator.ValidateAmount(amountETB);

                Console.WriteLine("Payment amount is valid.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(
                    $"Exception caught: {ex.Message}");
            }
            finally
            {
                Console.WriteLine(
                    "Finally block executed.");
            }
        }
    }
}
