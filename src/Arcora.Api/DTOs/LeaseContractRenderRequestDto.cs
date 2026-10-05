using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;

public class LeaseContractRenderRequestDto
{
    [Required]
    public Guid LeaseID { get; set; }

    public Guid? LeaseContractTemplateID { get; set; }
}
