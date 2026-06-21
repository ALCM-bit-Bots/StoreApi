using StoreApi.Domain.Entities;
using StoreApi.Domain.Repositories;

namespace StoreApi.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();
    private readonly object _lock = new();
    private int _nextId = 1;

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<Product>>(_products.ToList());
        }
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        lock (_lock)
        {
            return Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
        }
    }

    public async Task<Product> AddAsync(Product product)
    {
        lock (_lock)
        {
            product.Id = _nextId++;
            _products.Add(product);
            return product;
        }
    }

    public async Task<Product?> UpdateAsync(int id, Product product)
    {
        lock (_lock)
        {
            var index = _products.FindIndex(p => p.Id == id);
            if (index == -1)
                return null;

            product.Id = id;
            _products[index] = product;
            return product;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        lock (_lock)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return false;

            _products.Remove(product);
            return true;
        }
    }
}
