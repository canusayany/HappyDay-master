using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyValentinesDay
{
    /// <summary>
    /// 存储一些设置
    /// </summary>
    [Serializable]
   public class Class_Setting
    {
        public int SnowflakeCount { get; set; }
        public int SnowflakeSize { get; set; }
         
        public int SnowflakeType { get; set; }
    }
}
