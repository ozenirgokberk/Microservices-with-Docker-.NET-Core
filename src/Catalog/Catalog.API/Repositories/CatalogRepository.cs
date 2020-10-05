using Catalog.API.Data.Interfaces;
using Catalog.API.Entities.Repositories.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Entities.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly ICatalogContext _context;

        public CatalogRepository(ICatalogContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products.Find(f=> true).ToListAsync();
        }

        public async Task<Product> GetProduct(string id)
        {
            return await _context.Products.Find(f => f.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetCatalogsByName(string name)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Name, name);
            return await _context.Products.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetCatalogsByCategoryName(string categoryName)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);
            return await _context.Products.Find(filter).ToListAsync();
        }

        public async Task Create(Product product)
        {
            await _context.Products.InsertOneAsync(product);
        }

        public async Task<bool> Update(Product product)
        {
            var updateResult = await _context.Products.ReplaceOneAsync(filter: g => g.Id == product.Id, replacement: product);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
        public async Task<bool> Delete(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            DeleteResult deleteResult = await _context.Products.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }
    }
}
