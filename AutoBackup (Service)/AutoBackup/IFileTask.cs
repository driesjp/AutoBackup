using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup
{
    // define a contract for all file tasks
    public interface IFileTask
    {
        void Execute();
    }
}