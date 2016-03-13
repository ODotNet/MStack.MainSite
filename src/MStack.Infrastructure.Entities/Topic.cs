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
    }
}
