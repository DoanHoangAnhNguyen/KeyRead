using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using System.Threading;
using System.Linq;

namespace KeyRead
{
    class KeyMonitoring
    {

        //Thread intance
        Thread threadModKey;
        Thread keyThread;

        //state of machine
        public bool isRunning = true;
        bool isShiftPressed = false;


        private Key pressedKey;
        private StringBuilder c;
        //private Label _label;

        private Key[] keys = { Key.A, Key.O, Key.U, Key.S };
        private Key[] shiftKeys = { Key.LeftShift, Key.RightShift };
        private string[] needChangeChars = { "a", "o", "u", "A", "O", "U", "ä", "ö", "ü", "Ä", "Ö", "Ü" };
        private string[] eszett = { "s", "S", "ß" };

        private Dictionary<string, string> convertTable = new Dictionary<string, string>()
        {
            { "aE","ä"},
            { "oE","ö"},
            { "uE","ü"},
            { "AE","Ä"},
            { "OE","Ö"},
            { "UE","Ü"},
            { "äE","ae"},
            { "öE","oe"},
            { "üE","ue"},
            { "ÄE","AE"},
            { "ÖE","OE"},
            { "ÜE","UE"},
            { "sS","ß"},
            { "ßS","ss"},
            { "SS","SS"}
        };

        // Listen to keyboard object
        private LowLevelKeyboardListener _listener;
        void _listener_OnkeyPressed(object sender, KeyPressedArgs e)
        {
            if (!shiftKeys.Contains(e.KeyPressed))
                pressedKey = e.KeyPressed;
            Thread.Sleep(1);

        }


        /// <summary>
        /// Constructor for KeyMonitoring object
        /// </summary>
        /// <param name="label"></param>
        public KeyMonitoring()//Label label
        {
            //_label = label;
            threadModKey = new Thread(modKey);
            threadModKey.SetApartmentState(ApartmentState.STA);
            keyThread = new Thread(keyModifier);
            keyThread.SetApartmentState(ApartmentState.STA);
            Control.CheckForIllegalCrossThreadCalls = false;

        }

        /// <summary>
        /// Start converting to German char
        /// </summary>
        public void Run()
        {
            c = new StringBuilder();
            isRunning = true;
            threadModKey.Start();
            keyThread.Start();
            //replaceKeyThread.Start();

            _listener = new LowLevelKeyboardListener();
            _listener.OnKeyPressed += _listener_OnkeyPressed;

            _listener.HookKeyboard();
        }

        /// <summary>
        /// Resume converting to German char
        /// </summary>
        public void Resume()
        {            
            threadModKey = new Thread(modKey);
            threadModKey.SetApartmentState(ApartmentState.STA);
            keyThread = new Thread(keyModifier);
            keyThread.SetApartmentState(ApartmentState.STA);
            Run();
        }

        /// <summary>
        /// Stop converting to German char
        /// </summary>
        public void Stop()
        {            
            isRunning = false;
            _listener.UnHookKeyboard();
        }

        /// <summary>
        /// Method to be executed in Germen char modifier thread
        /// </summary>
        private void keyModifier()
        {
            while (isRunning)
            {
                Thread.Sleep(40);
                if (!keys.Contains(pressedKey))
                {
                    replaceChar();
                    continue;
                }

                if ((pressedKey == Key.S) && eszett.Contains(c.ToString()))
                {
                    replaceChar();
                    continue;
                }
                if ((isShiftPressed && !Control.IsKeyLocked(Keys.CapsLock)) ||
                                    (!isShiftPressed && Control.IsKeyLocked(Keys.CapsLock)))
                {
                    c.Clear();
                    c.Append(pressedKey.ToString());

                    pressedKey = Key.None;
                }
                else
                {
                    c.Clear();
                    c.Append(pressedKey.ToString().ToLower());
                    pressedKey = Key.None;
                }

                replaceChar();
            }
        }

        /// <summary>
        /// Method to be executed in check state of shift thread
        /// </summary>
        private void modKey()
        {
            while (isRunning)
            {
                Thread.Sleep(40);

                if (shiftKeyCheck())
                {
                    isShiftPressed = true;
                    continue;
                }
                isShiftPressed = false;

            }
        }

        /// <summary>
        /// Check shift key press
        /// </summary>
        /// <returns></returns>
        private static bool shiftKeyCheck()
        {
            return (Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) > 0 ||
                                (Keyboard.GetKeyStates(Key.RightShift) & KeyStates.Down) > 0;
        }

        /// <summary>
        /// Convert to Germen char method
        /// </summary>
        private void replaceChar()
        {
            if (pressedKeyCheck())
            {
                c.Append(pressedKey.ToString());
                string temp = convertTable[c.ToString()];
                c.Clear().Append(temp);
                SendKeys.SendWait("{BACKSPACE}");
                SendKeys.SendWait("{BACKSPACE}");
                SendKeys.SendWait(c.ToString());
                SendKeys.Flush();
                if (c.Length > 1) c.Clear();
            }
            else if (!(keys.Contains(pressedKey) || pressedKey == Key.None))
            {
                c.Clear();
            }
            pressedKey = Key.None;
        }

        /// <summary>
        /// Check condition of pressedKey value
        /// </summary>
        /// <returns></returns>
        private bool pressedKeyCheck()
        {
            return (pressedKey == Key.E && needChangeChars.Contains(c.ToString())) || (pressedKey == Key.S && eszett.Contains(c.ToString()));
        }
    }

}
