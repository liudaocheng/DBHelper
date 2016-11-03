using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Utils
{
    /// <summary>
    /// 公用
    /// </summary>
    public class CommonHelper
    {
        #region 数字转汉字
        /// <summary>
        /// 数字转汉字
        /// </summary>
        public static string Number2Chinese(double doubleValue)
        {
            StringBuilder result = new StringBuilder();
            string negativeTag = "";//负数标识
            string strDecimalPart = "";//小数部分

            try
            {
                //是否为负数
                bool isNegative = false;
                if (doubleValue < 0)
                {
                    isNegative = true;
                    negativeTag = "负";
                }

                string[] str = doubleValue.ToString().Split('.');

                StringBuilder sbIntegerPart = null;
                //整数部分
                if (isNegative)
                {
                    sbIntegerPart = new StringBuilder(str[0].Substring(1));
                }
                else
                {
                    sbIntegerPart = new StringBuilder(str[0]);
                }
                //小数部分
                if (str.Length > 1)
                {
                    strDecimalPart = "点" + str[1];
                }

                List<string> unitsList = new List<string>();
                unitsList.Add("");
                unitsList.Add("十");
                unitsList.Add("百");
                unitsList.Add("千");
                unitsList.Add("万");
                unitsList.Add("十");
                unitsList.Add("百");
                unitsList.Add("千");
                unitsList.Add("亿");
                unitsList.Add("十");
                unitsList.Add("百");
                unitsList.Add("千");
                unitsList.Add("万");
                unitsList.Add("十");
                unitsList.Add("百");
                unitsList.Add("千");
                unitsList.Add("亿");

                //先作一般处理
                for (int i = 0; i < sbIntegerPart.Length; i++)
                {
                    result.Append(sbIntegerPart[i]);
                    result.Append(unitsList[sbIntegerPart.Length - i - 1]);
                }

                //下面语句的先后顺序不可随意更改
                //下面五条语句处理后，不存在0十、0百、0千的情况
                //但仍可能存在0亿、0万的情况
                result.Replace("0十", "0");
                result.Replace("0百", "0");
                result.Replace("0千", "0");
                result.Replace("0亿", "亿0");
                //例：101000读作十万零1千，而不是十万一千
                //所以0万的0应该置后，上面0亿的情况同理
                result.Replace("0万", "万0");

                //while循环处理后，不存在连续两个0的情况
                while (result.ToString().IndexOf("00") != -1)
                {
                    result.Replace("00", "0");//又可能会产生如：0万
                }

                //由于已经不存在连续两个0的情况
                //下面两条语句处理后，不存在0亿、0万的情况
                //也不会导致连续两个0的情况
                result.Replace("0亿", "亿");
                result.Replace("0万", "万");

                //处理末尾
                if (result.ToString()[result.Length - 1] == '0'
                    && result.Length > 1)
                {
                    result.Remove(result.Length - 1, 1);
                }

                //特殊情况
                result.Replace("亿万", "亿");

                //特殊情况
                if (result.ToString().IndexOf("1十万") == 0)
                {
                    result.Replace("1十万", "十万");
                }
                if (result.ToString().IndexOf("1十亿") == 0)
                {
                    result.Replace("1十亿", "十亿");
                }

                //特殊情况
                if (result.ToString().IndexOf("1十") == 0)
                {
                    result.Replace("1十", "十", 0, 2);
                }
            }
            catch { }

            string r = negativeTag + result.ToString() + strDecimalPart;
            StringBuilder sbResult = new StringBuilder();
            for (int i = 0; i < r.Length; i++)
            {
                sbResult.Append(ToChinese(r[i].ToString()));
            }
            return sbResult.ToString();
        }
        #endregion

        #region 单个字符转换成大写
        /// <summary>
        /// 单个字符转换成大写
        /// </summary>
        private static string ToChinese(string tag)
        {
            string result = "";
            switch (tag)
            {
                case "0":
                    result = "零";
                    break;
                case "1":
                    result = "一";
                    break;
                case "2":
                    result = "二";
                    break;
                case "3":
                    result = "三";
                    break;
                case "4":
                    result = "四";
                    break;
                case "5":
                    result = "五";
                    break;
                case "6":
                    result = "六";
                    break;
                case "7":
                    result = "七";
                    break;
                case "8":
                    result = "八";
                    break;
                case "9":
                    result = "九";
                    break;
                default:
                    result = tag;
                    break;
            }
            return result;
        }
        #endregion

        #region 杀死进程
        /// <summary>
        /// 杀死进程
        /// </summary>
        public static void KillProcess(string startupPath, string killFilePath)
        {
            string handlePath = @"C:\Windows\system32\handle.exe";
            if (!File.Exists(handlePath))
            {
                string path = Path.Combine(startupPath, "handle.exe");
                File.Copy(path, handlePath);
            }

            Process tool = new Process();
            tool.StartInfo.FileName = "handle.exe";
            tool.StartInfo.Arguments = killFilePath + " /accepteula";
            tool.StartInfo.UseShellExecute = false;
            tool.StartInfo.RedirectStandardOutput = true;
            tool.Start();
            tool.WaitForExit();
            string outputTool = tool.StandardOutput.ReadToEnd();

            string matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";
            foreach (Match match in Regex.Matches(outputTool, matchPattern))
            {
                Process.GetProcessById(int.Parse(match.Value)).Kill();
            }
        }
        #endregion

    }
}
