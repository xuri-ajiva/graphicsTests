﻿using System;
using System.Drawing;
using System.Security.Cryptography;
using GameFramework;
using OpenTK;

namespace OpenTKFramework {
    public abstract class MainClass {
        public int    totalFramesRendert = 0;
        public int    totalFramesUpdated = 0;
        public int    numFrames          = 0;
        public double framesTime         = 0.0;
        public int    frameRate          = 0;


        public GraphicsManager I;
        public GameWindow      Window = null;

        /// <summary>
        /// Hook up the initialize callback
        /// already done:
        ///     GraphicsManager.Instance.Initialize( this.Window );
        ///     this.I = GraphicsManager.Instance;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void Initialize(object sender, EventArgs e);

        /// <summary>
        /// Hook up the update callback
        /// already done:
        ///     Fps Calculated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void Update(object sender, FrameEventArgs e);

        /// <summary>
        /// Hook up the render callback
        /// already done:
        ///     this.I.SwapBuffers();
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void Render(object sender, FrameEventArgs e);

        /// <summary>
        /// Hook up the shutdown callback
        /// already done:
        ///     GraphicsManager.Instance.Shutdown();
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void Shutdown(object sender, EventArgs e);

        public GameWindow Create(Size s = default,string title = "Game Name") {
            if ( s == default ) s = new Size( 800, 600 );

            // Create static (global) window instance
            this.Window = new GameWindow();

            //// Hook up the initialize callback
            //Window.Load += new EventHandler<EventArgs>( Initialize );
            //// Hook up the update callback
            //Window.UpdateFrame += new EventHandler<FrameEventArgs>( Update );
            //// Hook up the render callback
            //Window.RenderFrame += new EventHandler<FrameEventArgs>( Render );
            //// Hook up the shutdown callback
            //Window.Unload += new EventHandler<EventArgs>( Shutdown );

            this.Window.Load += delegate(object sender, EventArgs e) {
                GraphicsManager.Instance.Initialize( this.Window );
                this.I = GraphicsManager.Instance;
                Initialize( sender, e );
            };
            
            this.Window.UpdateFrame += delegate(object sender, FrameEventArgs e) {
                this.totalFramesUpdated++;
                this.numFrames  += 1;
                this.framesTime += e.Time;

                if ( this.framesTime >= 1.0f ) {
                    this.frameRate  = (int) ( Convert.ToDouble( this.numFrames ) / this.framesTime );
                    this.framesTime = 0.0;
                    this.numFrames  = 0;
                }

                Update( sender, e );
            };

            this.Window.RenderFrame += delegate(object sender, FrameEventArgs e) {
                this.totalFramesRendert++;
                Render( sender, e );
                this.I.SwapBuffers();
            };
            
            this.Window.Unload += delegate(object sender, EventArgs e) {
                GraphicsManager.Instance.Shutdown();
                Shutdown( sender, e );
            };

            // Set window title and size
            this.Window.Title      = title;
            this.Window.ClientSize = s;

            return this.Window;
        }

        public void Run(float updateRate = 60.0f) {
            // Run the game at 60 frames per second. This method will NOT return
            // until the window is closed.
            this.Window.Run( updateRate );

            // If we made it down here the window was closed. Call the windows
            // Dispose method to free any resources that the window might hold
            this.Window.Dispose();

        #if DEBUG
            Console.ReadLine();
        #endif
        }
    }
}