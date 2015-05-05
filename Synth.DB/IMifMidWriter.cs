using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Synth.DB
{
  public interface IMifMidWriter
  {
    void WriteLine(string line = "");
    void Flush();
    void Close();
  }
}
