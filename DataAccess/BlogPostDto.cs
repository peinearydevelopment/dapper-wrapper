namespace DataAccess
{
    using DataAccess.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [CanDeprecate]
    [Table("Posts", Schema = "blog")]
    public class BlogPostDto
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTimeOffset? DeprecatedDate { get; set; }
    }
}
