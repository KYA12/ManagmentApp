using Grpc.Core;
using AutoMapper;
using BakeryOrderManagementSystem.Grpc;
using Empty = BakeryOrderManagementSystem.Grpc.Empty;
using Microsoft.EntityFrameworkCore;
using BakeryOrderManagmentSystem.Models;

public class BakeryGrpcService : BakeryService.BakeryServiceBase
{
    private readonly BakeryDbContext _context;
    private readonly IMapper _mapper;

    public BakeryGrpcService(BakeryDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public override async Task<ProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = (decimal)request.Price
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProductResponse>(product);
    }

    public override async Task<ProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        var product = await _context.Products.FindAsync(request.ProductId);
        if (product == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));
        }

        return _mapper.Map<ProductResponse>(product);
    }

    public override async Task<ProductResponse> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
    {
        var product = await _context.Products.FindAsync(request.ProductId);
        if (product == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = (decimal)request.Price;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProductResponse>(product);
    }

    public override async Task<Empty> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
    {
        var product = await _context.Products.FindAsync(request.ProductId);
        if (product == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return new Empty();
    }

    public override async Task<ProductListResponse> GetAllProducts(Empty request, ServerCallContext context)
    {
        var products = await _context.Products.ToListAsync();
        var productResponses = products.Select(p => _mapper.Map<ProductResponse>(p)).ToList();

        return new ProductListResponse { Products = { productResponses } };
    }

    public override async Task<OrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        var order = new Order
        {
            CustomerId = request.CustomerId,
            OrderDate = DateTime.Now,
            Status = (BakeryOrderManagmentSystem.Models.OrderStatus)request.Status,
            OrdersProducts = request.OrderProducts.Select(op => new OrdersProducts
            {
                ProductId = op.ProductId,
                Quantity = op.Quantity
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return _mapper.Map<OrderResponse>(order);
    }

    public override async Task<OrderResponse> GetOrder(GetOrderRequest request, ServerCallContext context)
    {
        var order = await _context.Orders.Include(o => o.OrdersProducts).ThenInclude(op => op.Product)
                                         .FirstOrDefaultAsync(o => o.OrderId == request.OrderId);
        return _mapper.Map<OrderResponse>(order);
    }

    public override async Task<OrderResponse> UpdateOrderStatus(UpdateOrderStatusRequest request, ServerCallContext context)
    {
        var order = await _context.Orders.Include(o => o.OrdersProducts)
                                         .FirstOrDefaultAsync(o => o.OrderId == request.OrderId);
        if (order == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Order not found"));
        }

        order.Status = (BakeryOrderManagmentSystem.Models.OrderStatus)request.Status;

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return _mapper.Map<OrderResponse>(order);
    }

    public override async Task<Empty> DeleteOrder(DeleteOrderRequest request, ServerCallContext context)
    {
        var order = await _context.Orders.Include(o => o.OrdersProducts)
                                         .FirstOrDefaultAsync(o => o.OrderId == request.OrderId);
        if (order == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Order not found"));
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return new Empty();
    }

    public override async Task<OrderListResponse> GetAllOrders(Empty request, ServerCallContext context)
    {
        var orders = await _context.Orders.Include(o => o.OrdersProducts).ThenInclude(op => op.Product).ToListAsync();
        var orderResponses = orders.Select(o => _mapper.Map<OrderResponse>(o)).ToList();

        return new OrderListResponse { Orders = { orderResponses } };
    }
}