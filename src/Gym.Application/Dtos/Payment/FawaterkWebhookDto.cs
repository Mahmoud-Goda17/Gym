
namespace Gym.Application.Dtos.Payment
{
    public class FawaterkWebhookDto
    {
        public string invoice_id { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public decimal amount { get; set; }
    }
}
