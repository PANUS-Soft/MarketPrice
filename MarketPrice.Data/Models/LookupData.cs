using System.ComponentModel.DataAnnotations.Schema;

namespace MarketPrice.Data.Models
{
    public class LookupData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public required int LookupDataId { get; set; }
        public required int LookupDataTypeId { get; set; }
        public required string LookupDataValue { get; set; }
        public required string LookupDataTextEnglish { get; set; }
        public required string LookupDataTextFrench { get; set; }

    }
}
