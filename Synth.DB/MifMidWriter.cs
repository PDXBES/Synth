using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Synth.DB
{
  public class MifMidWriter : IMifMidWriter, IDisposable
  {
    StreamWriter mifMidFile;

    public MifMidWriter(string fileName)
    {
      mifMidFile = new StreamWriter(fileName);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
        if (mifMidFile != null)
        {
          mifMidFile.Dispose();
          mifMidFile = null;
        }
    }

    ~MifMidWriter()
    {
      Dispose(false);
    }

    public void WriteLine(string line = "")
    {
      mifMidFile.WriteLine(line);
    }

    public void Flush()
    {
      mifMidFile.Flush();
    }

    public void Close()
    {
      mifMidFile.Close();
    }
  }
}
