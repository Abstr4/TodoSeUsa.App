using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Common.Events;
using TodoSeUsa.Application.Features.Payments.DTOs;
using TodoSeUsa.Application.Features.Products;
using TodoSeUsa.Application.Features.Sales.DTOs;
using TodoSeUsa.Application.Features.Sales.Interfaces;
using TodoSeUsa.Application.Features.Sales.Validators;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Sales.Services;

public sealed class SaleService : ISaleService
{
    private readonly ILogger<SaleService> _logger;
    private readonly IApplicationDbContextFactory _contextFactory;
    private readonly UniqueSaleCodeService _uniqueSaleCodeService;
    private readonly AppEvents _events;

    public SaleService(ILogger<SaleService> logger, IApplicationDbContextFactory contextFactory, UniqueSaleCodeService uniqueSaleCodeService, AppEvents events)
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _uniqueSaleCodeService = uniqueSaleCodeService;
        _events = events;
    }

    public async Task<Result<PagedItems<SaleDto>>> GetAllAsync(QueryRequest request, CancellationToken ct)
    {
        var _context = await _contextFactory.CreateDbContextAsync(ct);

        var query = _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .AsQueryable();

        query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator);

        query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts);

        var totalCount = await query.CountAsync(ct);

        query = query.Skip(request.Skip).Take(request.Take);

        var items = await query
            .Select(s => new SaleDto
            {
                Id = s.Id,
                Code = s.Code,
                TotalAmount = s.TotalAmount,
                AmountPaid = s.AmountPaid,
                Status = s.Status,
                DateIssued = s.DateIssued,
                CreatedAt = s.CreatedAt,
                Notes = s.Notes
            })
        .ToListAsync(ct);

        return Result.Success(new PagedItems<SaleDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<DetailedSaleDto>> GetByIdAsync(int saleId, CancellationToken ct)
    {
        if (saleId < 1)
            return Result.Failure<DetailedSaleDto>(SaleErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var saleDto = await _context.Sales
                .Where(c => c.Id == saleId)
                .Select(c => new DetailedSaleDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    TotalAmount = c.TotalAmount,
                    AmountPaid = c.AmountPaid,
                    Status = c.Status,
                    TotalItems = c.Items.Count(),
                    TotalPayments = c.Payments.Count(),
                    DateIssued = c.DateIssued,
                    Notes = c.Notes,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (saleDto == null)
                return Result.Failure<DetailedSaleDto>(SaleErrors.NotFound(saleId));

            return Result.Success(saleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to retrieve the sale with ID {saleId}.", saleId);
            return Result.Failure<DetailedSaleDto>(SaleErrors.Failure("Ocurrió un error inesperado al intentar recuperar la venta."));
        }
    }

    public async Task<Result<PagedItems<SaleItemDto>>> GetItemsAsync(int saleId, QueryRequest request, CancellationToken ct)
    {
        if (saleId < 1)
        {
            return Result.Failure<PagedItems<SaleItemDto>>(ProductErrors.Failure("El Id de la venta debe ser mayor que cero."));
        }
        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var query = context.SaleItems
                .Where(si => si.SaleId == saleId)
                .AsQueryable();

            query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator);

            query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts);

            var totalCount = await query.CountAsync(ct);

            query = query.Skip(request.Skip).Take(request.Take);

            var items = await query
                .Select(si => new SaleItemDto
                {
                    ProductId = si.ProductId,
                    Price = si.Price,
                    Size = si.Size,
                    Category = si.Category,
                    Description = si.Description,
                    Quality = si.Quality,
                    Body = si.Body
                })
                .ToListAsync(ct);

            return Result.Success(new PagedItems<SaleItemDto>
            {
                Items = items,
                Count = totalCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving products for sale ID {saleId}.", saleId);
            return Result.Failure<PagedItems<SaleItemDto>>(ProductErrors.Failure("Ocurrió un error inesperado al intentar recuperar los productos de la venta."));
        }
    }

    public async Task<Result<PagedItems<PaymentDto>>> GetPaymentsAsync(int saleId, QueryRequest request, CancellationToken ct)
    {
        if (saleId < 1)
        {
            return Result.Failure<PagedItems<PaymentDto>>(SaleErrors.Failure("El Id de la venta debe ser mayor que cero."));
        }

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var query = context.Payments
                .Where(p => p.SaleId == saleId)
                .AsQueryable();

            query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator);
            query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts);

            var totalCount = await query.CountAsync(ct);

            query = query.Skip(request.Skip).Take(request.Take);

            var payments = await query
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    SaleId = p.SaleId,
                    Amount = p.Amount,
                    Method = p.Method,
                    Date = p.Date,
                    RefundedAt = p.RefundedAt,
                    RefundReason = p.RefundReason
                })
                .ToListAsync(ct);

            return Result.Success(new PagedItems<PaymentDto>
            {
                Items = payments,
                Count = totalCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving payments for sale ID {saleId}.", saleId);
            return Result.Failure<PagedItems<PaymentDto>>(SaleErrors.Failure("Ocurrió un error inesperado al intentar recuperar los pagos de la venta."));
        }
    }

    public async Task<Result<int>> CreateAsync(CreateSaleDto createSaleDto, CancellationToken ct)
    {
        var validator = new CreateSaleDtoValidator();

        var validationResult = await validator.ValidateAsync(createSaleDto, ct);

        if (!validationResult.IsValid)
        {
            return Result.Failure<int>(SaleErrors.Failure(validationResult.ToString()));
        }

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var existingProducts = await context.Products
                .Include(p => p.Consignment)
                    .ThenInclude(c => c.Consignor)
                .Where(p => createSaleDto.ProductsIds.Contains(p.Id))
                .ToListAsync(ct);

            var soldProductsIds = existingProducts.Where(p => p.Status == ProductStatus.Sold)
                .Select(p => p.Id);

            if (soldProductsIds.Any())
            {
                _logger.LogError("Products with Codes [{soldProductsIds}] are already sold.", string.Join(", ", soldProductsIds));
                return Result.Failure<int>(
                    SaleErrors.Failure($"Los productos con los siguientes códigos ya están vendidos: {string.Join(", ", soldProductsIds)}")
                );
            }

            var missingProducts = createSaleDto.ProductsIds
                .Except(existingProducts.Select(p => p.Id))
                .ToList();

            if (missingProducts.Count != 0)
            {
                _logger.LogError("Products with Codes {missingProducts} not found.", string.Join(", ", missingProducts));
                return Result.Failure<int>(
                    SaleErrors.Failure($"No se encontraron los productos con los siguientes IDs: {string.Join(", ", missingProducts)}")
                );
            }

            var code = await _uniqueSaleCodeService.GenerateAsync(ct);

            var sale = new Sale
            {
                Code = code,
                DateIssued = createSaleDto.DateIssued ?? DateTime.Now,
                Notes = createSaleDto.Notes
            };

            foreach (var product in existingProducts)
            {
                var result = AddProductToSale(sale, product);
                if (result.IsFailure)
                {
                    return Result.Failure<int>(result.Error);
                }
            }

            foreach (var paymentDto in createSaleDto.Payments)
            {
                var payment = new Payment
                {
                    Amount = paymentDto.Amount,
                    Method = paymentDto.Method,
                    Date = paymentDto.Date ?? DateTime.Now
                };

                var result = AddPaymentToSale(sale, payment);
                if (result.IsFailure)
                {
                    return Result.Failure<int>(result.Error);
                }
            }

            var recalculateResult = RecalculateSale(sale);
            if (recalculateResult.IsFailure)
            {
                return Result.Failure<int>(recalculateResult.Error);
            }

            var entry = await context.Sales.AddAsync(sale, ct);
            var saved = await context.SaveChangesAsync(ct);

            if (saved > 0)
            {
                _events.RaiseLiquidationsChanged();
                return Result.Success(entry.Entity.Id);
            }

            return Result.Failure<int>(SaleErrors.Failure("No se pudo crear la venta."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to create the sale.");
            return Result.Failure<int>(SaleErrors.Failure("Ocurrió un error inesperado al intentar crear la venta."));
        }
    }

    public async Task<Result> EditAsync(EditSaleDto editSaleDto, CancellationToken ct)
    {
        if (editSaleDto.Id < 1)
            return Result.Failure(SaleErrors.Failure("El Id debe ser mayor que cero."));

        var validator = new EditSaleDtoValidator();
        var validationResult = await validator.ValidateAsync(editSaleDto, ct);

        if (!validationResult.IsValid)
            return Result.Failure(SaleErrors.Failure(validationResult.ToString()));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            Sale? sale = await _context.Sales.FirstOrDefaultAsync(b => b.Id == editSaleDto.Id, ct);

            if (sale == null)
            {
                return Result.Failure(SaleErrors.NotFound(editSaleDto.Id));
            }

            if (sale.Status == SaleStatus.Cancelled)
            {
                return Result.Failure(SaleErrors.Failure("No se puede editar una venta cancelada."));
            }

            sale.Notes = editSaleDto.Notes;

            var saved = await _context.SaveChangesAsync(ct);
            if (saved > 0)
            {
                return Result.Success();
            }
            return Result.Failure(SaleErrors.Failure("No se pudo editar la venta."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to edit the sale with ID {editSaleDto.Id}.", editSaleDto.Id);
            return Result.Failure(SaleErrors.Failure($"Ocurrió un error inesperado al intentar editar la venta."));
        }
    }

    public async Task<Result> CancelByCodeAsync(string code, string? reason, CancellationToken ct)
    {
        var validator = new SaleCodeValidator();
        var validationResult = await validator.ValidateAsync(code, ct);

        if (!validationResult.IsValid)
            return Result.Failure(SaleErrors.Failure(validationResult.ToString()));

        var context = await _contextFactory.CreateDbContextAsync(ct);

        var saleId = await context.Sales
            .Where(s => s.Code == code)
            .Select(s => s.Id)
            .SingleOrDefaultAsync(ct);

        if (saleId == 0)
            return Result.Failure(SaleErrors.NotFound(code));

        return await CancelInternalAsync(saleId, reason, ct);
    }

    public Task<Result> CancelByIdAsync(int saleId, string? reason, CancellationToken ct)
    {
        return CancelInternalAsync(saleId, reason, ct);
    }

    private async Task<Result> CancelInternalAsync(int saleId, string? reason, CancellationToken ct)
    {
        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var sale = await context.Sales
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .Include(s => s.Payments)
                .SingleOrDefaultAsync(s => s.Id == saleId, ct);

            if (sale is null)
                return Result.Failure(SaleErrors.NotFound(saleId));

            if (sale.Status == SaleStatus.Cancelled)
                return Result.Success();

            foreach (var item in sale.Items)
            {
                if (item.ReturnedAt.HasValue)
                    continue;

                item.ReturnedAt = DateTime.Now;
                item.ReturnReason = "Venta cancelada.";

                if (item.Product != null)
                    item.Product.Status = ProductStatus.Available;
            }

            foreach (var payment in sale.Payments)
            {
                if (payment.RefundedAt.HasValue)
                    continue;

                payment.RefundedAt = DateTime.Now;
                payment.RefundReason = "Venta cancelada.";
            }

            sale.Status = SaleStatus.Cancelled;
            sale.CancelledAt = DateTime.Now;
            sale.CancelReason = reason;

            var saved = await context.SaveChangesAsync(ct);

            return saved > 0
                ? Result.Success()
                : Result.Failure(SaleErrors.Failure("No se pudo borrar la venta."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while cancelling sale {saleId}", saleId);
            return Result.Failure(SaleErrors.Failure("Ocurrió un error inesperado al intentar borrar la venta."));
        }
    }

    public async Task<Result> AddProductAsync(int saleId, int productId, CancellationToken ct)
    {
        if (productId < 1)
        {
            return Result.Failure(SaleErrors.Failure("El ID del producto debe ser mayor a cero."));
        }

        if (saleId < 1)
        {
            return Result.Failure(SaleErrors.Failure("El ID de la venta debe ser mayor a cero."));
        }

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var sale = await context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == saleId, ct);

            if (sale == null)
            {
                return Result.Failure(SaleErrors.NotFound(saleId));
            }

            if (sale.Status == SaleStatus.Cancelled)
            {
                return Result.Failure(SaleErrors.Failure("No se puede agregar productos a una venta cancelada."));
            }

            var product = await context.Products.SingleOrDefaultAsync(p => p.Id == productId, ct);
            if (product == null)
            {
                return Result.Failure(ProductErrors.NotFound(productId));
            }

            var addResult = AddProductToSale(sale, product);
            if (addResult.IsFailure)
            {
                return Result.Failure(SaleErrors.Failure(addResult.Error.Description));
            }

            var saved = await context.SaveChangesAsync(ct);
            if (saved > 0)
            {
                return Result.Success();
            }

            return Result.Failure(SaleErrors.Failure("No se pudo agregar el producto a la venta."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product {ProductId} to sale {SaleId}", productId, saleId);
            return Result.Failure(SaleErrors.Failure("Ocurrió un error inesperado al intentar agregar el producto a la venta."));
        }
    }

    public async Task<Result> ReturnProductAsync(int saleId, int saleItemId, CancellationToken ct)
    {
        if (saleId < 1)
            return Result.Failure(SaleErrors.Failure("El ID de la venta debe ser mayor a cero."));

        if (saleItemId < 1)
            return Result.Failure(SaleErrors.Failure("El ID del producto en la venta debe ser mayor a cero."));

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var sale = await context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == saleId, ct);

            if (sale == null)
                return Result.Failure(SaleErrors.NotFound(saleId));

            var item = sale.Items.SingleOrDefault(i => i.Id == saleItemId);

            if (item == null)
                return Result.Failure(SaleErrors.SaleItemNotFound(saleId));

            var result = ReturnProduct(sale, item);
            if (result.IsFailure)
                return result;

            await context.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error returning product {SaleItemId} for sale {SaleId}", saleItemId, saleId);
            return Result.Failure(SaleErrors.Failure("Ocurrió un error inesperado al intentar devolver el producto."));
        }
    }

    public async Task<Result> RegisterPaymentAsync(int saleId, CreatePaymentDto paymentDto, CancellationToken ct)
    {
        var validator = new CreatePaymentDtoValidator();

        var validatonResult = await validator.ValidateAsync(paymentDto, ct);

        if (!validatonResult.IsValid)
        {
            return Result.Failure(SaleErrors.Failure(validatonResult.ToString()));
        }

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var sale = await context.Sales
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.Id == saleId, ct);

            if (sale == null)
            {
                return Result.Failure(SaleErrors.NotFound(saleId));
            }

            if (sale.Status == SaleStatus.Cancelled)
            {
                return Result.Failure(SaleErrors.Failure("No se puede agregar pagos a una venta cancelada."));
            }

            var payment = new Payment
            {
                Amount = paymentDto.Amount,
                Method = paymentDto.Method,
                Date = paymentDto.Date ?? DateTime.Now,
                SaleId = sale.Id,
                Sale = sale
            };

            var addResult = AddPaymentToSale(sale, payment);
            if (!addResult.IsSuccess)
            {
                return Result.Failure(SaleErrors.Failure(addResult.Error.Description));
            }

            var save = await context.SaveChangesAsync(ct);
            if (save > 0)
            {
                return Result.Success();
            }
            return Result.Failure(SaleErrors.Failure("No se pudo agregar el pago a la venta."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding payment to sale {SaleId}", saleId);
            return Result.Failure(SaleErrors.Failure("Ocurrió un error inesperado al intentar agregar el pago."));
        }
    }

    public async Task<Result> RefundPaymentAsync(int saleId, RefundPaymentDto refundPayment, CancellationToken ct)
    {
        if (saleId < 1)
            return Result.Failure(SaleErrors.Failure("El ID de la venta debe ser mayor a cero."));

        if (refundPayment.PaymentId < 1)
            return Result.Failure(SaleErrors.Failure("El ID del pago debe ser mayor a cero."));

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var payment = await context.Payments.FirstOrDefaultAsync(p => p.Id == refundPayment.PaymentId && p.SaleId == saleId, ct);

            if (payment is null)
                return Result.Failure(SaleErrors.PaymentNotFound(refundPayment.PaymentId));

            var sale = await context.Sales
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.Id == saleId, ct);

            if (sale == null)
                return Result.Failure(SaleErrors.NotFound(saleId));

            var result = RefundPayment(sale, payment, refundPayment.Reason);
            if (result.IsFailure)
                return result;

            await context.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding payment '{PaymentId}' for sale '{SaleId}'", refundPayment.PaymentId, saleId);
            return Result.Failure(SaleErrors.Failure("Ocurrió un error inesperado al intentar reembolsar el pago."));
        }
    }

    private static Result AddProductToSale(Sale sale, Product product)
    {
        if (sale.Status == SaleStatus.Cancelled)
            return Result.Failure(SaleErrors.Failure("No se puede agregar un producto a una venta cancelada."));

        if (product.Status == ProductStatus.Sold)
            return Result.Failure(SaleErrors.Failure("El producto ya fue vendido."));

        if (sale.Items.Any(i => i.ProductId == product.Id && !i.ReturnedAt.HasValue))
            return Result.Failure(SaleErrors.Failure("El producto ya está incluido en la venta."));

        var saleItem = CreateSaleItemSnapshot(product);

        sale.Items.Add(saleItem);

        var result = RecalculateSale(sale);
        if (result.IsFailure)
        {
            sale.Items.Remove(saleItem);
            return Result.Failure(result.Error);
        }

        MarkProductAsSold(product, sale);

        return Result.Success();
    }

    private static SaleItem CreateSaleItemSnapshot(Product product)
    {
        var consignment = product.Consignment
            ?? throw new InvalidOperationException("Product must belong to a consignment.");

        var consignor = consignment.Consignor
            ?? throw new InvalidOperationException("Consignment must have a consignor.");

        return new SaleItem
        {
            ProductId = product.Id,
            Price = product.Price,
            Size = product.Size,
            Category = product.Category,
            Description = product.Description,
            Quality = product.Quality,
            Body = product.Body,
            Brand = product.Brand,
            Season = product.Season,

            ConsignorId = consignor.Id,
            ConsignorPercent = consignor.CommissionPercent,

            AmountPaidOut = 0,
            CreatedAt = DateTime.Now
        };
    }

    private static void MarkProductAsSold(Product product, Sale sale)
    {
        product.Box = null;
        product.BoxId = null;
        product.Status = ProductStatus.Sold;
        product.Sale = sale;
    }

    private static Result ReturnProduct(Sale sale, SaleItem item)
    {
        var saleItem = sale.Items.SingleOrDefault(i => i.Id == item.Id);

        if (saleItem == null)
            return Result.Failure(SaleErrors.SaleItemNotFound(item.Id));

        if (saleItem.ReturnedAt != null)
            return Result.Failure(SaleErrors.SaleItemAlreadyReturned(item.Id));

        var originalReturnedAt = saleItem.ReturnedAt;

        saleItem.ReturnedAt = DateTime.Now;

        var result = RecalculateSale(sale);
        if (result.IsFailure)
        {
            saleItem.ReturnedAt = originalReturnedAt;
            return Result.Failure(result.Error);
        }

        return Result.Success();
    }

    private static Result AddPaymentToSale(Sale sale, Payment payment)
    {
        if (sale.AmountPaid + payment.Amount > sale.TotalAmount)
            return Result.Failure(SaleErrors.Failure("El monto total abonado no puede exceder el total a pagar."));

        sale.Payments.Add(payment);

        var result = RecalculateSale(sale);
        if (result.IsFailure)
        {
            sale.Payments.Remove(payment);
            return Result.Failure(result.Error);
        }

        payment.Sale = sale;

        return Result.Success();
    }

    private static Result RefundPayment(Sale sale, Payment payment, string? reason = null)
    {
        if (payment.RefundedAt.HasValue)
            return Result.Failure(SaleErrors.Failure("El pago ya fue reembolsado."));

        var originalRefundedAt = payment.RefundedAt;
        var originalRefundReason = payment.RefundReason;

        payment.RefundedAt = DateTime.UtcNow;
        payment.RefundReason = reason;

        var result = RecalculateSale(sale);
        if (result.IsFailure)
        {
            payment.RefundedAt = originalRefundedAt;
            payment.RefundReason = originalRefundReason;
            return Result.Failure(result.Error);
        }

        return Result.Success();
    }

    private static Result RecalculateSale(Sale sale)
    {
        sale.TotalAmount = sale.Items
            .Where(i => !i.ReturnedAt.HasValue)
            .Sum(i => i.Price);

        sale.AmountPaid = sale.Payments
            .Where(p => !p.RefundedAt.HasValue)
            .Sum(p => p.Amount);

        if (sale.AmountPaid > sale.TotalAmount)
        {
            return Result.Failure(SaleErrors.Failure(
                $"El monto abonado ({sale.AmountPaid}) excede el total a pagar ({sale.TotalAmount})."));
        }

        UpdateSaleStatus(sale);
        return Result.Success();
    }

    private static void UpdateSaleStatus(Sale sale)
    {
        sale.Status = sale.AmountPaid switch
        {
            0 => SaleStatus.Pending,
            var paid when paid < sale.TotalAmount => SaleStatus.PartiallyPaid,
            var paid when paid == sale.TotalAmount => SaleStatus.Paid,
            _ => SaleStatus.Cancelled
        };
    }
}