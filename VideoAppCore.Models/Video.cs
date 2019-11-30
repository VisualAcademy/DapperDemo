using System;

namespace VideoAppCore.Models
{
    public class Video
    {
        /// <summary>
        /// 일련번호
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 동영상 제목
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 동영상 제공 URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 동영상 작성자
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 회사
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 등록자: CreatedBy, Creator
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 등록일: Created
        /// </summary>
        //public DateTimeOffset Created { get; set; }
        public DateTime Created { get; set; }

        /// <summary>
        /// 수정자: LastModifiedBy, ModifiedBy
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// 수정일: LastModified, Modified
        /// </summary>
        public DateTime? Modified { get; set; }
    }
}
