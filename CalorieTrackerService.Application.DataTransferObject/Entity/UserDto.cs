namespace CalorieTrackerService.Application.DataTransferObject.Entity
{
    public class UserDto
    {
        public string UserEmail { get; set; } = string.Empty;
        public byte[] UserPassword { get; set; } = Array.Empty<byte>();
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}