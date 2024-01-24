using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace LanzouCloudSolve.Utility
{
    public static class utils
    {

        /// <summary>
        /// 随机抽取数组中的数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetRandomElement<T>(T[] value)
        {
            Random rnd = new Random();
            int index = rnd.Next(value.Length);
            return value[index];
        }
        /// <summary>
        /// 将对象转换为json数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string JsonToString<T>(T source)
        {
            return JsonSerializer.Serialize(source);
        }
        /// <summary>
        /// 将string转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T StringToJson<T>(string str)
        {
            return JsonSerializer.Deserialize<T>(str);
        }
    }
}
