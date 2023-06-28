using Ecommerce.Contracts.Models.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Tables
{
    public class AddressResponse : ResponseDto
    {
        public AddressDto Address { get; set; }
    }
    public class AddressesResponse : ResponseDto
    {
        public IEnumerable<AddressDto> Addresses { get; set; }
    }
    public class AddressDto
    {
        public int Address_Id { get; set; }

        public string User_Id { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string Zipcode { get; set; }

        public DateTime Created_At { get; set; }

        public DateTime Updated_At { get; set; }
    }
}
