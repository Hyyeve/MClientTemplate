using DuckGame;
using MClient.Core.EventSystem.Events.Drawing.Screen;
using MClient.Core.EventSystem.Events.Drawing.World;
using MClient.Core.EventSystem.Events.Game;
using MClient.RenderSystem;
using MClient.RenderSystem.RenderTasks;
using Microsoft.Xna.Framework.Graphics;

namespace MClient.MTemplate
{
    /// <summary>
    /// Demonstration class for the render system!
    /// </summary>
    public class MRenderSystemDemo
    {
        /*
        The render system is fairly complex, but relatively easy
        to use. I have plans to implement a different/additional
        render system in future, too.
        
        Most rendering should be done through the render events.
        The render system comes with a large number of events for
        drawing on different "layers", as well as ones to differentiate
        between world/game space and screen space, which I'll go 
        over shortly.
        
        If you try to do any rendering outside of render events, you
        will usually crash the game, as the renderers are not
        "active" at that time. There's some cases where you CAN render
        outside of render events though, which I'll go over later.
        */

        [MEventWorldDrawBackground]
        public static void RenderBackgroundWorld()
        {
            /*
            This will be called every time the Background
            layer is drawn
            */
        }
        
        /*
        Duck Game uses "layers" to organise it's rendering. These are
        arbitrary separations between different types of object, each
        of which are drawn at a specific time, to create a specific
        order of "depth". The Blocks layer, for example, is where all
        solid blocks are drawn, and is in front of the Background layer, 
        but behind the HUD layer.
        
        Generally, if you want things to appear as if they're part of
        the level, you'll want to draw on the Blocks layer. There's
        two different events you could use for that - the
        MEventScreenDrawBlocks, and the MEventWorldDrawBlocks. Indeed,
        that's the case for all layers. Which event you use is determined
        by what kind of thing you're trying to draw. If you want to
        draw something that stays in position relative to the level - for
        example, a pathfinding trail - then you will need to use the World
        event, as the renderer will assume positions are in World/Game space
        for it.
        
        Alternatively, if you're trying to draw some HUD elements, then you'll
        want to use the Screen version, as drawing done with that is assumed to
        be in screen space.
        */

        [MEventScreenDrawBackground]
        public static void RenderBackgroundScreen()
        {
            /*
            This will be called every time the Background
            layer is drawn, but using screen positions.
            */
        }
        
        /*
        The difference between world/game space and screen space is fairly
        simple. World space refers to the positions that are used in-game.
        If you draw something at the location of the duck in a world space
        event, it will show up at the location of the duck.
        
        Screen space refers to the positions that are actually used on your
        screen. If you draw something at 100,100 in a screen-space event,
        it will be drawn 100 pixels across and down from the top left of your
        screen, and will not move with the camera. Of importance when using
        screen-space drawing - because it's by pixel, you can't assume it'll
        be the same for everyone! The exact position of things is reliant on
        the size of your screen, since it's using the screen pixel positions
        for drawing. I've included some tools to help with that, but first
        let's go over how to actually draw things!
        */

        [MEventWorldDrawBlocks]
        public static void RenderBlocksWorld()
        {
            /*
            The MRenderer class contains a lot of methods for drawing
            different things. We won't go over all of them, but we'll
            go over some of the simple ones, and the rest are very similar. 
            
            First up, as simple as we can get - DrawPoint. This will, well,
            draw a singular "point" (square) at the given position, with
            the given size and color.
            */
            
            MRenderer.DrawPoint(Vec2.Zero, 1, Color.Aqua);
            
            /*
            Next up, a line. Pretty much the same, just with an extra position to pass.
            However, I've also included an optional parameter here - the BlendState!
            The blend state determines how the polygon is composited - added onto - what's
            already been drawn. Here I've set it as AlphaBlend, which means it will be
            transparent! - Or, at least, it would if I passed it a transparent colour.
            There's a few other blend states available, and all are pretty simple.
            
            The only one that's a bit more complex is NonPremultiplied. It's really
            intended for a specific type of more technical rendering, but for this,
            all you need to know is that it will draw the colour solid, but use the
            alpha as a multiplication factor - so if you have a bright red, but alpha
            at half, then it will draw a darker, "half-red" colour.
            */

            MRenderer.DrawLine(Vec2.Zero, Vec2.One, 1, Color.Aqua, BlendState.AlphaBlend);
            
            /*
            Most functions also have a "DrawGrad" version. This lets you specify a colour for each vertex 
            (each end, for a line) in the shape, and will create a smooth gradient between the colours!
            */
            
            MRenderer.DrawGradLine(Vec2.Zero, Vec2.One, 1, Color.Aqua, Color.Orange);
            
            /*
            Finally for drawing polygons, if none of the built in shape methods work for what you want, you
            can use DrawArray - this lets you draw whatever shape you like, provided you have it in the right
            format to draw. It takes an array of vertexes, a colour, and the type of primitive you want to
            draw. That last part is the most important - internally, we can only draw two different kinds of
            shape - triangles and lines. All other shapes have to be made up of combinations of them. (Usually
            just triangles, though)
            
            For both triangles and lines, we can choose either "strip" or "list". This is important and will depend
            a lot on what you're trying to draw. The list option will use sets of vertices for each shape - so
            each 2 points will be a separate line, or each 3 will be a triangle - whereas the strip option lets
            you combine them together to draw more complex shapes. A strip will use the previous vertices as well
            as the new ones - it's a little hard to describe, but, let's say you pass 5 vertices. A, B, C, D, E.
            (also we're drawing triangles in this example)
            
            The first triangle will be drawn A->B->C, but instead of doing D->E->? for the next one, the next triangle
            will actually be B->C->D, and then a third one will be done C->D->E! This lets you draw lots of connected
            triangles with less vertices, and can be used for really complex shapes. For lines, it's the same, but
            A->B, B->C, C->D and so on for each line.
            */
            
            MRenderer.DrawArray(new []{Vec2.Zero, Vec2.One, Vec2.Unitx}, Color.Aqua, PrimitiveType.TriangleList);
            
            /*
            The renderer also has some functions for drawing other things, too! They're mostly for convenience, so
            everything is done in the same place, though they do have some of their own functionality too.
            
            We have a DrawSprite method with a few different overloads (optional parameters), which I'll go
            over. You can just use it with a sprite and position (Here I'm just giving it null for the sprite,
            but for real drawing you obviously need to give it a sprite to draw!), but there's some more
            options you can add too. You can give it a float or Vec2 for scaling the sprite (A Vec2 lets you
            scale it differently in different directions), and you can give it a layer, and a "adjust position"
            setting. 
            
            Those last two are connected. If you set adjustPosition to true, the renderer will assume
            the position you've given it is in screen space, and convert it to world space for you, using
            the HUD layer for the conversion. However, different layers will sometimes have different
            resolutions or scales, so if you pass a Layer yourself, then adjust position will automatically
            be true, and it will use that layer for the conversion, instead of defaulting to using the HUD
            layer.
            
            Generally though, you're probably just going to be passing a sprite, position, and sometimes scale.
            */
            
            MRenderer.DrawSprite(null, Vec2.Zero,  Vec2.One, Layer.Blocks, true);
            
            /*
            We also have a couple of text drawing methods, which work in roughly the same way, just
            with text! The only differences here are that instead of adjust position, there's a 
            "gamePos" setting (the renderer assumes positions are game positions by default, for
            text), and the option to add a custom font. (Currently I don't have any kind of custom
            font system, but I have plans for one in future. You can do them yourself fairly easily,
            though!)
            */
            
            MRenderer.DrawText("hi!", Vec2.Zero, Color.Aqua, 1f, true, null, Layer.Background);
            
            /*
            And the second version, which draws with an outline (the second 
            colour parameter is the color for the outline!)
            */
            MRenderer.DrawOutlineText("hi!", Vec2.Zero, Color.Aqua, Color.Black, 1f, true, null, Layer.Background);
        }

        /*
        As I mentioned earlier, it is possible to do rendering outside of
        render events, too, with the use of RenderTasks! The render task
        system isn't very developed as I haven't had need for it much myself,
        but it does have basic functionality, and I would like to expand it in future. 
        */
        [MEventPostGameUpdate]
        public static void RenderTask()
        {
            /*
            Render tasks allow you to queue up rendering, which will then be drawn later.
            Specifically, they will be drawn in the World draw event, on the HUD layer.
            They're really intended for debugging things where you can't use the normal
            rendering events.
            
            Currently there's only two render tasks implemented - a base one that acts
            like a DrawArray, and the one shown here, for drawing a single point.
            */
            MRenderer.AddRenderTask(new MPointRenderTask(Vec2.Zero, 5, Color.Aqua));
        }
    }
}