using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Synth.DB
{
  public static class SqlDataReaderExt
  {
    public static string SafeGetString(this SqlDataReader reader, int colIndex)
    {
      if (!reader.IsDBNull(colIndex))
        return reader.GetString(colIndex);
      else
        return string.Empty;
    }

    public static int SafeGetInt32(this SqlDataReader reader, int colIndex)
    {
      if (!reader.IsDBNull(colIndex))
        return reader.GetInt32(colIndex);
      else
        return 0;
    }

    public static double SafeGetDouble(this SqlDataReader reader, int colIndex)
    {
      if (!reader.IsDBNull(colIndex))
        return reader.GetDouble(colIndex);
      else
        return 0.0;
    }

    public static DateTime SafeGetDateTime(this SqlDataReader reader, int colIndex)
    {
      if (!reader.IsDBNull(colIndex))
        return DateTime.Parse(reader.GetString(colIndex));
      else
        return DateTime.MinValue;
    }
  }
}
