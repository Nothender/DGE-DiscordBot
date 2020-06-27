#V0.5.3
- Added scaling methods : `NEAREST` and `CLEAR`, method : `RenderingCore.ResizeBuffer(originalBuffer, targetBuffer, imageScalingMethod)`
- The frame buffers can now have a draw size and a display size which means you can have a 16x16 px buffer to draw onto, and then display it as 2048x2048 px, the scaling method cannot be `CLEAR`

#V0.5.0
- Created the `PixelBuffer` which works like a surface we can draw onto, `FrameBuffer` now extends from it and so will sprites.
- Put the `PixelRenderMode` into the `RenderingCore` with the method `AlphaBlend` and `ToOpaqueColor` (which work per pixel)

#V0.4.2
- Fixed alpha blending now works correctly, fix for supported imageBuffers file formats

#V0.4.1
- Alpha blending can now be used to draw to the `FrameBuffer` -> always returns background value when trying to blend a color1 to a color2

#V0.4.0
- Created `FrameBuffer` a real frame buffers which uses/contains pixels, and can be saved to disk, to be displayed as a discord message
- Added specifics commands to the use the `FrameBuffer` like Init, Display, Clear, Draw and DrawRect

#V0.3.0 :
- Created `StringFrameBuffer` a frame buffer of that can be displayed as messages within strings containing strings/emojis as pixels
- Added commands to use the SFB, like Init, Display, Draw and Clear

#V0.2.2 :
- Created `LogManager` : contains LogPrefixes (WARN, LOG, ERROR, DEBUG) and has the `LogDebug` method moved from Program
- Created `ProgramsInstance` and `ProgramsManager` which will Serve as holders for programs made with the engine
- Added the Method `GetArgsAsSingleStringFrom` read the documention in the XML file after building/code summary for more information

#V0.1.0 :
- Extended base + command support : ready to implement features

#V0.0.0 :
- Bot base, nothing more
