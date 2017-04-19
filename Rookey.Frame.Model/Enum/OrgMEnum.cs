/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System.ComponentModel;

namespace Rookey.Frame.Model.EnumSpace
{
    /// <summary>
    /// 性别
    /// </summary>
    public enum GenderEnum
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown = 0,

        /// <summary>
        /// 女
        /// </summary>
        [Description("女")]
        M = 1,

        /// <summary>
        /// 男
        /// </summary>
        [Description("男")]
        F = 2
    }

    /// <summary>
    /// 血型
    /// </summary>
    public enum BloodTypeEnum
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown = 0,

        /// <summary>
        /// A
        /// </summary>
        [Description("A")]
        A = 1,

        /// <summary>
        /// B
        /// </summary>
        [Description("B")]
        B = 2,

        /// <summary>
        /// AB
        /// </summary>
        [Description("AB")]
        AB = 3,

        /// <summary>
        /// O
        /// </summary>
        [Description("O")]
        O = 4
    }

    /// <summary>
    /// 学历
    /// </summary>
    public enum EducationEnum
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown = 0,

        /// <summary>
        /// 小学
        /// </summary>
        [Description("小学")]
        Primary = 1,

        /// <summary>
        /// 初中
        /// </summary>
        [Description("初中")]
        JuniorHigh = 2,

        /// <summary>
        /// 高中
        /// </summary>
        [Description("高中")]
        SeniorMiddle = 3,

        /// <summary>
        /// 专科
        /// </summary>
        [Description("专科")]
        Professional = 4,

        /// <summary>
        /// 本科
        /// </summary>
        [Description("本科")]
        Undergraduate = 5,

        /// <summary>
        /// 硕士
        /// </summary>
        [Description("硕士")]
        Master = 6,

        /// <summary>
        /// 博士
        /// </summary>
        [Description("博士")]
        Doctor = 7
    }

    /// <summary>
    /// 员工类型
    /// </summary>
    public enum EmployeeTypeEnum
    {
        /// <summary>
        /// 正式
        /// </summary>
        [Description("正式")]
        Official = 0,

        /// <summary>
        /// 试用期
        /// </summary>
        [Description("试用期")]
        Probation = 1,

        /// <summary>
        /// 实习生
        /// </summary>
        [Description("实习生")]
        Practice = 2,

        /// <summary>
        /// 临时员工
        /// </summary>
        [Description("临时员工")]
        Temporary = 3,
    }

    /// <summary>
    /// 员工状态
    /// </summary>
    public enum EmpStatusEnum
    {
        /// <summary>
        /// 在职
        /// </summary>
        [Description("在职")]
        Work = 0,

        /// <summary>
        /// 离职
        /// </summary>
        [Description("离职")]
        Resignation = 1,

        /// <summary>
        /// 退休
        /// </summary>
        [Description("退休")]
        Retired = 2
    }
}
