namespace DataAccess
{
    using DataAccess.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;

    [View("V_Comments", Schema = "blog", CreateViewScript = @"
SELECT
    CommentDto.Id,
    CommentDto.Comment,
    CommentDto.CommentedOn,
    CommentDto.CommentorsEmail,
    CommentDto.BlogPostId
FROM CommentDto
WHERE CommentDto.IsApproved = 1;", RequiredTypes = new[] { typeof(CommentDto) })]
    public class ModeratedCommentDto : DtoBase
    {
        [Key]
        public int Id { get; set; }

        public string Comment { get; set; }

        public DateTimeOffset? CommentedOn { get; set; }

        public string CommentorsEmail { get; set; }

        public int BlogPostId { get; set; }
    }
}
