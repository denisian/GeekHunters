using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using GeekHunter.ViewModels;

namespace GeekHunter.Models
{
    [Table("Candidate")]
    public class Candidate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [NotMapped]
        public virtual ICollection<CandidateSkillViewModel> Skills { get; set; }
    }
}
