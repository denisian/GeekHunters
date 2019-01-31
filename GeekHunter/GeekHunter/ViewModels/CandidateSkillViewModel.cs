using System.ComponentModel.DataAnnotations.Schema;
using GeekHunter.Models;

namespace GeekHunter.ViewModels
{
    [Table("CandidateSkill")]
    public class CandidateSkillViewModel
    {
        public int CandidateId { get; set; }

        public Candidate Candidate { get; set; }

        public int SkillId { get; set; }

        public Skill Skill { get; set; }
    }
}
