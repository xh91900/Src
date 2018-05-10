using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SuperProducer.Core.Utility
{
    public class ProductionHelper
    {
        private const string EnvironmentFilePath = @"config\Environment.config";

        /// <summary>
        /// 运行环境类型
        /// </summary>
        public enum RunningEnvironmentType
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknown,

            /// <summary>
            /// 开发
            /// </summary>
            Development = 1,

            /// <summary>
            /// 测试
            /// </summary>
            Test = 2,

            /// <summary>
            /// 仿真环境
            /// </summary>
            Simulation = 98,

            /// <summary>
            /// 生产
            /// </summary>
            Production = 99,
        }

        /// <summary>
        /// 是否是生产环境
        /// </summary>
        public static bool IsProductionEnvironment()
        {
            return GetRunningEnvironment() == RunningEnvironmentType.Production;
        }

        /// <summary>
        /// 是否是仿真环境
        /// </summary>
        public static bool IsSimulationEnvironment()
        {
            return GetRunningEnvironment() == RunningEnvironmentType.Simulation;
        }

        /// <summary>
        /// 是否是测试环境
        /// </summary>
        public static bool IsTestEnvironment()
        {
            return GetRunningEnvironment() == RunningEnvironmentType.Test;
        }

        /// <summary>
        /// 是否是开发环境
        /// </summary>
        public static bool IsDevelopmentEnvironment()
        {
            return GetRunningEnvironment() == RunningEnvironmentType.Development;
        }

        /// <summary>
        /// 获取当前的运行环境
        /// </summary>
        public static RunningEnvironmentType GetRunningEnvironment()
        {
            try
            {
                var fileContent = FileHelper.GetFileContent(Path.Combine(AssemblyHelper.GetBaseDirectory(), EnvironmentFilePath));
                if (!string.IsNullOrEmpty(fileContent))
                {
                    var environmentConfigObject = SerializationHelper.XmlDeserialize<Environment>(fileContent);
                    if (environmentConfigObject != null)
                    {
                        if (EnumHelper.IsDefined(typeof(RunningEnvironmentType), environmentConfigObject.Type))
                        {
                            return (RunningEnvironmentType)environmentConfigObject.Type;
                        }
                    }
                }
            }
            catch { }
            return RunningEnvironmentType.Unknown;
        }

        [Serializable]
        [XmlRoot(ElementName = "Environment")]
        public class Environment
        {
            [XmlElement(ElementName = "type")]
            public int Type { get; set; }
        }
    }
}
