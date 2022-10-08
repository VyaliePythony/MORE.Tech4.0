using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreTech.Parser
{
    /// <summary>
    /// селектор для выделения текста на сайте 1
    /// ссылка на RSS 1
    /// ссылка на RSS 2
    /// ### разделитель
    /// селектор для выделения текста на сайте 2
    /// ссылка на RSS 1
    /// ссылка на RSS 2
    /// </summary>
    public static class Sites
    {
        public static string All =
@"p.box-paragraph__text
https://www.vedomosti.ru/rss/rubric/economics
###
p
https://news.ru/rss/category/post/economics";

    }
}
