using Contracts;
using Mapster;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductServices.Data;
using ProductServices.Dtos;
using ProductServices.Entities;

namespace ProductServices.Services
{
    public interface IProductService
    {
        List<ProductDto> Get();
        ProductDto GetById(Guid id);
        Task<ProductDto> Post(CreateProductDto request);
        Task<ProductDto> Put(Guid id, UpdateProductDto request);
        Task<string> Delete(Guid id);
    }
    public class ProductService : IProductService
    {
        private readonly ProductDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public ProductService(ProductDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public List<ProductDto> Get()
        {
            try
            {
                var products = _context.Products.ToList().Adapt<List<ProductDto>>();
                return products;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ProductDto GetById(Guid id)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(x => x.Id.Equals(id)).Adapt<ProductDto>();
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProductDto> Post(CreateProductDto request)
        {
            try
            {
                var product = request.Adapt<Product>();

                _context.Products.Add(product);

                var productCreated = product.Adapt<ProductCreated>();

                await _publishEndpoint.Publish(productCreated);

                _context.SaveChanges();

                var productDto = product.Adapt<ProductDto>();
                return productDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProductDto> Put(Guid id, UpdateProductDto request)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var result = _context.Products.FirstOrDefault(x => x.Id.Equals(id));

                if (result == null)
                    throw new Exception("Invalid Id");

                request.Adapt(result);
                
                _context.Update(result);

                var productUpdated = result.Adapt<ProductUpdated>();

                await _publishEndpoint.Publish(productUpdated);

                _context.SaveChanges();

                transaction.Commit();
                return result.Adapt<ProductDto>();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> Delete(Guid id)
        {
            try
            {
                var result = _context.Products.FirstOrDefault(x => x.Id.Equals(id));

                if (result != null)
                {
                    _context.Remove(result);

                    var productDeleted = new ProductDeleted
                    {
                        Id = id
                    };

                    await _publishEndpoint.Publish(productDeleted);

                    _context.SaveChanges();

                    return "Delete Successfully";
                }

                return "Product not found for deletion.";
            }
            catch (DbUpdateException ex)
            {
                // Handle DbUpdateException or other specific exceptions
                return $"Error deleting product. Details: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Handle other generic exceptions
                return $"An error occurred. Details: {ex.Message}";
            }
        }

    }
}
