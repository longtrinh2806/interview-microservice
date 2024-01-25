using Contracts;
using Mapster;
using MassTransit;
using OrderServices.Data;

namespace OrderServices.Consumers
{
    public class ProductUpdatedConsumer : IConsumer<ProductUpdated>
    {
        private readonly OrderDbContext _dbContext;

        public ProductUpdatedConsumer(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<ProductUpdated> context)
        {
            Console.WriteLine($"Consuming product updated: {context.Message.Id}");

            var product = _dbContext.Products.Find(context.Message.Id);

            if (product == null)
            {
                throw new Exception($"Do not have product with id: {context.Message.Id} in OrderDB");
            }

            product.Code = context.Message.Code;
            product.Name = context.Message.Name;
            product.Price = context.Message.Price;

            _dbContext.Products.Update(product);

            await _dbContext.SaveChangesAsync();
        }
    }
}
