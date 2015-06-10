using System;
using System.Collections.Generic;
using System.Text;

using SDNUOJ.Controllers.Exception;
using SDNUOJ.Configuration;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Utilities.Text.RegularExpressions;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 题目类型数据管理类
    /// </summary>
    internal static class ProblemCategoryItemManager
    {
        #region 管理方法
        /// <summary>
        /// 设置指定ID的题目类型对
        /// </summary>
        /// <param name="problemID">题目ID</param>
        /// <param name="sourceIDs">旧逗号分隔的题目类型ID</param>
        /// <param name="targetIDs">新逗号分隔的题目类型ID</param>
        /// <returns>是否成功设置</returns>
        public static IMethodResult AdminUpdateProblemCategoryItems(Int32 problemID, String sourceIDs, String targetIDs)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            if (problemID < ConfigurationManager.ProblemSetStartID)
            {
                return MethodResult.InvalidRequest(RequestType.Problem);
            }

            if ((!String.IsNullOrEmpty(sourceIDs) && !RegexVerify.IsNumericIDs(sourceIDs)) || (!String.IsNullOrEmpty(targetIDs) && !RegexVerify.IsNumericIDs(targetIDs)))
            {
                return MethodResult.InvalidRequest(RequestType.ProblemCategory);
            }

            StringBuilder deleteIDs = new StringBuilder();
            StringBuilder insertIDs = new StringBuilder();
            List<String> sourceid = (String.IsNullOrEmpty(sourceIDs) ? new List<String>() : new List<String>(sourceIDs.Split(',')));
            List<String> targetid = (String.IsNullOrEmpty(targetIDs) ? new List<String>() : new List<String>(targetIDs.Split(',')));

            for (Int32 i = 0; i < targetid.Count; i++)//删除sourceIDs和targetIDs中相同的数据
            {
                for (Int32 j = 0; j < sourceid.Count; j++)
                {
                    if (String.Equals(targetid[i], sourceid[j], StringComparison.OrdinalIgnoreCase))
                    {
                        targetid.RemoveAt(i);
                        sourceid.RemoveAt(j);
                        i--;
                        j--;
                        break;
                    }
                }
            }

            for (Int32 i = 0; i < sourceid.Count; i++)//设置deleteIDs
            {
                if (i > 0) deleteIDs.Append(',');
                deleteIDs.Append(sourceid[i]);
            }

            for (Int32 i = 0; i < targetid.Count; i++)//设置insertIDs
            {
                if (i > 0) insertIDs.Append(',');
                insertIDs.Append(targetid[i]);
            }

            Boolean ret = true;
            if (deleteIDs.Length > 0) ret &= (ProblemCategoryItemRepository.Instance.DeleteEntities(problemID, deleteIDs.ToString()) > 0);
            if (insertIDs.Length > 0) ret &= (ProblemCategoryItemRepository.Instance.InsertEntity(problemID, insertIDs.ToString()) > 0);

            if (!ret)
            {
                return MethodResult.FailedAndLog("No problem's category was updated!");
            }

            return MethodResult.SuccessAndLog("Admin update problem's category, id = {0}, new category = {1}", problemID.ToString(), targetIDs);
        }

        /// <summary>
        /// 获取题目类型列表
        /// </summary>
        /// <param name="problemID">题目ID</param>
        /// <returns>题目选择的类型列表</returns>
        public static IMethodResult AdminGetProblemCategoryItemList(Int32 problemID)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            List<ProblemCategoryItemEntity> lstPT = ProblemCategoryItemRepository.Instance.GetEntities(problemID);
            StringBuilder sb = new StringBuilder();

            List<ProblemCategoryEntity> lstSelectedList = new List<ProblemCategoryEntity>();
            List<ProblemCategoryEntity> lstUnSelectedList = new List<ProblemCategoryEntity>(ProblemCategoryManager.GetProblemCategoryList());
            
            if (lstPT == null)
            {
                lstPT = new List<ProblemCategoryItemEntity>();
            }

            for (Int32 i = 0; i < lstPT.Count; i++)
            {
                sb.Append(lstPT[i].TypeID.ToString()).Append(",");

                for (Int32 j = 0; j < lstUnSelectedList.Count; j++)
                {
                    if (lstUnSelectedList[j].TypeID == lstPT[i].TypeID)
                    {
                        lstSelectedList.Add(lstUnSelectedList[j]);
                        lstUnSelectedList.RemoveAt(j);
                        break;
                    }
                }
            }

            return MethodResult.Success(new Tuple<String, List<ProblemCategoryEntity>, List<ProblemCategoryEntity>>(sb.ToString(), lstUnSelectedList, lstSelectedList));
        }
        #endregion
    }
}