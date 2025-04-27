using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// --- Nội dung file ButtonConfig.cs ---
namespace AorusM2 // Đảm bảo cùng namespace với Form của bạn
{
    public class ButtonConfig
    {
        // Corresponds to BA placeholder
        public int FnType { get; set; } = 0;
        // Corresponds to AA placeholder
        public int MouseButton { get; set; } = 0;
        // Corresponds to BB placeholder
        public int MouseWheelCount { get; set; } = 0;
        // Corresponds to AAA placeholder
        public int KeyboardModifier { get; set; } = 0;
        // Corresponds to BBB placeholder
        public int KeyboardKeyCode { get; set; } = 0;
        // Corresponds to AAAA placeholder
        public int MacrosIndex { get; set; } = 0; // Assuming numeric index input
        // Corresponds to BBBB placeholder
        public int MediaKey { get; set; } = 0;

        public void Reset()
        {
            FnType = 0;
            MouseButton = 0;
            MouseWheelCount = 0;
            KeyboardModifier = 0;
            KeyboardKeyCode = 0;
            MacrosIndex = 0;
            MediaKey = 0;
        }
    }
}