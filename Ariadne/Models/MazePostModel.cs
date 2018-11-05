using System.ComponentModel.DataAnnotations;

namespace Ariadne.Models
{
    public class MazePostModel
    {
        [Required]
        public string mazeString {get; set; }
    }
}