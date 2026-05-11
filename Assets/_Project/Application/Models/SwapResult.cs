// ...existing code...
namespace SolarPhobia.Application.Models
{
    /// <summary>
    /// Result of a Swap operation.
    /// </summary>
    public class SwapResult
    {
        public bool Success { get; set; }
        public string PlayerId { get; set; }
        public string SoulId { get; set; }
    }
}
// ...existing code...
