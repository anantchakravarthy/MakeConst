﻿//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Threading.Tasks;
//using VerifyCS = MakeConst.Test.CSharpCodeFixVerifier<
//    MakeConst.MakeConstAnalyzer,
//    MakeConst.MakeConstCodeFixProvider>;

//namespace MakeConst.Test
//{
//    [TestMethod]
//    public async Task LocalIntCouldBeConstant_Diagnostic()
//    {
//        await VerifyCS.VerifyCodeFixAsync(@"
//using System;

//class Program
//{
//    static void Main()
//    {
//        [|int i = 0;|]
//        Console.WriteLine(i);
//    }
//}
//", @"
//using System;

//class Program
//{
//    static void Main()
//    {
//        const int i = 0;
//        Console.WriteLine(i);
//    }
//}
//");
//    }

//    [TestMethod]
//    public async Task VariableIsAssigned_NoDiagnostic()
//    {
//        await VerifyCS.VerifyAnalyzerAsync(@"
//using System;

//class Program
//{
//    static void Main()
//    {
//        int i = 0;
//        Console.WriteLine(i++);
//    }
//}
//");
//    }

//    [TestMethod]
//    public async Task VariableIsAlreadyConst_NoDiagnostic()
//    {
//        await VerifyCS.VerifyAnalyzerAsync(@"
//using System;

//class Program
//{
//    static void Main()
//    {
//        const int i = 0;
//        Console.WriteLine(i);
//    }
//}
//");
//    }

//    [TestMethod]
//    public async Task NoInitializer_NoDiagnostic()
//    {
//        await VerifyCS.VerifyAnalyzerAsync(@"
//using System;

//class Program
//{
//    static void Main()
//    {
//        int i;
//        i = 0;
//        Console.WriteLine(i);
//    }
//}
//");
//    }


//}
