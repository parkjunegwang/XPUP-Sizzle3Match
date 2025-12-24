

namespace Assets.Scripts.FrameWork.Job
{    
  
    public class JMActionMgr
    {
       
        public virtual JMAction OverrideAction<T>(params object[] args) where T : JMAction
        {
           
            T t = (T)System.Activator.CreateInstance(typeof(T), args);
            return t;
        }
    }
    
}