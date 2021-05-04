using System;
using Ultraviolet;
using Ultraviolet.BASS;
using Ultraviolet.Content;
using Ultraviolet.FreeType2;
using Ultraviolet.Input;
using Ultraviolet.OpenGL;
using Ultraviolet.Presentation;
using Ultraviolet.Presentation.Styles;
using Ultraviolet.SDL2;
using UpfGame.UI.Screens;

namespace UpfGame
{
    public partial class Game : UltravioletApplication
    {
        public Game()
            : this(GameFlags.None)
        { }

        public Game(GameFlags flags)
            : base("DEVELOPER_PLACEHOLDER", "APPLICATION_PLACEHOLDER")
        {
            this.flags = flags;
        }

        protected override UltravioletContext OnCreatingUltravioletContext()
        {
            var configuration = new SDL2UltravioletConfiguration();
            configuration.EnableServiceMode = ShouldRunInServiceMode();
            configuration.WatchViewFilesForChanges = ShouldDynamicallyReloadContent();
            configuration.Plugins.Add(new OpenGLGraphicsPlugin());
            configuration.Plugins.Add(new BASSAudioPlugin());
            configuration.Plugins.Add(new FreeTypeFontPlugin());
            configuration.Plugins.Add(new PresentationFoundationPlugin());

//-:cnd:noEmit
#if DEBUG
            configuration.Debug = true;
            configuration.DebugLevels = DebugLevels.Error | DebugLevels.Warning;
            configuration.DebugCallback = (uv, level, message) =>
            {
                System.Diagnostics.Debug.WriteLine(message);
            };
#endif
//+:cnd:noEmit

            return new SDL2UltravioletContext(this, configuration);
        }

        protected override void OnInitialized()
        {
            UsePlatformSpecificFileSource();
            base.OnInitialized();
        }

        protected override void OnLoadingContent()
        {
            this.contentManager = ContentManager.Create("Content");

            LoadContentManifests();
            LoadPresentation();

            if (Ultraviolet.IsRunningInServiceMode)
            {
                CompileBindingExpressions();
                Environment.Exit(0);
            }
            else
            {
                this.gamePlayScreen = new GamePlayScreen(this.contentManager);
                Ultraviolet.GetUI().GetScreens().Open(gamePlayScreen);
            }

            base.OnLoadingContent();
        }

        protected void LoadContentManifests()
        {
            var uvContent = Ultraviolet.GetContent();

            var contentManifestFiles = this.contentManager.GetAssetFilePathsInDirectory("Manifests");
            uvContent.Manifests.Load(contentManifestFiles);
        }

        protected void LoadPresentation()
        {
            var upf = Ultraviolet.GetUI().GetPresentationFoundation();
            upf.RegisterKnownTypes(GetType().Assembly);

            if (!ShouldRunInServiceMode())
            {
                globalStyleSheet = GlobalStyleSheet.Create();
                globalStyleSheet.Append(contentManager, "UI/DefaultUIStyles");
                globalStyleSheet.Append(contentManager, "UI/GameStyles");
                upf.SetGlobalStyleSheet(globalStyleSheet);

                CompileBindingExpressions();
                upf.LoadCompiledExpressions();
            }
        }

        protected override void OnUpdating(UltravioletTime time)
        {
            if (Ultraviolet.GetInput().GetKeyboard().IsKeyPressed(Key.Escape))
            {
                Exit();
            }
            base.OnUpdating(time);
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (this.gamePlayScreen != null)
                    this.gamePlayScreen.Dispose();

                if (this.globalStyleSheet != null)
                    this.globalStyleSheet.Dispose();

                if (this.contentManager != null)
                    this.contentManager.Dispose();
            }
            base.Dispose(disposing);
        }

        private Boolean ShouldRunInServiceMode()
        {
            return (flags & GameFlags.CompileExpressions) == GameFlags.CompileExpressions;
        }

        private Boolean ShouldCompileBindingExpressions()
        {
//-:cnd:noEmit
#if DEBUG
            return true;
#else
            return (flags & GameFlags.CompileExpressions) == GameFlags.CompiledExpressions || System.Diagnostics.Debugger.IsAttached;
#endif
//+:cnd:noEmit
        }

        private Boolean ShouldDynamicallyReloadContent()
        {
//-:cnd:noEmit
#if DEBUG
            return true;
#else
            return false;
#endif
//+:cnd:noEmit
        }

        private void CompileBindingExpressions()
        {
            if (!ShouldCompileBindingExpressions())
                return;

            var compilationFlags = CompileExpressionsFlags.None;
            if ((this.flags & GameFlags.CompileExpressions) == GameFlags.CompileExpressions)
            {
                compilationFlags |= CompileExpressionsFlags.ResolveContentFiles;
                compilationFlags |= CompileExpressionsFlags.IgnoreCache;
            }

            var upf = Ultraviolet.GetUI().GetPresentationFoundation();
            upf.CompileExpressionsIfSupported("Content", compilationFlags);
        }

        private readonly GameFlags flags;
        private ContentManager contentManager;
        private GamePlayScreen gamePlayScreen;
        private GlobalStyleSheet globalStyleSheet;
    }
}
