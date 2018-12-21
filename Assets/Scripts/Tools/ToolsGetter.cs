namespace Tools
{
    public static class ToolsGetter
    {
        public static Assembly Assembly
        {
            get { return Assembly.Instance; }
        }

        public static URL URL
        {
            get { return URL.Instance; }
        }

        public static Cookies Cookies
        {
            get { return Cookies.Instance; }
        }

#if UNITY_EDITOR
        public static IO IO
        {
            get { return IO.Instance; }
        }
#endif
    }
}
