namespace DataAccess
{
    using DataAccess.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [CanDeprecate]
    [Table("Posts", Schema = "blog")]
    [Trigger(
        TriggerType.BeforeDelete,
        CreateTriggerScript = "DELETE FROM CommentDto WHERE CommentDto.BlogPostId = (deleted/OLD).Id;",
        RequiredTypes = new[] { typeof(CommentDto) }
    )]
    public class BlogPostDto : DtoBase
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTimeOffset? DeprecatedDate { get; set; }
    }
}
