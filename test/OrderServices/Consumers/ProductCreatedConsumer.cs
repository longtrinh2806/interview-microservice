using Contracts;
using Mapster;
using MassTransit;
using OrderServices.Data;
using OrderServices.Entities;

namespace OrderServices.Consumers
{
    public class ProductCreatedConsumer : IConsumer<ProductCreated>
    {
        private readonly OrderDbContext _dbContext;

        public ProductCreatedConsumer(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<ProductCreated> context)
        {
            Console.WriteLine($"Consuming product created: {context.Message.Id}");

            var product = context.Message.Adapt<Product>();

            _dbContext.Add(product);
            await _dbContext.SaveChangesAsync();

        }
    }
}
