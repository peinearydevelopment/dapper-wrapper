﻿namespace DataAccess
{
    using DataAccess.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UnmoderatedComments", Schema = "blog")]
    public class CommentDto : DtoBase
    {
        [Key]
        public int Id { get; set; }

        public string Comment { get; set; }

        public DateTimeOffset? CommentedOn { get; set; }

        public string CommentorsEmail { get; set; }

        public bool IsApproved { get; set; }

        [ForeignConstraint(typeof(BlogPostDto))]
        public int BlogPostId { get; set; }
    }
}
