using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.DTOs.ProductionCompanies;

public class CreateProductionCompanyDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
}
