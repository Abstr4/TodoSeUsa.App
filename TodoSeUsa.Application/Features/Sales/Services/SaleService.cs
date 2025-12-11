using System.Linq.Dynamic.Core;
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

    public SaleService(ILogger<SaleService> logger, IApplicationDbContextFactory contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    public async Task<Result<PagedItems<SaleDto>>> GetAllAsync(QueryRequest request, CancellationToken ct)
    {
        var _context = await _contextFactory.CreateDbContextAsync(ct);

        var query = _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .AsQueryable();

/*        if (request.Filters != null && request.Filters.Count > 0)
        {
            var predicate = PredicateBuilder.BuildPredicate<Sale>(request);
            query = query.Where(predicate);
        }*/

        query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator);

        query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts);

        var totalCount = await query.CountAsync(ct);

        query = query.Skip(request.Skip).Take(request.Take);

        var items = await query
            .Select(c => new SaleDto
            {
                Id = c.Id,
                TotalAmount = c.TotalAmount,
                AmountPaid = c.AmountPaid,
                Status = c.Status,
                DateIssued = c.DateIssued,
                Notes = c.Notes
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
        if (saleId <= 0)
            return Result.Failure<DetailedSaleDto>(SaleErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var saleDto = await _context.Sales
                .Where(c => c.Id == saleId)
                .Select(c => new DetailedSaleDto
                {
                    Id = c.Id,
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
                    ProductCode = si.ProductCode,
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
                .Where(p => createSaleDto.ProductCodes.Contains(p.ProductCode))
                .ToListAsync(ct);

            var soldProductsCodes = existingProducts.Where(p => p.Status == ProductStatus.Sold || p.SaleId != null)
                .Select(p => p.ProductCode);

            if (soldProductsCodes.Any())
            {
                _logger.LogError("Products with Codes [{soldProductsCodes}] are already sold.", string.Join(", ", soldProductsCodes));
                return Result.Failure<int>(
                    SaleErrors.Failure($"Los productos con los siguientes códigos ya están vendidos: {string.Join(", ", soldProductsCodes)}")
                );
            }

            var missingProducts = createSaleDto.ProductCodes
                .Except(existingProducts.Select(p => p.ProductCode))
                .ToList();

            if (missingProducts.Count != 0)
            {
                _logger.LogError("Products with Codes {missingProducts} not found.", string.Join(", ", missingProducts));
                return Result.Failure<int>(
                    SaleErrors.Failure($"No se encontraron los productos con los siguientes IDs: {string.Join(", ", missingProducts)}")
                );
            }

            var sale = new Sale
            {
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

            var entry = await context.Sales.AddAsync(sale, ct);
            var saved = await context.SaveChangesAsync(ct);

            if (saved > 0)
            {
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

    public async Task<Result<int>> AddProductAsync(int saleId, int productId, CancellationToken ct)
    {
        if (productId < 1)
        {
            return Result.Failure<int>(SaleErrors.Failure("El ID del producto debe ser mayor a cero."));
        }
        if (saleId < 1)
        {
            return Result.Failure<int>(SaleErrors.Failure("El ID de la venta debe ser mayor a cero."));
        }
        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var sale = await context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == saleId, ct);

            if (sale == null)
            {
                return Result.Failure<int>(SaleErrors.NotFound(saleId));
            }

            var product = await context.Products.FirstOrDefaultAsync(p => p.Id == productId, ct);
            if (product == null)
            {
                return Result.Failure<int>(ProductErrors.NotFound(productId));
            }

            AddProductToSale(sale, product);

            var saved = await context.SaveChangesAsync(ct);
            if (saved > 0)
            {
                return Result.Success(sale.Items.First(i => i.ProductId == productId).Id);
            }

            return Result.Failure<int>(SaleErrors.Failure("No se pudo agregar el producto a la venta."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product {ProductId} to sale {SaleId}", productId, saleId);
            return Result.Failure<int>(SaleErrors.Failure("Ocurrió un error inesperado al intentar agregar el producto a la venta."));
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

            var result = ReturnProduct(sale, saleItemId);
            if (!result.IsSuccess)
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

    public async Task<Result<int>> RegisterPayment(int saleId, CreatePaymentDto paymentDto, CancellationToken ct)
    {
        var validator = new CreatePaymentDtoValidator();
        var validatonResult = await validator.ValidateAsync(paymentDto, ct);
        if (!validatonResult.IsValid)
        {
            return Result.Failure<int>(SaleErrors.Failure(validatonResult.ToString()));
        }

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var sale = await context.Sales
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.Id == saleId, ct);

            if (sale == null)
            {
                return Result.Failure<int>(SaleErrors.NotFound(saleId));
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
                return Result.Failure<int>(SaleErrors.Failure(addResult.Error.Description));
            }

            var save = await context.SaveChangesAsync(ct);
            if (save > 0)
            {
                return Result.Success(payment.Id);
            }
            return Result.Failure<int>(SaleErrors.Failure("No se pudo agregar el pago a la venta."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding payment to sale {SaleId}", saleId);
            return Result.Failure<int>(SaleErrors.Failure("Ocurrió un error inesperado al intentar agregar el pago."));
        }
    }

    public async Task<Result> RefundPaymentAsync(int saleId, int paymentId, decimal amount, string? reason, CancellationToken ct)
    {
        if (saleId < 1)
            return Result.Failure(SaleErrors.Failure("El ID de la venta debe ser mayor a cero."));

        if (paymentId < 1)
            return Result.Failure(SaleErrors.Failure("El ID del pago debe ser mayor a cero."));

        if (amount <= 0)
            return Result.Failure(SaleErrors.Failure("El monto del reembolso debe ser mayor que cero."));

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var sale = await context.Sales
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.Id == saleId, ct);

            if (sale == null)
                return Result.Failure(SaleErrors.NotFound(saleId));

            var result = RefundPayment(sale, paymentId, amount, reason);
            if (!result.IsSuccess)
                return result;

            await context.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding payment {PaymentId} for sale {SaleId}", paymentId, saleId);
            return Result.Failure(SaleErrors.Failure("Ocurrió un error inesperado al intentar reembolsar el pago."));
        }
    }

    public async Task<Result> EditByIdAsync(int saleId, EditSaleDto editSaleDto, CancellationToken ct)
    {
        if (saleId < 1)
            return Result.Failure(SaleErrors.Failure("El Id debe ser mayor que cero."));

        var validator = new EditSaleDtoValidator();
        var validationResult = await validator.ValidateAsync(editSaleDto, ct);

        if (!validationResult.IsValid)
            return Result.Failure(SaleErrors.Failure(validationResult.ToString()));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            Sale? sale = await _context.Sales.FirstOrDefaultAsync(b => b.Id == saleId, ct);
            if (sale == null)
            {
                return Result.Failure(SaleErrors.NotFound(saleId));
            }

            sale.DateIssued = editSaleDto.DateIssued;
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
            _logger.LogError(ex, "An error occurred while trying to edit the sale with ID {saleId}.", saleId);
            return Result.Failure(SaleErrors.Failure($"Ocurrió un error inesperado al intentar editar la venta."));
        }
    }

    public async Task<Result> DeleteByIdAsync(int saleId, CancellationToken ct)
    {
        if (saleId < 1)
            return Result.Failure(SaleErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var sale = await _context.Sales
                .Include(s => s.Items)
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(b => b.Id == saleId, ct);

            if (sale is null)
                return Result.Failure(SaleErrors.NotFound(saleId));

            if (sale.Items.Count > 0)
            {
                return Result.Failure(SaleErrors.Failure("No se puede borrar una venta que contiene productos vendidos."));
            }

            if (sale.Payments.Count > 0)
            {
                return Result.Failure(SaleErrors.Failure("No se puede borrar una venta que contiene pagos."));
            }

            _context.Sales.Remove(sale);
            var saved = await _context.SaveChangesAsync(ct);
            if (saved > 0)
            {
                return Result.Success(true);
            }
            return Result.Failure(SaleErrors.Failure("No se pudo borrar la venta."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to delete the sale with ID {saleId}", saleId);
            return Result.Failure(SaleErrors.Failure($"Ocurrió un error inesperado al intentar borrar la venta."));
        }
    }

    private static Result AddProductToSale(Sale sale, Product product)
    {
        RecalculateSale(sale);

        if (product.Status == ProductStatus.Sold || product.SaleId != null)
            return Result.Failure(SaleErrors.Failure("El producto ya fue vendido."));

        if (sale.Items.Any(i => i.ProductId == product.Id))
            return Result.Failure(SaleErrors.Failure("El producto ya está incluido en la venta."));

        product.Box = null;
        product.Status = ProductStatus.Sold;
        product.Sale = sale;

        var saleItem = new SaleItem
        {
            ProductId = product.Id,
            ProductCode = product.ProductCode,
            Price = product.Price,
            Size = product.Size,
            Category = product.Category,
            Description = product.Description,
            Quality = product.Quality,
            Body = product.Body
        };

        sale.Items.Add(saleItem);
        RecalculateSale(sale);

        return Result.Success();
    }

    private static Result ReturnProduct(Sale sale, int saleItemId)
    {
        RecalculateSale(sale);

        var saleItem = sale.Items.SingleOrDefault(i => i.Id == saleItemId);
        if (saleItem == null)
            return Result.Failure(SaleErrors.SaleItemNotFound(saleItemId));

        if (saleItem.ReturnedAt != null)
            return Result.Failure(SaleErrors.SaleItemAlreadyReturned(saleItemId));

        saleItem.ReturnedAt = DateTime.Now;
        RecalculateSale(sale);

        return Result.Success();
    }

    private static Result AddPaymentToSale(Sale sale, Payment payment)
    {
        RecalculateSale(sale);

        if (sale.AmountPaid + payment.Amount > sale.TotalAmount)
            return Result.Failure(SaleErrors.Failure("El monto total abonado no puede exceder el total a pagar."));

        payment.Sale = sale;
        sale.Payments.Add(payment);
        RecalculateSale(sale);

        return Result.Success();
    }

    private static Result RefundPayment(Sale sale, int paymentId, decimal amount, string? reason = null)
    {
        RecalculateSale(sale);

        if (amount <= 0)
            return Result.Failure(SaleErrors.Failure("El monto del reembolso debe ser mayor que cero."));

        var payment = sale.Payments.SingleOrDefault(p => p.Id == paymentId);
        if (payment == null)
            return Result.Failure(SaleErrors.Failure("No se encontró el pago asociado al reembolso."));

        if (payment.RefundedAt.HasValue)
            return Result.Failure(SaleErrors.Failure("El pago ya fue reembolsado."));

        if (amount > payment.Amount)
            return Result.Failure(SaleErrors.Failure("El monto del reembolso no puede exceder el pago original."));

        payment.RefundedAt = DateTime.UtcNow;
        payment.RefundReason = reason;
        RecalculateSale(sale);

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