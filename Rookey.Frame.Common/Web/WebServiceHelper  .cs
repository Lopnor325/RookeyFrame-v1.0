using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web.Services.Description;
using Microsoft.CSharp;

namespace Rookey.Frame.Common.Web
{
    /// <summary>  
    /// 动态调用WebService（支持SaopHeader）  
    /// </summary>  
    public class WebServiceHelper
    {
        /// <summary>     
        /// 获取WebService的类名     
        /// </summary>     
        /// <param name="wsUrl">WebService地址</param>     
        /// <returns>返回WebService的类名</returns>     
        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }

        /// <summary>     
        /// 调用WebService（不带SoapHeader）  
        /// </summary>     
        /// <param name="wsUrl">WebService地址</param>     
        /// <param name="methodName">方法名称</param>     
        /// <param name="args">参数列表</param>     
        /// <returns>返回调用结果</returns>     
        public static object InvokeWebService(string wsUrl, string methodName, object[] args)
        {
            return InvokeWebService(wsUrl, null, methodName, null, args);
        }

        /// <summary>     
        /// 调用WebService（带SoapHeader）  
        /// </summary>     
        /// <param name="wsUrl">WebService地址</param>     
        /// <param name="methodName">方法名称</param>     
        /// <param name="soapHeader">SOAP头</param>     
        /// <param name="args">参数列表</param>     
        /// <returns>返回调用结果</returns>  
        public static object InvokeWebService(string wsUrl, string methodName, SoapHeader soapHeader, object[] args)
        {
            return InvokeWebService(wsUrl, null, methodName, soapHeader, args);
        }

        /// <summary>     
        /// 调用WebService  
        /// </summary>     
        /// <param name="wsUrl">WebService地址</param>     
        /// <param name="className">类名</param>     
        /// <param name="methodName">方法名称</param>     
        /// <param name="soapHeader">SOAP头</param>  
        /// <param name="args">参数列表</param>     
        /// <returns>返回调用结果</returns>     
        public static object InvokeWebService(string wsUrl, string className, string methodName, SoapHeader soapHeader, object[] args)
        {
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((className == null) || (className == ""))
            {
                className = GetWsClassName(wsUrl);
            }
            try
            {
                //获取WSDL     
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(wsUrl + "?wsdl");
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);

                //生成客户端代理类代码     
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider csc = new CSharpCodeProvider();
                ICodeCompiler icc = csc.CreateCompiler();

                //设定编译参数     
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");

                //编译代理类     
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }


                //生成代理实例，并调用方法     
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + className, true, true);

                FieldInfo[] arry = t.GetFields();

                FieldInfo client = null;
                object clientkey = null;
                if (soapHeader != null)
                {
                    //Soap头开始     
                    client = t.GetField(soapHeader.ClassName + "Value");

                    //获取客户端验证对象     
                    Type typeClient = assembly.GetType(@namespace + "." + soapHeader.ClassName);

                    //为验证对象赋值     
                    clientkey = Activator.CreateInstance(typeClient);

                    foreach (KeyValuePair<string, object> property in soapHeader.Properties)
                    {
                        typeClient.GetField(property.Key).SetValue(clientkey, property.Value);
                    }
                    //Soap头结束     
                }

                //实例类型对象     
                object obj = Activator.CreateInstance(t);

                if (soapHeader != null)
                {
                    //设置Soap头  
                    client.SetValue(obj, clientkey);
                }

                System.Reflection.MethodInfo mi = t.GetMethod(methodName);

                return mi.Invoke(obj, args);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }

        /// <summary>  
        /// SOAP头  
        /// </summary>  
        public class SoapHeader
        {
            /// <summary>  
            /// 构造一个SOAP头  
            /// </summary>  
            public SoapHeader()
            {
                this.Properties = new Dictionary<string, object>();
            }

            /// <summary>  
            /// 构造一个SOAP头  
            /// </summary>  
            /// <param name="className">SOAP头的类名</param>  
            public SoapHeader(string className)
            {
                this.ClassName = className;
                this.Properties = new Dictionary<string, object>();
            }

            /// <summary>  
            /// 构造一个SOAP头  
            /// </summary>  
            /// <param name="className">SOAP头的类名</param>  
            /// <param name="properties">SOAP头的类属性名及属性值</param>  
            public SoapHeader(string className, Dictionary<string, object> properties)
            {
                this.ClassName = className;
                this.Properties = properties;
            }

            /// <summary>  
            /// SOAP头的类名  
            /// </summary>  
            public string ClassName { get; set; }

            /// <summary>  
            /// SOAP头的类属性名及属性值  
            /// </summary>  
            public Dictionary<string, object> Properties { get; set; }

            /// <summary>  
            /// 为SOAP头增加一个属性及值  
            /// </summary>  
            /// <param name="name">SOAP头的类属性名</param>  
            /// <param name="value">SOAP头的类属性值</param>  
            public void AddProperty(string name, object value)
            {
                if (this.Properties == null)
                {
                    this.Properties = new Dictionary<string, object>();
                }
                Properties.Add(name, value);
            }
        }
    }
}
