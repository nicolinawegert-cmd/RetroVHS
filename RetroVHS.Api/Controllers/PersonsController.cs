using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroVHS.Api.Data;
using RetroVHS.Shared.DTOs.Persons;

namespace RetroVHS.Api.Controllers;

[ApiController]
[Route("api/persons")]
public class PersonsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PersonsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<PersonDto>>> GetPersons([FromQuery] string? search)
    {
        var query = _context.Persons.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.FullName.ToLower().Contains(search.ToLower()));
        }

        var persons = await query
            .OrderBy(p => p.FullName)
            .Take(30)
            .Select(p => new PersonDto { Id = p.Id, FullName = p.FullName })
            .ToListAsync();

        return Ok(persons);
    }
}
