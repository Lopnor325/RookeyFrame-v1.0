/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

namespace Rookey.Frame.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection.Emit;
    using System.Reflection;
    using System.Threading;

    /**/
    /// <summary>
    /// 类帮助器，可以动态对类,类成员进行控制（添加，删除）,目前只支持属性控制。
    /// 注意，属性以外的其它成员会被清空，功能还有待完善，使其不影响其它成员。
    /// </summary>
    public class ClassHelper
    {
        #region 公有方法
        /**/
        /// <summary>
        /// 根据类的类型型创建类实例。
        /// </summary>
        /// <param name="t">将要创建的类型。</param>
        /// <returns>返回创建的类实例。</returns>
        public static object CreateInstance(Type t)
        {
            return Activator.CreateInstance(t);
        }

        /**/
        /// <summary>
        /// 根据类的名称，属性列表创建型实例。
        /// </summary>
        /// <param name="className">将要创建的类的名称。</param>
        /// <param name="lcpi">将要创建的类的属性列表。</param>
        /// <returns>返回创建的类实例</returns>
        public static object CreateInstance(string className, List<CustPropertyInfo> lcpi)
        {
            Type t = BuildType(className);
            t = AddProperty(t, lcpi);
            return Activator.CreateInstance(t);
        }

        /**/
        /// <summary>
        /// 根据属性列表创建类的实例，默认类名为DefaultClass，由于生成的类不是强类型，所以类名可以忽略。
        /// </summary>
        /// <param name="lcpi">将要创建的类的属性列表</param>
        /// <returns>返回创建的类的实例。</returns>
        public static object CreateInstance(List<CustPropertyInfo> lcpi)
        {
            return CreateInstance("DefaultClass", lcpi);
        }

        /**/
        /// <summary>
        /// 根据类的实例设置类的属性。
        /// </summary>
        /// <param name="classInstance">将要设置的类的实例。</param>
        /// <param name="propertyName">将要设置属性名。</param>
        /// <param name="propertSetValue">将要设置属性值。</param>
        public static void SetPropertyValue(object classInstance, string propertyName, object propertSetValue)
        {
            object obj = propertSetValue != null ? Convert.ChangeType(propertSetValue, propertSetValue.GetType()) : null;
            classInstance.GetType().InvokeMember(propertyName, BindingFlags.SetProperty,
                                          null, classInstance, new object[] { obj });
        }

        /**/
        /// <summary>
        /// 根据类的实例获取类的属性。
        /// </summary>
        /// <param name="classInstance">将要获取的类的实例</param>
        /// <param name="propertyName">将要设置的属性名。</param>
        /// <returns>返回获取的类的属性。</returns>
        public static object GetPropertyValue(object classInstance, string propertyName)
        {
            return classInstance.GetType().InvokeMember(propertyName, BindingFlags.GetProperty,
                                                          null, classInstance, new object[] { });
        }

        /**/
        /// <summary>
        /// 创建一个没有成员的类型的实例，类名为"DefaultClass"。
        /// </summary>
        /// <returns>返回创建的类型的实例。</returns>
        public static Type BuildType()
        {
            return BuildType("DefaultClass");
        }

        /**/
        /// <summary>
        /// 根据类名创建一个没有成员的类型的实例。
        /// </summary>
        /// <param name="className">将要创建的类型的实例的类名。</param>
        /// <param name="assemblyName">自定义程序集名称</param>
        /// <returns>返回创建的类型的实例。</returns>
        public static Type BuildType(string className, string assemblyName = "MyDynamicAssembly")
        {
            AppDomain myDomain = Thread.GetDomain();
            AssemblyName myAsmName = new AssemblyName();
            myAsmName.Name = assemblyName;
  
            //创建一个永久程序集，设置为AssemblyBuilderAccess.RunAndSave。
            AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.RunAndSave);

            //创建一个永久单模程序块。
            ModuleBuilder myModBuilder = myAsmBuilder.DefineDynamicModule(myAsmName.Name, myAsmName.Name + ".dll");
            //创建TypeBuilder。
            TypeBuilder myTypeBuilder = myModBuilder.DefineType(className, TypeAttributes.Public);

            //创建类型。
            Type retval = myTypeBuilder.CreateType();

            //保存程序集，以便可以被Ildasm.exe解析，或被测试程序引用。
            //myAsmBuilder.Save(myAsmName.Name + ".dll");
            return retval;
        }

        /**/
        /// <summary>
        /// 添加属性到类型的实例，注意：该操作会将其它成员清除掉，其功能有待完善。
        /// </summary>
        /// <param name="classType">指定类型的实例。</param>
        /// <param name="lcpi">表示属性的一个列表。</param>
        /// <returns>返回处理过的类型的实例。</returns>
        public static Type AddProperty(Type classType, List<CustPropertyInfo> lcpi)
        {
            //合并先前的属性，以便一起在下一步进行处理。
            MergeProperty(classType, lcpi);
            //把属性加入到Type。
            return AddPropertyToType(classType, lcpi);
        }

        /**/
        /// <summary>
        /// 添加属性到类型的实例，注意：该操作会将其它成员清除掉，其功能有待完善。
        /// </summary>
        /// <param name="classType">指定类型的实例。</param>
        /// <param name="cpi">表示一个属性。</param>
        /// <returns>返回处理过的类型的实例。</returns>
        public static Type AddProperty(Type classType, CustPropertyInfo cpi)
        {
            List<CustPropertyInfo> lcpi = new List<CustPropertyInfo>();
            lcpi.Add(cpi);
            //合并先前的属性，以便一起在下一步进行处理。
            MergeProperty(classType, lcpi);
            //把属性加入到Type。
            return AddPropertyToType(classType, lcpi);
        }

        /**/
        /// <summary>
        /// 从类型的实例中移除属性，注意：该操作会将其它成员清除掉，其功能有待完善。
        /// </summary>
        /// <param name="classType">指定类型的实例。</param>
        /// <param name="cpi">要移除的属性。</param>
        /// <returns>返回处理过的类型的实例。</returns>
        public static Type DeleteProperty(Type classType, string propertyName)
        {
            List<string> ls = new List<string>();
            ls.Add(propertyName);

            //合并先前的属性，以便一起在下一步进行处理。
            List<CustPropertyInfo> lcpi = SeparateProperty(classType, ls);
            //把属性加入到Type。
            return AddPropertyToType(classType, lcpi);
        }

        /**/
        /// <summary>
        /// 从类型的实例中移除属性，注意：该操作会将其它成员清除掉，其功能有待完善。
        /// </summary>
        /// <param name="classType">指定类型的实例。</param>
        /// <param name="lcpi">要移除的属性列表。</param>
        /// <returns>返回处理过的类型的实例。</returns>
        public static Type DeleteProperty(Type classType, List<string> ls)
        {
            //合并先前的属性，以便一起在下一步进行处理。
            List<CustPropertyInfo> lcpi = SeparateProperty(classType, ls);
            //把属性加入到Type。
            return AddPropertyToType(classType, lcpi);
        }
        #endregion

        #region 私有方法
        /**/
        /// <summary>
        /// 把类型的实例t和lcpi参数里的属性进行合并。
        /// </summary>
        /// <param name="t">实例t</param>
        /// <param name="lcpi">里面包含属性列表的信息。</param>
        private static void MergeProperty(Type t, List<CustPropertyInfo> lcpi)
        {
            CustPropertyInfo cpi;
            foreach (PropertyInfo pi in t.GetProperties())
            {
                cpi = new CustPropertyInfo(pi.PropertyType.FullName, pi.Name);
                lcpi.Add(cpi);
            }
        }

        /**/
        /// <summary>
        /// 从类型的实例t的属性移除属性列表lcpi，返回的新属性列表在lcpi中。
        /// </summary>
        /// <param name="t">类型的实例t。</param>
        /// <param name="lcpi">要移除的属性列表。</param>
        private static List<CustPropertyInfo> SeparateProperty(Type t, List<string> ls)
        {
            List<CustPropertyInfo> ret = new List<CustPropertyInfo>();
            CustPropertyInfo cpi;
            foreach (PropertyInfo pi in t.GetProperties())
            {
                foreach (string s in ls)
                {
                    if (pi.Name != s)
                    {
                        cpi = new CustPropertyInfo(pi.PropertyType.FullName, pi.Name);
                        ret.Add(cpi);
                    }
                }
            }

            return ret;
        }

        /**/
        /// <summary>
        /// 把lcpi参数里的属性加入到myTypeBuilder中。注意：该操作会将其它成员清除掉，其功能有待完善。
        /// </summary>
        /// <param name="myTypeBuilder">类型构造器的实例。</param>
        /// <param name="lcpi">里面包含属性列表的信息。</param>
        private static void AddPropertyToTypeBuilder(TypeBuilder myTypeBuilder, List<CustPropertyInfo> lcpi)
        {
            FieldBuilder customerNameBldr;
            PropertyBuilder custNamePropBldr;
            MethodBuilder custNameGetPropMthdBldr;
            MethodBuilder custNameSetPropMthdBldr;
            MethodAttributes getSetAttr;
            ILGenerator custNameGetIL;
            ILGenerator custNameSetIL;

            // 属性Set和Get方法要一个专门的属性。这里设置为Public。
            getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

            // 添加属性到myTypeBuilder。
            foreach (CustPropertyInfo cpi in lcpi)
            {
                //定义字段。
                customerNameBldr = myTypeBuilder.DefineField(cpi.FieldName,
                                                                 Type.GetType(cpi.Type),
                                                                 FieldAttributes.Private);

                //定义属性。
                //最后一个参数为null，因为属性没有参数。
                custNamePropBldr = myTypeBuilder.DefineProperty(cpi.PropertyName,
                                                                 PropertyAttributes.HasDefault,
                                                                 Type.GetType(cpi.Type),
                                                                 null);

                //定义Get方法。
                custNameGetPropMthdBldr =
                    myTypeBuilder.DefineMethod(cpi.GetPropertyMethodName,
                                               getSetAttr,
                                               Type.GetType(cpi.Type),
                                               Type.EmptyTypes);

                custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();

                custNameGetIL.Emit(OpCodes.Ldarg_0);
                custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
                custNameGetIL.Emit(OpCodes.Ret);

                //定义Set方法。
                custNameSetPropMthdBldr =
                    myTypeBuilder.DefineMethod(cpi.SetPropertyMethodName,
                                               getSetAttr,
                                               null,
                                               new Type[] { Type.GetType(cpi.Type) });

                custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

                custNameSetIL.Emit(OpCodes.Ldarg_0);
                custNameSetIL.Emit(OpCodes.Ldarg_1);
                custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
                custNameSetIL.Emit(OpCodes.Ret);

                //把创建的两个方法(Get,Set)加入到PropertyBuilder中。
                custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
                custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);
            }
        }

        /**/
        /// <summary>
        /// 把属性加入到类型的实例。
        /// </summary>
        /// <param name="classType">类型的实例。</param>
        /// <param name="lcpi">要加入的属性列表。</param>
        /// <returns>返回处理过的类型的实例。</returns>
        public static Type AddPropertyToType(Type classType, List<CustPropertyInfo> lcpi)
        {
            AppDomain myDomain = Thread.GetDomain();
            AssemblyName myAsmName = new AssemblyName();
            myAsmName.Name = "MyDynamicAssembly";

            //创建一个永久程序集，设置为AssemblyBuilderAccess.RunAndSave。
            AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName,
                                                            AssemblyBuilderAccess.RunAndSave);

            //创建一个永久单模程序块。
            ModuleBuilder myModBuilder =
                myAsmBuilder.DefineDynamicModule(myAsmName.Name, myAsmName.Name + ".dll");
            //创建TypeBuilder。
            TypeBuilder myTypeBuilder = myModBuilder.DefineType(classType.FullName,
                                                            TypeAttributes.Public);

            //把lcpi中定义的属性加入到TypeBuilder。将清空其它的成员。其功能有待扩展，使其不影响其它成员。
            AddPropertyToTypeBuilder(myTypeBuilder, lcpi);

            //创建类型。
            Type retval = myTypeBuilder.CreateType();

            //保存程序集，以便可以被Ildasm.exe解析，或被测试程序引用。
            //myAsmBuilder.Save(myAsmName.Name + ".dll");
            return retval;
        }
        #endregion

        #region 辅助类
        /**/
        /// <summary>
        /// 自定义的属性信息类型。
        /// </summary>
        public class CustPropertyInfo
        {
            private string propertyName;
            private string type;

            /**/
            /// <summary>
            /// 空构造。
            /// </summary>
            public CustPropertyInfo() { }

            /**/
            /// <summary>
            /// 根据属性类型名称，属性名称构造实例。
            /// </summary>
            /// <param name="type">属性类型名称。</param>
            /// <param name="propertyName">属性名称。</param>
            public CustPropertyInfo(string type, string propertyName)
            {
                this.type = type;
                this.propertyName = propertyName;
            }

            /**/
            /// <summary>
            /// 获取或设置属性类型名称。
            /// </summary>
            public string Type
            {
                get { return type; }
                set { type = value; }
            }

            /**/
            /// <summary>
            /// 获取或设置属性名称。
            /// </summary>
            public string PropertyName
            {
                get { return propertyName; }
                set { propertyName = value; }
            }

            /**/
            /// <summary>
            /// 获取属性字段名称。
            /// </summary>
            public string FieldName
            {
                get { return propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1); }
            }

            /**/
            /// <summary>
            /// 获取属性在IL中的Set方法名。
            /// </summary>
            public string SetPropertyMethodName
            {
                get { return "set_" + PropertyName; }
            }

            /**/
            /// <summary>
            ///  获取属性在IL中的Get方法名。
            /// </summary>
            public string GetPropertyMethodName
            {
                get { return "get_" + PropertyName; }
            }
        }
        #endregion
    }

    class ClassHelperDemo
    {
        public static void Main()
        {
            //演示一：动态生成类。
            #region 演示一：动态生成类。
            //生成一个类t。
            Type t = ClassHelper.BuildType("MyClass");
            #endregion

            //演示二：动态添加属性到类。
            #region 演示二：动态添加属性到类。
            //先定义两个属性。
            List<ClassHelper.CustPropertyInfo> lcpi = new List<ClassHelper.CustPropertyInfo>();
            ClassHelper.CustPropertyInfo cpi;

            cpi = new ClassHelper.CustPropertyInfo("System.String", "S1");
            lcpi.Add(cpi);
            cpi = new ClassHelper.CustPropertyInfo("System.String", "S2");
            lcpi.Add(cpi);

            //再加入上面定义的两个属性到我们生成的类t。
            t = ClassHelper.AddProperty(t, lcpi);

            //把它显示出来。
            DispProperty(t);

            //再定义两个属性。
            lcpi.Clear();
            cpi = new ClassHelper.CustPropertyInfo("System.Int32", "I1");
            lcpi.Add(cpi);
            cpi = new ClassHelper.CustPropertyInfo("System.Int32", "I2");
            lcpi.Add(cpi);

            //再加入上面定义的两个属性到我们生成的类t。
            t = ClassHelper.AddProperty(t, lcpi);

            //再把它显示出来，看看有没有增加到4个属性。
            DispProperty(t);
            #endregion

            //演示三：动态从类里删除属性。
            #region 演示三：动态从类里删除属性。
            //把'S1'属性从类t中删除。
            t = ClassHelper.DeleteProperty(t, "S1");
            //显示出来，可以看到'S1'不见了。
            DispProperty(t);

            #endregion

            //演示四：动态获取和设置属性值。
            #region 演示四：动态获取和设置属性值。
            //先生成一个类t的实例o。
            object o = ClassHelper.CreateInstance(t);

            //给S2,I2属性赋值。
            ClassHelper.SetPropertyValue(o, "S2", "abcd");
            ClassHelper.SetPropertyValue(o, "I2", 1234);

            //再把S2,I2的属性值显示出来。
            Console.WriteLine("S2 = {0}", ClassHelper.GetPropertyValue(o, "S2"));
            Console.WriteLine("I2 = {0}", ClassHelper.GetPropertyValue(o, "I2"));
            #endregion

            Console.Read();
        }

        public static void DispProperty(Type t)
        {
            Console.WriteLine("ClassName '{0}'", t.Name);
            foreach (PropertyInfo pInfo in t.GetProperties())
            {
                Console.WriteLine("Has Property '{0}'", pInfo.ToString());
            }
            Console.WriteLine("");
        }
    }
}
