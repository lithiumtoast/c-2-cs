﻿// <auto-generated/>
namespace helloworld
{
    public static unsafe partial class my_c_library
    {
        [global::System.Runtime.InteropServices.DllImportAttribute("my_c_library", EntryPoint = "hw_hello_world", ExactSpelling = true)]
        public static extern partial void hw_hello_world();
    }
}
namespace helloworld
{
    public static unsafe partial class my_c_library
    {
        [global::System.Runtime.InteropServices.DllImportAttribute("my_c_library", EntryPoint = "hw_invoke_callback1", ExactSpelling = true)]
        public static extern partial void hw_invoke_callback1(global::helloworld.my_c_library.hw_callback f, global::Interop.Runtime.CString s);
    }
}
namespace helloworld
{
    public static unsafe partial class my_c_library
    {
        [global::System.Runtime.InteropServices.DllImportAttribute("my_c_library", EntryPoint = "hw_invoke_callback2", ExactSpelling = true)]
        public static extern partial void hw_invoke_callback2(global::helloworld.my_c_library.FnPtr_CString_Void f, global::Interop.Runtime.CString s);
    }
}
namespace helloworld
{
    public static unsafe partial class my_c_library
    {
        [global::System.Runtime.InteropServices.DllImportAttribute("my_c_library", EntryPoint = "hw_pass_enum", ExactSpelling = true)]
        public static extern partial void hw_pass_enum(global::helloworld.my_c_library.hw_my_enum_week_day e);
    }
}
namespace helloworld
{
    public static unsafe partial class my_c_library
    {
        [global::System.Runtime.InteropServices.DllImportAttribute("my_c_library", EntryPoint = "hw_pass_integers_by_reference", ExactSpelling = true)]
        public static extern partial void hw_pass_integers_by_reference(ushort* a, int* b, ulong* c);
    }
}
namespace helloworld
{
    public static unsafe partial class my_c_library
    {
        [global::System.Runtime.InteropServices.DllImportAttribute("my_c_library", EntryPoint = "hw_pass_integers_by_value", ExactSpelling = true)]
        public static extern partial void hw_pass_integers_by_value(ushort a, int b, ulong c);
    }
}
namespace helloworld
{
    public static unsafe partial class my_c_library
    {
        [global::System.Runtime.InteropServices.DllImportAttribute("my_c_library", EntryPoint = "hw_pass_string", ExactSpelling = true)]
        public static extern partial void hw_pass_string(global::Interop.Runtime.CString s);
    }
}
