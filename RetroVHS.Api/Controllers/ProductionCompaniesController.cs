using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Shared.DTOs.ProductionCompanies;

namespace RetroVHS.Api.Controllers;

[ApiController]
[Route("api/production-companies")]
public class ProductionCompaniesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductionCompaniesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductionCompanyDto>>> GetAll()
    {
        var companies = await _context.ProductionCompanies
            .OrderBy(pc => pc.Name)
            .Select(pc => new ProductionCompanyDto { Id = pc.Id, Name = pc.Name })
            .ToListAsync();
        return Ok(companies);
    }
}
