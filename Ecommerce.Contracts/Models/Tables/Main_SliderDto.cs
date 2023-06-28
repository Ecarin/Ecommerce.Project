using Ecommerce.Contracts.Models.Utility;

namespace Ecommerce.Contracts.Models.Tables
{
    public class Main_SlidersResponse : ResponseDto
    {
        public IEnumerable<Main_SliderDto> Sliders { get; set; }
    }
    public class Main_SliderDto
    {
        public string Id { get; set; }
        public string Photo_Path { get; set; }
        public string? Url { get; set; }
    }
}
