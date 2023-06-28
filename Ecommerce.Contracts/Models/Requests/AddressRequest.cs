namespace Ecommerce.API.Models
{
    public class AddressRequest
    {
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string zipcode { get; set; }
    }
}
