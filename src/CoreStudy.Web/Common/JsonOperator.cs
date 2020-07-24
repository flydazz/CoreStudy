using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreStudy.Web.Common
{
    /// <summary>
    /// 使用System.Text.Json进行序列化和反序列化操作
    /// </summary>
    public class JsonOperator
    {
        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string JsonSerialize(object value)
        {
            return JsonSerializer.Serialize(value);

        }

        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public T JsonDeserialize<T>(string value)
        {
            return JsonSerializer.Deserialize<T>(value);
        }
    }
}
