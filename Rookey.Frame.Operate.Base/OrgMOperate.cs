using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rookey.Frame.Common;
using Rookey.Frame.Model.OrgM;
using System.Linq.Expressions;
using Rookey.Frame.Operate.Base.TempModel;
using Rookey.Frame.Model.EnumSpace;
using Rookey.Frame.Base.Set;

namespace Rookey.Frame.Operate.Base
{
    /// <summary>
    /// 组织机构操作
    /// </summary>
    public static class OrgMOperate
    {
        #region 部门

        #region 基本

        /// <summary>
        /// 获取所有部门
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public static List<OrgM_Dept> GetAllDepts(Expression<Func<OrgM_Dept, bool>> expression = null)
        {
            string errMsg = string.Empty;
            Expression<Func<OrgM_Dept, bool>> exp = x => !x.IsDeleted && !x.IsDraft && x.IsValid;
            if (expression != null) exp = ExpressionExtension.And<OrgM_Dept>(exp, expression);
            List<OrgM_Dept> depts = CommonOperate.GetEntities<OrgM_Dept>(out errMsg, exp, null, false, new List<string>() { "Sort" }, new List<bool> { false });
            if (depts == null) depts = new List<OrgM_Dept>();
            return depts;
        }

        /// <summary>
        /// 获取部门根结点
        /// </summary>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static OrgM_Dept GetDeptRoot(Guid? companyId = null)
        {
            if (companyId.HasValue && companyId.Value != Guid.Empty)
                return GetDeptById(companyId.Value);
            List<OrgM_Dept> list = GetAllDepts(x => x.ParentId == null || x.ParentId == Guid.Empty);
            if (list.Count == 1) return list.FirstOrDefault();
            return null;
        }

        /// <summary>
        /// 获取所有的分管部门，公司下的一级部门
        /// </summary>
        /// <returns></returns>
        public static List<OrgM_Dept> GetChargeDepts()
        {
            OrgM_Dept root = GetDeptRoot();
            if (root != null)
                return GetAllDepts(x => x.ParentId == root.Id);
            return new List<OrgM_Dept>();
        }

        /// <summary>
        /// 根据部门ID获取部门信息
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
        public static OrgM_Dept GetDeptById(Guid deptId)
        {
            return GetAllDepts(x => x.Id == deptId).FirstOrDefault();
        }

        /// <summary>
        /// 根据部门编码获取部门信息
        /// </summary>
        /// <param name="deptCode">部门编码</param>
        /// <returns></returns>
        public static OrgM_Dept GetDeptByCode(string deptCode)
        {
            return GetAllDepts(x => x.Code == deptCode).FirstOrDefault();
        }

        /// <summary>
        /// 根据部门名称获取部门信息
        /// </summary>
        /// <param name="deptName">部门名称</param>
        /// <returns></returns>
        public static OrgM_Dept GetDeptByName(string deptName)
        {
            return GetAllDepts(x => x.Name == deptName).FirstOrDefault();
        }

        /// <summary>
        /// 获取层级部门，第一层级、第二层级、...，最多5层
        /// </summary>
        /// <param name="levelDepth">层级，小于等于5</param>
        /// <param name="deptId">当前部门ID</param>
        /// <param name="companyId">公司ID</param>
        /// <returns></returns>
        public static List<OrgM_Dept> GetLevelDepthDepts(int levelDepth, Guid? deptId = null, Guid? companyId = null)
        {
            List<OrgM_Dept> list = new List<OrgM_Dept>();
            OrgM_Dept root = GetDeptRoot(companyId);
            if (root != null)
            {
                if (levelDepth == 0) //第0层
                {
                    list.Add(root);
                }
                else if (levelDepth == 1) //第1层
                {
                    list = GetAllDepts(x => x.ParentId == root.Id);
                }
                else if (levelDepth == 2) //第2层
                {
                    List<Guid?> level1DeptIds = GetAllDepts(x => x.ParentId == root.Id).Select(x => (Guid?)x.Id).ToList();
                    list = GetAllDepts(x => level1DeptIds.Contains(x.ParentId));
                }
                else if (levelDepth == 3) //第3层
                {
                    List<Guid?> level1DeptIds = GetAllDepts(x => x.ParentId == root.Id).Select(x => (Guid?)x.Id).ToList();
                    List<Guid?> level2DeptIds = GetAllDepts(x => level1DeptIds.Contains(x.ParentId)).Select(x => (Guid?)x.Id).ToList();
                    list = GetAllDepts(x => level2DeptIds.Contains(x.ParentId));
                }
                else if (levelDepth == 4) //第4层
                {
                    List<Guid?> level1DeptIds = GetAllDepts(x => x.ParentId == root.Id).Select(x => (Guid?)x.Id).ToList();
                    List<Guid?> level2DeptIds = GetAllDepts(x => level1DeptIds.Contains(x.ParentId)).Select(x => (Guid?)x.Id).ToList();
                    List<Guid?> level3DeptIds = GetAllDepts(x => level2DeptIds.Contains(x.ParentId)).Select(x => (Guid?)x.Id).ToList();
                    list = GetAllDepts(x => level3DeptIds.Contains(x.ParentId));
                }
                else if (levelDepth == 5) //第5层
                {
                    List<Guid?> level1DeptIds = GetAllDepts(x => x.ParentId == root.Id).Select(x => (Guid?)x.Id).ToList();
                    List<Guid?> level2DeptIds = GetAllDepts(x => level1DeptIds.Contains(x.ParentId)).Select(x => (Guid?)x.Id).ToList();
                    List<Guid?> level3DeptIds = GetAllDepts(x => level2DeptIds.Contains(x.ParentId)).Select(x => (Guid?)x.Id).ToList();
                    List<Guid?> level4DeptIds = GetAllDepts(x => level3DeptIds.Contains(x.ParentId)).Select(x => (Guid?)x.Id).ToList();
                    list = GetAllDepts(x => level4DeptIds.Contains(x.ParentId));
                }
                if (list.Count > 0 && deptId.HasValue && deptId.Value != Guid.Empty)
                {
                    List<Guid> parentDeptIds = GetParentsDepts(deptId.Value, companyId).Select(x => x.Id).ToList();
                    list = list.Where(x => parentDeptIds.Contains(x.Id)).ToList();
                }
            }
            return list;
        }

        /// <summary>
        /// 获取员工的层级部门，第一层级、第二层级、...，最多5层
        /// </summary>
        /// <param name="levelDepth">层级，小于等于5</param>
        /// <param name="empId">员工ID</param>
        /// <param name="companyId">公司ID</param>
        /// <param name="deptId">找兼职部门时，要传兼职部门ID</param>
        /// <returns></returns>
        public static OrgM_Dept GetEmpLevelDepthDept(int levelDepth, Guid empId, Guid? companyId = null, Guid? deptId = null)
        {
            OrgM_Dept dept = null;
            if (deptId.HasValue && deptId.Value != Guid.Empty)
                dept = GetDeptById(deptId.Value);
            else
                dept = GetEmpMainDept(empId, companyId);
            if (dept == null) return null;
            List<OrgM_Dept> depthDepts = GetLevelDepthDepts(levelDepth, dept.Id, companyId);
            return depthDepts.FirstOrDefault();
        }

        #endregion

        #region 父部门

        /// <summary>
        /// 获取部门的父部门
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
        public static OrgM_Dept GetParentDept(Guid deptId)
        {
            OrgM_Dept dept = GetDeptById(deptId);
            if (dept.ParentId.HasValue && dept.ParentId.Value != Guid.Empty)
            {
                return GetDeptById(dept.ParentId.Value);
            }
            return null;
        }

        /// <summary>
        /// 获取部门的父部门
        /// </summary>
        /// <param name="deptCode">部门编码</param>
        /// <returns></returns>
        public static OrgM_Dept GetParentDeptByCode(string deptCode)
        {
            OrgM_Dept dept = GetDeptByCode(deptCode);
            if (dept.ParentId.HasValue && dept.ParentId.Value != Guid.Empty)
            {
                return GetDeptById(dept.ParentId.Value);
            }
            return null;
        }

        /// <summary>
        /// 获取部门的父部门
        /// </summary>
        /// <param name="deptName">部门名称</param>
        /// <returns></returns>
        public static OrgM_Dept GetParentDeptByName(string deptName)
        {
            OrgM_Dept dept = GetDeptByName(deptName);
            if (dept.ParentId.HasValue && dept.ParentId.Value != Guid.Empty)
            {
                return GetDeptById(dept.ParentId.Value);
            }
            return null;
        }

        /// <summary>
        /// 获取所有父级部门
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static List<OrgM_Dept> GetParentsDepts(Guid deptId, Guid? companyId = null)
        {
            List<OrgM_Dept> depts = new List<OrgM_Dept>();
            OrgM_Dept dept = GetDeptById(deptId);
            bool flag = dept != null && dept.ParentId.HasValue && dept.ParentId.Value != Guid.Empty;
            if (companyId.HasValue && companyId.Value != Guid.Empty)
                flag = flag && deptId != companyId.Value;
            depts.Add(dept); //添加当前部门
            if (flag) //存在上级部门
            {
                depts.AddRange(GetParentsDepts(dept.ParentId.Value));
            }
            return depts;
        }

        /// <summary>
        /// 获取当前部门的分管部门
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static OrgM_Dept GetCurrChargeDept(Guid deptId, Guid? companyId = null)
        {
            List<OrgM_Dept> depts = GetParentsDepts(deptId, companyId);
            OrgM_Dept root = GetDeptRoot(companyId);
            if (root != null)
                return depts.Where(x => x.ParentId == root.Id).FirstOrDefault();
            return null;
        }

        #endregion

        #region 子部门

        /// <summary>
        /// 获取子部门
        /// </summary>
        /// <param name="parentDeptId">父级部门ID</param>
        /// <param name="isDirect">是否直接子部门，否则取所有子级部门</param>
        /// <param name="expression">过滤表达式</param>
        /// <returns></returns>
        public static List<OrgM_Dept> GetChildDepts(Guid parentDeptId, bool isDirect = false, Expression<Func<OrgM_Dept, bool>> expression = null)
        {
            Expression<Func<OrgM_Dept, bool>> exp = x => x.ParentId != null && x.ParentId == parentDeptId;
            if (expression != null)
                exp = ExpressionExtension.And(exp, expression);
            List<OrgM_Dept> depts = GetAllDepts(exp);
            if (isDirect) //取直接子部门
            {
                return depts;
            }
            List<OrgM_Dept> list = new List<OrgM_Dept>();
            foreach (OrgM_Dept dept in depts)
            {
                list.Add(dept);
                list.AddRange(GetChildDepts(dept.Id, isDirect, expression));
            }
            return list;
        }

        #endregion

        #region 部门树

        /// <summary>
        /// 加载部门树
        /// </summary>
        /// <param name="deptId">部门根节点Id，为空是加载整棵树</param>
        /// <param name="expression">条件过滤表达式</param>
        /// <param name="isAsync">是否异步加载</param>
        /// <returns></returns>
        public static TreeNode LoadDeptTree(Guid? deptId, Expression<Func<OrgM_Dept, bool>> expression = null, bool isAsync = false)
        {
            OrgM_Dept root = !deptId.HasValue || deptId == Guid.Empty ? GetDeptRoot() : GetDeptById(deptId.Value);
            if (root == null)
            {
                TreeNode node = null;
                //加载部门节点
                List<OrgM_Dept> listDelts = GetAllDepts(expression);
                if (listDelts != null && listDelts.Count > 0)
                {
                    node = new TreeNode()
                    {
                        id = Guid.Empty.ToString(),
                        text = "根结点",
                        iconCls = "eu-icon-dept"
                    };
                    List<TreeNode> deptNodes = listDelts.Select(x => new TreeNode()
                    {
                        id = x.Id.ToString(),
                        text = x.Name,
                        iconCls = "eu-icon-dept"
                    }).ToList();
                    node.children = deptNodes;
                }
                return node;
            }
            else
            {
                if (expression != null)
                {
                    TreeNode node = new TreeNode()
                    {
                        id = root.Id.ToString(),
                        text = string.IsNullOrEmpty(root.Alias) ? root.Name : root.Alias,
                        iconCls = "eu-icon-dept"
                    };
                    //加载部门节点
                    List<OrgM_Dept> listDelts = GetAllDepts(expression);
                    if (listDelts != null && listDelts.Count > 0)
                    {
                        List<TreeNode> deptNodes = listDelts.Select(x => new TreeNode()
                        {
                            id = x.Id.ToString(),
                            text = x.Name,
                            iconCls = "eu-icon-dept"
                        }).ToList();
                        node.children = deptNodes;
                    }
                    return node;
                }
                else
                {
                    List<OrgM_Dept> listChilds = GetChildDepts(root.Id, isAsync, expression); //获取子部门
                    if (expression != null)
                    {
                        listChilds = listChilds.Where(expression.Compile()).ToList();
                    }
                    var tree = CommonOperate.GetTree<OrgM_Dept>(listChilds, root, null, "ParentId", "Alias", "eu-icon-dept", isAsync);
                    return tree;
                }
            }
        }

        #endregion

        #region 部门岗位

        /// <summary>
        /// 获取部门岗位
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
        public static List<OrgM_DeptDuty> GetDeptPositions(Guid deptId)
        {
            return GetAllPositions(x => x.OrgM_DeptId == deptId);
        }

        #endregion

        #region 部门职务

        /// <summary>
        /// 获取部门所有职务
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
        public static List<OrgM_Duty> GetDeptDutys(Guid deptId)
        {
            List<OrgM_DeptDuty> positions = GetDeptPositions(deptId);
            if (positions.Count > 0)
            {
                List<Guid> dutyIds = positions.Where(x => x.OrgM_DutyId.HasValue).Select(x => x.OrgM_DutyId.Value).ToList();
                return GetAllDutys(x => dutyIds.Contains(x.Id));
            }
            return new List<OrgM_Duty>();
        }

        #endregion

        #region 部门员工

        /// <summary>
        /// 获取部门人员
        /// </summary>
        /// <param name="deptId">部门Id</param>
        /// <param name="isDirect">是否取直接部门下人员,为否时包含子部门下人员</param>
        /// <param name="expression">过滤表达式</param>
        /// <returns></returns>
        public static List<OrgM_Emp> GetDeptEmps(Guid deptId, bool isDirect = false, Expression<Func<OrgM_Emp, bool>> expression = null)
        {
            List<OrgM_Emp> listEmps = new List<OrgM_Emp>();
            if (isDirect) //取部门人员，不包括子部门人员
            {
                List<OrgM_EmpDeptDuty> empPositions = GetAllEmpPositions(x => x.OrgM_DeptId == deptId);
                if (empPositions.Count > 0)
                {
                    List<Guid> empIds = empPositions.Where(x => x.OrgM_EmpId.HasValue).Select(x => x.OrgM_EmpId.Value).ToList();
                    Expression<Func<OrgM_Emp, bool>> exp = x => empIds.Contains(x.Id);
                    if (expression != null)
                        exp = ExpressionExtension.And(exp, expression);
                    listEmps = GetAllEmps(exp);
                }
            }
            else //取部门人员，包含所有子部门人员
            {
                listEmps.AddRange(GetDeptEmps(deptId, true, expression)); //添加当前部门人员
                List<OrgM_Dept> listDepts = GetChildDepts(deptId); //取所有子部门
                foreach (OrgM_Dept dept in listDepts)
                {
                    listEmps.AddRange(GetDeptEmps(dept.Id, true, expression)); //添加子部门人员
                }
            }
            return listEmps;
        }

        /// <summary>
        /// 获取部门负责人，部门leader
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
        public static OrgM_Emp GetDeptLeader(Guid deptId)
        {
            List<OrgM_DeptDuty> positions = GetDeptPositions(deptId);
            if (positions.Count > 0)
            {
                OrgM_DeptDuty position = positions.Where(x => x.IsDeptCharge).FirstOrDefault();
                if (position != null)
                    return GetPositionEmps(position.Id).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// 获取没有部门的员工
        /// </summary>
        /// <param name="expression">过滤表达式</param>
        /// <returns></returns>
        public static List<OrgM_Emp> GetNoDeptEmps(Expression<Func<OrgM_Emp, bool>> expression = null)
        {
            Expression<Func<OrgM_Emp, bool>> exp = expression;
            List<Guid> empIds = GetAllEmpPositions(x => x.OrgM_EmpId != null && x.OrgM_EmpId != Guid.Empty).Select(x => x.OrgM_EmpId.Value).ToList();
            if (empIds.Count > 0)
            {
                if (exp == null)
                    exp = x => !empIds.Contains(x.Id);
                else
                    exp = ExpressionExtension.And(exp, x => !empIds.Contains(x.Id));
            }
            return GetAllEmps(exp);
        }

        #endregion

        #endregion

        #region 岗位

        #region 职务

        /// <summary>
        /// 获取所有职务信息
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public static List<OrgM_Duty> GetAllDutys(Expression<Func<OrgM_Duty, bool>> expression = null)
        {
            string errMsg = string.Empty;
            Expression<Func<OrgM_Duty, bool>> exp = x => !x.IsDeleted && !x.IsDraft && x.IsValid;
            if (expression != null) exp = ExpressionExtension.And<OrgM_Duty>(exp, expression);
            List<OrgM_Duty> dutys = CommonOperate.GetEntities<OrgM_Duty>(out errMsg, exp, null, false, new List<string>() { "Sort" }, new List<bool>() { false });
            if (dutys == null) dutys = new List<OrgM_Duty>();
            return dutys;
        }

        /// <summary>
        /// 获取职务
        /// </summary>
        /// <param name="dutyId">职务ID</param>
        /// <returns></returns>
        public static OrgM_Duty GetDuty(Guid dutyId)
        {
            List<OrgM_Duty> dutys = GetAllDutys(x => x.Id == dutyId);
            return dutys.FirstOrDefault();
        }

        /// <summary>
        /// 获取职务人员
        /// </summary>
        /// <param name="dutyId">职务ID</param>
        /// <returns></returns>
        public static List<OrgM_Emp> GetDutyEmps(Guid dutyId)
        {
            List<OrgM_Emp> list = new List<OrgM_Emp>();
            List<OrgM_DeptDuty> positions = GetAllPositions(x => x.OrgM_DutyId == dutyId);
            foreach (OrgM_DeptDuty position in positions)
            {
                list.AddRange(GetPositionEmps(position.Id));
            }
            return list;
        }

        #endregion

        #region 岗位

        /// <summary>
        /// 获取所有岗位信息
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public static List<OrgM_DeptDuty> GetAllPositions(Expression<Func<OrgM_DeptDuty, bool>> expression = null)
        {
            string errMsg = string.Empty;
            Expression<Func<OrgM_DeptDuty, bool>> exp = x => !x.IsDeleted && !x.IsDraft && x.IsValid;
            if (expression != null) exp = ExpressionExtension.And<OrgM_DeptDuty>(exp, expression);
            List<OrgM_DeptDuty> positions = CommonOperate.GetEntities<OrgM_DeptDuty>(out errMsg, exp, null, false);
            if (positions == null) positions = new List<OrgM_DeptDuty>();
            return positions;
        }

        /// <summary>
        /// 获取岗位根结点
        /// </summary>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static OrgM_DeptDuty GetPositionRoot(Guid? companyId = null)
        {
            List<OrgM_DeptDuty> list = null;
            if (companyId.HasValue && companyId.Value != Guid.Empty)
            {
                list = GetAllPositions(x => x.OrgM_DeptId == companyId);
            }
            else
            {
                list = GetAllPositions(x => x.ParentId == null || x.ParentId == Guid.Empty);
            }
            if (list.Count == 1) return list.FirstOrDefault();
            return null;
        }

        /// <summary>
        /// 获取岗位信息
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns></returns>
        public static OrgM_DeptDuty GetPosition(Guid positionId)
        {
            return GetAllPositions(x => x.Id == positionId).FirstOrDefault();
        }

        /// <summary>
        /// 获取上级岗位
        /// </summary>
        /// <param name="positionId">岗位ID</param>
        /// <returns></returns>
        public static OrgM_DeptDuty GetParentPosition(Guid positionId)
        {
            OrgM_DeptDuty position = GetPosition(positionId);
            if (position != null && position.ParentId.HasValue && position.ParentId.Value != Guid.Empty)
                return GetPosition(position.ParentId.Value);
            return null;
        }

        /// <summary>
        /// 获取上级岗位
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <param name="dutyId">职务ID</param>
        /// <returns></returns>
        public static OrgM_DeptDuty GetParentPosition(Guid deptId, Guid dutyId)
        {
            OrgM_DeptDuty postion = GetAllPositions(x => x.OrgM_DeptId == deptId && x.OrgM_DutyId == dutyId).FirstOrDefault();
            if (postion != null && postion.ParentId.HasValue)
            {
                return GetPosition(postion.ParentId.Value);
            }
            return null;
        }

        /// <summary>
        /// 获取子岗位
        /// </summary>
        /// <param name="parentId">父岗位ID</param>
        /// <param name="isDirect">是否直接子岗位，否则获取所有下级岗位</param>
        /// <param name="expression">过滤表达式</param>
        /// <returns></returns>
        public static List<OrgM_DeptDuty> GetChildPositions(Guid parentId, bool isDirect = false, Expression<Func<OrgM_DeptDuty, bool>> expression = null)
        {
            Expression<Func<OrgM_DeptDuty, bool>> exp = x => x.ParentId != null && x.ParentId == parentId;
            if (expression != null)
                exp = ExpressionExtension.And(exp, expression);
            List<OrgM_DeptDuty> listPositions = GetAllPositions(exp);
            if (isDirect) //取直接子岗位
            {
                return listPositions;
            }
            List<OrgM_DeptDuty> list = new List<OrgM_DeptDuty>();
            foreach (OrgM_DeptDuty position in listPositions)
            {
                list.Add(position);
                list.AddRange(GetChildPositions(position.Id, isDirect, expression));
            }
            return list;
        }

        /// <summary>
        /// 加载岗位树
        /// </summary>
        /// <param name="positionId">岗位根结点Id，为空时加载整棵树</param>
        /// <param name="expression">岗位过滤表达式</param>
        /// <param name="isAsync">是否异步加载</param>
        /// <returns></returns>
        public static TreeNode LoadPositionTree(Guid? positionId, Expression<Func<OrgM_DeptDuty, bool>> expression = null, bool isAsync = false)
        {
            OrgM_DeptDuty root = !positionId.HasValue || positionId == Guid.Empty ? GetPositionRoot() : GetPosition(positionId.Value);
            if (root == null)
            {
                //加载岗位节点
                TreeNode node = null;
                List<OrgM_DeptDuty> listPositions = GetAllPositions(expression);
                if (listPositions != null && listPositions.Count > 0)
                {
                    node = new TreeNode()
                    {
                        id = Guid.Empty.ToString(),
                        text = "根结点",
                        iconCls = "eu-icon-dept"
                    };
                    List<TreeNode> positionNodes = listPositions.Select(x => new TreeNode()
                    {
                        id = x.Id.ToString(),
                        text = x.Name,
                        iconCls = "eu-icon-dept"
                    }).ToList();
                    node.children = positionNodes;
                }
                return node;
            }
            else
            {
                if (expression != null)
                {
                    TreeNode node = new TreeNode()
                    {
                        id = root.Id.ToString(),
                        text = root.Name,
                        iconCls = "eu-icon-dept"
                    };
                    List<OrgM_DeptDuty> listPositions = GetAllPositions(expression);
                    if (listPositions != null && listPositions.Count > 0)
                    {
                        List<TreeNode> positionNodes = listPositions.Select(x => new TreeNode()
                        {
                            id = x.Id.ToString(),
                            text = x.Name,
                            iconCls = "eu-icon-dept"
                        }).ToList();
                        node.children = positionNodes;
                    }
                    return node;
                }
                else
                {
                    List<OrgM_DeptDuty> listChilds = GetChildPositions(root.Id, isAsync, expression); //获取子岗位列表
                    TreeNode tree = CommonOperate.GetTree<OrgM_DeptDuty>(listChilds, root, null, "ParentId", "Name", "eu-icon-dept", isAsync);
                    return tree;
                }
            }
        }

        /// <summary>
        /// 加载部门岗位树
        /// </summary>
        /// <param name="deptId">部门根结点ID，为空是加载整棵树</param>
        /// <returns></returns>
        public static TreeNode LoadDeptPositionTree(Guid? deptId)
        {
            TreeNode node = null;
            OrgM_Dept root = deptId.HasValue && deptId.Value != Guid.Empty ? GetDeptById(deptId.Value) : GetDeptRoot();
            List<TreeNode> list = new List<TreeNode>();
            if (root != null)
            {
                //部门根结点
                node = new TreeNode()
                {
                    id = root.Id.ToString(),
                    text = root.Alias,
                    iconCls = "eu-icon-dept"
                };
                //加载岗位节点
                //获取部门下岗位
                List<OrgM_DeptDuty> positionList = GetDeptPositions(root.Id);
                if (positionList != null && positionList.Count > 0)
                {
                    List<TreeNode> positionNodes = positionList.Select(x => new TreeNode()
                    {
                        id = x.Id.ToString(),
                        text = x.Name,
                        iconCls = string.Empty
                    }).ToList();
                    if (positionNodes != null && positionNodes.Count > 0)
                    {
                        list.AddRange(positionNodes);
                    }
                }
                //加载部门子结点
                List<OrgM_Dept> listDepts = GetChildDepts(root.Id, true);
                foreach (OrgM_Dept dept in listDepts)
                {
                    TreeNode tempNode = LoadDeptPositionTree(dept.Id);
                    if (tempNode != null)
                    {
                        list.Add(tempNode);
                    }
                }
                node.children = list;
            }
            else
            {
                //加载岗位节点
                List<OrgM_DeptDuty> listPositions = GetAllPositions();
                if (listPositions != null && listPositions.Count > 0)
                {
                    node = new TreeNode()
                    {
                        id = Guid.Empty.ToString(),
                        text = "根结点",
                        iconCls = "eu-icon-dept"
                    };
                    List<TreeNode> positionNodes = listPositions.Select(x => new TreeNode()
                    {
                        id = x.Id.ToString(),
                        text = x.Name,
                        iconCls = string.Empty
                    }).ToList();
                    list.AddRange(positionNodes);
                    node.children = list;
                }
            }
            return node;
        }

        /// <summary>
        /// 获取岗位人员
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <param name="dutyId">职务ID</param>
        /// <returns></returns>
        public static List<OrgM_Emp> GetPositionEmps(Guid deptId, Guid dutyId)
        {
            List<OrgM_EmpDeptDuty> empPositions = GetAllEmpPositions(x => x.OrgM_DeptId == deptId && x.OrgM_DutyId == dutyId);
            if (empPositions.Count > 0)
            {
                List<Guid> empIds = empPositions.Where(x => x.OrgM_EmpId.HasValue).Select(x => x.OrgM_EmpId.Value).ToList();
                return GetAllEmps(x => empIds.Contains(x.Id));
            }
            return new List<OrgM_Emp>();
        }

        /// <summary>
        /// 获取岗位人员
        /// </summary>
        /// <returns></returns>
        public static List<OrgM_Emp> GetPositionEmps(Guid positionId)
        {
            OrgM_DeptDuty position = GetPosition(positionId);
            if (position != null && position.OrgM_DeptId.HasValue &&
                position.OrgM_DeptId.Value != Guid.Empty && position.OrgM_DutyId.HasValue &&
                position.OrgM_DutyId.Value != Guid.Empty)
            {
                return GetPositionEmps(position.OrgM_DeptId.Value, position.OrgM_DutyId.Value);
            }
            return new List<OrgM_Emp>();
        }

        #endregion

        #endregion

        #region 员工

        #region 基本

        /// <summary>
        /// 获取所有员工，只包含在职员工
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public static List<OrgM_Emp> GetAllEmps(Expression<Func<OrgM_Emp, bool>> expression = null)
        {
            string errMsg = string.Empty;
            int empStatus = (int)EmpStatusEnum.Work;
            Expression<Func<OrgM_Emp, bool>> exp = x => x.EmpStatus == empStatus && !x.IsDeleted && !x.IsDraft;
            if (expression != null) exp = ExpressionExtension.And<OrgM_Emp>(exp, expression);
            List<OrgM_Emp> emps = CommonOperate.GetEntities<OrgM_Emp>(out errMsg, exp, null, false, new List<string>() { "Sort" }, new List<bool>() { false });
            if (emps == null) emps = new List<OrgM_Emp>();
            return emps;
        }

        /// <summary>
        /// 获取员工信息
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <returns></returns>
        public static OrgM_Emp GetEmp(Guid empId)
        {
            List<OrgM_Emp> emps = GetAllEmps(x => x.Id == empId);
            return emps.FirstOrDefault();
        }

        /// <summary>
        /// 根据员工编号获取员工信息
        /// </summary>
        /// <param name="empCode">员工编号</param>
        /// <returns></returns>
        public static OrgM_Emp GetEmpByCode(string empCode)
        {
            if (string.IsNullOrEmpty(empCode)) return null;
            List<OrgM_Emp> emps = GetAllEmps(x => x.Code != null && x.Code == empCode);
            return emps.FirstOrDefault();
        }

        /// <summary>
        /// 获取员工姓名
        /// </summary>
        /// <param name="empCode">员工编号</param>
        /// <returns></returns>
        public static string GetEmpNameByCode(string empCode)
        {
            OrgM_Emp emp = GetEmpByCode(empCode);
            if (emp != null)
                return emp.Name.ObjToStr();
            return string.Empty;
        }

        /// <summary>
        /// 获取员工姓名
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <returns></returns>
        public static string GetEmpName(Guid empId)
        {
            OrgM_Emp emp = GetEmp(empId);
            if (emp != null)
                return emp.Name.ObjToStr();
            return string.Empty;
        }

        /// <summary>
        /// 根据手机号获取员工信息
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <returns></returns>
        public static OrgM_Emp GetEmpByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return null;
            List<OrgM_Emp> emps = GetAllEmps(x => x.Mobile != null && x.Mobile == mobile);
            return emps.FirstOrDefault();
        }

        /// <summary>
        /// 根据邮箱获取员工信息
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns></returns>
        public static OrgM_Emp GetEmpByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;
            List<OrgM_Emp> emps = GetAllEmps(x => x.Email != null && x.Email.ToLower() == email.ToLower());
            return emps.FirstOrDefault();
        }

        /// <summary>
        /// 根据邮箱前缀获取员工信息
        /// </summary>
        /// <param name="emailPrex">邮箱前缀</param>
        /// <returns></returns>
        public static OrgM_Emp GetEmpByEmailPrex(string emailPrex)
        {
            if (string.IsNullOrEmpty(emailPrex)) return null;
            List<OrgM_Emp> emps = GetAllEmps(x => x.Email != null && x.Email.Contains("@") && x.Email.Substring(0, x.Email.IndexOf("@")).ToLower() == emailPrex.ToLower());
            return emps.FirstOrDefault();
        }

        #endregion

        #region 员工岗位

        /// <summary>
        /// 获取所有员工岗位
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public static List<OrgM_EmpDeptDuty> GetAllEmpPositions(Expression<Func<OrgM_EmpDeptDuty, bool>> expression = null)
        {
            string errMsg = string.Empty;
            Expression<Func<OrgM_EmpDeptDuty, bool>> exp = x => x.IsDeleted == false && x.IsDraft == false && x.IsValid == true;
            if (expression != null) exp = ExpressionExtension.And(expression, exp);
            List<OrgM_EmpDeptDuty> empPositions = CommonOperate.GetEntities<OrgM_EmpDeptDuty>(out errMsg, exp, null, false);
            if (empPositions == null) empPositions = new List<OrgM_EmpDeptDuty>();
            return empPositions;
        }

        /// <summary>
        /// 获取员工岗位，包含兼职
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static List<OrgM_EmpDeptDuty> GetEmpPositions(Guid empId, Guid? companyId = null)
        {
            if (companyId.HasValue && companyId.Value != Guid.Empty)
            {
                List<Guid?> childDeptIds = GetChildDepts(companyId.Value).Select(x => (Guid?)x.Id).ToList();
                return GetAllEmpPositions(x => x.OrgM_EmpId == empId && childDeptIds.Contains(x.OrgM_DeptId));
            }
            return GetAllEmpPositions(x => x.OrgM_EmpId == empId);
        }

        /// <summary>
        /// 获取员工主职岗位
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static OrgM_EmpDeptDuty GetEmpMainPosition(Guid empId, Guid? companyId = null)
        {
            if (companyId.HasValue && companyId.Value != Guid.Empty)
            {
                List<Guid?> childDeptIds = GetChildDepts(companyId.Value).Select(x => (Guid?)x.Id).ToList();
                return GetAllEmpPositions(x => x.OrgM_EmpId == empId && x.IsMainDuty && childDeptIds.Contains(x.OrgM_DeptId)).FirstOrDefault();
            }
            return GetAllEmpPositions(x => x.OrgM_EmpId == empId && x.IsMainDuty).FirstOrDefault();
        }

        /// <summary>
        /// 获取员工兼职岗位集合
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static List<OrgM_EmpDeptDuty> GetEmpPartTimePosition(Guid empId, Guid? companyId = null)
        {
            if (companyId.HasValue && companyId.Value != Guid.Empty)
            {
                List<Guid?> childDeptIds = GetChildDepts(companyId.Value).Select(x => (Guid?)x.Id).ToList();
                return GetAllEmpPositions(x => x.OrgM_EmpId == empId && !x.IsMainDuty && childDeptIds.Contains(x.OrgM_DeptId));
            }
            return GetAllEmpPositions(x => x.OrgM_EmpId == empId && !x.IsMainDuty);
        }

        /// <summary>
        /// 获取员工兼职岗位集合
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static List<OrgM_DeptDuty> GetPartTimePositions(Guid empId, Guid? companyId = null)
        {
            List<OrgM_DeptDuty> positions = new List<OrgM_DeptDuty>();
            List<OrgM_EmpDeptDuty> empPositions = GetEmpPartTimePosition(empId, companyId);
            foreach (OrgM_EmpDeptDuty empPosition in empPositions)
            {
                OrgM_DeptDuty position = GetAllPositions(x => x.OrgM_DeptId == empPosition.OrgM_DeptId && x.OrgM_DutyId == empPosition.OrgM_DutyId).FirstOrDefault();
                if (position != null)
                    positions.Add(position);
            }
            return positions;
        }

        /// <summary>
        /// 获取员工职务，包含兼职
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static List<OrgM_Duty> GetEmpDutys(Guid empId, Guid? companyId = null)
        {
            List<OrgM_EmpDeptDuty> empPositions = GetEmpPositions(empId, companyId);
            if (empPositions.Count > 0)
            {
                List<Guid> dutyIds = empPositions.Where(x => x.OrgM_DutyId.HasValue).Select(x => x.OrgM_DutyId.Value).ToList();
                return GetAllDutys(x => dutyIds.Contains(x.Id));
            }
            return new List<OrgM_Duty>();
        }

        /// <summary>
        /// 获取员工主职职务
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static OrgM_Duty GetEmpMainDuty(Guid empId, Guid? companyId = null)
        {
            OrgM_EmpDeptDuty empPosition = GetEmpMainPosition(empId, companyId);
            if (empPosition != null && empPosition.OrgM_DutyId.HasValue && empPosition.OrgM_DutyId.Value != Guid.Empty)
                return GetDuty(empPosition.OrgM_DutyId.Value);
            return null;
        }

        /// <summary>
        /// 获取员工兼职职务集合
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static List<OrgM_Duty> GetEmpPartTimeDutys(Guid empId, Guid? companyId = null)
        {
            List<OrgM_EmpDeptDuty> empPositions = GetEmpPartTimePosition(empId, companyId);
            if (empPositions != null && empPositions.Count > 0)
            {
                List<Guid> dutyIds = empPositions.Where(x => x.OrgM_DutyId.HasValue).Select(x => x.OrgM_DutyId.Value).ToList();
                return GetAllDutys(x => dutyIds.Contains(x.Id));
            }
            return new List<OrgM_Duty>();
        }

        #endregion

        #region 员工部门

        /// <summary>
        /// 获取员工所属部门，包括兼职部门
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static List<OrgM_Dept> GetEmpDepts(Guid empId, Guid? companyId = null)
        {
            List<OrgM_Dept> depts = new List<OrgM_Dept>();
            List<OrgM_EmpDeptDuty> empPositions = GetEmpPositions(empId, companyId);
            if (empPositions.Count > 0)
            {
                List<Guid> deptIds = empPositions.Where(x => x.OrgM_DeptId.HasValue).Select(x => x.OrgM_DeptId.Value).ToList();
                depts = GetAllDepts(x => deptIds.Contains(x.Id));
            }
            return depts;
        }

        /// <summary>
        /// 获取员工主职部门
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static OrgM_Dept GetEmpMainDept(Guid empId, Guid? companyId = null)
        {
            OrgM_EmpDeptDuty empPosition = GetEmpMainPosition(empId, companyId);
            if (empPosition != null && empPosition.OrgM_DeptId.HasValue && empPosition.OrgM_DeptId.Value != Guid.Empty)
            {
                return GetDeptById(empPosition.OrgM_DeptId.Value);
            }
            return null;
        }

        /// <summary>
        /// 获取员工部门显示名称（部门简称）
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static string GetEmpMainDeptName(Guid empId, Guid? companyId = null)
        {
            OrgM_Dept dept = GetEmpMainDept(empId, companyId);
            if (dept != null)
                return string.IsNullOrEmpty(dept.Alias) ? dept.Name : dept.Alias;
            return string.Empty;
        }

        /// <summary>
        /// 获取员工兼职部门
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static List<OrgM_Dept> GetEmpPartTimeDepts(Guid empId, Guid? companyId = null)
        {
            List<OrgM_Dept> depts = new List<OrgM_Dept>();
            List<OrgM_EmpDeptDuty> empPositions = GetEmpPartTimePosition(empId, companyId);
            if (empPositions.Count > 0)
            {
                List<Guid> deptIds = empPositions.Where(x => x.OrgM_DeptId.HasValue).Select(x => x.OrgM_DeptId.Value).ToList();
                depts = GetAllDepts(x => deptIds.Contains(x.Id));
            }
            return depts;
        }

        /// <summary>
        /// 获取员工所属公司
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <returns></returns>
        public static List<OrgM_Dept> GetEmpCompanys(Guid empId)
        {
            List<Guid> deptIds = GetAllEmpPositions(x => x.OrgM_EmpId == empId && x.IsMainDuty).Where(x => x.OrgM_DeptId.HasValue && x.OrgM_DeptId.Value != Guid.Empty).Select(x => x.OrgM_DeptId.Value).ToList();
            return GetAllDepts(x => deptIds.Contains(x.Id) && x.IsCompany);
        }

        #endregion

        #region 部门员工树

        /// <summary>
        /// 部门人员树
        /// </summary
        /// <param name="deptId">部门根结点ID，为空是加载整棵树</param>
        /// <param name="isAsync">是否异步加载</param>
        /// <param name="expression">过滤表达式</param>
        /// <returns></returns>
        public static TreeNode LoadDeptEmpTree(Guid? deptId, bool isAsync = false, Expression<Func<OrgM_Emp, bool>> expression = null)
        {
            TreeNode node = null;
            OrgM_Dept deptRoot = GetDeptRoot();
            OrgM_Dept root = deptId.HasValue && deptId.Value != Guid.Empty ? GetDeptById(deptId.Value) : deptRoot;
            List<TreeNode> list = new List<TreeNode>();
            if (root != null)
            {
                if (expression == null)
                {
                    //部门根结点
                    node = new TreeNode()
                    {
                        id = root.Id.ToString(),
                        text = string.IsNullOrEmpty(root.Alias) ? root.Name : root.Alias,
                        iconCls = "eu-icon-dept",
                        state = isAsync ? "closed" : "open"
                    };
                    //加载人员节点
                    List<OrgM_Emp> listEmps = GetDeptEmps(root.Id, true, expression); //该部门下人员
                    if (listEmps != null && listEmps.Count > 0)
                    {
                        List<TreeNode> empNodes = listEmps.Select(x => new TreeNode()
                        {
                            id = x.Id.ToString(),
                            text = string.Format("{0} {1}", x.Name, x.Code.ObjToStr()),
                            iconCls = "eu-icon-user",
                        }).ToList();
                        list.AddRange(empNodes);
                    }
                    //加载部门子结点
                    List<OrgM_Dept> listDepts = GetChildDepts(root.Id, true);
                    if (!isAsync) //同步方式
                    {
                        foreach (OrgM_Dept dept in listDepts)
                        {
                            TreeNode tempNode = LoadDeptEmpTree(dept.Id, isAsync, expression);
                            if (tempNode != null && tempNode.children != null && tempNode.children.ToList().Count > 0)
                            {
                                list.Add(tempNode);
                            }
                        }
                    }
                    else //异步方式
                    {
                        list.AddRange(listDepts.Select(x => new TreeNode() { id = x.Id.ToString(), text = string.IsNullOrEmpty(x.Alias) ? root.Name : x.Alias, iconCls = "eu-icon-dept", state = isAsync ? "closed" : "open" }));
                    }
                    node.children = list;
                    if (deptRoot != null && deptRoot.Id == root.Id)
                    {
                        //加载没有部门的员工
                        List<OrgM_Emp> noDeptEmps = OrgMOperate.GetNoDeptEmps(expression);
                        if (noDeptEmps.Count > 0)
                        {
                            TreeNode tempNode = new TreeNode()
                            {
                                id = Guid.Empty.ToString(),
                                text = "根结点",
                                iconCls = "eu-icon-dept"
                            };
                            List<TreeNode> tempChildren = new List<TreeNode>() { node };
                            List<TreeNode> noEmpNodes = noDeptEmps.Select(x => new TreeNode()
                            {
                                id = x.Id.ToString(),
                                text = string.Format("{0} {1}", x.Name, x.Code.ObjToStr()),
                                iconCls = "eu-icon-user"
                            }).ToList();
                            tempChildren.AddRange(noEmpNodes);
                            tempNode.children = tempChildren;
                            node = tempNode;
                        }
                    }
                }
                else
                {
                    node = new TreeNode()
                    {
                        id = root.Id.ToString(),
                        text = string.IsNullOrEmpty(root.Alias) ? root.Name : root.Alias,
                        iconCls = "eu-icon-dept"
                    };
                    List<OrgM_Emp> listEmps = GetAllEmps(expression);
                    if (listEmps != null && listEmps.Count > 0)
                    {
                        List<TreeNode> empNodes = listEmps.Select(x => new TreeNode()
                        {
                            id = x.Id.ToString(),
                            text = string.Format("{0} {1}", x.Name, x.Code.ObjToStr()),
                            iconCls = "eu-icon-user"
                        }).ToList();
                        list.AddRange(empNodes);
                        node.children = list;
                    }
                }
            }
            else
            {
                //加载人员节点
                List<OrgM_Emp> listEmps = GetAllEmps(expression);
                if (listEmps != null && listEmps.Count > 0)
                {
                    node = new TreeNode()
                    {
                        id = Guid.Empty.ToString(),
                        text = "根结点",
                        iconCls = "eu-icon-dept"
                    };
                    List<TreeNode> empNodes = listEmps.Select(x => new TreeNode()
                    {
                        id = x.Id.ToString(),
                        text = string.Format("{0} {1}", x.Name, x.Code.ObjToStr()),
                        iconCls = "eu-icon-user"
                    }).ToList();
                    list.AddRange(empNodes);
                    node.children = list;
                }
            }
            return node;
        }

        #endregion

        #region 员工上下级

        /// <summary>
        /// 获取员工上级人员
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="deptId">针对有兼职的员工需要传部门ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static OrgM_Emp GetEmpParentEmp(Guid empId, Guid? deptId = null, Guid? companyId = null)
        {
            List<OrgM_EmpDeptDuty> empPositions = GetEmpPositions(empId, companyId);
            if (empPositions.Count > 0)
            {
                if (empPositions.Count > 1 && deptId.HasValue && deptId.Value != Guid.Empty) //有兼职岗位
                    empPositions = empPositions.Where(x => x.OrgM_DeptId == deptId.Value).ToList();
                OrgM_EmpDeptDuty empPosition = empPositions.FirstOrDefault();
                if (empPosition != null && empPosition.OrgM_DeptId.HasValue && empPosition.OrgM_DutyId.HasValue)
                {
                    OrgM_DeptDuty parentPosition = GetParentPosition(empPosition.OrgM_DeptId.Value, empPosition.OrgM_DutyId.Value);
                    if (parentPosition != null)
                        return GetPositionEmps(parentPosition.Id).FirstOrDefault();
                }
            }
            return null;
        }

        /// <summary>
        /// 获取员工下级人员
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="isDirect">是否直接下属，否则所有下属</param>
        /// <param name="deptId">针对有兼职的员工需要传部门ID，否则取所有</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static List<OrgM_Emp> GetEmpChildsEmps(Guid empId, bool isDirect = true, Guid? deptId = null, Guid? companyId = null)
        {
            List<OrgM_Emp> emps = new List<OrgM_Emp>();
            List<OrgM_EmpDeptDuty> empPositions = GetEmpPositions(empId, companyId);
            if (empPositions.Count > 0)
            {
                if (empPositions.Count > 1 && deptId.HasValue && deptId.Value != Guid.Empty) //有兼职岗位
                    empPositions = empPositions.Where(x => x.OrgM_DeptId == deptId.Value).ToList();
                if (empPositions.Count > 0)
                {
                    foreach (OrgM_EmpDeptDuty empPosition in empPositions)
                    {
                        OrgM_DeptDuty positionCurr = GetAllPositions(x => x.OrgM_DeptId != null && x.OrgM_DutyId != null && x.OrgM_DeptId == empPosition.OrgM_DeptId && x.OrgM_DutyId == empPosition.OrgM_DutyId).FirstOrDefault();
                        if (positionCurr != null)
                        {
                            List<OrgM_DeptDuty> childPositions = GetChildPositions(positionCurr.Id, isDirect);
                            foreach (OrgM_DeptDuty position in childPositions)
                            {
                                emps.AddRange(GetPositionEmps(position.Id));
                            }
                        }
                    }
                }
            }
            return emps;
        }

        /// <summary>
        /// 获取员工下级人员用户ID集合
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="isDirect">是否直接下属，否则所有下属</param>
        /// <param name="deptId">针对有兼职的员工需要传部门ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static List<Guid> GetEmpChildUserIds(Guid empId, bool isDirect = true, Guid? deptId = null, Guid? companyId = null)
        {
            List<Guid> list = new List<Guid>();
            List<OrgM_Emp> emps = GetEmpChildsEmps(empId, isDirect, deptId, companyId);
            foreach (OrgM_Emp emp in emps)
            {
                list.Add(GetUserIdByEmpId(emp.Id));
            }
            return list;
        }

        /// <summary>
        /// 获取员工的部门负责人
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <param name="deptId">针对有兼职的员工需要传部门ID</param>
        /// <param name="companyId">所属公司ID</param>
        /// <returns></returns>
        public static OrgM_Emp GetEmpChargeLeader(Guid empId, Guid? deptId = null, Guid? companyId = null)
        {
            if (deptId.HasValue && deptId.Value != Guid.Empty)
            {
                return GetDeptLeader(deptId.Value);
            }
            OrgM_Dept dept = GetEmpMainDept(empId, companyId);
            return GetDeptLeader(dept.Id);
        }

        #endregion

        #region 员工用户

        /// <summary>
        /// 根据用户名获取员工对象
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public static OrgM_Emp GetEmpByUserName(string username)
        {
            OrgM_Emp emp = null;
            if (!string.IsNullOrEmpty(username))
            {
                switch (GlobalSet.EmpUserNameConfigRule)
                {
                    case UserNameAndEmpConfigRule.EmpCode:
                        emp = GetEmpByCode(username.ToLower());
                        break;
                    case UserNameAndEmpConfigRule.EmailPre:
                        emp = GetAllEmps(x => x.Email != null && x.Email.Contains("@")).Where(x => x.Email.Substring(0, x.Email.IndexOf("@")).ToLower() == username.ToLower()).FirstOrDefault();
                        break;
                    case UserNameAndEmpConfigRule.Email:
                        emp = GetAllEmps().Where(x => x.Email != null && x.Email.ToLower() == username.ToLower()).FirstOrDefault();
                        break;
                    case UserNameAndEmpConfigRule.Mobile:
                        emp = GetAllEmps(x => x.Mobile != null && x.Mobile == username).FirstOrDefault();
                        break;
                }
            }
            return emp;
        }

        /// <summary>
        /// 根据用户ID获取员工
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public static OrgM_Emp GetEmpByUserId(Guid userId)
        {
            string username = UserOperate.GetUserNameByUserId(userId);
            OrgM_Emp emp = GetEmpByUserName(username);
            return emp;
        }

        /// <summary>
        /// 根据用户名集合获取员工集合
        /// </summary>
        /// <param name="usernames">用户名集合，小写</param>
        /// <returns></returns>
        public static List<OrgM_Emp> GetEmpsByUserNames(List<string> usernames)
        {
            List<OrgM_Emp> emps = new List<OrgM_Emp>();
            if (usernames != null && usernames.Count > 0)
            {
                switch (GlobalSet.EmpUserNameConfigRule)
                {
                    case UserNameAndEmpConfigRule.EmpCode:
                        emps = GetAllEmps(x => x.Code != null && usernames.Contains(x.Code.ToLower()));
                        break;
                    case UserNameAndEmpConfigRule.EmailPre:
                        emps = GetAllEmps(x => x.Email != null && x.Email.Contains("@")).Where(x => usernames.Contains(x.Email.Substring(0, x.Email.IndexOf("@")).ToLower())).ToList();
                        break;
                    case UserNameAndEmpConfigRule.Email:
                        emps = GetAllEmps(x => x.Email != null && usernames.Contains(x.Email.ToLower()));
                        break;
                    case UserNameAndEmpConfigRule.Mobile:
                        emps = OrgMOperate.GetAllEmps(x => x.Mobile != null && usernames.Contains(x.Mobile));
                        break;
                }
            }
            return emps;
        }

        /// <summary>
        /// 根据员工信息获取用户名
        /// </summary>
        /// <param name="emp">员工信息</param>
        /// <returns></returns>
        public static string GetUserNameByEmp(OrgM_Emp emp)
        {
            if (emp == null) return string.Empty;
            string username = string.Empty;
            switch (GlobalSet.EmpUserNameConfigRule)
            {
                case UserNameAndEmpConfigRule.EmpCode:
                    username = emp.Code;
                    break;
                case UserNameAndEmpConfigRule.EmailPre:
                    username = emp.Email != null && emp.Email.Contains("@") ? emp.Email.Substring(0, emp.Email.IndexOf("@")) : string.Empty;
                    break;
                case UserNameAndEmpConfigRule.Email:
                    username = emp.Email;
                    break;
                case UserNameAndEmpConfigRule.Mobile:
                    username = emp.Mobile;
                    break;
            }
            return username;
        }

        /// <summary>
        /// 根据员工ID获取用户名
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <returns></returns>
        public static string GetUserNameByEmpId(Guid empId)
        {
            OrgM_Emp emp = GetEmp(empId);
            if (emp == null) return string.Empty;
            return GetUserNameByEmp(emp);
        }

        /// <summary>
        /// 根据员工ID获取用户ID
        /// </summary>
        /// <param name="empId">员工ID</param>
        /// <returns></returns>
        public static Guid GetUserIdByEmpId(Guid empId)
        {
            string username = GetUserNameByEmpId(empId);
            if (!string.IsNullOrEmpty(username))
            {
                return UserOperate.GetUserIdByUserName(username);
            }
            return Guid.Empty;
        }

        #endregion

        #region 员工邮箱

        /// <summary>
        /// 获取人员邮箱地址，返回Dictionary<string,string>格式，key-邮件地址，value-员工姓名
        /// </summary>
        /// <param name="empIds">员工Id集合</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetEmployeeEmails(List<Guid> empIds)
        {
            if (empIds == null) return new Dictionary<string, string>();
            Dictionary<string, string> emails = new Dictionary<string, string>();
            List<OrgM_Emp> list = GetAllEmps(x => empIds.Contains(x.Id));
            emails = list.Distinct(new DistinctComparer<OrgM_Emp>("Email")).ToDictionary(x => x.Email.Trim(), y => y.Name.Trim());
            return emails;
        }

        #endregion

        #endregion
    }
}
