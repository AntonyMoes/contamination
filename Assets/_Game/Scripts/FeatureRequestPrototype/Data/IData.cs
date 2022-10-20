using System.Collections.Generic;

namespace _Game.Scripts.FeatureRequestPrototype.Data {
    public interface IData {
        /**
         * Returns validation errors list
         */
        List<string> LoadAndValidate();
    }
}