# Discord "Game Engine"
A game engine using discord's Unofficial .NET API to make bots

This project was inspired by a video from someone on Reddit and or Youtube : https://youtu.be/0fWdU8JCT6Y

will contain multiple features to be explained later

# Documentation :
> The documentation is deprecated, and will be updated in a wiki once the repository is set to public

Command string prefix = "//"

Debug/Test Methods :
- when entering the command `ping` the bot replies with `pong`
- when entering the command `logTest` with argument 1 : the logMode, and other arguments the logInfo the bot will reply with the DGE logMode + Concatenation of logInfos

FrameBuffers manipulations (2 FrameBuffers - Each frame buffer runs on a single instance for the whole bot) :

The `StringFrameBuffer`<FBType => SFB> using stringSymbols for each pixels, though limited to 2000 chars per message so multiple message buffering is required.
and the `FrameBuffer`<FBType => FB> using real pixels drawing onto a Bitmap then saving it to send it to discord -> really useful for pixelBlending, spriteDrawing, high res, etc... n.b. the frameBuffers will not work in Frames per second because of buffering/drawing speed (CPU Driven) and is recommend to be used in a turn-based/with great rendering delays program;
to execute the following commands use <the_command> + <FBType\> e.g. //initFBS
- the command `init<FBType>` Creates the FrameBuffer in memory and initializes the FrameBuffer
- the command `display<FBType>` Displays the FrameBuffer via the bot sending messages in the current channel
- the command `drawTo<FBType> x y stringPx` will draw a pixel to the coordinates x, y with the symbol stringPx/Color for FrameBuffer
- the command `drawRectTo<FBType> x y stringPx` will draw a pixel to the coordinates x, y with the symbol/Color for FrameBuffer //Works only with FrameBuffer (not SFB)
- the command `clear<FBType> clearColor` will clear the Frame Buffer with the `clearColor` symbol/Color (FrameBuffer value, cannot be modified)
- the command `setFBPixelRenderingMode` will allow for transparency or not depending on the mode set : AlphaBlending, Normal, Replace // /!\ Alpha blending is really slow, and must be used only when needed

Color can take a 4th argument when using the FrameBuffer which is alpha (by default 255)
