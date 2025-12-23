using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CarRentalMVC.TagHelpers
{
    /// <summary>
    /// TagHelper hiển thị trạng thái người dùng (Khóa/Mở)
    /// Sử dụng: <user-status is-locked="true"></user-status>
    /// </summary>
    [HtmlTargetElement("user-status")]
    public class UserStatusTagHelper : TagHelper
    {
        /// <summary>
        /// Trạng thái khóa (true = đã khóa, false = hoạt động)
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Render TagHelper
        /// </summary>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Thay đổi tag name
            output.TagName = "span";

            // Xóa các attributes mặc định
            output.Attributes.RemoveAll("is-locked");

            // Thêm class và nội dung dựa trên trạng thái
            if (IsLocked)
            {
                output.Attributes.SetAttribute("class", "badge bg-danger");
                output.Content.SetHtmlContent("<i class=\"bi bi-lock-fill me-1\"></i>Đã Khóa");
            }
            else
            {
                output.Attributes.SetAttribute("class", "badge bg-success");
                output.Content.SetHtmlContent("<i class=\"bi bi-unlock-fill me-1\"></i>Hoạt Động");
            }
        }
    }
}

