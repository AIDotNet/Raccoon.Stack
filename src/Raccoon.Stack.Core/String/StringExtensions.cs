using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace Raccoon.Stack.Core.String;

public static class StringExtensions
{
    /// <summary>
    /// 指示所指定的正则表达式在指定的输入字符串中是否找到了匹配项
    /// </summary>
    /// <param name="value">要搜索匹配项的字符串</param>
    /// <param name="pattern">要匹配的正则表达式模式</param>
    /// <param name="isContains">是否包含，否则全匹配</param>
    /// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false</returns>
    public static bool IsMatch(this string value, string pattern, bool isContains = true)
    {
        if (value == null)
        {
            return false;
        }
        return isContains
            ? Regex.IsMatch(value, pattern)
            : Regex.Match(value, pattern).Success;
    }
    
    /// <summary>
    /// 是否是IP地址
    /// </summary>
    public static bool IsIpAddress(this string value)
    {
        const string pattern = @"^((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))$";
        return value.IsMatch(pattern);
    }
    
    /// <summary>
    /// 是否身份证号，验证如下3种情况：
    /// 1.身份证号码为15位数字；
    /// 2.身份证号码为18位数字；
    /// 3.身份证号码为17位数字+1个字母
    /// </summary>
    public static bool IsIdentityCardId(this string value)
    {
        if (value.Length != 15 && value.Length != 18)
        {
            return false;
        }
        Regex regex;
        string[] array;
        DateTime time;
        if (value.Length == 15)
        {
            regex = new Regex(@"^(\d{6})(\d{2})(\d{2})(\d{2})(\d{3})_");
            if (!regex.Match(value).Success)
            {
                return false;
            }
            array = regex.Split(value);
            return DateTime.TryParse($"{"19" + array[2]}-{array[3]}-{array[4]}", out time);
        }
        regex = new Regex(@"^(\d{6})(\d{4})(\d{2})(\d{2})(\d{3})([0-9Xx])$");
        if (!regex.Match(value).Success)
        {
            return false;
        }
        array = regex.Split(value);
        if (!DateTime.TryParse($"{array[2]}-{array[3]}-{array[4]}", out time))
        {
            return false;
        }
        //校验最后一位
        string[] chars = value.ToCharArray().Select(m => m.ToString()).ToArray();
        int[] weights = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            int num = int.Parse(chars[i]);
            sum += num * weights[i];
        }
        int mod = sum % 11;
        string vCode = "10X98765432";//检验码字符串
        string last = vCode.ToCharArray().ElementAt(mod).ToString();
        return chars.Last().Equals(last, StringComparison.CurrentCultureIgnoreCase);
    }

    /// <summary>
    /// 是否手机号码
    /// </summary>
    /// <param name="value"></param>
    /// <param name="isRestrict">是否按严格格式验证</param>
    public static bool IsMobileNumber(this string value, bool isRestrict = false)
    {
        var pattern = isRestrict ? @"^[1][3-8]\d{9}$" : @"^[1]\d{10}$";
        return value.IsMatch(pattern);
    }
    
    /// <summary>
    /// 是否电子邮件
    /// </summary>
    public static bool IsEmail(this string value)
    {
        const string pattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
        return value.IsMatch(pattern);
    }
    
    /// <summary>
    /// 是否是整数
    /// </summary>
    public static bool IsNumeric(this string value)
    {
        const string pattern = @"^\-?[0-9]+$";
        return value.IsMatch(pattern);
    }

    /// <summary>
    /// 是否是Unicode字符串
    /// </summary>
    public static bool IsUnicode(this string value)
    {
        const string pattern = @"^[\u4E00-\u9FA5\uE815-\uFA29]+$";
        return value.IsMatch(pattern);
    }
    
    /// <summary>
    /// 如果str不是以start开始，则在句首追加start
    /// </summary>
    /// <param name="str"></param>
    /// <param name="start"></param>
    /// <param name="comparisonType"></param>
    /// <returns>追加后的结果</returns>
    /// <exception cref="ArgumentNullException">str is null</exception>
    public static string EnsureStartWith([NotNull] this string str, [NotNull] string start,
        StringComparison comparisonType = StringComparison.Ordinal)
    {
        return str.StartsWith(start, comparisonType) ? str : start + str;
    }
    
    /// <summary>
    /// 如果str不是以end结尾，则在句尾追加end
    /// </summary>
    /// <param name="str"></param>
    /// <param name="end"></param>
    /// <param name="comparisonType"></param>
    /// <returns>追加后的结果</returns>
    /// <exception cref="ArgumentNullException">str is null</exception>
    public static string EnsureEndsWith([NotNull] this string str, [NotNull] string end,
        StringComparison comparisonType = StringComparison.Ordinal)
    {
        return str.EndsWith(end, comparisonType) ? str : str + end;
    }
    
    /// <summary>
    /// 单词变成单数形式
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public static string ToSingular(this string word)
    {
        var plural1 = new Regex("(?<keep>[^aeiou])ies$");
        var plural2 = new Regex("(?<keep>[aeiou]y)s$");
        var plural3 = new Regex("(?<keep>[sxzh])es$");
        var plural4 = new Regex("(?<keep>[^sxzhyu])s$");

        if (plural1.IsMatch(word))
        {
            return plural1.Replace(word, "${keep}y");
        }
        if (plural2.IsMatch(word))
        {
            return plural2.Replace(word, "${keep}");
        }
        if (plural3.IsMatch(word))
        {
            return plural3.Replace(word, "${keep}");
        }
        if (plural4.IsMatch(word))
        {
            return plural4.Replace(word, "${keep}");
        }

        return word;
    }

    /// <summary>
    /// 单词变成复数形式
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public static string ToPlural(this string word)
    {
        var plural1 = new Regex("(?<keep>[^aeiou])y$");
        var plural2 = new Regex("(?<keep>[aeiou]y)$");
        var plural3 = new Regex("(?<keep>[sxzh])$");
        var plural4 = new Regex("(?<keep>[^sxzhy])$");

        if (plural1.IsMatch(word))
        {
            return plural1.Replace(word, "${keep}ies");
        }
        if (plural2.IsMatch(word))
        {
            return plural2.Replace(word, "${keep}s");
        }
        if (plural3.IsMatch(word))
        {
            return plural3.Replace(word, "${keep}es");
        }
        if (plural4.IsMatch(word))
        {
            return plural4.Replace(word, "${keep}s");
        }

        return word;
    }
    
    /// <summary>
    /// 支持汉字的字符串长度，汉字长度计为2
    /// </summary>
    /// <param name="value">参数字符串</param>
    /// <returns>当前字符串的长度，汉字长度为2</returns>
    public static int TextLength(this string value)
    {
        var ascii = new ASCIIEncoding();
        var tempLen = 0;
        var bytes = ascii.GetBytes(value);
        foreach (var b in bytes)
        {
            if (b == 63)
            {
                tempLen += 2;
            }
            else
            {
                tempLen += 1;
            }
        }
        return tempLen;
    }
    
    public static string TrimEnd(this string value, string trimParameter)
        => value.TrimEnd(trimParameter, StringComparison.CurrentCulture);

    public static string TrimEnd(this string value,
        string trimParameter,
        StringComparison stringComparison)
    {
        if (!value.EndsWith(trimParameter, stringComparison))
            return value;

        return value.Substring(0, value.Length - trimParameter.Length);
    }

}