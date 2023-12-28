using Microsoft.EntityFrameworkCore;

namespace Order.API.Model
{
    //bu address sınıfının ayrı bir tabloda olmasını istemiyoruz order tablosunda gözüksün o sebeple owned entitytype uyguyluycaz

    [Owned]
    public class Address
    {
        public string Line { get; set; }
        public string Province { get; set; }
        public string District { get; set; } //order tablosunda Address.District diye gözükecek.
    }
}