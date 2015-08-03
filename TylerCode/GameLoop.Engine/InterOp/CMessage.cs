using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.InterOp
{
   
   /// <summary>
   /// Defines the C structs and functions for peeking at messages in the application
   /// </summary>
   public class CMessage
   {
       /// <summary>
       /// Peeks at the Application event queue finds all messages in the queue
       /// </summary>
       /// <param name="msg">The C Struct Message for peeking at the application event queue</param>
       /// <returns></returns>
       public static bool PeekMessage(out Message msg)
       {
           return PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
       }
       
       [System.Security.SuppressUnmanagedCodeSecurity]
       [DllImport("User32.dll", CharSet = CharSet.Auto)]
       public static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);
      
       [StructLayout(LayoutKind.Sequential)]
       public struct Message
       {
           /// <summary>
           /// Windows Handle for application
           /// </summary>
           public IntPtr hWnd;
           
           /// <summary>
           /// Number of unhandled event messages in the application event queue
           /// </summary>
           public Int32 msg;
 
           public IntPtr wParam; //Width
           public IntPtr lParam; // Length
           public uint time;
           public Point p;
       }

   }

    
}
