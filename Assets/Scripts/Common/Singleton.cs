
namespace Common
{
    public class Singleton<T> where T : new()
    {
        public static T instance => calc_instance();
        static T m_instance;

        //=============================================================================================

        static T calc_instance()
        {
            m_instance ??= new();

            return m_instance;
        }
    }
}

