using System;

using SDNUOJ.Controllers.Exception;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Utilities.Text;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 提交错误数据管理类
    /// </summary>
    /// <remarks>
    /// HtmlEncode : 程序代码 / 调取时转换
    /// </remarks>
    internal static class SolutionErrorManager
    {
        #region 用户方法
        /// <summary>
        /// 根据ID得到一个提交错误实体
        /// </summary>
        /// <param name="id">提交ID</param>
        /// <returns>提交错误实体</returns>
        public static SolutionErrorEntity GetSolutionError(Int32 id)
        {
            if (id <= 0)
            {
                throw new InvalidRequstException(RequestType.SolutionError);
            }

            SolutionErrorEntity entity = SolutionErrorRepository.Instance.GetEntity(id);

            if (entity == null)
            {
                throw new NullResponseException(RequestType.SolutionError);
            }

            entity.ErrorInfo = HtmlEncoder.HtmlEncode(entity.ErrorInfo, 0, false, true);

            return entity;
        }
        #endregion
    }
}