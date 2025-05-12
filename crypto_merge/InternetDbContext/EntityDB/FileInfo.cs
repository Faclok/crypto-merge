using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetDatabase.EntityDB;

public class FileInfo: BaseEntity
{
    public byte[] Data { get; set; }
    public string Name { get; set; }
    public string Extension { get; set; }
}
