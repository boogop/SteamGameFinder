using System;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;

namespace SteamGameFinder.Utils
{
  /// <summary>
  /// Some general utility functions, frequently duplicating handy
  /// VB6 functions that MS left out of C#. Some of these stem from
  /// the .NET 1.0 days when the libraries weren't mature
  /// </summary>
  internal sealed class GeneralTools
  {
    public static Color CSRTS_SearchColor = Color.WhiteSmoke;
    public static Color CSRTS_AccountColor = Color.WhiteSmoke;
    public static Color CSRTS_JobColor = Color.WhiteSmoke;
    public static Color CSRTS_CallColor = Color.WhiteSmoke;
    public static Color CSRTS_DefaultColor = Color.WhiteSmoke;

    public static bool highres = false;

    public static string CustomerFax = "";

    public static string ProofHTML = "www.stouse.com";

    public static string SigBlock = "";

    public static bool proofHash = false;

    public static bool proofPointers = false;


    public static bool IsWindows8OrNewer()
    {
      OperatingSystem os = Environment.OSVersion;
      return os.Platform == PlatformID.Win32NT &&
             (os.Version.Major > 6 || (os.Version.Major == 6 && os.Version.Minor >= 2));
    }


    public static void DeleteFiles(string path)
    {
      string[] dirs = null;
      try
      {
        dirs = Directory.GetFiles(path);
      }
      catch
      {
        try
        {
          Directory.CreateDirectory(path);
        }
        catch
        {
          return;
        }
      }

      try
      {
        foreach (string dir in dirs)
        {
          File.Delete(dir);
        }
      }
      catch
      {
        return;
      }
    }

    public static void DeleteFiles(string path, string ext)
    {
      string[] dirs = null;
      try
      {
        dirs = Directory.GetFiles(path,ext);
      }
      catch
      {
        try
        {
          Directory.CreateDirectory(path);
        }
        catch
        {
          return;
        }
      }

      try
      {
        foreach (string dir in dirs)
        {
          File.Delete(dir);
        }
      }
      catch
      {
        return;
      }
    }

    public static void DeleteFile(string filename)
    {
      try
      {
        if (File.Exists(filename))
          File.Delete(filename);
      }
      catch { }
    }

  

    public static void ShowTime(Stopwatch s, ToolStripStatusLabel t)
    {
      TimeSpan ts = s.Elapsed;
      t.Text = "Elapsed: " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
          ts.Hours, ts.Minutes, ts.Seconds,
          ts.Milliseconds / 10);
    }

    public static void intRedim(ref int[] myArray)
    {
      // not a good idea but if you ever need to
      // redimension an array here ya go
      int[] Temp1 = new int[myArray.Length + 1];
      if (myArray != null)
        System.Array.Copy(myArray, Temp1, myArray.Length);
      myArray = Temp1;
    }

    public static void WriteAnythingToFile(string path, string filename, string[] obj)
    {
      // these next two come in very handy for writing settings and crap out
      // to files without having to deal with .net's settings file garbage
      try
      {
        StreamWriter stream_writer = new StreamWriter(path + filename);

        for (int i = 0; i < obj.Length; i++)
          stream_writer.WriteLine(obj[i]);

        stream_writer.Flush();
        stream_writer.Close();
      }
      catch
      {
      }
    }

    public static void WriteAnythingToFile(string path, string filename, ArrayList obj)
    {
      try
      {
        StreamWriter stream_writer = new StreamWriter(path + filename);

        for (int i = 0; i < obj.Count; i++)
          stream_writer.WriteLine(obj[i]);

        stream_writer.Flush();
        stream_writer.Close();
      }
      catch
      {
      }
    }

    public static void ReadAnythingFromFile(string filename, ref string[] obj)
    {
      int i;

      if (!File.Exists(filename)) return;

      FileStream file_stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
      StreamReader s = new StreamReader(file_stream);
      string input = null;

      i = 0;

      while ((input = s.ReadLine()) != null)
      {
        obj[i] = input;
        i++;
      }

      file_stream.Close();
    }

    public static void ReadAnythingFromFile(string filename, ref ArrayList obj)
    {
      // overload. If you need an array and have no clue how many elements
      // are in the text file

      if (!File.Exists(filename)) return;

      FileStream file_stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
      StreamReader s = new StreamReader(file_stream);
      string input = null;

      while ((input = s.ReadLine()) != null)
        obj.Add(input);

      file_stream.Close();
    }

    private static string cleanTextCrap(string foo)
    {
      string s = foo.Replace("\"", "");
      string t = s.Replace(@"\", "");
      return t;
    }

    public static void GetFiles(DirectoryInfo di, string searchPattern, ref ArrayList MyFiles)
    {
      foreach (FileInfo fi in di.GetFiles(searchPattern))
        MyFiles.Add(fi.Name);

      // Search in subdirectories 
      foreach (DirectoryInfo d in di.GetDirectories())
        GetFiles(d, searchPattern, ref MyFiles);
    }

    public static string Left(string param, int length)
    {
      string result = param.Substring(0, length);

      return result;
    }

    public static string Right(string param, int length)
    {
      string result = param.Substring(param.Length - length, length);

      return result;
    }

    public static string Mid(string param, int startIndex, int length)
    {
      // the mid function is heavily used in VBA

      //start at the specified index in the string and get N number of
      //characters depending on the lenght and assign it to a variable
      string result = param.Substring(startIndex, length);
      //return the result of the operation
      return result;
    }

    public static string Mid(string param, int startIndex)
    {
      // overload 

      //start at the specified index and return all characters after it
      //and assign it to a variable
      string result = param.Substring(startIndex);
      //return the result of the operation
      return result;
    }

    public static bool IsNumeric(object Expression)
    {
      // IsNumeric Function. C# is great so why didn't MS include
      // a basic IsNumeric function???

      // Variable to collect the Return value of the TryParse method.
      bool isNum;

      // Define variable to collect out parameter of the TryParse method. 
      // If the conversion fails, the out parameter is zero.
      double retNum;

      // The TryParse method converts a string in a specified style and culture-specific
      // format to its double-precision floating point number equivalent.
      // The TryParse method does not generate an exception if the conversion fails.
      // If the conversion passes, True is returned. If it does not, False is returned.
      isNum = Double.TryParse(Convert.ToString(Expression),
          System.Globalization.NumberStyles.Any,
          System.Globalization.NumberFormatInfo.InvariantInfo,
          out retNum);

      return isNum;
    }

    public static bool IsDate(object Expression)
    {
      if (!chkNull.isNull(Expression))
      {
        if (Expression is DateTime) return true;

        if (Expression is string)
        {
          try
          {
            //Mike Kane: changed this to use "en-US" culture. But, you could probably start using TryParse instead, just saying.
            //System.DateTime dt = System.DateTime.Parse((string)Expression);
            System.DateTime.Parse((string)Expression, new System.Globalization.CultureInfo("en-US"));
            return true;
          }
          catch
          {
            return false;
          }
        }
      }
      return false;
    }

   

    /// <summary>
    /// A case-insensitive replace function
    /// </summary>
    /// <param name="original"></param>
    /// <param name="pattern"></param>
    /// <param name="replacement"></param>
    /// <returns></returns>
    public static string ReplaceEx(string original, string pattern, string replacement)
    {
      /* this could be done through regex but that's not very efficient and
       * case sensitivity isn't dealt with unless you define the RegExpOption
       * as IgnoreCase so wtf why bother? 
       */
      int count, position0, position1;

      count = position0 = position1 = 0;

      if (chkNull.isNull(original)) return "";

      string upperString = original.ToUpper();
      string upperPattern = pattern.ToUpper();

      int inc = (original.Length / pattern.Length) *
                (replacement.Length - pattern.Length);

      char[] chars = new char[original.Length + Math.Max(0, inc)];

      while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1)
      {
        for (int i = position0; i < position1; ++i)
          chars[count++] = original[i];
        for (int i = 0; i < replacement.Length; ++i)
          chars[count++] = replacement[i];
        position0 = position1 + pattern.Length;
      }

      if (position0 == 0) return original;

      for (int i = position0; i < original.Length; ++i)
        chars[count++] = original[i];

      return new string(chars, 0, count);
    }

    public static string Replace(string strText, string strFind, string strReplace)
    {
      int iPos = strText.IndexOf(strFind);
      String strReturn = "";

      while (iPos != -1)
      {
        strReturn += strText.Substring(0, iPos) + strReplace;
        strText = strText.Substring(iPos + strFind.Length);
        iPos = strText.IndexOf(strFind);
      }

      if (strText.Length > 0)
        strReturn += strText;

      return strReturn;
    }

    public static double DateDiff(string howtocompare, System.DateTime startDate, System.DateTime endDate)
    {
      double diff = 0;
      System.TimeSpan TS = new System.TimeSpan(endDate.Ticks - startDate.Ticks);

      switch (howtocompare.ToLower())
      {
        case "year":
          diff = Convert.ToDouble(TS.TotalDays / 365);
          break;
        case "month":
          diff = Convert.ToDouble((TS.TotalDays / 365) * 12);
          break;
        case "day":
          diff = Convert.ToDouble(TS.TotalDays);
          break;
        case "hour":
          diff = Convert.ToDouble(TS.TotalHours);
          break;
        case "minute":
          diff = Convert.ToDouble(TS.TotalMinutes);
          break;
        case "second":
          diff = Convert.ToDouble(TS.TotalSeconds);
          break;
      }

      return diff;
    }

    public static void ClearForm(Control parent)
    {
      // clears text out of a form
      foreach (Control ctrControl in parent.Controls)
      {
        if (ctrControl.Name.ToUpper() == "TXTPRESS") continue;

        //Loop through all controls 
        if (object.ReferenceEquals(ctrControl.GetType(), typeof(System.Windows.Forms.MaskedTextBox)))
          ((System.Windows.Forms.MaskedTextBox)ctrControl).Text = string.Empty;
        if (object.ReferenceEquals(ctrControl.GetType(), typeof(System.Windows.Forms.TextBox)))
          ((System.Windows.Forms.TextBox)ctrControl).Text = string.Empty;
        else if (object.ReferenceEquals(ctrControl.GetType(), typeof(System.Windows.Forms.RichTextBox)))
          ((System.Windows.Forms.RichTextBox)ctrControl).Text = string.Empty;
        else if (object.ReferenceEquals(ctrControl.GetType(), typeof(System.Windows.Forms.ComboBox)))
          ((System.Windows.Forms.ComboBox)ctrControl).SelectedIndex = -1;
        else if (object.ReferenceEquals(ctrControl.GetType(), typeof(System.Windows.Forms.CheckBox)))
          ((System.Windows.Forms.CheckBox)ctrControl).Checked = false;
        else if (object.ReferenceEquals(ctrControl.GetType(), typeof(System.Windows.Forms.RadioButton)))
          ((System.Windows.Forms.RadioButton)ctrControl).Checked = false;
        //else if (object.ReferenceEquals(ctrControl.GetType(), typeof(DevExpress.XtraEditors.ComboBoxEdit)))
        //  ((DevExpress.XtraEditors.ComboBoxEdit)ctrControl).Text = "";
        //else if (object.ReferenceEquals(ctrControl.GetType(), typeof(DevExpress.XtraEditors.TextEdit)))
        //  ((DevExpress.XtraEditors.TextEdit)ctrControl).Text = "";
        //else if (object.ReferenceEquals(ctrControl.GetType(), typeof(DevExpress.XtraEditors.SpinEdit)))
        //  ((DevExpress.XtraEditors.SpinEdit)ctrControl).Text = "";
        //else if (object.ReferenceEquals(ctrControl.GetType(), typeof(DevExpress.XtraEditors.CheckEdit)))
        //  ((DevExpress.XtraEditors.CheckEdit)ctrControl).Checked = false;
        //else if (object.ReferenceEquals(ctrControl.GetType(), typeof(DevExpress.XtraEditors.MemoEdit)))
        //  ((DevExpress.XtraEditors.MemoEdit)ctrControl).Text = "";

        if (ctrControl.Controls.Count > 0)
          ClearForm(ctrControl);
      }
    }

    public static void ClearForm(Control parent, ArrayList toSkip)
    {
      // clears text out of a form
      foreach (Control ctrControl in parent.Controls)
      {
        if (toSkip.Contains(ctrControl.Name)) continue;

        //Loop through all controls 
        if (object.ReferenceEquals(ctrControl.GetType(), typeof(System.Windows.Forms.MaskedTextBox)))
          ((System.Windows.Forms.MaskedTextBox)ctrControl).Text = string.Empty;
        if (object.ReferenceEquals(ctrControl.GetType(), typeof(System.Windows.Forms.TextBox)))
          ((System.Windows.Forms.TextBox)ctrControl).Text = string.Empty;
        else if (object.ReferenceEquals(ctrControl.GetType(), typeof(System.Windows.Forms.RichTextBox)))
          ((System.Windows.Forms.RichTextBox)ctrControl).Text = string.Empty;
        else if (object.ReferenceEquals(ctrControl.GetType(), typeof(System.Windows.Forms.ComboBox)))
          ((System.Windows.Forms.ComboBox)ctrControl).SelectedIndex = -1;
        else if (object.ReferenceEquals(ctrControl.GetType(), typeof(System.Windows.Forms.CheckBox)))
          ((System.Windows.Forms.CheckBox)ctrControl).Checked = false;
        else if (object.ReferenceEquals(ctrControl.GetType(), typeof(System.Windows.Forms.RadioButton)))
          ((System.Windows.Forms.RadioButton)ctrControl).Checked = false;
        //else if (object.ReferenceEquals(ctrControl.GetType(), typeof(DevExpress.XtraEditors.ComboBoxEdit)))
        //  ((DevExpress.XtraEditors.ComboBoxEdit)ctrControl).Text = "";
        //else if (object.ReferenceEquals(ctrControl.GetType(), typeof(DevExpress.XtraEditors.TextEdit)))
        //  ((DevExpress.XtraEditors.TextEdit)ctrControl).Text = "";
        //else if (object.ReferenceEquals(ctrControl.GetType(), typeof(DevExpress.XtraEditors.SpinEdit)))
        //  ((DevExpress.XtraEditors.SpinEdit)ctrControl).Text = "";
        //else if (object.ReferenceEquals(ctrControl.GetType(), typeof(DevExpress.XtraEditors.CheckEdit)))
        //  ((DevExpress.XtraEditors.CheckEdit)ctrControl).Checked = false;
        //else if (object.ReferenceEquals(ctrControl.GetType(), typeof(DevExpress.XtraEditors.MemoEdit)))
        //  ((DevExpress.XtraEditors.MemoEdit)ctrControl).Text = "";

        if (ctrControl.Controls.Count > 0)
          ClearForm(ctrControl);
      }
    }

    public static string cleanJob(string job)
    {
      try
      {
        if (chkNull.isNull(job)) return "";
        if (job.IndexOf('-') == -1) return job;

        string[] foo = job.Split('-');
        if (foo.Length == 0) return job;
        if (foo.Length == 1) return job;

        if (!IsNumeric(foo[1])) return job;

        return foo[0];
      }
      catch 
      {
        return chkNull.whenNull(job);
      }


    }

    public static void getSigBlock(out string[] ret)
    {
      // a Kassi Porter thing
      string foo = Application.StartupPath + @"\" + "sigblock.pref";

      if (File.Exists(foo))
      {
        // read their sig block from a file saved by the options form.
        // I don't know how many lines they're going to have in there
        // so I'll initialize the array to some massive number then read
        // until I get a blank line
        string[] sig = new string[20]; // a 20 line sig is some serious narcissism

        for (int i = 0; i < sig.Length; i++)
          sig[i] = "";

        ReadAnythingFromFile(foo, ref sig);
        int count = 0;
        // count the lines of text
        for (int i = 0; i < sig.Length; i++)
        {
          if (sig[i] != "")
            count++;
        }

        ret = new string[count];

        // initialize it
        for (int i = 0; i < ret.Length; i++)
          ret[i] = "";

        count = 0;
        // populate the return array
        for (int i = 0; i < sig.Length; i++)
        {
          if (sig[i] != "")
          {
            ret[count] = sig[i];
            count++;
          }
        }
      }
      else
      {
        ret = new string[1];
        ret[0] = "";
      }
    }

    public static double TruncateSignificant(double num, int digits)
    {
      // rounding is a serious problem around here and I find it happening
      // a lot
      if (digits < 1) return num;

      if (num == 0) return num;

      int p;
      double f;

      p = (int)Math.Ceiling(Math.Log10(num));
      f = Math.Pow(10, digits - p);

      return ((int)(num * f)) / f;
    }

    public static bool faxSigBlock = false;

    public static string RemoveTrailingZeroes(string input)
    {
      // regular expressions have their utility but I try to avoid
      // them because they're unreadable
      Regex reg1 = new Regex("\\.\\d*(?<1>0+)[^\\d]*$", RegexOptions.IgnoreCase);

      Match m = reg1.Match(input);
      if (m.Success)
      {
        input = input.Replace(m.Groups["1"].Value, "");
        Regex reg2 = new Regex("(?<1>\\.)[^\\d]*$", RegexOptions.IgnoreCase);
        m = reg2.Match(input);

        if (m.Success)
          input = input.Replace(".", "");

        Regex reg3 = new Regex("\\d", RegexOptions.IgnoreCase);
        m = reg3.Match(input);

        if (!m.Success)
          input = "0" + input;
      }

      if (input.StartsWith("."))
        input = "0" + input;

      return input;
    }

    public static string formatMoney(object s)
    {
      // keeps me from having to do a format with args each
      // time I need a money val        
      return String.Format("{0:C}", s);
    }

    public static string formatShortDate(object o)
    {
      try
      {
        if (IsDate(o))
        {
          DateTime d = Convert.ToDateTime(o);
          return String.Format("{0:MM/dd/yy}", d);
        }
        else
          return String.Format("{0:MM/dd/yy}", o);
      }
      catch
      {
        return o.ToString();
      }
    }

  }
}
