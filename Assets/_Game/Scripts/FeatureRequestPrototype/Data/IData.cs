using System.Collections.Generic;

namespace _Game.Scripts.Data {
    public interface IData {
        /**
         * Returns validation errors list
         */
        List<string> LoadAndValidate();
    }
}