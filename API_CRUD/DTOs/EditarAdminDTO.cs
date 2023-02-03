using System.ComponentModel.DataAnnotations;

namespace API_CRUD.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
