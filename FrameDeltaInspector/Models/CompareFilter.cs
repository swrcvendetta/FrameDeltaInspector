using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FrameDeltaInspector.Models
{
    public class CompareFilter
    {
        public virtual BitmapImage Compare(BitmapImage imageA, BitmapImage imageB)
        {
            return imageA;
        }
        public override string ToString()
        {
            return "CompareFilterDefault";
        }
    }
}
