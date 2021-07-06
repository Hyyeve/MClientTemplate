using MClient.SettingsSystem;

namespace MClient.MTemplate
{
    /// <summary>
    /// Demonstration class for the settings system!
    /// </summary>
    public class MSettingsSystemDemo
    {
        /*
        The settings system is super simple currently, though
        I have some plans to expand it in future. If you want
        to add support for more types of value, see the
        MSettingHandler class.
        
        The settings system purely uses auto-attributes at the
        moment - in fact, just one of them, this one:
        */

        [MSerializeSetting]
        public static string Value;
        
        /*
        ...And that's literally all it takes! That value there
        will now be saved on exit, and reloaded on start,
        automatically! As I mentioned, I do want to expand
        this system to implement more customization and
        functionality in future, but this suffices for
        simple easy settings.
        
        Currently supported types of value are
        the following:
        
        - string
        - int
        - float
        - double
        - bool
        - Color
        - Vec2
        
        But adding support for more is pretty easy to do.
        And, again, I plan to expand the settings system 
        at some point in future too.
        
        Of note: Values are loaded in the EarlyInit event,
        and saved in the GameExit event. Modifying or 
        accessing them during these events may cause issues!
        */
    }
}