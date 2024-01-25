using Mapster;
using OrderServices.Data;
using OrderServices.Dtos;
using OrderServices.Entities;
using System;

namespace OrderServices.Services
{
    public interface IOrderService
    {
        ResponseModel Create(List<OrderDetailDto> request);
    }
    public class OrderService : IOrderService
    {
        private readonly OrderDbContext _dbContext;

        public OrderService(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResponseModel Create(List<OrderDetailDto> request)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                if (request.Any(tmp => tmp.Quantity < 1 || tmp.Quantity > 99))
                {
                    throw new Exception("Invalid Quantity");
                }

                using var transaction = _dbContext.Database.BeginTransaction();

                var order = new Order
                {
                    Code = DateTime.Now.Ticks.ToString(),
                    TotalPrice = 0
                };

                List<OrderDetail> orderDetails = new();

                request.ForEach(o =>
                {
                    var p = _dbContext.Products.FirstOrDefault(tmp => tmp.Id.Equals(o.ProductId));
                    if (p == null)
                        throw new Exception("Product not existed");

                    if (p.Quantity < o.Quantity)
                        throw new Exception("Out of stock");

                    order.TotalPrice += p.Price * o.Quantity;

                    p.Quantity -= o.Quantity;

                    _dbContext.Update(p);

                    var detail = o.Adapt<OrderDetail>();

                    orderDetails.Add(detail);
                });

                order.Date = DateTime.UtcNow;

                _dbContext.Orders.Add(order);

                orderDetails.ForEach(od =>
                {
                    od.OrderId = order.Id;
                    _dbContext.OrderDetails.Add(od);
                });

                _dbContext.SaveChanges();

                transaction.Commit();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Create Successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = ex.Message
                };
            }

        }
    }
}
