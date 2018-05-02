// =========================================================
// 　　 CRL版本：4.0.30319.239
// 　  创建时间：2017-07-07 10:33
//      修改人：WH09 （Xiao Peng）
//    修改时间：2017-07-07 10:46
// =========================================================
// Copyright © 2015 Beijing BKC Technology CO.,LTD
// All rights reserved.
// =========================================================

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的常规信息通过以下
// 特性集控制。更改这些特性值可修改
// 与程序集关联的信息。

[assembly: AssemblyTitle("ServerStartOrStop")]
[assembly: AssemblyDescription("Windows服务管理程序")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("北京必可测科技股份有限公司")]
[assembly: AssemblyProduct("Windows服务管理程序")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 使此程序集中的类型
// 对 COM 组件不可见。  如果需要从 COM 访问此程序集中的类型，
// 则将该类型上的 ComVisible 特性设置为 true。

[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID

[assembly: Guid("7e04ef25-f305-4c7c-8267-41a2f98469d8")]

// 程序集的版本信息由下面四个值组成: 
//
//      主版本
//      次版本 
//      生成号
//      修订号
//
// 可以指定所有这些值，也可以使用“生成号”和“修订号”的默认值，
// 方法是按如下所示使用“*”: 
// [assembly: AssemblyVersion("1.0.*")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: log4net.Config.XmlConfigurator(Watch = true)]//注入