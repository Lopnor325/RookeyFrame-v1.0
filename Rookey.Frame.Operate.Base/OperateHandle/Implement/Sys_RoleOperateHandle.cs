using Rookey.Frame.Base;
using Rookey.Frame.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.OperateHandle.Implement
{
    class Sys_RoleOperateHandle : ITreeOperateHandle<Sys_Role>
    {
        public void TreeNodeHandle(TempModel.TreeNode node)
        {
        }

        public List<Sys_Role> ChildNodesDataHandler(List<Sys_Role> childDatas, UserInfo currUser)
        {
            if (childDatas != null)
                return childDatas.Where(x => x.IsValid).ToList();
            return new List<Sys_Role>();
        }
    }
}
