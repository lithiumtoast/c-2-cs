﻿// <auto-generated/>
public static unsafe partial class my_c_library
{
    [global::System.Runtime.InteropServices.DllImportAttribute("my_c_library", EntryPoint = "hw_hello_world", ExactSpelling = true)]
    public static extern partial void hw_hello_world();
}
public static unsafe partial class my_c_library
{
    [global::System.Runtime.InteropServices.DllImportAttribute("my_c_library", EntryPoint = "hw_invoke_callback", ExactSpelling = true)]
    public static extern partial void hw_invoke_callback(global::my_c_library.FnPtr_CString_Void f, global::bottlenoselabs.c2cs.Interop.Runtime.CString s);
}
public static unsafe partial class my_c_library
{
    [global::System.Runtime.InteropServices.DllImportAttribute("my_c_library", EntryPoint = "hw_pass_enum", ExactSpelling = true)]
    public static extern partial void hw_pass_enum(global::my_c_library.hw_my_enum_week_day e);
}
public static unsafe partial class my_c_library
{
    [global::System.Runtime.InteropServices.DllImportAttribute("my_c_library", EntryPoint = "hw_pass_integers_by_reference", ExactSpelling = true)]
    public static extern partial void hw_pass_integers_by_reference(ushort* a, int* b, ulong* c);
}
public static unsafe partial class my_c_library
{
    [global::System.Runtime.InteropServices.DllImportAttribute("my_c_library", EntryPoint = "hw_pass_integers_by_value", ExactSpelling = true)]
    public static extern partial void hw_pass_integers_by_value(ushort a, int b, ulong c);
}
public static unsafe partial class my_c_library
{
    [global::System.Runtime.InteropServices.DllImportAttribute("my_c_library", EntryPoint = "hw_pass_string", ExactSpelling = true)]
    public static extern partial void hw_pass_string(global::bottlenoselabs.c2cs.Interop.Runtime.CString s);
}
