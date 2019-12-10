using System.Text;

namespace iMvcCore.Utils
{
    public static class Pagination
    {
        /// <summary>
        ///     生成导航菜单
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="prefix"></param>
        /// <param name="ext"></param>
        /// <param name="firstPage"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Build(int currentPage, int totalPages, string prefix, string ext, string firstPage, int count = 10)
        {
            if(totalPages == 0) return string.Empty;

            // 如果当前页码大于总页数,则取总页码
            currentPage = currentPage > totalPages ? totalPages : currentPage;

            var builder = new StringBuilder();
            string url;

            // If current page is not first page, add prev page.
            if(currentPage != 1)
            {
                url = currentPage - 1 == 1 ? firstPage : $"{prefix}{currentPage - 1}{ext}";
                builder.Append("<a target=\"_self\" href=\"" + url + "\">&#171; 上一页</a>");
            }

            count = count > totalPages ? totalPages : count;

            // Left navigation
            var min = currentPage - count <= 0 ? 1 : currentPage - count;
            for(var i = min; i < currentPage; i++)
            {
                url = i == 1 ? firstPage : $"{prefix}{i}{ext}";
                builder.Append("<a target=\"_self\" href=\"" + url + "\">" + i + "</a>");
            }

            // Current page
            builder.Append("<span>" + currentPage + "</span>");

            // Right navigation
            var max = currentPage + count > totalPages ? totalPages : currentPage + count;
            for(var i = currentPage + 1; i <= max; i++)
            {
                url = $"{prefix}{i}{ext}";
                builder.Append($"<a target=\"_self\" href=\"{url}\">{i}</a>");
            }

            // if last page is not the end page, add next page navigator
            if(currentPage != totalPages)
            {
                builder.Append($"<a target=\"_self\" href=\"{prefix}{currentPage + 1}{ext}\">下一页&#187;</a>");
            }

            return builder.ToString();
        }
    }
}