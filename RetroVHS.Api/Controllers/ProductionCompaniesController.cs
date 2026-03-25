using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Api.Models;
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

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductionCompanyDto>> Create([FromBody] CreateProductionCompanyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var exists = await _context.ProductionCompanies
            .AnyAsync(pc => pc.Name.ToLower() == dto.Name.ToLower());
        if (exists)
            return Conflict(new { message = $"Produktionsbolaget '{dto.Name}' finns redan." });

        var company = new ProductionCompany { Name = dto.Name.Trim() };
        _context.ProductionCompanies.Add(company);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new ProductionCompanyDto { Id = company.Id, Name = company.Name });
    }
}
