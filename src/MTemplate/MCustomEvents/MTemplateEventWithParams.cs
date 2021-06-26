using MClient.EventSystem.Events;

namespace MClient.MTemplate.MCustomEvents
{
    public class MTemplateEventWithParams : MEvent
    {
        public object Param;

        public static MTemplateEventWithParams Get(object param)
        {
            MTemplateEventWithParams temp = new MTemplateEventWithParams {Param = param};
            return temp;
        }
    }
}