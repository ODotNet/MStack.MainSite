using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MStack.Infrastructure.Entities
{
    public class Topic : BaseEntity
    {
        [DisplayName("标题")]
        public string Title { get; set; }
        [DisplayName("内容")]
        public string Content { get; set; }
        [DisplayName("发表日期")]
        public DateTime PublishDateTime { get; set; }
        [DisplayName("作者")]
        public User Author { get; set; }
        [DisplayName("分类")]
        public EnumCategory Category { get; set; }
        [DisplayName("回复")]
        public int Replies { get; set; }
        [DisplayName("查看")]
        public int Views { get; set; }
    }

    public enum EnumCategory
    {
        DoNet = 0,
        UI =1,
        Other=2
    }
}
