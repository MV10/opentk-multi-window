
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace opentkmulti;

public unsafe class Program
{
    private static readonly Vector3[] ClearColors =
    [
        new Vector3(1.0f, 0.0f, 0.0f), // red
        new Vector3(0.0f, 1.0f, 0.0f), // green
        new Vector3(0.0f, 0.0f, 1.0f)  // blue
    ];

    private static bool running = true;
    private static readonly NativeWindow[] win = new NativeWindow[3];

    public static void Main(string[] args)
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // GLFW isn't compatible with Wayland, use X11 or XWayland
            GLFW.InitHint(InitHintPlatform.Platform, Platform.X11);
        }

        if (!GLFW.Init())
        {
            Console.WriteLine("Failed to initialize GLFW");
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            var settings = new NativeWindowSettings
            {
                ClientSize = new Vector2i(400, 300),
                Location = new Vector2i(100 + i * 450, 100), // h-offset
                Title = $"Window {i + 1} (Color: {ClearColors[i]})",
                API = ContextAPI.OpenGL,
                APIVersion = new Version(4, 5),
                Profile = ContextProfile.Core
            };

            win[i] = new NativeWindow(settings);
        }

        while (running)
        {
            for (int i = 0; i < win.Length; i++)
            {
                win[i].NewInputFrame();
            }

            NativeWindow.ProcessWindowEvents(waitForEvents: false);

            for (int i = 0; i < win.Length; i++)
            {
                if (win[i].KeyboardState.IsKeyReleased(Keys.Escape) || GLFW.WindowShouldClose(win[i].WindowPtr))
                {
                    running = false;
                    break;
                }
            }

            for (int i = 0; i < win.Length; i++)
            {
                if (win[i].Exists)
                {
                    win[i].MakeCurrent();

                    GL.ClearColor(ClearColors[i].X, ClearColors[i].Y, ClearColors[i].Z, 1.0f);
                    GL.Clear(ClearBufferMask.ColorBufferBit);

                    win[i].Context.SwapBuffers();
                }
            }
        }

        for (int i = 0; i < win.Length; i++)
        {
            if (win[i].Exists)
            {
                win[i].Close();
                win[i].Dispose();
            }
        }

        GLFW.Terminate();
    }
}
