using System.ComponentModel;
using Manager.Abstractions;

namespace MyGarageAPI.BusinessLogic
{
    public class MyGarageBusiness
    {
        private readonly IMyGarageManager _myGarageManager;

        public MyGarageBusiness(IMyGarageManager myGarageManager)
        {
            _myGarageManager = myGarageManager;
        }

        
    }
}
