using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using PharmaGO.Api.Controllers.Common;
using PharmaGO.Application.Products.Queries.ListProducts;
using PharmaGO.Contract.Product;

namespace PharmaGO.Api.Controllers.OData;

[Route("odata/Products")]
public class ProductsODataController(ISender mediator, IMapper mapper) : ApiController
{
    [HttpGet]
    [EnableQuery]
    public async Task<IActionResult> Get()
    {
        var result = await mediator.Send(new ListProductsQuery());

        return Ok(mapper.Map<List<ProductResponse>>(result));
    }
}