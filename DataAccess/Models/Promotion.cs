using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Promotion : IValidatableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PromotionId { get; set; }

        [StringLength(50)]
        public string PromotionName { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set;}

        [Range(0, 100, ErrorMessage = "Promotion value must be between 0 and 100.")]
        public int Promote {  get; set; }
        public string? PromotionImg {  get; set; }

        public virtual ICollection<ProductPromote> ProductPromotes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndAt < StartAt)
            {
                yield return new ValidationResult(
                    "The end time cannot be less than the start time.",
                    new[] { nameof(EndAt) }
                );
            }
        }
    }
}
