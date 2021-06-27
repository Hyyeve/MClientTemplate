using MClient.Core.EventSystem.Events;

namespace MClient.MTemplate.EventSystemDemo
{
    /// <summary>
    /// A slightly more complex template event that can pass parameters.
    /// Due to how the event system works, parameters must be passed via
    /// some kind of "Get" method, and cannot be included in the constructor,
    /// since the event class is also used as the attribute. See <see cref="MEventSystemDemo"/> for usage.
    /// </summary>
    public class MEventTemplateWithParams : MEvent
    {
        public object Param;

        public static MEventTemplateWithParams Get(object param)
        {
            var temp = new MEventTemplateWithParams {Param = param};
            return temp;
        }
    }
}