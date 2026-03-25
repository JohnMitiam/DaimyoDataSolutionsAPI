namespace DaimyoDataSolutions.Application.DTOs.User
{
    public class BaseUserDTO
    {
        public string UserName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Status { get; set; }
    }
}
