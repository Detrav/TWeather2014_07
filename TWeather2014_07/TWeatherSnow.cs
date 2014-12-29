using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TWeather2014_07
{
    internal sealed class TWeatherSnow : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private Margins marg;

        public struct Margins
        {
            public int Left, Right, Top, Bottom;
        }

        Texture[] s;
        Vector2[] o;
        SpriteBatch batch;
        Color c = new Color(255, 255, 255, 150);
        Particle[] pr;
        Random rand = new Random();
        int size = 1000;
        int max = 10000;

        NotifyIcon notifyIcon;
        
        


        [DllImport("user32.dll", SetLastError = true)]
        private static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        [DllImport("dwmapi.dll")]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);
        [DllImport("user32.dll")]
        public static extern void SetWindowPos(uint Hwnd, int Level, int X, int Y, int W, int H, uint Flags);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);


        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;
        public static long WS_CHILD = 0x40000000; //child window
        public static long WS_BORDER = 0x00800000L; //window with border
        public static long WS_DLGFRAME = 0x00400000; //window with double border but no title
        public static long WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar 
        public static long WS_SYSMENU = 0x00080000; //window menu
        public static long WS_THICKFRAME = 0x00040000L;
        public static long WS_MAXIMIZE = 0x01000000;
        public static long WS_POPUP = 0x80000000L;
        public static long WS_MAXIMIZEBOX = 0x00010000;
        public static long WS_MINIMIZE = 0x20000000;
        public static long WS_MINIMIZEBOX = 0x00020000;
        public static long WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;


        public TWeatherSnow( int m )
        {
            this.max = m;
            graphics = new GraphicsDeviceManager(this);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            marg = new Margins();
            marg.Left = -1;
            marg.Top = -1;
            marg.Right = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width + 1; ;
            marg.Bottom = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height + 1; ;
            graphics.PreferredBackBufferHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height + 2;
            graphics.PreferredBackBufferWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width + 2;
            graphics.ApplyChanges();
            ((RenderForm)this.Window.NativeWindow).FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            IntPtr h = ((RenderForm)this.Window.NativeWindow).Handle;
            SetWindowLong(h, GWL_STYLE, (IntPtr)(unchecked((int)0x80000000) | WS_BORDER | WS_SYSMENU));
            SetWindowLong(h, GWL_EXSTYLE, (IntPtr)(((GetWindowLong(h, GWL_EXSTYLE) ^ WS_EX_LAYERED ^ WS_EX_TRANSPARENT) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW));
            SetWindowPos((uint)h, -1, -1, -1, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0);
            SetLayeredWindowAttributes(h, 0, 255, LWA_ALPHA);
            DwmExtendFrameIntoClientArea(h, ref marg);
            Content.RootDirectory ="assets";
            //Content.ServiceProvider
            s = new Texture[]{
                Content.Load<Texture>("snf0"),
                Content.Load<Texture>("snf1"),
                Content.Load<Texture>("snf2"),
                Content.Load<Texture>("snf3"),
                Content.Load<Texture>("snf4"),
                Content.Load<Texture>("snf5")
            };
            o = new Vector2[]{
                new Vector2(s[0].Width/2f,s[0].Height/2f),
                new Vector2(s[1].Width/2f,s[0].Height/2f),
                new Vector2(s[2].Width/2f,s[0].Height/2f),
                new Vector2(s[3].Width/2f,s[0].Height/2f),
                new Vector2(s[4].Width/2f,s[0].Height/2f),
                new Vector2(s[5].Width/2f,s[0].Height/2f)
            };
            batch = new SpriteBatch(graphics.GraphicsDevice);
           // e = new Effect(graphics.GraphicsDevice,)
            //s.Save("test.png", ImageFileType.Png);

            pr = new Particle[max];
            
            for(int i = 0;i<pr.Length;i++)
            {
                pr[i].p.X = rand.Next(-500, graphics.PreferredBackBufferWidth + 500);
                pr[i].p.Y = rand.Next(-500, graphics.PreferredBackBufferHeight + 500);
                pr[i].l.X = (float)rand.Next(20, 50) / 1000f;
                //pr[i].l.X += (float)rand.Next(10, 30) / 1000f;
                pr[i].l.Y = pr[i].l.X;
                pr[i].n = rand.Next(0, 5);
                pr[i].r = (float)((double)rand.Next(0, 1000) * Math.PI / 1000.0);
                while(Math.Abs(pr[i].c)<0.0002) pr[i].c = (float)((double)rand.Next(-8, 8) * Math.PI / 1000.0);
                pr[i].s.X = (float)rand.Next(-1000, 1000) / 1000f + pr[i].l.X*10f;
                pr[i].s.Y = (float)rand.Next(0, 2000) / 1000f + pr[i].l.X * 10f;
            }

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon("TWeather2014_07.ico");
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(2500, "TWeather2014", "Я здесь!", ToolTipIcon.Info);
            MenuItem mi = new MenuItem("Выход");
            mi.Click += mi_Click;
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] {
                mi
            });
        }

        void mi_Click(object sender, EventArgs e)
        {
            Exit();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float dt = (float)gameTime.ElapsedGameTime.Milliseconds / 1000f;
            if (gameTime.ElapsedGameTime.Milliseconds > 33)
            {
                size--;
                dt = 0.033f;
            }
            else size++;
            if (size < 500) size = 500;
            if (size > max) size = max;

            for (int i = 0; i < pr.Length; i++)
            {
                
                pr[i].r += pr[i].c;
                pr[i].p += pr[i].s * (dt /0.016f);
                if (pr[i].p.X > graphics.PreferredBackBufferWidth + 500) pr[i].p.X = -490;
                else if(pr[i].p.X<-500) pr[i].p.X = graphics.PreferredBackBufferWidth + 490;
                if (pr[i].p.Y > graphics.PreferredBackBufferHeight + 500) pr[i].p.Y = -500;
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.Transparent);
            batch.Begin();
            for (int i = 0; i < size; i++)
            {
                batch.Draw(s[pr[i].n], pr[i].p, null, Color.White, pr[i].r, o[pr[i].n], pr[i].l, SpriteEffects.None, 0);
            }
            batch.End();
            base.Draw(gameTime);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        public struct Particle
        {
            public int n;//Number
            public Vector2 p;//Position
            public Vector2 s;//Speed
            public float r;//rotation
            public float c;//rotation speed
            public Vector2 l;//scale
        }

        protected override void UnloadContent()
        {
            notifyIcon.Visible = false;
            //notifyIcon.Dispose();
        }
    }
}
