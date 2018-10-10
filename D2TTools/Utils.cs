using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace D2TTools
{
    public static class Utils
    {
        public static string ToVietnamese(this string s)
        {
            s = s.ReplaceByRegex("[àáảãạâầấẩẫậăằắẳẵặ]", "a")
                .ReplaceByRegex("[đ]", "d")
                .ReplaceByRegex("[èéẽẻẹêềếễểệ]", "e")
                .ReplaceByRegex("[ìíĩỉị]", "i")
                .ReplaceByRegex("[òóõỏọôồốỗổộơờớỡởợ]", "o")
                .ReplaceByRegex("[ùúũủụưừứữửự]", "u")
                .ReplaceByRegex("[ỳýỹỷỵ]", "y")
                .ReplaceByRegex("[ÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶ]", "A")
                .ReplaceByRegex("[Đ]", "D")
                .ReplaceByRegex("[ÈÉẼẺẸÊỀẾỄỂỆ]", "E")
                .ReplaceByRegex("[ÌÍĨỈỊ]", "I")
                .ReplaceByRegex("[ÒÓÕỎỌÔỒỐỖỔỘƠỜỚỠỞỢ]", "O")
                .ReplaceByRegex("[ÙÚŨỦỤƯỪỨỮỬỰ]", "U")
                .ReplaceByRegex("[ỲÝỸỶỴ]", "Y")
                ;
            return s;
        }

        public static string ReplaceByRegex(this string s, string pattern, string replacement)
        {
            Regex regex = new Regex(pattern);
            return regex.Replace(s, replacement);
        }
    }
}
