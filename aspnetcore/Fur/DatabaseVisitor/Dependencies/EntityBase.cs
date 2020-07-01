﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fur.DatabaseVisitor.Dependencies
{
    public abstract class EntityBase<TKey> : Entity<TKey> where TKey : struct
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedTime { get; set; }
        /// <summary>
        /// 假删除
        /// </summary>
        [DefaultValue(0)]
        public bool IsDeleted { get; set; }
    }
}
