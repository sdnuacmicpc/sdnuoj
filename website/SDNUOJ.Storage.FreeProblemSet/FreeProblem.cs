using System;
using System.Collections.Generic;
using System.Xml.Linq;

using SDNUOJ.Storage.FreeProblemSet.Exception;

namespace SDNUOJ.Storage.FreeProblemSet
{
    /// <summary>
    /// Free Problem
    /// </summary>
    public class FreeProblem
    {
        #region 字段
        private String _title;
        private Int32 _timeLimit;
        private Int32 _memoryLimit;
        private String _description;
        private String _input;
        private String _output;
        private String _sampleInput;
        private String _sampleOutput;
        private String _hint;
        private String _source;
        private List<FreeProblemDataPair> _testdatas;
        private List<FreeProblemImage> _images;
        #endregion

        #region 属性
        /// <summary>
        /// 获取题目标题
        /// </summary>
        public String Title
        {
            get { return this._title; }
        }

        /// <summary>
        /// 获取时间限制(MS)
        /// </summary>
        public Int32 TimeLimit
        {
            get { return this._timeLimit; }
        }

        /// <summary>
        /// 获取内存限制(KB)
        /// </summary>
        public Int32 MemoryLimit
        {
            get { return this._memoryLimit; }
        }

        /// <summary>
        /// 获取题目描述
        /// </summary>
        public String Description
        {
            get { return this._description; }
        }

        /// <summary>
        /// 获取题目输入
        /// </summary>
        public String Input
        {
            get { return this._input; }
        }

        /// <summary>
        /// 获取题目输出
        /// </summary>
        public String Output
        {
            get { return this._output; }
        }

        /// <summary>
        /// 获取题目样例输入
        /// </summary>
        public String SampleInput
        {
            get { return this._sampleInput; }
        }

        /// <summary>
        /// 获取题目样例输出
        /// </summary>
        public String SampleOutput
        {
            get { return this._sampleOutput; }
        }

        /// <summary>
        /// 获取题目提示
        /// </summary>
        public String Hint
        {
            get { return this._hint; }
        }

        /// <summary>
        /// 获取题目来源
        /// </summary>
        public String Source
        {
            get { return this._source; }
        }

        /// <summary>
        /// 获取测试数据
        /// </summary>
        public List<FreeProblemDataPair> TestData
        {
            get { return this._testdatas; }
        }

        /// <summary>
        /// 获取图像文件
        /// </summary>
        public List<FreeProblemImage> Images
        {
            get { return this._images; }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化行的Free Problem
        /// </summary>
        /// <param name="xe">Xml数据</param>
        public FreeProblem(XElement xe)
        {
            if (xe == null || xe.Name != "item")
            {
                throw new FreeProblemSetInvalidException();
            }

            this._title = this.GetElementStringValue(xe, "title", false);
            this._timeLimit = this.GetTimeLimitValue(xe, "time_limit");
            this._memoryLimit = this.GetMemoryLimitValue(xe, "memory_limit");
            this._description = this.GetElementStringValue(xe, "description", true);
            this._input = this.GetElementStringValue(xe, "input", true);
            this._output = this.GetElementStringValue(xe, "output", true);
            this._sampleInput = this.GetElementStringValue(xe, "sample_input", true);
            this._sampleOutput = this.GetElementStringValue(xe, "sample_output", true);
            this._hint = this.GetElementStringValue(xe, "hint", true);
            this._source = this.GetElementStringValue(xe, "source", true);

            IEnumerable<XElement> inputs = xe.Elements("test_input");
            IEnumerable<XElement> outputs = xe.Elements("test_output");

            if (inputs != null && outputs != null)
            {
                List<String> ins = new List<String>();
                List<String> outs = new List<String>();

                foreach (XElement input in inputs)
                {
                    ins.Add(input.Value);
                }

                foreach (XElement output in outputs)
                {
                    outs.Add(output.Value);
                }

                if (ins.Count != outs.Count)
                {
                    throw new FreeProblemSetDataInvalidException();
                }

                this._testdatas = new List<FreeProblemDataPair>();

                for (Int32 i = 0; i < ins.Count; i++)
                {
                    this._testdatas.Add(new FreeProblemDataPair(ins[i], outs[i]));
                }
            }

            IEnumerable<XElement> images = xe.Elements("img");

            if (images != null)
            {
                this._images = new List<FreeProblemImage>();

                foreach (XElement image in images)
                {
                    this._images.Add(new FreeProblemImage(GetElementStringValue(image, "src", false), GetElementStringValue(image, "base64", false)));
                }
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取子元素的字符串值
        /// </summary>
        /// <param name="xe">父节点</param>
        /// <param name="name">子节点名称</param>
        /// <param name="canEmpty">是否可以为空</param>
        /// <returns>字符串值</returns>
        private String GetElementStringValue(XElement xe, String name, Boolean canEmpty)
        {
            XElement element = xe.Element(name);

            if (element == null || String.IsNullOrEmpty(element.Value))
            {
                if (canEmpty)
                {
                    return String.Empty;
                }
                else
                {
                    throw new FreeProblemSetInvalidException();
                }
            }

            return element.Value;
        }

        /// <summary>
        /// 获取时间限制
        /// </summary>
        /// <param name="xe">父节点</param>
        /// <param name="name">子节点名称</param>
        /// <param name="canEmpty">是否可以为空</param>
        /// <returns>时间限制</returns>
        private Int32 GetTimeLimitValue(XElement xe, String name)
        {
            IEnumerable<XElement> elements = xe.Elements(name);

            if (elements == null)
            {
                throw new FreeProblemSetInvalidException();
            }

            Int32 value = 0;
            Double scale = 1000;//FPS默认单位为秒，SDNUOJ默认单位为毫秒
            Boolean success = false;

            foreach (XElement element in elements)
            {
                if (element == null || String.IsNullOrEmpty(element.Value))
                {
                    continue;
                }

                if (element.Attribute("unit") != null)
                {
                    String unit = element.Attribute("unit").Value;

                    if ("s".Equals(unit, StringComparison.OrdinalIgnoreCase))
                    {
                        scale = 1000;
                    }
                    else if ("ms".Equals(unit, StringComparison.OrdinalIgnoreCase))
                    {
                        scale = 1;
                    }
                }

                if (Int32.TryParse(element.Value, out value))
                {
                    success = true;
                    break;
                }
            }

            if (!success)
            {
                throw new FreeProblemSetInvalidException();
            }

            return (Int32)(value * scale);
        }

        /// <summary>
        /// 获取内存限制
        /// </summary>
        /// <param name="xe">父节点</param>
        /// <param name="name">子节点名称</param>
        /// <param name="canEmpty">是否可以为空</param>
        /// <returns>内存限制</returns>
        private Int32 GetMemoryLimitValue(XElement xe, String name)
        {
            IEnumerable<XElement> elements = xe.Elements(name);

            if (elements == null)
            {
                throw new FreeProblemSetInvalidException();
            }

            Int32 value = 0;
            Double scale = 1024;//FPS默认单位为MB，SDNUOJ默认单位为KB
            Boolean success = false;

            foreach (XElement element in elements)
            {
                if (element == null || String.IsNullOrEmpty(element.Value))
                {
                    continue;
                }

                if (element.Attribute("unit") != null)
                {
                    String unit = element.Attribute("unit").Value;

                    if ("mb".Equals(unit, StringComparison.OrdinalIgnoreCase))
                    {
                        scale = 1024;
                    }
                    else if ("kb".Equals(unit, StringComparison.OrdinalIgnoreCase))
                    {
                        scale = 1;
                    }
                    else if ("b".Equals(unit, StringComparison.OrdinalIgnoreCase))
                    {
                        scale = 1.0 / 1024;
                    }
                }

                if (Int32.TryParse(element.Value, out value))
                {
                    success = true;
                    break;
                }
            }

            if (!success)
            {
                throw new FreeProblemSetInvalidException();
            }

            return (Int32)(value * scale);
        }
        #endregion
    }
}