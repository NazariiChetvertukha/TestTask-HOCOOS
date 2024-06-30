using ConsoleApp.Interfaces;

namespace ConsoleApp.Models;

public record Product(long Id, string Name, string Description, decimal Price)
    : IPrimaryEntity
{
    public long Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    public string Description { get; set; } = Description;
    public decimal Price { get; set; } = Price;
}