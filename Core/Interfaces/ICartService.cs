using System;
using Core.Entities;

namespace Core.Interfaces;

public interface ICartService
{
    Task<ShoppingCart?> GetCardAsync(string key);
    Task<ShoppingCart?> SetCardAsync(ShoppingCart cart);
    Task<bool> DeleteCardAsync(string key);
}
