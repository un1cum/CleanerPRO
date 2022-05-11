using System.Runtime.InteropServices;
using System.Text;

internal class IniFile
{
    private readonly string FileName;
    public IniFile(string FileName = null) => this.FileName = new System.IO.FileInfo(FileName).FullName.ToString();

    #region
    [DllImport("kernel32", CharSet = CharSet.Auto)]
    private static extern long WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

    [DllImport("kernel32", CharSet = CharSet.Auto)]
    private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

    [DllImport("kernel32", CharSet = CharSet.Auto)]
    private static extern int GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

    [DllImport("kernel32", CharSet = CharSet.Auto)]
    private static extern int GetPrivateProfileSection(string lpAppName, System.IntPtr lpReturnedString, int nSize, string lpFileName);

    [DllImport("kernel32", CharSet = CharSet.Auto)]
    private static extern int GetPrivateProfileSectionNames(System.IntPtr lpReturnedString, int nSize, string lpFileName);

    #endregion

    #region
    public void Write(string Key, string Value, string Section = null) => WritePrivateProfileString(Section, Key, Value, FileName);

    public string ReadString(string Key, string Section = null, int Size = 255, string Default = "")
    {
        StringBuilder tmp = new StringBuilder(Size);
        GetPrivateProfileString(Section, Key, Default, tmp, Size, FileName);
        return tmp.ToString();
    }

    public int ReadInt(string Key, string Section = null, int Default = -1) => GetPrivateProfileInt(Section, Key, Default, FileName);

    public bool ReadBool(string Key, string Section = null, int Size = 255)
    {
        StringBuilder tmp = new StringBuilder(Size);
        GetPrivateProfileString(Section, Key, "", tmp, Size, FileName);
        return System.Convert.ToBoolean(tmp.ToString());
    }

    public string[] GetAllDataSection(string Section, int Size = 255)
    {
        System.IntPtr pMem = Marshal.AllocHGlobal(4096 * sizeof(char));
        string temp = string.Empty;

        int count = GetPrivateProfileSection(Section, pMem, Size * sizeof(char), FileName) - 1;
        if (count > 0) temp = Marshal.PtrToStringUni(pMem, count);
        Marshal.FreeHGlobal(pMem);

        return temp.Split('\0');
    }
    public string[] GetAllSections(int Size = 255)
    {
        System.IntPtr pMem = Marshal.AllocHGlobal(4096 * sizeof(char));
        string temp = string.Empty;

        int count = GetPrivateProfileSectionNames(pMem, Size * sizeof(char), FileName) - 1;
        if (count > 0) temp = Marshal.PtrToStringUni(pMem, count);
        Marshal.FreeHGlobal(pMem);

        return temp.Split('\0');
    }
    public void DeleteKey(string Key, string Section = null) => Write(Key, null, Section);
    public void DeleteSection(string Section = null) => Write(null, null, Section);
    public bool KeyExists(string Key, string Section = null) => ReadString(Key, Section).Length > 0;
    #endregion
}