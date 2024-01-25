using Contracts;
using MassTransit;
using OrderServices.Data;

namespace OrderServices.Consumers
{
    public class ProductDeletedConsumer : IConsumer<ProductDeleted>
    {
        private readonly OrderDbContext _dbContext;

        public ProductDeletedConsumer(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<ProductDeleted> context)
        {
            Console.WriteLine($"Consuming product deleted: {context.Message.Id}");

            var product = _dbContext.Products.FirstOrDefault(x => x.Id.Equals(context.Message.Id));

            if (product == null)
            {
                throw new Exception($"Do not have product with id: {context.Message.Id} in OrderDB");
            }

            _dbContext.Products.Remove(product);

            await _dbContext.SaveChangesAsync();

        }
    }
}
