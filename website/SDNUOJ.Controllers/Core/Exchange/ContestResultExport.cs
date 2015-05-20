using System;
using System.Collections.Generic;
using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

using SDNUOJ.Entity;
using SDNUOJ.Entity.Complex;

namespace SDNUOJ.Controllers.Core.Exchange
{
    /// <summary>
    /// 导出竞赛结果类
    /// </summary>
    internal static class ContestResultExport
    {
        /// <summary>
        /// 将竞赛结果导出为Excel
        /// </summary>
        /// <param name="contest">竞赛实体</param>
        /// <param name="problems">竞赛题目</param>
        /// <param name="rank">竞赛排名信息</param>
        /// <param name="userdict">用户名姓名对照表</param>
        /// <returns>Excel文件</returns>
        public static Byte[] ExportResultToExcel(ContestEntity contest, IList<ContestProblemEntity> problems, IList<RankItem> rank, Dictionary<String, String> userdict)
        {
            MemoryStream ms = new MemoryStream();

            //相关参数
            Int32 problemCount = (problems == null ? 0 : problems.Count);
            Int32 rankCount = (rank == null ? 0 : rank.Count);

            //新建表格
            HSSFWorkbook workBook = new HSSFWorkbook();
            ISheet sheet = workBook.CreateSheet("Contest Ranklist");

            //设置样式
            ICellStyle headerStyle = workBook.CreateCellStyle();
            IFont headerFont = workBook.CreateFont();
            headerFont.FontHeightInPoints = 20;
            headerFont.Boldweight = 700;
            headerStyle.Alignment = HorizontalAlignment.Center;
            headerStyle.SetFont(headerFont);

            //设置标题行
            IRow rowTitle = sheet.CreateRow(0);
            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(contest.Title);
            cell.CellStyle = headerStyle;

            CellRangeAddress range = new CellRangeAddress(0, 0, 0, 4 + problemCount - 1);
            sheet.AddMergedRegion(range);

            //设置头部
            IRow rowHeader = sheet.CreateRow(1);
            rowHeader.HeightInPoints = 15;
            rowHeader.CreateCell(0).SetCellValue("Rank");
            rowHeader.CreateCell(1).SetCellValue("User Name");
            rowHeader.CreateCell(2).SetCellValue("Solved");
            rowHeader.CreateCell(3).SetCellValue("Penalty");

            sheet.SetColumnWidth(0, GetExcelWidth(5));
            sheet.SetColumnWidth(1, GetExcelWidth(27));
            sheet.SetColumnWidth(2, GetExcelWidth(7));
            sheet.SetColumnWidth(3, GetExcelWidth(10));

            for (Int32 i = 0; i < problemCount; i++)
            {
                rowHeader.CreateCell(rowHeader.Cells.Count).SetCellValue(problems[i].ContestProblemID.ToString());
                sheet.SetColumnWidth(rowHeader.Cells.Count - 1, GetExcelWidth(14));
            }

            //录入数据
            IRow rowUser = null;

            for (Int32 i = 0; i < rankCount; i++)
            {
                rowUser = sheet.CreateRow(sheet.PhysicalNumberOfRows);
                rowUser.HeightInPoints = 15;

                rowUser.CreateCell(0).SetCellValue(i + 1);
                rowUser.CreateCell(1).SetCellValue(GetShowName(rank[i].UserName, userdict));
                rowUser.CreateCell(2).SetCellValue(rank[i].SolvedCount);
                rowUser.CreateCell(3).SetCellValue(rank[i].Penalty.ToString());

                for (Int32 j = 0; j < problemCount; j++)
                {
                    rowUser.CreateCell(rowUser.Cells.Count).SetCellValue(
                        (rank[i][problems[j].ContestProblemID].Penalty != TimeSpan.Zero ? rank[i][problems[j].ContestProblemID].Penalty.ToString() : "")
                        + (rank[i][problems[j].ContestProblemID].WrongCount > 0 ? "(-" + rank[i][problems[j].ContestProblemID].WrongCount.ToString() + ")" : ""));
                }
            }

            workBook.Write(ms);
            return ms.ToArray();
        }

        private static String GetShowName(String username, Dictionary<String, String> userdict)
        {
            if (userdict == null || userdict.Count < 1)
            {
                return username;
            }

            String showname = String.Empty;
            return (userdict.TryGetValue(username, out showname) ? showname : username);
        }

        private static Int32 GetExcelWidth(Int32 width)
        {
            return width * 256;
        }
    }
}